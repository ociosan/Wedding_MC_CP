using Data.Dto;

namespace Core.Interfaces.Helper
{
    public interface IMessagesHelper
    {
        Task SendWhatsappAsync(ConfirmAssitanceDto confirmAssitanceDto);
    }
}
