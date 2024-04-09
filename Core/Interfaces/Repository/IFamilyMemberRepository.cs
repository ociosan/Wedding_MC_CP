using Data.Dto;

namespace Core.Interfaces.Repository
{
    public interface IFamilyMemberRepository
    {
        Task AddFamilyMemberAsync(FamilyMemberDto familyMemberDto);
        void Delete(FamilyMemberDto familyMemberDto);
    }
}
