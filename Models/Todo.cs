using TodoTask.DTOs;

namespace TodoTask.Models;

public record Todo
{
    public long TodoId { get; set; }
    public String Title { get; set; }

    public String Description { get; set; }

    public long UserId { get; set; }

//     public TodoDTO asDto => new TodoDTO
//     {
//         TodoId = TodoId,
//         Description = Description,
//         Title = Title,
//         UserId = UserId,
//     };
 }
