using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities.common;
using iot_management_api.Models;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IScheduleService
    {
        Task<Dictionary<WeekEnum, Dictionary<DayEnum, List<ScheduleModel>>>?> GetFull(UserRole userRole, int userId);
    }
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(AppDbContext context,
            IMapper mapper,
            ILogger<ScheduleService> logger)
        {
            _context=context;
            _mapper=mapper;
            _logger=logger;
        }
        public async Task<Dictionary<WeekEnum, Dictionary<DayEnum, List<ScheduleModel>>>?> GetFull(UserRole userRole, int userId)
        {
            if (userRole==UserRole.Student)
                return await GetStudentSchedule(userId);
            if (userRole==UserRole.Teacher)
                return await GetTeacherSchedule(userId);
            return null;
        }
        private async Task<Dictionary<WeekEnum, Dictionary<DayEnum, List<ScheduleModel>>>?> GetStudentSchedule(int userId)
        {
            var user = await _context.Students.FirstOrDefaultAsync(x => x.Id == userId);
            if (user==null) return null;

            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == user.GroupId);
            if (group==null) return null;

            var entities = await _context.Schedules
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

            if (entities==null)
                return null;

            return entities
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
        }

        private async Task<Dictionary<WeekEnum, Dictionary<DayEnum, List<ScheduleModel>>>?> GetTeacherSchedule(int userId)
        {
            var user = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == userId);
            if (user==null) return null;

            var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.TeacherId == user.Id);
            if (subject==null) return null;

            var entities = await _context.Schedules
                .Include(x => x.Groups)
                .Include(x => x.Period)
                .Include(x => x.Subject)
                .Include(x => x.Room)
                .AsSplitQuery()
                .Where(x => x.SubjectId==subject.Id
                    && x.Period.Year==DateTime.Now.Year
                    && x.Period.Semester == GetCurrentSemester())
                .ToListAsync();

            if (entities==null)
                return null;

            return entities
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
