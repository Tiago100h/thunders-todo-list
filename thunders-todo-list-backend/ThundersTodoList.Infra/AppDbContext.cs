using Microsoft.EntityFrameworkCore;
using ThundersTodoList.Domain.Entities;

namespace ThundersTodoList.Infra;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<Todo> Todos { get; set; }
}