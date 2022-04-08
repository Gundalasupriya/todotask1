using TodoTask.DTOs;
using TodoTask.Models;
using TodoTask.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TodoTask.Controllers;

[ApiController]
[Route("api/Todo")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly ITodoRepository _todo;


    public TodoController(ILogger<TodoController> logger, ITodoRepository todo)
    {
        _logger = logger;
        _todo = todo;

    }
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TodoDTO>>> GetList()
    {
        // var res = await _todo.GetList();
        // return Ok(res.Select(x => x.asDto));
        var TodoList = await _todo.GetList();
        var dtoList = TodoList.Select(x => x);
        return Ok(dtoList);
    }
    [HttpGet("GetUserTodos")]
    [Authorize]
    public async Task<ActionResult<List<TodoDTO>>> GetUserTodos()
    {
        var UserId = GetCurrentuserId();
        var Todo = await _todo.GetUserTodos(Convert.ToInt32(UserId));
        if (Todo == null)
            return NotFound("No User found with given id number");
    
        return Ok(Todo);
    }


    [HttpGet("{todo_id}")]
    [Authorize]

    public async Task<ActionResult> GetById([FromRoute] long todo_id)
    {
        var res = await _todo.GetById(todo_id);
        if (res == null)
            return NotFound("No User found with given id number");
        var dto = res;
        return Ok(dto);
    }

    [HttpPost]
    [Authorize]

    public async Task<ActionResult<TodoDTO>> CreateTodo([FromBody] TodoCreateDTO Data)
    {
        var id = GetCurrentuserId();
        var toCreateTodo = new Todo
        {
            UserId = Data.UserId,
            Title = Data.Title,
            Description = Data.Description,

        };
        var createdTodo = await _todo.Create(toCreateTodo);

        return StatusCode(StatusCodes.Status201Created, createdTodo);
    }

    [HttpPut("{todo_id}")]
    [Authorize]
    public async Task<ActionResult> UpdateTodo([FromRoute] long todo_id, [FromBody] TodoUpdateDTO Data)
    {
        var existing = await _todo.GetById(todo_id);
        var currentUserId = GetCurrentuserId();
        if(existing.UserId != Int32.Parse(currentUserId))
        return Unauthorized("you are not authorized");
        if (existing is null)
            return NotFound("No user found with given user number");

        var toUpdateTodolist = existing with
        {
            Description = Data.Description?.Trim() ?? existing.Description,
        };

        var didUpdate = await _todo.Update(toUpdateTodolist);

        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not update");
        return NoContent();
    }

    [HttpDelete("{todo_id}")]
    [Authorize]
    public async Task<ActionResult> DeleteTodo([FromRoute] long todo_id)
    {
        var existing = await _todo.GetById(todo_id);
        var currentUserId = GetCurrentuserId();
        if(existing.UserId != Int32.Parse(currentUserId))
        return Unauthorized("you are not authorized");
        if (existing is null)
         return NotFound("No user found with given user number");
       var didDelete = await _todo.Delete(todo_id);
        return NoContent();
    }

    private string GetCurrentuserId()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        var userClaim = identity.Claims;

        return (userClaim.FirstOrDefault(x =>x.Type == ClaimTypes.NameIdentifier) ?.Value);
    }
}