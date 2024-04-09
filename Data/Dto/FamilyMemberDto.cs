namespace Data.Dto
{
    public class FamilyMemberDto
    {
        public int Id { get; set; }
        public int? FamilyId { get; set; }
        public required string Names { get; set; }
        public bool IsKid { get; set; }
    }

    public class NewFamilyMemberDto
    {
        public required string Names { get; set; }
        public bool IsKid { get; set; }
    }
}
