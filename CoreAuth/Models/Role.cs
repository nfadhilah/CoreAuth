using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CoreAuth.Models
{
  public class Role
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public Role Parent { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RoleApi> RoleApis { get; set; }
    public ICollection<Role> Childrens { get; set; }

    public Role()
    {
      RoleApis = new Collection<RoleApi>();
    }
  }
}
