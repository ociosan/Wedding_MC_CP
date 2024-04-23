using Data.Dto;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using Core.Interfaces.Helper;
using Core.Enum;
using Core.Interfaces.UnitOfWork;

namespace Core.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IAzureUow _azureUow;

        public EmailHelper(IAzureUow azureUow)
        {
            _azureUow = azureUow;
        }

        public async Task SendEmailAsync(MailRequestDto mailRequestDto)
        {
            string mail = await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.Mail);

            SmtpClient client = new SmtpClient();
            client.Host = await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.Host);
            client.Port = int.Parse(await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.Port));
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(mail, await _azureUow.KeyVault.GetSecretAsync(KeyVaultSecretsEnum.Password));
            client.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.To.Add(mailRequestDto.ToEmail);
            message.From = new MailAddress(mail);
            message.Subject = mailRequestDto.Subject;
            message.IsBodyHtml = true;
            message.Body = mailRequestDto.Body;

            Attachment imageAttachment = new Attachment(mailRequestDto.InvitationAsJpg, MediaTypeNames.Image.Jpeg);
            imageAttachment.ContentId = "image1";
            imageAttachment.ContentDisposition.Inline = true;

            message.Attachments.Add(imageAttachment);

            await client.SendMailAsync(message);
        }
    }
}
