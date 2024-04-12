using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Data.Dto;
using Data.Entities;

namespace Core.Repository
{
    public class FamilyRepository : IFamilyRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private List<string> _tables;

        public FamilyRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tables = new List<string>() { "FamilyMembers" };
        }

        public async Task<List<FamilyDto>> GetAllByEmailAddresAsync(string emailAddress)
        {
            return _mapper.Map<List<FamilyDto>>(await _unitOfWork.Family.FindAllAsync(x => x.EmailAddress == emailAddress, _tables));
        }

        public async Task<List<FamilyDto>> GetAllByInvitationCodeAsync(string invitationCode)
        {
            return _mapper.Map<List<FamilyDto>>(await _unitOfWork.Family.FindAllAsync(x => x.InvitationCode == invitationCode, _tables));
        }

        public async Task<FamilyDto> GetOneByInvitationCodeAsync(string invitationCode)
        {
            return _mapper.Map<FamilyDto>(await _unitOfWork.Family.FindOneAsync(x => x.InvitationCode == invitationCode, _tables));
        }

        public async Task CreateAsync(NewFamilyDto familyDto)
        {
            await _unitOfWork.Family.CreateAsync(_mapper.Map<Family>(familyDto));
            await _unitOfWork.SaveAsync();
        }

        public void Update(FamilyDto familyDto)
        {
            _unitOfWork.Family.Update(_mapper.Map<Family>(familyDto));
            _unitOfWork.Save();
        }

        public async Task<FamilyDto> GetOneByInvitationCode(string invitationCode)
        {
            return _mapper.Map<FamilyDto>(await _unitOfWork.Family.FindOneAsync(x => x.InvitationCode == invitationCode));
        }

    }
}
