using FluentValidation;
using FluentValidation.Results;
using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Validation;

internal sealed class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x!.MessageText)
            .NotEmpty()
            .WithMessage("empty message text");

        RuleFor(x => x!.SentAt)
            .NotNull()
            .NotEqual(new DateTimeOffset())
            .WithMessage("empty 'SentAt' timestamp");
    }

    protected override bool PreValidate(
        ValidationContext<SendMessageRequest> context,
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