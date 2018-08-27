using CoreAuth.Helpers;
using CoreAuth.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAuth.Data
{
  public class AuthRepository : IAuthRepository
  {
    private readonly string _connectionString;

    public AuthRepository(IConfiguration config)
    {
      _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public IDbConnection Connection => new SqlConnection(_connectionString);

    public async Task<User> Register(User user, string password)
    {
      user.PasswordHash = Hasher.Create(password);

      using (var conn = Connection)
      {
        conn.Open();

        var affectedRow = await conn
          .ExecuteAsync("INSERT INTO Users VALUES (@Name, @PasswordHash)",
            new { user.Name, user.PasswordHash });

        var userCreated = await conn
          .QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Name = @Name", new { user.Name });

        return userCreated;
      }
    }

    public async Task<User> Login(string username, string password)
    {
      using (var conn = Connection)
      {
        conn.Open();

        var user = await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Name = @Name",
          new { Name = username });

        if (user == null) return null;

        return !Hasher.Validate(user.PasswordHash, password) ? null : user;
      }
    }


    public async Task<bool> UserExists(string username)
    {
      using (var conn = Connection)
      {
        conn.Open();

        var users = await conn.QuerySingleOrDefaultAsync("SELECT * FROM Users WHERE Name = @Name",
          new { Name = username });

        if (users == null) return false;
      }

      return true;
    }

    public async Task<List<Api>> GetApiAccByUserId(int userId, string method)
    {
      using (var conn = Connection)
      {
        conn.Open();

        var api = await conn.QueryAsync<Api>(@"SELECT a.* FROM RoleApi AS ra 
              INNER JOIN Apis AS a
              ON a.Id = ra.ApiId
              INNER JOIN UserRole AS ur
              ON ur.RoleId = ra.RoleId
              WHERE ur.UserId = @UserId AND UPPER(a.Method) = @Method",
          new { UserId = userId, Method = method });

        return api.ToList();
      }
    }
  }
}