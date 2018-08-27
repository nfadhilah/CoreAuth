using CoreAuth.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreAuth.Data
{
  public interface IAuthRepository
  {
    Task<User> Register(User user, string password);
    Task<User> Login(string username, string password);
    Task<bool> UserExists(string username);
    Task<List<Api>> GetApiAccByUserId(int userId, string method);
  }
}
