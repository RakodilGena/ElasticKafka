﻿using FluentValidation;
using FluentValidation.Results;
using MessagingService;

namespace StorageService.Grpc.Validation;

internal sealed class GetMessagesRequestRpcValidator : AbstractValidator<GetMessagesRequestRpc>
{
    public GetMessagesRequestRpcValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0)
            .WithMessage("invalid count");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("invalid offset");
    }

    protected override bool PreValidate(
        ValidationContext<GetMessagesRequestRpc> context,
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