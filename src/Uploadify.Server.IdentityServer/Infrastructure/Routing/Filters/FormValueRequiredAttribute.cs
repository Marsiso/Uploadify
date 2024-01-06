using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using static System.String;

namespace Uploadify.Server.IdentityServer.Infrastructure.Routing.Filters;

public class FormValueRequiredAttribute : ActionMethodSelectorAttribute
{
    private readonly string _name;

    public FormValueRequiredAttribute(string name)
    {
        _name = name;
    }

    public override bool IsValidForRequest(RouteContext context, ActionDescriptor action)
    {
        if (HttpMethods.Get.Equals(context.HttpContext.Request.Method) ||
            HttpMethods.Head.Equals(context.HttpContext.Request.Method) ||
            HttpMethods.Delete.Equals(context.HttpContext.Request.Method) ||
            HttpMethods.Trace.Equals(context.HttpContext.Request.Method) ||
            IsNullOrEmpty(context.HttpContext.Request.ContentType) ||
            !context.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded"))
        {
            return false;
        }

        return !IsNullOrEmpty(context.HttpContext.Request.Form[_name]);
    }
}
