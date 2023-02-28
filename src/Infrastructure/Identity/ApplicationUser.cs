using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
#pragma warning disable CS8618
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
#pragma warning restore CS8618
}