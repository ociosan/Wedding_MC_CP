using Data.Entities;

namespace Data.Dto
{
    public class FamilyDto
    {
        public int Id { get; set; }
        public required string InvitationCode { get; set; }
        public required string LastName { get; set; }
        public required string EmailAddress { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public ICollection<FamilyMemberDto> FamilyMembers { get; set; }
    }

    public class NewFamilyDto
    {
        public required string InvitationCode { get; set; }
        public required string LastName { get; set; }
        public string? EmailAddress { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public required ICollection<NewFamilyMemberDto> FamilyMembers { get; set; }
    }
}
