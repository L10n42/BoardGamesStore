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
    public class BoardGamesController : Controller
    {
        private readonly BoardGamesStoreContext _context;

        public BoardGamesController(BoardGamesStoreContext context)
        {
            _context = context;
        }

        // GET: BoardGames
        public async Task<IActionResult> Index()
        {
            var boardGamesStoreContext = _context.BoardGames
                .Include(b => b.Category)
                .Include(bg => bg.BoardGameImages);
            return View(await boardGamesStoreContext.ToListAsync());
        }

        // GET: BoardGames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BoardGameID == id);
            if (boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        // GET: BoardGames/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: BoardGames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoardGameID,Name,Description,Price,StockQuantity,CreatedAt,NumberOfPlayers,GameTimeMinutes,SuggestedAge,CategoryID")] BoardGame boardGame, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                boardGame.CreatedAt = DateTime.Now;
                _context.BoardGames.Add(boardGame);
                await _context.SaveChangesAsync();

                var imageUrls = await UploadImages(images);
                foreach (var imageUrl in imageUrls)
                {
                    var boardGameImage = new BoardGameImage
                    {
                        ImageUrl = imageUrl,
                        BoardGameID = boardGame.BoardGameID
                    };

                    _context.BoardGameImages.Add(boardGameImage);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", boardGame.CategoryID);
            return View(boardGame);
        }

        // GET: BoardGames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames.FindAsync(id);
            if (boardGame == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", boardGame.CategoryID);
            return View(boardGame);
        }

        // POST: BoardGames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoardGameID,Name,Description,Price,StockQuantity,CreatedAt,NumberOfPlayers,GameTimeMinutes,SuggestedAge,CategoryID")] BoardGame boardGame)
        {
            if (id != boardGame.BoardGameID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boardGame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardGameExists(boardGame.BoardGameID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", boardGame.CategoryID);
            return View(boardGame);
        }

        // GET: BoardGames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BoardGameID == id);
            if (boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        // POST: BoardGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boardGame = await _context.BoardGames.FindAsync(id);
            if (boardGame != null)
            {
                _context.BoardGames.Remove(boardGame);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardGameExists(int id)
        {
            return _context.BoardGames.Any(e => e.BoardGameID == id);
        }

        private async Task<List<string>> UploadImages(List<IFormFile> imageFiles)
        {
            var imageUrls = new List<string>();

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "boardgames");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    if (imageFile.Length > 0)
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                        var filePath = Path.Combine(folderPath, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        imageUrls.Add($"/images/boardgames/{uniqueFileName}");
                    }
                }
            }

            return imageUrls;
        }
    }
}
