using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ThundersTodoList.Api;
using ThundersTodoList.Domain.Dtos;
using ThundersTodoList.Domain.Entities;
using ThundersTodoList.Domain.Interfaces.Services;

namespace ThundersTodoList.Test;

public class TodoApiTests()
{
    [Fact]
    public async Task GetAllReturnsTodosFromDatabase()
    {
        // Arrange
        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetAllAsync())
            .ReturnsAsync(
            [
                new() 
                { 
                    Id = 1, 
                    Title = "Walk the dog" 
                },
                new() 
                { 
                    Id = 2, 
                    Title = "Do the dishes", 
                    IsCompleted = true 
                }
            ]);

        // Act
        var result = await TodoEndpoints.GetAllAsync(mock.Object);

        //Assert
        Assert.IsType<Ok<List<TodoDto>>>(result);

        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
        Assert.Collection(result.Value, todo1 =>
        {
            Assert.Equal(1, todo1.Id);
            Assert.Equal("Walk the dog", todo1.Title);
            Assert.False(todo1.IsCompleted);
        }, todo2 =>
        {
            Assert.Equal(2, todo2.Id);
            Assert.Equal("Do the dishes", todo2.Title);
            Assert.True(todo2.IsCompleted);
        });
    }

    [Fact]
    public async Task GetByIdReturnsNotFoundIfNotExists()
    {
        // Arrange
        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetByIdAsync(It.Is<int>(id => id == 1)))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await TodoEndpoints.GetByIdAsync(1, mock.Object);

        //Assert
        Assert.IsType<Results<Ok<TodoDto>, NotFound>>(result);

        var notFoundResult = (NotFound)result.Result;

        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetByIdReturnsTodoFromDatabase()
    {
        // Arrange
        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetByIdAsync(It.Is<int>(id => id == 1)))
            .ReturnsAsync(new Todo
            {
                Id = 1, 
                Title = "Walk the dog"
            });

        // Act
        var result = await TodoEndpoints.GetByIdAsync(1, mock.Object);

        //Assert
        Assert.IsType<Results<Ok<TodoDto>, NotFound>>(result);

        var okResult = (Ok<TodoDto>)result.Result;

        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
        Assert.Equal(1, okResult.Value.Id);
    }

    [Fact]
    public async Task CreateTodoCreatesTodoInDatabase()
    {
        //Arrange
        var todos = new List<Todo>();

        var newTodo = new TodoAddDto("Test title");

        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.CreateAsync(It.Is<Todo>(x => x.Title == newTodo.Title)))
            .Callback<Todo>(todo => todos.Add(todo))
            .Returns(Task.CompletedTask);

        //Act
        var result = await TodoEndpoints.CreateAsync(newTodo, mock.Object);

        //Assert
        Assert.IsType<Created<TodoDto>>(result);

        Assert.NotNull(result);
        Assert.NotNull(result.Location);

        Assert.NotEmpty(todos);
        Assert.Collection(todos, todo =>
        {
            Assert.Equal("Test title", todo.Title);
            Assert.False(todo.IsCompleted);
        });
    }

    [Fact]
    public async Task AlterTodoUpdatesTodoInDatabase()
    {
        //Arrange
        var existingTodo = new Todo
        {
            Id = 1,
            Title = "Exiting test title",
        };

        var updatedTodo = new TodoAddDto("Updated test title");

        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetByIdAsync(It.Is<int>(id => id == 1)))
            .ReturnsAsync(existingTodo);

        mock.Setup(m => m.AlterAsync(It.Is<Todo>(t => t.Id == 1 && t.Title == updatedTodo.Title)))
            .Callback<Todo>(todo => existingTodo = todo)
            .Returns(Task.CompletedTask);

        //Act
        var result = await TodoEndpoints.AlterAsync(1, updatedTodo, mock.Object);

        //Assert
        Assert.IsType<Results<Created<TodoDto>, NotFound>>(result);

        var createdResult = (Created<TodoDto>)result.Result;

        Assert.NotNull(createdResult);
        Assert.NotNull(createdResult.Location);

        Assert.Equal("Updated test title", existingTodo.Title);
    }

    [Fact]
    public async Task DeleteTodoDeletesTodoInDatabase()
    {
        //Arrange
        var existingTodo = new Todo
        {
            Id = 1,
            Title = "Test title 1"
        };

        var todos = new List<Todo> { existingTodo };

        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetByIdAsync(It.Is<int>(id => id == existingTodo.Id)))
            .ReturnsAsync(existingTodo);

        mock.Setup(m => m.DeleteAsync(It.Is<Todo>(t => t.Id == 1)))
            .Callback<Todo>(t => todos.Remove(t))
            .Returns(Task.CompletedTask);

        //Act
        var result = await TodoEndpoints.DeleteAsync(1, mock.Object);

        //Assert
        Assert.IsType<Results<NoContent, NotFound>>(result);

        var noContentResult = (NoContent)result.Result;

        Assert.NotNull(noContentResult);
        Assert.Empty(todos);
    }

    [Fact]
    public async Task CheckTodoChecksTodoInDatabase()
    {
        //Arrange
        var existingTodo = new Todo
        {
            Id = 1,
            Title = "Test title 1"
        };

        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetByIdAsync(It.Is<int>(id => id == existingTodo.Id)))
            .ReturnsAsync(existingTodo);

        mock.Setup(m => m.AlterAsync(It.Is<Todo>(t => t.Id == 1 && t.IsCompleted == existingTodo.IsCompleted)))
            .Callback<Todo>(todo => existingTodo = todo)
            .Returns(Task.CompletedTask);

        //Act
        var result = await TodoEndpoints.CheckAsync(1, mock.Object);

        //Assert
        Assert.IsType<Results<Created<TodoDto>, NotFound>>(result);

        var createdResult = (Created<TodoDto>)result.Result;

        Assert.NotNull(createdResult);
        Assert.NotNull(createdResult.Location);

        Assert.True(existingTodo.IsCompleted);
    }

    [Fact]
    public async Task UncheckTodoUnchecksTodoInDatabase()
    {
        //Arrange
        var existingTodo = new Todo
        {
            Id = 1,
            Title = "Test title 1",
            IsCompleted = true,
        };

        var mock = new Mock<ITodoService>();

        mock.Setup(m => m.GetByIdAsync(It.Is<int>(id => id == existingTodo.Id)))
            .ReturnsAsync(existingTodo);

        mock.Setup(m => m.AlterAsync(It.Is<Todo>(t => t.Id == 1 && t.IsCompleted == existingTodo.IsCompleted)))
            .Callback<Todo>(todo => existingTodo = todo)
            .Returns(Task.CompletedTask);

        //Act
        var result = await TodoEndpoints.UncheckAsync(1, mock.Object);

        //Assert
        Assert.IsType<Results<Created<TodoDto>, NotFound>>(result);

        var createdResult = (Created<TodoDto>)result.Result;

        Assert.NotNull(createdResult);
        Assert.NotNull(createdResult.Location);

        Assert.False(existingTodo.IsCompleted);
    }

}