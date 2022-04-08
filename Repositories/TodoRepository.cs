using Dapper;
using TodoTask.Models;
using TodoTask.Utilities;

namespace TodoTask.Repositories;
public interface ITodoRepository
{
    Task<Todo> Create(Todo Item);
    Task<bool> Update(Todo item);
    Task<bool> Delete(long TodoId);
    Task<Todo> GetById(long TodoId);
    Task<List<Todo>> GetList();
    Task<List<Todo>> GetUserTodos(long UserId);

}
public class TodoRepository : BaseRepository, ITodoRepository
{
    public TodoRepository(IConfiguration configuration) : base(configuration)
    {
    }
    public async Task<Todo> Create(Todo item)
    {
        var query = $@"INSERT INTO ""{TableNames.todo}""
        (todo_id,user_id,description,title)
        VALUES (@TodoId,  @UserId, @Description, @Title) RETURNING *";

        using (var con = NewConnection)
        {
            var res = await con.QuerySingleOrDefaultAsync<Todo>(query, item);

            return res;
        }

    }

    public async Task<bool> Delete(long TodoId)
    {
        var query = $@"DELETE FROM ""{TableNames.todo}""
        WHERE todo_id = @TodoId";

        using (var con = NewConnection)
        {
            var res = await con.ExecuteAsync(query, new { TodoId });
            return res > 0;
        }
    }

    public async Task<Todo> GetById(long TodoId)
    {
        var query = $@"SELECT * FROM ""{TableNames.todo}"" WHERE todo_id = @TodoId";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Todo>(query, new { TodoId });


    }
    public async Task<List<Todo>> GetList()
    {
        var query = $@"SELECT * FROM ""{TableNames.todo}""";
        List<Todo> res;
        using (var con = NewConnection)
            res = (await con.QueryAsync<Todo>(query)).AsList();
        return res;
    }

    public async Task<List<Todo>> GetUserTodos(long UserId)
    {
        var query = $@"SELECT * FROM ""{TableNames.todo}"" WHERE user_id = @UserId";

        using (var con = NewConnection)
         return (await con.QueryAsync<Todo>(query, new { UserId })).ToList();

    }

    public async Task<bool> Update(Todo item)
    {
        var query = $@"UPDATE ""{TableNames.todo}"" SET description = @Description, title = @Title
         WHERE todo_id = @TodoId";

        using (var con = NewConnection)
        {
            var rowCount = await con.ExecuteAsync(query, item);
            return rowCount == 1;
        }
    }
}