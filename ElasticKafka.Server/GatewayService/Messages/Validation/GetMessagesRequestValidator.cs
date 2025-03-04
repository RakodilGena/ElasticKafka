using FluentValidation;
using FluentValidation.Results;
using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Validation;

internal sealed class GetMessagesRequestValidator : AbstractValidator<GetMessagesRequest>
{
    public GetMessagesRequestValidator()
    {
        RuleFor(x => x!.Count)
            .GreaterThan(0)
            .WithMessage("invalid count");

        RuleFor(x => x!.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("invalid offset");
    }

    protected override bool PreValidate(
        ValidationContext<GetMessagesRequest> context,
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