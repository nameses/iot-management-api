using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IBookingService
    {
        Task BookDeviceAsync(int deviceId, int studentId, DateOnly date, int scheduleId);
        Task<IEnumerable<Booking>?> ShowStudentsRequestsForTeacherAsync(int userId);
    }
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(AppDbContext context,
            IDeviceService deviceService,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _context=context;
            _deviceService=deviceService;
            _mapper=mapper;
            _logger=logger;
        }
        public async Task BookDeviceAsync(int deviceId, int studentId, DateOnly date, int scheduleId)
        {
            var booking = new Booking()
            {
                Date = date,
                Status = BookingStatus.Pending,
                ScheduleId = scheduleId,
                StudentId = studentId,
                DeviceId = deviceId
            };

            await _context.Bookings.AddAsync(booking);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Booking>?> ShowStudentsRequestsForTeacherAsync(int userId)
        {
            List<int> subjects = await _context.Subjects
                .Where(x => x.TeacherId==userId)
                .Select(x => x.Id)
                .ToListAsync();

            var bookings = await _context.Bookings
                .Include(x => x.Schedule)
                .Include(x => x.Schedule!.Period)
                .Include(x => x.Schedule!.Period!.DayMapping)
                .Include(x => x.Device)
                .Include(x => x.Device!.DeviceInfo)
                .Where(x => subjects.Contains(x.Schedule!.SubjectId!.Value))
                .ToListAsync();

            return bookings;
        }
    }
}
