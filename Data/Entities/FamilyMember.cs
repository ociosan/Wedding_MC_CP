using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class FamilyMember : BaseEntity
    {
        public required int FamilyId { get; set; }
        [MaxLength(256)]
        public required string Names { get; set; }
        public required bool IsKid { get; set; }
        public required Family Family { get; set; }
    }
}
