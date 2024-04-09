using AutoMapper;
using Data.Dto;
using Data.Entities;

namespace Core.AutoMapperProfile
{
    public class FamilyProfile : Profile 
    {
        public FamilyProfile()
        {
            CreateMap<Family, FamilyDto>();
            CreateMap<FamilyDto, Family>();

            CreateMap<NewFamilyDto, Family>();
        }
    }
}
