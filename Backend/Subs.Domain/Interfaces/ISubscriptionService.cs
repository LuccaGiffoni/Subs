using Subs.Domain.Models;

namespace Subs.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        public Subscription Handle(Subscription subscription);
    }
}