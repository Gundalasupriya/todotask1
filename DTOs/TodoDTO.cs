using System.Text.Json.Serialization;

namespace TodoTask.DTOs;

public record TodoDTO
{
    [JsonPropertyName("todo_id")]

    public long TodoId { get; set; }

    [JsonPropertyName("title")]
    public String Title { get; set; }

    [JsonPropertyName("description")]

    public String Description { get; set; }

    [JsonPropertyName("user_id")]


    public long UserId { get; set; }
}
public record TodoCreateDTO
{
    [JsonPropertyName("title")]
    public String Title { get; set; }

    [JsonPropertyName("description")]
    public String Description { get; set; }

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

}

public record TodoUpdateDTO
{
    [JsonPropertyName("title")]
    public String Title { get; set; }

    [JsonPropertyName("description")]
    public String Description { get; set; }
}