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
        [MaxLength(12)]
        public string? PhoneNumber { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public required IEnumerable<FamilyMember> FamilyMembers { get; set; } = new HashSet<FamilyMember>();
        //public ICollection<WhatsApp> WhatsApps { get; set; } = new HashSet<WhatsApp>();

    }
}
