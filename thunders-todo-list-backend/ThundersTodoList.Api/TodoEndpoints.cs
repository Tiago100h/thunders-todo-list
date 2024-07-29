using Microsoft.AspNetCore.Http.HttpResults;
using ThundersTodoList.Domain.Dtos;
using ThundersTodoList.Domain.Entities;
using ThundersTodoList.Domain.Interfaces.Services;

namespace ThundersTodoList.Api;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodosApi(this RouteGroupBuilder todosApi)
    {
        todosApi.MapGet("/", GetAllAsync);
        todosApi.MapGet("/{id}", GetByIdAsync);
        todosApi.MapPost("/", CreateAsync);
        todosApi.MapPut("/{id}", AlterAsync);
        todosApi.MapDelete("/{id}", DeleteAsync);
        todosApi.MapPatch("/{id}/check", CheckAsync);
        todosApi.MapPatch("/{id}/uncheck", UncheckAsync);

        return todosApi;
    }

    public static async Task<Ok<List<TodoDto>>> GetAllAsync(ITodoService service)
    {
        var entities = await service.GetAllAsync();

        var dtos = entities
            .Select(x => new TodoDto(x.Id, x.Title, x.IsCompleted))
            .ToList();

        return TypedResults.Ok(dtos);
    }

    public static async Task<Results<Ok<TodoDto>, NotFound>> GetByIdAsync(int id, ITodoService service)
    {
        var entity = await service.GetByIdAsync(id);

        if (entity != null)
        {
            var dto = new TodoDto(entity.Id, entity.Title, entity.IsCompleted);
            return TypedResults.Ok(dto);
        }
        
        return TypedResults.NotFound();
    }

    public static async Task<Created<TodoDto>> CreateAsync(TodoAddDto model, ITodoService service)
    {
        var newEntity = new Todo 
        { 
            Title = model.Title,
        };

        await service.CreateAsync(newEntity);

        var dto = new TodoDto(newEntity.Id, newEntity.Title, newEntity.IsCompleted);

        return TypedResults.Created($"/todos/{dto.Id}", dto);
    }

    public static async Task<Results<Created<TodoDto>, NotFound>> AlterAsync(int id, TodoAddDto model, ITodoService service)
    {
        var existingEntity = await service.GetByIdAsync(id);

        if (existingEntity != null)
        {
            existingEntity.Title = model.Title;

            await service.AlterAsync(existingEntity);

            var dto = new TodoDto(existingEntity.Id, existingEntity.Title, existingEntity.IsCompleted);

            return TypedResults.Created($"/todos/{dto.Id}", dto);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound>> DeleteAsync(int id, ITodoService service)
    {
        var entity = await service.GetByIdAsync(id);

        if (entity != null)
        {
            await service.DeleteAsync(entity);
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Created<TodoDto>, NotFound>> CheckAsync(int id, ITodoService service)
    {
        var existingEntity = await service.GetByIdAsync(id);

        if (existingEntity != null)
        {
            existingEntity.IsCompleted = true;

            await service.AlterAsync(existingEntity);

            var dto = new TodoDto(existingEntity.Id, existingEntity.Title, existingEntity.IsCompleted);

            return TypedResults.Created($"/todos/{dto.Id}", dto);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Created<TodoDto>, NotFound>> UncheckAsync(int id, ITodoService service)
    {
        var existingEntity = await service.GetByIdAsync(id);

        if (existingEntity != null)
        {
            existingEntity.IsCompleted = false;

            await service.AlterAsync(existingEntity);

            var dto = new TodoDto(existingEntity.Id, existingEntity.Title, existingEntity.IsCompleted);

            return TypedResults.Created($"/todos/{dto.Id}", dto);
        }

        return TypedResults.NotFound();
    }
}
