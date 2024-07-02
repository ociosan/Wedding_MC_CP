using System.ComponentModel.DataAnnotations;

namespace Data.Dto
{
    public class EmailDto
    {
        public int Id { get; set; }
        public int FamilyId { get; set; }
        [MaxLength(250)]
        public string To { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateSent { get; set; }
    }
}
