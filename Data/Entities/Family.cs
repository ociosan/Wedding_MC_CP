using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Family : BaseEntity
    {
        [MaxLength(32)]
        public required string InvitationCode { get; set; }
        [MaxLength(256)]
        public required string LastName { get; set; }
        [MaxLength(256)]
        public string? EmailAddress { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public required ICollection<FamilyMember> FamilyMembers { get; set; }
    }
}
