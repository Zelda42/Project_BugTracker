using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTracker1._2025.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Bug> AssignedBugs { get; set; } = new List<Bug>();
    }
}