using FluentValidation;
using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Validation;

internal sealed class SearchMessagesRequestValidator : AbstractValidator<SearchMessagesRequest?>
{
    public SearchMessagesRequestValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("invalid or null request")
            
            .DependentRules(() =>
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
            });
    }
}