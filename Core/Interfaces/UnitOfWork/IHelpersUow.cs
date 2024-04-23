using Core.Interfaces.Helper;

namespace Core.Interfaces.UnitOfWork
{
    public interface IHelpersUow
    {
        public IPdfHelper Pdf { get; }
        public IEmailHelper Email { get; }
        public IMessagesHelper Messages { get; }
    }
}
