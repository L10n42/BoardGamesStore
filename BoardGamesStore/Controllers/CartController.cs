using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardGamesStore.Data;
using BoardGamesStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BoardGamesStore.ViewModels;

namespace BoardGamesStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly BoardGamesStoreContext _context;

        public CartController(BoardGamesStoreContext context)
        {
            _context = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartItem = await _context.ShoppingCarts.Include(c => c.BoardGame)
                            .FirstOrDefaultAsync(c => c.UserID == userId && c.BoardGameID == id);

            if (cartItem != null)
            {
                _context.ShoppingCarts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartItems = await _context.ShoppingCarts.Include(c => c.BoardGame)
                            .Where(c => c.UserID == userId).ToListAsync();

            if (cartItems.Any())
            {
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");

        }

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

    }
}
