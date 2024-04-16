namespace Core.Interfaces.Helper
{
    public interface IPdfHelper
    {
        string MakePDF(string invitationCode, string lastName, List<string> members);
        Task<string> ConvertPdfToImage(string sourceFilePath, string invitationCode);
    }
}
