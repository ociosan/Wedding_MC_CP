namespace Data.Dto
{
    public class MailRequestDto
    {
        public MailRequestDto(string toEmail, string subject, string body, string invitationFilePath)
        {
            ToEmail = toEmail;
            Subject = subject;
            Body = body;
            InvitationFilePath = invitationFilePath;
        }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string InvitationFilePath { get; set; }
     }
}
