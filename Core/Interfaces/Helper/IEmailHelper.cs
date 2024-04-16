using Data.Dto;

namespace Core.Interfaces.Helper
{
    public interface IEmailHelper
    {
        Task SendEmailAsync(MailRequestDto mailRequestDto);
    }
}
