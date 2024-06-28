using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Email : BaseEntity
    {
        public int FamilyId { get; set; }
        [MaxLength(250)]
        public string To { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateSent { get; set; }

        public Family Family { get; set; }
    }
}
