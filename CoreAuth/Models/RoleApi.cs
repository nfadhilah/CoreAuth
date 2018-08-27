namespace CoreAuth.Models
{
  public class RoleApi
  {
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public int ApiId { get; set; }
    public Api Api { get; set; }
  }
}