using CoreAuth.Data;
using CoreAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreAuth.Filters
{
  public class ApiAccessRequirement : IAuthorizationRequirement
  {
  }

  public class ApiAccessHandler : AuthorizationHandler<ApiAccessRequirement>
  {
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAuthRepository _authRepository;

    public ApiAccessHandler(IHttpContextAccessor contextAccessor, IAuthRepository authRepository)
    {
      _contextAccessor = contextAccessor;
      _authRepository = authRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAccessRequirement requirement)
    {
      var nameIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (int.TryParse(nameIdClaim, out var userId))
      {
        var reqPath = _contextAccessor.HttpContext.Request.Path.Value;

        var reqMethod = _contextAccessor.HttpContext.Request.Method;

        var apiAcc = await _authRepository.GetApiAccByUserId(userId, reqMethod);

        if (apiAcc.Count > 0 && ValidateUri(apiAcc, reqPath))
        {
          context.Succeed(requirement);
        }
      }
    }

    private static bool ValidateUri(IEnumerable<Api> apiEnums, string reqPath)
    {
      var isAuthenticated = false;

      foreach (var api in apiEnums)
      {
        var arrReqPath = reqPath.Split('/');

        var arrApiPath = api.Path.Split('/');

        if (arrReqPath.Length != arrApiPath.Length) continue;

        for (var i = 0; i < arrReqPath.Length; i++)
        {
          switch (arrApiPath[i])
          {
            case "{param}":
              if (int.TryParse(arrReqPath[i], out _) || !string.IsNullOrWhiteSpace(arrReqPath[i]))
                isAuthenticated = true;
              break;
            default:
              if (arrApiPath[i] == arrReqPath[i])
                isAuthenticated = true;
              break;
          }
        }

        if (isAuthenticated) break;
      }

      return isAuthenticated;
    }
  }
}
