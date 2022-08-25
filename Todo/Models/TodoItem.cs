using Microsoft.EntityFrameworkCore;
namespace Todo.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public int? UserId { get; set; }
    }
}
