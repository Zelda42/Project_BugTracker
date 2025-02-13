using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker1._2025.Models
{
    public class Bug
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; } = Status.Open;

        [Required]
        [EnumDataType(typeof(Priority))]
        public Priority Priority { get; set; } = Priority.Medium;

        [ForeignKey("AssignedToUser")]
        public string? AssignedToUserId { get; set; } // Nullable if unassigned

        public virtual ApplicationUser? AssignedToUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Add a list of comments to fix the error
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum Status
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public enum Priority
    {
        Low,
        Medium,
        High,
        Critical
    }
}