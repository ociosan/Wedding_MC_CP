using Data.Dto;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using Core.Interfaces.Helper;
using Azure.Interfaces.Repository;
using Azure.Enum;

namespace Core.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IKeyVaultRepository _keyVaultRepository;

        public EmailHelper(IKeyVaultRepository keyVaultRepository)
        {
            _keyVaultRepository = keyVaultRepository;
        }

        public async Task SendEmailAsync(MailRequestDto mailRequestDto)
        {
            string mail = await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Mail);

            SmtpClient client = new SmtpClient();
            client.Host = await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Host);
            client.Port = int.Parse(await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Port));
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(mail, await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Password));
            client.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.To.Add(mailRequestDto.ToEmail);
            message.From = new MailAddress(mail);
            message.Subject = mailRequestDto.Subject;
            message.IsBodyHtml = true;
            message.Body = mailRequestDto.Body;

            Attachment imageAttachment = new Attachment(mailRequestDto.InvitationFilePath, MediaTypeNames.Image.Jpeg);
            imageAttachment.ContentId = "image1";
            imageAttachment.ContentDisposition.Inline = true;

            message.Attachments.Add(imageAttachment);

            client.Send(message);
            imageAttachment.Dispose();
        }
    }
}
