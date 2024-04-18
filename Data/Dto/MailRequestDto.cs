namespace Data.Dto
{
    public class MailRequestDto
    {
        public MailRequestDto(string toEmail, string subject, string body, Stream invitationAsJpg)
        {
            ToEmail = toEmail;
            Subject = subject;
            Body = body;
            InvitationAsJpg = invitationAsJpg;
        }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Stream InvitationAsJpg { get; set; }
     }
}
