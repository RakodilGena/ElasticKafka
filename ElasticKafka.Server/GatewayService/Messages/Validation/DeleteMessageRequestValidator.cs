﻿using FluentValidation;
using FluentValidation.Results;
using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Validation;

internal sealed class DeleteMessageRequestValidator : AbstractValidator<DeleteMessageRequest>
{
    public DeleteMessageRequestValidator()
    {
        RuleFor(x => x!.MessageId)
            .NotNull()
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("invalid message id");
    }

    protected override bool PreValidate(
        ValidationContext<DeleteMessageRequest> context,
        ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("", "invalid or null request"));
            return false;
        }

        return base.PreValidate(context, result);
    }
}