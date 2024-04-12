using AutoMapper;
using Data.Dto;
using Data.Entities;

namespace Core.AutoMapperProfile
{
    public class FamilyProfile : Profile 
    {
        public FamilyProfile()
        {
            CreateMap<Family, FamilyDto>()
                .ForMember(fm => fm.FamilyMembers, s => s.MapFrom(mf => mf.FamilyMembers));

            CreateMap<FamilyDto, Family>();

            CreateMap<NewFamilyDto, Family>();
        }
    }
}
