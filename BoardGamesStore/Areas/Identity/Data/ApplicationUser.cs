using BoardGamesStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BoardGamesStore.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    public ICollection<Order>? Orders { get; set; }
    public ICollection<CartBoardGame>? CartBoardGames { get; set; }
}

