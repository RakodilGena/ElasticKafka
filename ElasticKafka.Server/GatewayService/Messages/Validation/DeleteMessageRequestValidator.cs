using FluentValidation;
using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Validation;

internal sealed class DeleteMessageRequestValidator : AbstractValidator<DeleteMessageRequest?>
{
    public DeleteMessageRequestValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("invalid or null request")
            
            .DependentRules(() =>
            {
                RuleFor(x => x!.MessageId)
                    .NotNull()
                    .NotEmpty()
                    .NotEqual(Guid.Empty)
                    .WithMessage("invalid message id");
            });
    }
}