using CleanArchitecture.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Infrastructure.Authentication;

public sealed class RoleFilterAttribute : TypeFilterAttribute
{
    public RoleFilterAttribute(string role) : base(typeof(RoleAttribute))
    {
        Arguments = new object[] { role };
    }
}
