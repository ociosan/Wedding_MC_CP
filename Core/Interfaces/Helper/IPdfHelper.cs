namespace Core.Interfaces.Helper
{
    public interface IPdfHelper
    {
        Task<string> MakePDF(string invitationCode, string lastName, List<string> members, byte[] invitationCodeTemplate);
        Task<string> ConvertPdfToImage(string sourceFilePath, string invitationCode);
    }
}
