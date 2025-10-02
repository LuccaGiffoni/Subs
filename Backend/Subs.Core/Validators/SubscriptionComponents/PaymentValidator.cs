using FluentValidation;
using Subs.Domain.Enums;
using Subs.Domain.Models.SubscriptionComponents;
using Subs.Utils.Extensions;

namespace Subs.Core.Validators.SubscriptionComponents;

public class PaymentValidator : AbstractValidator<Payment>
{
    public PaymentValidator()
    {
        RuleFor(x => x.Frequency)
            .NotNull()
            .WithMessage("Payment's frequency must not be null.")
            .IsInEnum()
            .WithMessage($"Frequency must be in enum. Options are: {string.Join(", ", EnumExtensions.GetAllNames<EPaymentFrequency>())}");

        RuleFor(x => x.Amount)
            .NotNull()
            .WithMessage("Payment's amount must not be null.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Payment's amount must not be less than 0.");

        When(x => x.Currency is not null, () =>
        {

        });
    }
}
