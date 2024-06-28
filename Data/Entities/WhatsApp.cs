using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class WhatsApp : BaseEntity
    {
        public int FamilyId { get; set; }
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateSent { get; set; }

        public Family Family { get; set; }
    }
}
