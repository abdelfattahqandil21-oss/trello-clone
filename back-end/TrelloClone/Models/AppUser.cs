using Microsoft.AspNetCore.Identity;

namespace TrelloClone.Models
{
    public class AppUser : IdentityUser
    {
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
