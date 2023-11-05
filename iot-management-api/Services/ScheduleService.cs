using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using iot_management_api.Models;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IScheduleService
    {
        Task<Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>?> GetFullAsync(UserRole userRole, int userId, DateOnly date);
        Task<bool> CheckUserAssignmentToSchedule(UserRole userRole, int userId, int scheduleId);
        Task<bool> CheckDateSchedule(DateOnly date, int scheduleId);
    }
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;
        private readonly StudyWeekService _weekService;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(AppDbContext context,
            StudyWeekService weekService,
            IMapper mapper,
            ILogger<ScheduleService> logger)
        {
            _context=context;
            _weekService=weekService;
            _mapper=mapper;
            _logger=logger;
        }
        public async Task<Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>?> GetFullAsync(UserRole userRole, int userId, DateOnly date)
        {
            List<Schedule>? entities = null;
            if (userRole==UserRole.Student)
                entities = await GetStudentSchedule(userId, date);
            if (userRole==UserRole.Teacher)
                entities = await GetTeacherSchedule(userId, date);

            if (entities==null)
                return null;

            var dict = entities
                .GroupBy(x => x.Period!.Week)
                .ToDictionary(
                    weekGroup => weekGroup.Key,
                    weekGroup => weekGroup
                        .GroupBy(x => x.Period!.Day)
                        //.OrderBy(m => m.Key)
                        .ToDictionary(
                            dayGroup => dayGroup.Key,
                            dayGroup => dayGroup
                            .Select(schedule => _mapper.Map<ScheduleModel>(schedule))
                            .OrderBy(scheduleModel => scheduleModel.Period.SubjectNumber)
                            .ToList()
                        )
                );

            var allDayEnums = Enum.GetValues(typeof(DayEnum)).Cast<DayEnum>().ToArray();

            foreach (var weekGroup in dict)
            {
                var list = new List<DayEnum>();
                foreach (var dayEnum in allDayEnums)
                {
                    if (!weekGroup.Value.ContainsKey(dayEnum))
                        list.Add(dayEnum);
                }
                foreach (var elem in list)
                    dict[weekGroup.Key].Add(elem, new List<ScheduleModel>());
            }

            dict = dict
                .ToDictionary(
                    outer => outer.Key,
                    outer => outer.Value
                        .OrderBy(inner => inner.Key)
                        .ToDictionary(inner => inner.Key, inner => inner.Value)
                );

            return ConvertDateEnumToDateOnly(dict, date);
        }
        private Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>? ConvertDateEnumToDateOnly(
            Dictionary<WeekEnum, Dictionary<DayEnum, List<ScheduleModel>>> originalData, DateOnly dateCopyFrom)
        {
            var convertedData = new Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>();

            var mondayFirstWeekDate = _weekService.GetMondayOfFirstWeek(dateCopyFrom);

            foreach (var weekEntry in originalData)
            {
                var weekEnum = weekEntry.Key;
                var dayData = weekEntry.Value;

                var convertedWeekData = new Dictionary<DateOnly, List<ScheduleModel>>();

                foreach (var dayEntry in dayData)
                {
                    var schedules = dayEntry.Value;

                    //var date = new DateOnly(mondayFirstWeekDate.Year, mondayFirstWeekDate.Month, mondayFirstWeekDate.Day);
                    //date = date.AddDays((int)weekEnum*7+(int)dayEntry.Key);
                    var date = mondayFirstWeekDate.AddDays((int)weekEnum*7+(int)dayEntry.Key);
                    convertedWeekData[date] = schedules;
                }

                convertedData[weekEnum] = convertedWeekData;
            }
            return convertedData;
        }

        private async Task<List<Schedule>?> GetStudentSchedule(int userId, DateOnly date)
        {
            var user = await _context.Students.FirstOrDefaultAsync(x => x.Id == userId);
            if (user==null) return null;

            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == user.GroupId);
            if (group==null) return null;

            return await _context.Schedules
                .Include(x => x.Groups)
                .Include(x => x.Period)
                .Include(x => x.Period!.DayMapping)
                .Include(x => x.Subject)
                .Include(x => x.Subject!.Teacher)
                .Include(x => x.Room)
                .AsSplitQuery()
                .Where(x => x.Groups.Contains(group)
                    && x.Period!.Year==date.Year
                    && x.Period.Semester == GetCurrentSemester(date))
                .ToListAsync();


        }

        private async Task<List<Schedule>?> GetTeacherSchedule(int userId, DateOnly date)
        {
            var user = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == userId);
            if (user==null) return null;

            var subjects = await _context.Subjects.Where(x => x.TeacherId == user.Id).Select(x => x.Id).ToListAsync();
            if (subjects==null) return null;

            return await _context.Schedules
                .Include(x => x.Groups)
                .Include(x => x.Period)
                .Include(x => x.Period!.DayMapping)
                .Include(x => x.Subject)
                .Include(x => x.Room)
                .AsSplitQuery()
                .Where(x => subjects.Contains(x.SubjectId!.Value)
                    && x.Period!.Year==date.Year
                    && x.Period.Semester == GetCurrentSemester(date))
                .ToListAsync();
        }

        private static SemesterEnum GetCurrentSemester(DateOnly date)
        {
            const int firstMonth = 9;
            const int secondMonth = 2;

            var currentMonth = date.Month;

            if (currentMonth>=firstMonth || currentMonth<secondMonth)
                return SemesterEnum.First;
            else return SemesterEnum.Second;
        }
        public async Task<bool> CheckUserAssignmentToSchedule(UserRole userRole, int userId, int scheduleId)
        {
            if (userRole == UserRole.Student)
            {
                var user = _context.Students.FirstOrDefault(x => x.Id==userId);
                if (user==null) return false;

                var schedule = await _context.Schedules
                    .Include(x => x.GroupSchedules)
                    .Where(x => x.GroupSchedules.Contains(new GroupSchedule { GroupId=user.GroupId, ScheduleId = x.Id }))
                    .FirstOrDefaultAsync(x => x.Id == scheduleId);

                if (schedule!=null) return true;
            }
            else if (userRole == UserRole.Teacher)
            {
                var user = _context.Teachers.Include(x => x.Subjects).FirstOrDefault(x => x.Id==userId);
                if (user==null) return false;

                var schedule = await _context.Schedules
                    .Include(x => x.Subject)
                    .Where(x => user.Subjects.Select(x => (int?)x.Id).Contains(x.SubjectId))
                    .FirstOrDefaultAsync(x => x.Id == scheduleId);

                if (schedule!=null) return true;
            }
            return false;
        }

        public async Task<bool> CheckDateSchedule(DateOnly date, int scheduleId)
        {
            (WeekEnum, DayEnum) wd = _weekService.GetWeekDateEnums(date);

            var schedule = await _context.Schedules
                .Include(x => x.Period)
                .FirstOrDefaultAsync(x => x.Id == scheduleId);

            if (schedule !=null && schedule!.Period != null
                && schedule.Period.Week == wd.Item1
                && schedule.Period.Day==wd.Item2)
                return true;
            return false;
        }
    }
}
