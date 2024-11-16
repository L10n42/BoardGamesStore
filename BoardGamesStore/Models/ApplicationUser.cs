using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }

    public ICollection<Order>? Orders { get; set; }
    public ICollection<CartBoardGame>? CartBoardGames { get; set; }
}

