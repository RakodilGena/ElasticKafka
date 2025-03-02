using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GatewayService.ExceptionFilters;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class GatewayExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        //var route = context.HttpContext.Request.Path.ToString();

        var resp = new
        {
            ExMessage = GetExMessage(context.Exception)
        };

        context.Result = new ObjectResult(resp)
            { StatusCode = StatusCodes.Status500InternalServerError };
        context.ExceptionHandled = true;
    }

    private static string GetExMessage(Exception ex)
    {
        if (ex is not RpcException rpcEx)
            return ex.Message;

        return rpcEx.Status.Detail;
    }
}
