using FluentValidation;
using FluentValidation.Results;
using MessagingService;

namespace StorageService.Grpc.Validation;

internal sealed class DeleteMessageRequestRpcValidator : AbstractValidator<DeleteMessageRequestRpc>
{
    public DeleteMessageRequestRpcValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty()
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("invalid message id");
    }

    protected override bool PreValidate(
        ValidationContext<DeleteMessageRequestRpc> context,
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