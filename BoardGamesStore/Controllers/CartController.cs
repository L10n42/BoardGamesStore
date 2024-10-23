using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> AddToCart(int id, string returnUrl)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

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
