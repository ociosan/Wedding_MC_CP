namespace Core.Interfaces.Service
{
    public interface IConfirmAssistanceService
    {
        Task ConfirmAssistanceAsync(string eMailTo, string invitationCode);
    }
}
