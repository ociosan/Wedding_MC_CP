namespace Core.Interfaces.Service
{
    public interface IConfirmAssistanceService
    {
        Task ConfirmAssistanceAsync(string eMailTo, string invitationCode);
        Task ReSendEmailAsync(string eMailTo, string invitationCode);
    }
}
