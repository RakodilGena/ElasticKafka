﻿using FluentValidation;
using GatewayService.Messages.Mapping;
using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;
using GatewayService.Messages.Services;
using GatewayService.Messages.Validation;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Messages.Controllers;

public partial class MessageController
{
    [HttpDelete]
    public async Task<ActionResult<DeleteMessageResponse>> DeleteMessageAsync(
        [FromBody] DeleteMessageRequest request,
        [FromServices] IDeleteMessageService service)
    {
        var validator = new DeleteMessageRequestValidator();
        await validator.ValidateAndThrowAsync(request);

        var requestDto = request.ToDto();

        return await service.DeleteMessageAsync(requestDto);
    }
}