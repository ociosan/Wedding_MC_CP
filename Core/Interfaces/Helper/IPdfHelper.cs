namespace Core.Interfaces.Helper
{
    public interface IPdfHelper
    {
        Task MakePDF(string invitationCode, string lastName, List<string> members, byte[] invitationCodeTemplate);
        Task<Stream> ConvertPdfToImage(string invitationCode);
    }
}
