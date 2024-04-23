using Data.Dto;

namespace Core.Interfaces.Service
{
    public interface IConfirmAssistanceService
    {
        Task ConfirmAssistanceAsync(ConfirmAssitanceDto confirmAssitanceDto);
        Task ReSendEmailAsync(ConfirmAssitanceDto confirmAssitanceDto);
    }
}
