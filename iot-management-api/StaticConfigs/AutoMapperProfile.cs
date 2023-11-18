using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Models;
using iot_management_api.Models.common;
using static iot_management_api.Controllers.AuthController;
using static iot_management_api.Controllers.BookingController;
using static iot_management_api.Controllers.DeviceController;
using static iot_management_api.Controllers.SubjectController;

namespace iot_management_api.StaticConfigs
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

            CreateMap<Group, GroupModel>();
            //.ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.Students));
            //CreateMap<GroupReq, Group>();

            CreateMap<Room, RoomModel>();
            //.ForMember(dest => dest.Devices, opt => opt.MapFrom(src => src.Devices))
            //.ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules));
            //CreateMap<RoomReq, Room>();

            CreateMap<DeviceInfo, DeviceInfo>();
            CreateMap<DeviceInfo, DeviceInfoModel>();
            CreateMap<DeviceReq, DeviceInfo>();

            CreateMap<Device, Device>();
            CreateMap<Device, DeviceModel>();
            CreateMap<DeviceReq, Device>()
                .ForPath(dest => dest.DeviceInfo.Name, opt => opt.MapFrom(src => src.Name))
                .ForPath(dest => dest.DeviceInfo.Type, opt => opt.MapFrom(src => src.Type))
                .ForPath(dest => dest.DeviceInfo.Model, opt => opt.MapFrom(src => src.Model))
                .ForPath(dest => dest.DeviceInfo.Description, opt => opt.MapFrom(src => src.Description))
                .ForPath(dest => dest.Room.Number, opt => opt.MapFrom(src => src.RoomNumber));
            CreateMap<DeviceUpdateReq, Device>()
                .ForPath(dest => dest.DeviceInfo.Name, opt => opt.MapFrom(src => src.Name))
                .ForPath(dest => dest.DeviceInfo.Type, opt => opt.MapFrom(src => src.Type))
                .ForPath(dest => dest.DeviceInfo.Model, opt => opt.MapFrom(src => src.Model))
                .ForPath(dest => dest.DeviceInfo.Description, opt => opt.MapFrom(src => src.Description))
                .ForPath(dest => dest.Room.Number, opt => opt.MapFrom(src => src.RoomNumber));


            CreateMap<Subject, SubjectModel>()
                .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.Teacher));
            CreateMap<SubjectReq, Subject>();

            //.ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules));
            //CreateMap<SubjectReq, Subject>();

            CreateMap<Schedule, ScheduleModel>()
                .ForMember(dest => dest.GroupCodes, opt => opt.MapFrom(src => src.Groups.Select(x => x.GroupCode)));
            CreateMap<Subject, SubjectDataModel>()
                .ForPath(dest => dest.TeacherFullName, opt => opt.MapFrom(src => src.Teacher.Name + " " + src.Teacher.Surname));

            CreateMap<Period, PeriodModel>()
                .ForPath(dest => dest.StartTime, opt => opt.MapFrom(src => src.DayMapping!.StartTime))
                .ForPath(dest => dest.EndTime, opt => opt.MapFrom(src => src.DayMapping!.EndTime))
                .ForPath(dest => dest.SubjectNumber, opt => opt.MapFrom(src => src.DayMapping!.SubjectNumber));


            CreateMap<DayMapping, DayMappingModel>();

            CreateMap<Booking, Booking>();
            CreateMap<Booking, BookingModel>();
            CreateMap<Booking, BookingForStudentModel>();
        }
    }
}
