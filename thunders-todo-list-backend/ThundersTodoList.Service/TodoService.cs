using Microsoft.EntityFrameworkCore;
using ThundersTodoList.Domain.Entities;
using ThundersTodoList.Domain.Interfaces.Services;
using ThundersTodoList.Infra;

namespace ThundersTodoList.Service;

public class TodoService(AppDbContext context) : ITodoService
{
    public async Task<List<Todo>> GetAllAsync()
    {
        return await context.Todos.ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(int id)
    {
        return await context.Todos.FindAsync(id);
    }

    public async Task CreateAsync(Todo todo)
    {
        await context.Todos.AddAsync(todo);
        await context.SaveChangesAsync();
    }

    public async Task AlterAsync(Todo todo)
    {
        context.Todos.Update(todo);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Todo todo)
    {
        context.Todos.Remove(todo);
        await context.SaveChangesAsync();
    }
}
