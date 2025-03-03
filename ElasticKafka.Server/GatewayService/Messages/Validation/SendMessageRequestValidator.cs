using FluentValidation;
using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Validation;

internal sealed class SendMessageRequestValidator : AbstractValidator<SendMessageRequest?>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("invalid or null request")
            
            .DependentRules(() =>
            {
                RuleFor(x => x!.MessageText)
                    .NotEmpty()
                    .WithMessage("empty message text");

                RuleFor(x => x!.SentAt)
                    .NotNull()
                    .NotEqual(new DateTimeOffset())
                    .WithMessage("empty 'SentAt' timestamp");
            });
    }
}