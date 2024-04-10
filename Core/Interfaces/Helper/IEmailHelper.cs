using Data.Dto;

namespace Core.Interfaces.Helper
{
    public interface IEmailHelper
    {
        void SendEmail(MailRequestDto mailRequestDto);
    }
}
