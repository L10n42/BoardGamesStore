using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoardGamesStore.Data;
using BoardGamesStore.Models;
using Microsoft.Data.SqlClient;
using BoardGamesStore.ViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace BoardGamesStore.Controllers
{
    public class BoardGamesController : Controller
    {
        private readonly BoardGamesStoreContext _context;

        public BoardGamesController(BoardGamesStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(BoardGamesViewModel viewModel)
        {
            IQueryable<BoardGame> query = _context.BoardGames
                .Include(b => b.Category)
                .Include(bg => bg.BoardGameImages);

            if (!string.IsNullOrEmpty(viewModel.SearchQuery))
            {
                query = query.Where(b => b.Name.Contains(viewModel.SearchQuery));
            }

            if (viewModel.CategoryID.HasValue)
            {
                query = query.Where(b => b.CategoryID == viewModel.CategoryID.Value);
            }

            if (viewModel.MinPrice.HasValue)
            {
                query = query.Where(b => b.Price >= viewModel.MinPrice.Value);
            }

            if (viewModel.MaxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= viewModel.MaxPrice.Value);
            }

            switch (viewModel.SortBy)
            {
                case "Name":
                    query = viewModel.SortOrder == "ASC" ? query.OrderBy(b => b.Name) : query.OrderByDescending(b => b.Name);
                    break;
                case "Price":
                    query = viewModel.SortOrder == "ASC" ? query.OrderBy(b => b.Price) : query.OrderByDescending(b => b.Price);
                    break;
                default:
                    query = viewModel.SortOrder == "ASC" ? query.OrderBy(b => b.CreatedAt) : query.OrderByDescending(b => b.CreatedAt);
                    break;
            }

            viewModel.BoardGames = await query.ToListAsync();
            viewModel.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName", viewModel.CategoryID);
            viewModel.SortOptions = new SelectList(
                new List<SelectListItem> {
                    new SelectListItem { Value = "Date", Text = "Date" },
                    new SelectListItem { Value = "Name", Text = "Name" },
                    new SelectListItem { Value = "Price", Text = "Price" }
                }, "Value", "Text", viewModel.SortBy
            );
            viewModel.SortOrderOptions = new SelectList(
                new List<SelectListItem> {
                    new SelectListItem { Value = "ASC", Text = "Ascending" },
                    new SelectListItem { Value = "DESC", Text = "Descending" },
                }, "Value", "Text", viewModel.SortOrder
            );

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames
                .Include(b => b.Category)
                .Include(bg => bg.BoardGameImages)
                .FirstOrDefaultAsync(m => m.BoardGameID == id);
            if (boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [Authorize(Roles = "Admin")]
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
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", boardGame.CategoryID);
            return View(boardGame);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames.Include(b => b.BoardGameImages).FirstOrDefaultAsync(b => b.BoardGameID == id);
            if (boardGame == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", boardGame.CategoryID);
            return View(boardGame);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("BoardGameID,Name,Description,Price,StockQuantity,CreatedAt,NumberOfPlayers,GameTimeMinutes,SuggestedAge,CategoryID")] BoardGame boardGame,
            List<IFormFile> images,
            List<int> existingImageIds)
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

                    await DeleteImages(existingImageIds, boardGame.BoardGameID);

                    if (images != null && images.Count > 0)
                    {
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
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!BoardGameExists(boardGame.BoardGameID)) return NotFound(); else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", boardGame.CategoryID);
            return View(boardGame);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var boardGame = await _context.BoardGames
                .Include(b => b.Category)
                .Include(b => b.BoardGameImages)
                .FirstOrDefaultAsync(m => m.BoardGameID == id);

            if (boardGame == null) return NotFound();

            return View(boardGame);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boardGame = await _context.BoardGames
                .Include(b => b.BoardGameImages)
                .FirstOrDefaultAsync(m => m.BoardGameID == id);

            if (boardGame != null)
            {
                await DeleteImages(boardGame.BoardGameImages.Select(i => i.BoardGameImageID).ToList(), boardGame.BoardGameID);

                _context.BoardGames.Remove(boardGame);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BoardGameExists(int id)
        {
            return _context.BoardGames.Any(e => e.BoardGameID == id);
        }

        private async Task DeleteImages(List<int> existingImageIds, int boardGameId)
        {
            var currentImages = await _context.BoardGameImages.Where(img => img.BoardGameID == boardGameId).ToListAsync();
            var imagesToDelete = currentImages.Where(img => !existingImageIds.Contains(img.BoardGameImageID)).ToList();

            foreach (var imgToDelete in imagesToDelete)
            {
                _context.BoardGameImages.Remove(imgToDelete);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imgToDelete.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
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
