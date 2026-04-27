using ASC.Model;
using AutoMapper;

namespace ASC.Web.Areas.Configuration.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MasterDataKey, MasterDataKeyViewModel>()
                .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PartitionKey, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<MasterDataKeyViewModel, MasterDataKey>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.MasterDataValues, opt => opt.Ignore());

            CreateMap<MasterDataValue, MasterDataValueViewModel>()
                .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<MasterDataValueViewModel, MasterDataValue>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey))
                .ForMember(dest => dest.MasterDataKeyId, opt => opt.Ignore())
                .ForMember(dest => dest.MasterDataKey, opt => opt.Ignore())
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
