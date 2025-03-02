using GatewayService.ExceptionFilters;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Messages.Controllers;

[Route("api/messages")]
[GatewayExceptionFilter]
public sealed partial class MessageController : ControllerBase;