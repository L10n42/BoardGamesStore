using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardGamesStore.Data;
using BoardGamesStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BoardGamesStore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BoardGamesStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly BoardGamesStoreContext _context;
        private readonly IStringLocalizer<CartController> _localizer;

        public CartController(BoardGamesStoreContext context, IStringLocalizer<CartController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cartItems = await _context.ShoppingCarts
                .Include(c => c.BoardGame)
                .ThenInclude(g => g.BoardGameImages)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            var viewModel = new CartViewModel
            {
                Items = cartItems,
                TotalPrice = cartItems.Sum(i => i.BoardGame.Price * i.Quantity),
                TotalItems = cartItems.Sum(i => i.Quantity)
            };

            return View(viewModel);
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartItem = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.UserID == userId && c.BoardGameID == id);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartItem = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.UserID == userId && c.BoardGameID == id);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity -= 1;
                }
                else
                {
                    _context.ShoppingCarts.Remove(cartItem);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cartItems = await _context.ShoppingCarts
                .Include(c => c.BoardGame)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index");

            var totalAmount = cartItems.Sum(i => i.BoardGame.Price * i.Quantity);

            var viewModel = new CheckoutViewModel
            {
                Items = cartItems,
                TotalAmount = totalAmount,
                AvailableDeliveryOptions = new SelectList(
                    new List<SelectListItem> {
                        new SelectListItem { Value = "Standard Delivery", Text = _localizer["Standard Delivery"] },
                        new SelectListItem { Value = "Express Delivery", Text = _localizer["Express Delivery"] }
                    }, "Value", "Text", "Standard Delivery"
                ),
                AvailablePaymentMethods = new SelectList(
                    new List<SelectListItem> {
                        new SelectListItem { Value = "Credit Card", Text = _localizer["Credit Card"] },
                        new SelectListItem { Value = "Cash on Delivery", Text = _localizer["Cash on Delivery"] }
                    }, "Value", "Text", "Credit Card"
                )
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cartItems = await _context.ShoppingCarts
                .Include(c => c.BoardGame)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index");

            model.Items = cartItems;
            model.TotalAmount = cartItems.Sum(i => i.BoardGame.Price * i.Quantity);

            model.AvailableDeliveryOptions = new SelectList(
                new List<SelectListItem> {
                    new SelectListItem { Value = "Standard Delivery", Text = _localizer["Standard Delivery"] },
                    new SelectListItem { Value = "Express Delivery", Text = _localizer["Express Delivery"] }
                }, "Value", "Text", model.DeliveryOption
            );
            model.AvailablePaymentMethods = new SelectList(
                new List<SelectListItem> {
                    new SelectListItem { Value = "Credit Card", Text = _localizer["Credit Card"] },
                    new SelectListItem { Value = "Cash on Delivery", Text = _localizer["Cash on Delivery"] }
                }, "Value", "Text", model.PaymentMethod
            );

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.PaymentMethod == "Credit Card" && string.IsNullOrWhiteSpace(model.CardNumber) || string.IsNullOrWhiteSpace(model.ExpiryDate) || string.IsNullOrWhiteSpace(model.CVV))
            {
                if (string.IsNullOrWhiteSpace(model.CardNumber))
                {
                    ModelState.AddModelError("CardNumber", _localizer["CardNumberIsRequired"]);
                }

                if (string.IsNullOrWhiteSpace(model.ExpiryDate))
                {
                    ModelState.AddModelError("ExpiryDate", _localizer["ExpiryDateIsRequired"]);
                }

                if (string.IsNullOrWhiteSpace(model.CVV))
                {
                    ModelState.AddModelError("CVV", _localizer["CVVIsRequired"]);
                }
                return View(model);
            }

            var order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(i => i.BoardGame.Price * i.Quantity),
                OrderStatus = "Pending",

                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                DeliveryOption = model.DeliveryOption,
                PaymentMethod = model.PaymentMethod,

                CardNumber = model.PaymentMethod == "Credit Card" ? model.CardNumber[^4..] : null,
                ExpiryDate = model.PaymentMethod == "Credit Card" ? model.ExpiryDate : null,

                OrderDetails = cartItems.Select(item => new OrderDetail
                {
                    BoardGameID = item.BoardGameID,
                    Quantity = item.Quantity,
                    Price = item.BoardGame.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.ShoppingCarts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { id = order.OrderID });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Checkout(int id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var cartItem = await _context.ShoppingCarts.Include(c => c.BoardGame)
        //                    .FirstOrDefaultAsync(c => c.UserID == userId && c.BoardGameID == id);

        //    if (cartItem != null)
        //    {
        //        _context.ShoppingCarts.Remove(cartItem);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CheckoutAll()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var cartItems = await _context.ShoppingCarts.Include(c => c.BoardGame)
        //                    .Where(c => c.UserID == userId).ToListAsync();

        //    if (cartItems.Any())
        //    {
        //        _context.ShoppingCarts.RemoveRange(cartItems);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("Index");

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, string returnUrl)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var boardGame = await _context.BoardGames.FindAsync(id);
            if (boardGame == null)
            {
                return NotFound();
            }

            var cartItem = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.UserID == userId && c.BoardGameID == id);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                cartItem = new CartBoardGame
                {
                    UserID = userId,
                    BoardGameID = id,
                    Quantity = 1
                };
                _context.ShoppingCarts.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "BoardGames");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartItem = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.UserID == userId && c.BoardGameID == id);

            if (cartItem != null)
            {
                _context.ShoppingCarts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
