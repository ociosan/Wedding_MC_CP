using Core.Helpers;
using Core.Interfaces.Helper;
using Core.Interfaces.UnitOfWork;

namespace Core.UnitOfWork
{
    public class HelpersUow : IHelpersUow
    {
        private readonly IAzureUow _azureUow;
        private IPdfHelper _pdfHelper;
        private IEmailHelper _emailHelper;
        private IMessagesHelper _messagesHelper;

        public HelpersUow(IAzureUow azureUow)
        {
            _azureUow = azureUow;
        }

        public IPdfHelper Pdf => _pdfHelper ??= new PdfHelper(_azureUow);
        public IEmailHelper Email => _emailHelper ??= new EmailHelper(_azureUow);
        public IMessagesHelper Messages => _messagesHelper ??= new MessagesHelper(_azureUow);
    }
}
