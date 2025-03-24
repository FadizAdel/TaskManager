using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TaskManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        
        public ICollection<TaskItem> Tasks { get; set; }
    }
}
