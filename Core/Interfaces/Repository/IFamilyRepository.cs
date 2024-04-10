using Data.Dto;

namespace Core.Interfaces.Repository
{
    public interface IFamilyRepository
    {
        Task<List<FamilyDto>> GetAllByEmailAddresAsync(string emailAddress);
        Task<List<FamilyDto>> GetAllByInvitationCodeAsync(string invitationCode);
        Task CreateAsync(NewFamilyDto familyDto);
        Task<FamilyDto> GetOneByInvitationCodeAsync(string invitationCode);
        void Update(FamilyDto familyDto);
    }
}
