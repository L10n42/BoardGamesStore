using Microsoft.AspNetCore.Identity;

namespace BoardGamesStore.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Order>? Orders { get; set; }
    public ICollection<CartBoardGame>? CartBoardGames { get; set; }
}

