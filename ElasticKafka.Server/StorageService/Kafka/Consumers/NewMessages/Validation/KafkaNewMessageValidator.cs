using FluentValidation;
using StorageService.Kafka.Consumers.NewMessages.Models;

namespace StorageService.Kafka.Consumers.NewMessages.Validation;

internal sealed class KafkaNewMessageValidator : AbstractValidator<KafkaNewMessage>
{
    public KafkaNewMessageValidator()
    {
        RuleFor(x => x!.Text)
            .NotEmpty()
            .WithMessage("new message text should not be empty");
    }

    //new message def. not null.
    // protected override bool PreValidate(
    //     ValidationContext<KafkaNewMessage> context,
    //     ValidationResult result)
    // {
    //     if (context.InstanceToValidate == null)
    //     {
    //         result.Errors.Add(new ValidationFailure("", "new message should not be null"));
    //         return false;
    //     }
    //
    //     return base.PreValidate(context, result);
    // }
}