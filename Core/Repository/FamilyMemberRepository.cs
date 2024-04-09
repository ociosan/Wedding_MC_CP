using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Data.Dto;
using Data.Entities;

namespace Core.Repository
{
    public class FamilyMemberRepository : IFamilyMemberRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FamilyMemberRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;        
        }

        public async Task AddFamilyMemberAsync(FamilyMemberDto familyMemberDto)
        {
            await _unitOfWork.FamilyMember.CreateAsync(_mapper.Map<FamilyMember>(familyMemberDto));
            await _unitOfWork.SaveAsync();
        }

        public void Delete(FamilyMemberDto familyMemberDto)
        {
            _unitOfWork.FamilyMember.Delete(_mapper.Map<FamilyMember>(familyMemberDto));
        }
    }
}
