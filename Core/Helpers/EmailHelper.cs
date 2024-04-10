using Data.Dto;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using Core.Interfaces.Helper;

namespace Core.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        public void SendEmail(MailRequestDto mailRequestDto)
        {
            string mail = "";  //await _keyVaultManager.GetSecret(KeyVaultEnum.Mail);

            SmtpClient client = new SmtpClient();
            client.Host = ""; //await _keyVaultManager.GetSecret(KeyVaultEnum.Host);
            client.Port = 0; // int.Parse(await _keyVaultManager.GetSecret(KeyVaultEnum.Port));
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(mail, "" /*await _keyVaultManager.GetSecret(KeyVaultEnum.Password)*/);
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
