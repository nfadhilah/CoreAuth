using System.Collections.Generic;

namespace CoreAuth.Models
{
  public class Api
  {
    public int Id { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public ICollection<RoleApi> RoleApis { get; set; }
  }
}
