using FluentValidation;
using Subs.Core.Validators.SubscriptionComponents;
using Subs.Domain.Models;

namespace Subs.Core.Validators;

public class SubscriptionValidator : AbstractValidator<Subscription>
{
    public SubscriptionValidator()
    {
        RuleFor(s => s.Client)
            .NotNull()
            .WithMessage("Client information is required.")
            .SetValidator(new ClientValidator());

        RuleFor(s => s.Payment)
            .NotNull()
            .WithMessage("Payment information is required.")
            .SetValidator(new PaymentValidator());

        RuleFor(s => s.Status)
            .IsInEnum()
            .WithMessage("Invalid subscription status.");

        RuleFor(s => s.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(s => s.CreatedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("CreatedAt cannot be in the future.");

        RuleFor(s => s.UpdatedAt)
            .GreaterThanOrEqualTo(s => s.CreatedAt)
            .WithMessage("UpdatedAt cannot be earlier than CreatedAt.");
    }
}