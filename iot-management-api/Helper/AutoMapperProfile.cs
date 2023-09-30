using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Models;
using static iot_management_api.Controllers.AuthController.RegRequest;

namespace iot_management_api.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<RegUser, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));
        }
    }
}
