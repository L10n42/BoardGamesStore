using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoardGamesStore.Data;
using BoardGamesStore.Models;

namespace BoardGamesStore.Controllers
{
    public class CartController : Controller
    {
        private readonly BoardGamesStoreContext _context;

        public CartController(BoardGamesStoreContext context)
        {
            _context = context;
        }

        // GET: CartBoardGames
        public async Task<IActionResult> Index()
        {
            var boardGamesStoreContext = _context.ShoppingCarts.Include(c => c.BoardGame).Include(c => c.User);
            return View(await boardGamesStoreContext.ToListAsync());
        }

        // GET: CartBoardGames/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartBoardGame = await _context.ShoppingCarts
                .Include(c => c.BoardGame)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (cartBoardGame == null)
            {
                return NotFound();
            }

            return View(cartBoardGame);
        }

        // GET: CartBoardGames/Create
        public IActionResult Create()
        {
            ViewData["BoardGameID"] = new SelectList(_context.BoardGames, "BoardGameID", "BoardGameID");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CartBoardGames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,BoardGameID,Quantity")] CartBoardGame cartBoardGame)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartBoardGame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoardGameID"] = new SelectList(_context.BoardGames, "BoardGameID", "BoardGameID", cartBoardGame.BoardGameID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cartBoardGame.UserID);
            return View(cartBoardGame);
        }

        // GET: CartBoardGames/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartBoardGame = await _context.ShoppingCarts.FindAsync(id);
            if (cartBoardGame == null)
            {
                return NotFound();
            }
            ViewData["BoardGameID"] = new SelectList(_context.BoardGames, "BoardGameID", "BoardGameID", cartBoardGame.BoardGameID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cartBoardGame.UserID);
            return View(cartBoardGame);
        }

        // POST: CartBoardGames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserID,BoardGameID,Quantity")] CartBoardGame cartBoardGame)
        {
            if (id != cartBoardGame.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartBoardGame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartBoardGameExists(cartBoardGame.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoardGameID"] = new SelectList(_context.BoardGames, "BoardGameID", "BoardGameID", cartBoardGame.BoardGameID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", cartBoardGame.UserID);
            return View(cartBoardGame);
        }

        // GET: CartBoardGames/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartBoardGame = await _context.ShoppingCarts
                .Include(c => c.BoardGame)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (cartBoardGame == null)
            {
                return NotFound();
            }

            return View(cartBoardGame);
        }

        // POST: CartBoardGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cartBoardGame = await _context.ShoppingCarts.FindAsync(id);
            if (cartBoardGame != null)
            {
                _context.ShoppingCarts.Remove(cartBoardGame);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartBoardGameExists(string id)
        {
            return _context.ShoppingCarts.Any(e => e.UserID == id);
        }
    }
}
