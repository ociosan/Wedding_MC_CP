
using AutoMapper;
using Data.Dto;
using Data.Entities;

namespace Core.AutoMapperProfile
{
    public class FamilyMemberProfile : Profile
    {
        public FamilyMemberProfile()
        {
            CreateMap<NewFamilyMemberDto, FamilyMember>();
            CreateMap<FamilyMember, FamilyMemberDto>();
            CreateMap<FamilyMemberDto, FamilyMember>();
        }
    }
}
