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
        Task<Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>?> GetFullAsync(UserRole userRole, int userId);
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
        public async Task<Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>?> GetFullAsync(UserRole userRole, int userId)
        {
            List<Schedule>? entities = null;
            if (userRole==UserRole.Student)
                entities = await GetStudentSchedule(userId);
            if (userRole==UserRole.Teacher)
                entities = await GetTeacherSchedule(userId);

            if (entities==null)
                return null;

            var dict = entities
                .GroupBy(x => x.Period.Week)
                .ToDictionary(
                    weekGroup => weekGroup.Key,
                    weekGroup => weekGroup
                        .GroupBy(x => x.Period.Day)
                        .ToDictionary(
                            dayGroup => dayGroup.Key,
                            dayGroup => dayGroup.Select(schedule => _mapper.Map<ScheduleModel>(schedule)).ToList()
                        )
                );

            return ConvertDateEnumToDateOnly(dict);
        }
        private Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>? ConvertDateEnumToDateOnly(
            Dictionary<WeekEnum, Dictionary<DayEnum, List<ScheduleModel>>> originalData)
        {
            var convertedData = new Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>();

            var date = _weekService.GetMondayOfFirstWeek();

            foreach (var weekEntry in originalData)
            {
                var weekEnum = weekEntry.Key;
                var dayData = weekEntry.Value;

                var convertedWeekData = new Dictionary<DateOnly, List<ScheduleModel>>();

                foreach (var dayEntry in dayData)
                {
                    var schedules = dayEntry.Value;

                    convertedWeekData[new DateOnly(date.Year, date.Month, date.Day)] = schedules;
                    date = date.AddDays(1);
                }

                convertedData[weekEnum] = convertedWeekData;
            }
            return convertedData;
        }

        private async Task<List<Schedule>?> GetStudentSchedule(int userId)
        {
            var user = await _context.Students.FirstOrDefaultAsync(x => x.Id == userId);
            if (user==null) return null;

            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == user.GroupId);
            if (group==null) return null;

            return await _context.Schedules
                .Include(x => x.Groups)
                .Include(x => x.Period)
                .Include(x => x.Subject)
                .Include(x => x.Subject.Teacher)
                .Include(x => x.Room)
                .AsSplitQuery()
                .Where(x => x.Groups.Contains(group)
                    && x.Period.Year==DateTime.Now.Year
                    && x.Period.Semester == GetCurrentSemester())
                .ToListAsync();


        }

        private async Task<List<Schedule>?> GetTeacherSchedule(int userId)
        {
            var user = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == userId);
            if (user==null) return null;

            var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.TeacherId == user.Id);
            if (subject==null) return null;

            return await _context.Schedules
                .Include(x => x.Groups)
                .Include(x => x.Period)
                .Include(x => x.Subject)
                .Include(x => x.Room)
                .AsSplitQuery()
                .Where(x => x.SubjectId==subject.Id
                    && x.Period.Year==DateTime.Now.Year
                    && x.Period.Semester == GetCurrentSemester())
                .ToListAsync();
        }

        private static SemesterEnum GetCurrentSemester()
        {
            const int firstMonth = 9;
            const int secondMonth = 2;

            var currentMonth = DateTime.Now.Month;

            if (currentMonth>=firstMonth || currentMonth<secondMonth)
                return SemesterEnum.First;
            else return SemesterEnum.Second;
        }
    }
}
