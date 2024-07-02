namespace Core.Interfaces.Helper
{
    public interface IPdfHelper
    {
        Task MakePDF(string invitationCode, string lastName, List<string> members, byte[] invitationCodeTemplate);
        Task<byte[]> ConvertPdfToImage(string invitationCode);
    }
}
