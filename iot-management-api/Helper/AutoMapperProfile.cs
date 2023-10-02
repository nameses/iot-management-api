using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Models;
using iot_management_api.Models.common;
using static iot_management_api.Controllers.AuthController;

namespace iot_management_api.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<User, UserModel>()
            //    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<UserSignUp, Teacher>();
            CreateMap<UserSignUp, Student>();

            CreateMap<Teacher, TeacherModel>();
            CreateMap<Student, StudentModel>()
                .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.Group.GroupCode));

            CreateMap<User, UserModel>();

            CreateMap<Group, GroupModel>()
                .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.Students));
        }
    }
}
