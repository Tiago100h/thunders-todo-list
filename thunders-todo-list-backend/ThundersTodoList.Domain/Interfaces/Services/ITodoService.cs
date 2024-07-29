using ThundersTodoList.Domain.Entities;

namespace ThundersTodoList.Domain.Interfaces.Services;

public interface ITodoService
{
    Task<List<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task CreateAsync(Todo todo);
    Task AlterAsync(Todo todo);
    Task DeleteAsync(Todo todo);
}