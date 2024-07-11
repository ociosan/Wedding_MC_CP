using System.Reflection;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ConvertApiDotNet;
using Core.Enum;
using Core.Interfaces.Helper;
using Core.Interfaces.UnitOfWork;
using System.IO;

namespace Core.Helpers
{
    public class PdfHelper : IPdfHelper
    {
        private readonly IAzureUow _azureUow;

        public PdfHelper(IAzureUow azureUow)
        {
            _azureUow = azureUow;
        }

        public async Task MakePDF(string invitationCode, string lastName, List<string> members, byte[] invitationCodeTemplate)
        {
            if(await _azureUow.StorageAccount.FileExistsAsync(invitationCode, FileTypeEnum.Pdf))
                return;

            string familyInvitationFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"Invitations\\{invitationCode}.pdf"); //CREATE NEW FILE
            // Creating watermark on a separate layer
            // Creating iTextSharp.text.pdf.PdfReader object to read the Existing PDF Document
            PdfReader reader1 = new PdfReader(invitationCodeTemplate);
            using (FileStream fs = new FileStream(familyInvitationFile, FileMode.Create, FileAccess.Write, FileShare.Delete))
            // Creating iTextSharp.text.pdf.PdfStamper object to write Data from iTextSharp.text.pdf.PdfReader object to FileStream object
            using (PdfStamper stamper = new PdfStamper(reader1, fs))
            {
                // Getting total number of pages of the Existing Document
                int pageCount = reader1.NumberOfPages;

                // Create New Layer for Watermark
                PdfLayer layer = new PdfLayer("Layer", stamper.Writer);
                // Loop through each Page
                for (int i = 1; i <= pageCount; i++)
                {
                    // Getting the Page Size
                    Rectangle rect = reader1.GetPageSize(i);

                    // Get the ContentByte object
                    PdfContentByte cb = stamper.GetOverContent(i);

                    // Tell the cb that the next commands should be "bound" to this new layer
                    cb.BeginLayer(layer);

                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb.SetColorFill(new BaseColor(0, 33, 71));
                    cb.SetFontAndSize(bf, 11);
                    cb.SetWordSpacing(3);

                    cb.BeginText();
                    cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, invitationCode, 92, 208, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, lastName, 200, 208, 0);


                    //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, $"width: {rect.Width} - Height: {rect.Height}" , 50, 50, 0);

                    int firstPos = 162;
                    foreach (string member in members)
                    {
                        cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, member, 148, firstPos - 14, 0);
                        firstPos -= 14;
                    }

                    cb.EndText();

                    // Close the layer
                    cb.EndLayer();
                }
            }
            await using (Stream fileStream = File.OpenRead(familyInvitationFile))
            {
                await _azureUow.StorageAccount.UploadInvitationCodeAsync(fileStream, invitationCode, FileTypeEnum.Pdf);
                File.Delete(familyInvitationFile);
            }
        }

        public async Task<byte[]> ConvertPdfToImage(string invitationCode)
        {

            if (await _azureUow.StorageAccount.FileExistsAsync(invitationCode, FileTypeEnum.Jpg))
                return await _azureUow.StorageAccount.DownloadInvitationAsync(invitationCode, FileTypeEnum.Jpg);

            //Download the new pdf that was created
            byte[] invitationAsPdf = await _azureUow.StorageAccount.DownloadInvitationAsync(invitationCode, FileTypeEnum.Pdf);
            Stream outputJpgFile;

            await using (Stream stream = new MemoryStream(invitationAsPdf))
            {
                var convertApi = new ConvertApi(await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.ConvertApiSecret));
                var convertToJPG = await convertApi.ConvertAsync(fromFormat: FileTypeEnum.Pdf, toFormat: FileTypeEnum.Jpg, new ConvertApiFileParam(stream, $"{invitationCode}.{FileTypeEnum.Pdf}"));
                outputJpgFile = await convertToJPG.Files[0].FileStreamAsync();
            }

            //upload the new jpg image to the storage account
            await _azureUow.StorageAccount.UploadInvitationCodeAsync(outputJpgFile, invitationCode, FileTypeEnum.Jpg);
            return await _azureUow.StorageAccount.DownloadInvitationAsync(invitationCode, FileTypeEnum.Jpg);
        }
    }
}
