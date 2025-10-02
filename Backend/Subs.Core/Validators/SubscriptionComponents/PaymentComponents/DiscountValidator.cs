using FluentValidation;
using Subs.Domain.Models.SubscriptionComponents.PaymentComponents;

namespace Subs.Core.Validators.SubscriptionComponents.PaymentComponents;

public class DiscountValidator : AbstractValidator<Discount>
{
    public DiscountValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount must be greater than or equal to 0.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid discount type.");
    }
}