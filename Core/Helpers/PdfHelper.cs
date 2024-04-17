using Azure.Interfaces.Repository;
using System.Reflection;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ConvertApiDotNet;
using Azure.Enum;
using Core.Interfaces.Helper;

namespace Core.Helpers
{
    public class PdfHelper : IPdfHelper
    {
        private readonly IKeyVaultRepository _keyVaultRepository;
        private readonly IStorageAccountRepository _storageAccountRepository;

        public PdfHelper(IKeyVaultRepository keyVaultRepository, IStorageAccountRepository storageAccountRepository)
        {
            _keyVaultRepository = keyVaultRepository;
            _storageAccountRepository = storageAccountRepository;
        }

        public async Task<string> MakePDF(string invitationCode, string lastName, List<string> members, byte[] invitationCodeTemplate)
        {
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
                    iTextSharp.text.Rectangle rect = reader1.GetPageSize(i);

                    // Get the ContentByte object
                    PdfContentByte cb = stamper.GetOverContent(i);

                    // Tell the cb that the next commands should be "bound" to this new layer
                    cb.BeginLayer(layer);

                    BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb.SetColorFill(new BaseColor(0, 33, 71));
                    cb.SetFontAndSize(bf, 11);
                    cb.SetWordSpacing(3);

                    cb.BeginText();
                    cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, invitationCode, 119, 243, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, lastName, 53, 194, 0);


                    //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, $"width: {rect.Width} - Height: {rect.Height}" , 50, 50, 0);

                    int firstPos = 208;
                    foreach (string member in members)
                    {
                        cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, member, 185, firstPos - 14, 0);
                        firstPos = firstPos - 14;
                    }

                    var anchor = new Chunk("View our Portfolio")
                    {
                        Font = new Font(
                            Font.FontFamily.HELVETICA, 25,
                            Font.NORMAL,
                            BaseColor.BLUE
                        )
                    };

                    // [2] set the anchor URL
                    anchor.SetAnchor("http://portfolio.xxxxx.com/");

                    cb.EndText();

                    // Close the layer
                    cb.EndLayer();
                }
            }


            await using (Stream fileStream = File.OpenRead(familyInvitationFile))
            {
                await _storageAccountRepository.UploadInvitationCodeAsync(fileStream, invitationCode);
                File.Delete(familyInvitationFile);
            }


            return familyInvitationFile;
        }

        public async Task<string> ConvertPdfToImage(string sourceFilePath, string invitationCode)
        {
            string destinationFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"Invitations\\");
            string destinationFileName = destinationFilePath + invitationCode + ".jpg";

            using (FileStream fs = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                MemoryStream pdfMs = new MemoryStream();
                fs.CopyTo(pdfMs);

                Stream stream = new MemoryStream(pdfMs.ToArray());

                var convertApi = new ConvertApi(await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.ConvertApiSecret));

                //PDF to JPG  Read more https://www.convertapi.com/html-to-pdf
                var convertToJPG = await convertApi.ConvertAsync("pdf", "jpg",
                    new ConvertApiFileParam(stream, invitationCode + ".pdf")
                );

                var outputStream = await convertToJPG.Files[0].FileStreamAsync();
                using (FileStream outputFileStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.Delete))
                {
                    outputStream.CopyTo(outputFileStream);
                }
            }

            return destinationFileName;
        }

    }
}
