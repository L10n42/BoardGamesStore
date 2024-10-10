using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGamesStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BoardGamesStore.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }


    public ICollection<Order>? Orders { get; set; }
    public ICollection<CartBoardGame>? CartBoardGames { get; set; }
}

