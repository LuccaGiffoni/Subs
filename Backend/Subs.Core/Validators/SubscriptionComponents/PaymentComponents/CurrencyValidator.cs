using FluentValidation;
using Subs.Domain.Models.SubscriptionComponents.PaymentComponents;

namespace Subs.Core.Validators.SubscriptionComponents.PaymentComponents;

public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator()
    {
        RuleFor(x => x.Rate)
            .GreaterThan(0)
            .WithMessage("Discount must be greater than 0.");

        // Add options from database to check against
        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage("Currency type must not be null.");
            //.WithMessage("Invalid currency type.")
    }
}