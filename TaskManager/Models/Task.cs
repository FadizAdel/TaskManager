using System.ComponentModel.DataAnnotations;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public enum TaskStatus {
        Pending = 0,
        InProgress = 1,
        Completed= 2
    }
    public class Task
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [Required] // Makes it required
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; } = false; // Default is false

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public TaskStatus Status { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

    }
}
