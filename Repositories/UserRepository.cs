using Dapper;
using TodoTask.Models;
using TodoTask.Utilities;

namespace TodoTask.Repositories;
public interface IUserRepository
{
    Task<User> Create(User Item);
    Task<bool> Update(User item);
    Task<User> GetById(long UserId);
   Task<User> GetByname(string name);

}
public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IConfiguration configuration) : base(configuration)
    {
    }
     
    public async Task<User> Create(User item)
    {


        var query = $@"INSERT INTO ""{TableNames.users}""
        (name,password)
        VALUES (@Name, @Password) RETURNING *";

        using (var con = NewConnection)
        {
            var res = await con.QuerySingleOrDefaultAsync<User>(query, item);
             return res;
        }

    }


    public async Task<User> GetById(long UserId)
    {
        var query = $@"SELECT * FROM users WHERE user_id = @UserId";

        using (var con = NewConnection)
        return await con.QuerySingleOrDefaultAsync<User>(query, new { UserId });


    }
     // public async Task<List<User>> GetList()
    // {
    //     var query = $@"SELECT * FROM ""{TableNames.users}""";
    //     using (var con = NewConnection);
    //          var res = (await con.QueryAsync<User>(query)).AsList();
    //     return res;
    // }
     public async Task<bool> Update(User item)
    {
        var query = $@"UPDATE ""{TableNames.users}"" SET Password = @password
         WHERE User_id = @UserId";

        using (var con = NewConnection)
        {
            var rowCount = await con.ExecuteAsync(query, item);
            return rowCount == 1;
        }
    }
        public async Task<User> GetByname(string name)
    {
        var query = $@"SELECT * FROM users WHERE name = @Name";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new {  name });


    } 
}
