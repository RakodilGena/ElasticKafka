﻿using FluentValidation;
using FluentValidation.Results;
using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Validation;

internal sealed class SearchMessagesRequestValidator : AbstractValidator<SearchMessagesRequest>
{
    public SearchMessagesRequestValidator()
    {
        RuleFor(x => x!.Count)
            .GreaterThan(0)
            .WithMessage("invalid count");

        RuleFor(x => x!.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("invalid offset");

        RuleFor(x => x!.Filter)
            .NotEmpty()
            .WithMessage("invalid search filter");
    }

    protected override bool PreValidate(
        ValidationContext<SearchMessagesRequest> context,
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