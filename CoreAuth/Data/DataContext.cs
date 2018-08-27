using CoreAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreAuth.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> options) :
      base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Api> Apis { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<UserRole>(userRole =>
      {
        userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

        userRole.HasOne(u => u.User)
          .WithMany(ur => ur.UserRoles)
          .HasForeignKey(ur => ur.UserId);

        userRole.HasOne(r => r.Role)
          .WithMany(ur => ur.UserRoles)
          .HasForeignKey(ur => ur.RoleId);
      });

      builder.Entity<Role>()
        .HasMany(r => r.Childrens)
        .WithOne(r => r.Parent)
        .OnDelete(DeleteBehavior.Restrict);

      builder.Entity<RoleApi>(roleApi =>
      {
        roleApi.HasKey(ra => new { ra.RoleId, ra.ApiId });

        roleApi.HasOne(ra => ra.Role)
          .WithMany(r => r.RoleApis)
          .HasForeignKey(ra => ra.RoleId);

        roleApi.HasOne(ra => ra.Api)
          .WithMany(r => r.RoleApis)
          .HasForeignKey(ra => ra.ApiId);
      });
    }
  }
}
