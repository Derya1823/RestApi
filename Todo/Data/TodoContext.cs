using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Todo.Models;

namespace Todo.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
        public DbSet<User> Users => Set<User>();
    }
}
