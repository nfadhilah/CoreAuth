using CoreAuth.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreAuth.Helpers;
using UserRole = CoreAuth.Models.UserRole;

namespace CoreAuth.Data
{
  public class Seeder
  {
    private readonly DataContext _context;

    public Seeder(DataContext context)
    {
      _context = context;
    }

    public void SeedUsers()
    {
      var users = new List<User>
      {
        new User {Name = "mono"},
        new User {Name = "hani"},
        new User {Name = "habibie"}
      };

      var roles = new List<Role>
      {
        new Role {Name = "administrator"},
        new Role {Name = "operator"}
      };

      var apis = new List<Api>
      {
        new Api{Path = "/api/values", Method = "GET"},
        new Api{Path = "/api/values/{param}", Method = "GET"},
        new Api{Path = "/api/values", Method = "POST"}
      };

      var mappingUserRole = new Dictionary<string, string>
      {
        ["hani"] = "administrator",
        ["mono"] = "administrator",
        ["habibie"] = "operator",
      };

      if (!_context.Apis.Any())
      {
        foreach (var api in apis)
        {
          _context.Apis.Add(api);
        }

        _context.SaveChanges();
      }


      foreach (var role in roles)
      {
        if (_context.Roles.Any(r => r.Name == role.Name)) continue;

        var apiFromDb = _context.Apis.ToList();

        foreach (var api in apiFromDb)
        {
          if (role.Name == "administrator" && !api.Path.Contains("{param}"))
          {
            role.RoleApis.Add(new RoleApi { ApiId = api.Id, RoleId = role.Id });
          }
          else
          {
            if (api.Method == "GET")
              role.RoleApis.Add(new RoleApi { ApiId = api.Id, RoleId = role.Id });
          }
        }

        _context.Roles.Add(role);

        _context.SaveChanges();
      }


      foreach (var user in users)
      {
        if (_context.Users.Any(u => u.Name == user.Name)) return;

        var role = _context.Roles
          .FirstOrDefault(r => r.Name == mappingUserRole[user.Name]);

        if (role == null) continue;

        user.PasswordHash = Hasher.Create("123456");

        _context.Users.Add(user);

        user.UserRoles = new List<UserRole>
        {
          new UserRole {UserId = user.Id, RoleId = role.Id}
        };

        _context.SaveChanges();

      }
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
      }
    }
  }

}
