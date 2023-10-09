using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;

namespace iot_management_api.Services
{
    public interface IBookingService
    {
        Task<bool> BookDeviceAsync(int deviceId, int studentId, DateOnly date, int scheduleId);
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
        public async Task<bool> BookDeviceAsync(int deviceId, int studentId, DateOnly date, int scheduleId)
        {
            var booking = new Booking()
            {
                Date = date,
                Status = BookingStatus.Pending,
                ScheduleId = scheduleId,
                StudentId = studentId,
                DeviceId = deviceId
            };

            var res = await _deviceService.CheckIfDeviceAvailableAsync(deviceId, date, scheduleId);
            if (!res) return false;

            await _context.Bookings.AddAsync(booking);

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
