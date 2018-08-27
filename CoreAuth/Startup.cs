using CoreAuth.Data;
using CoreAuth.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace CoreAuth
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DataContext>(config => config.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      services.AddTransient<Seeder>();

      services.AddTransient<IAuthRepository, AuthRepository>();

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddTransient<IAuthorizationHandler, ApiAccessHandler>();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
              new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenOptions:Key").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
          };
        });

      services.AddMvc(config =>
        {
          var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new ApiAccessRequirement())
            .Build();

          config.Filters.Add(new AuthorizeFilter(policy));
        })
        .AddJsonOptions(options =>
        {
          options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
          options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        })
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seeder seed)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      seed.SeedUsers();

      app.UseAuthentication();

      app.UseMvc();
    }
  }
}
