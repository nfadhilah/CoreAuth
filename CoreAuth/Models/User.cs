using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreAuth.Models
{
  public class User
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
  }
}
