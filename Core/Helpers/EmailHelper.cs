using Data.Dto;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using Core.Interfaces.Helper;
using Azure.Interfaces.Repository;
using Core.Enum;

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
            string mail = "nuestraboda@mayra-y-carlos.com";//await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Mail);

            SmtpClient client = new SmtpClient();
            client.Host = "smtpout.secureserver.net";//await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Host);
            client.Port = 587;//int.Parse(await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Port));
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(mail, "M4yr4IC4rl0s.c0m" /*await _keyVaultRepository.GetSecretAsync(KeyVaultSecretsEnum.Password)*/);
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

            client.Send(message);
        }
    }
}
