using Data.Dto;
using Wedding.Api;

namespace UI.Models
{
    public class FamilyModel
    {
        public FamilyDto Family { get; set; }

        public FamilyModel(FamilyDto family)
        {
            Family = family;
        }
    }
}
