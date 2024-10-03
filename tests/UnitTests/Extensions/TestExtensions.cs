using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagementSystem.UnitTests.Extensions
{
    public static class TestExtensions
    {
        public static void InitializeClaims(this ControllerBase controller, params Claim[] claims)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "fake auth"))
            };
        }
    }
}
