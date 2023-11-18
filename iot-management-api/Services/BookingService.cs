using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace iot_management_api.Services
{
    public interface IBookingService
    {
        Task BookDeviceAsync(int deviceId, int studentId, DateOnly date, int scheduleId);
        Task<IEnumerable<Booking>?> ShowStudentsRequestsForTeacherAsync(int userId);
        Task<(bool res, string? message)> ApproveBooking(int teacherId, int bookingId);
        Task<(bool res, string? message)> RejectBooking(int teacherId, int bookingId);
        Task<IEnumerable<Booking>?> GetStudentBookings(DateOnly date, int scheduleId, int studentId);
        Task<IEnumerable<Booking>?> GetStudentBookings(int studentId, DateOnly? dateFrom = null, DateOnly? dateTo = null);
        Task<IEnumerable<Booking>?> GetBookingsForTeacher(DateOnly date, int scheduleId, int studentId);
        Task<IEnumerable<Booking>?> GetBookingsForTeacher(int teacherId, DateOnly? dateFrom = null, DateOnly? dateTo = null);

    }
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public record struct DateId(DateOnly date, int? id);

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

        public async Task<IEnumerable<Booking>?> GetBookingsForTeacher(DateOnly date, int scheduleId, int teacherId)
        {
            var subjects = await _context.Subjects
                .Where(x => x.TeacherId==teacherId)
                .Select(x => x.Id)
                .ToListAsync();

            var bookings = await _context.Bookings
                .Include(x => x.Schedule)
                .Include(x => x.Schedule!.Period)
                .Include(x => x.Student)
                .Include(x => x.Device)
                .Include(x => x.Device!.DeviceInfo)
                .Where(x => subjects.Contains(x.Schedule!.SubjectId!.Value) && x.ScheduleId==scheduleId && x.Date==date)
                .ToListAsync();

            bookings = RecountDeviceAmount(bookings);

            return bookings.IsNullOrEmpty() ? null : bookings;
        }
        public async Task<IEnumerable<Booking>?> GetBookingsForTeacher(int teacherId, DateOnly? dateFrom = null, DateOnly? dateTo = null)
        {
            dateFrom ??= DateOnly.FromDateTime(DateTime.Now);

            var subjects = await _context.Subjects
                .Where(x => x.TeacherId==teacherId)
                .Select(x => x.Id)
                .ToListAsync();

            if (subjects.IsNullOrEmpty()) return null;

            var query = _context.Bookings
                .Include(x => x.Schedule)
                .Include(x => x.Schedule!.Period)
                .Include(x => x.Student)
                .Include(x => x.Device)
                .Include(x => x.Device!.DeviceInfo)
                .Where(x => subjects.Contains(x.Schedule!.SubjectId!.Value)
                    && (dateTo==null ? (x.Date>=dateFrom) : (x.Date>=dateFrom&&x.Date<=dateTo)));

            var bookings = await query.ToListAsync();

            bookings = RecountDeviceAmount(bookings);

            return bookings.IsNullOrEmpty() ? null : bookings;
        }
        public async Task<IEnumerable<Booking>?> GetStudentBookings(DateOnly date, int scheduleId, int studentId)
        {
            if (date.ToDateTime(new TimeOnly(0))<DateTime.Now)
                return null;

            var bookings = await _context.Bookings
                .Include(x => x.Schedule)
                .Include(x => x.Schedule!.Period)
                .Include(x => x.Device)
                .Include(x => x.Device!.DeviceInfo)
                .Where(x => x.StudentId==studentId && x.ScheduleId==scheduleId && x.Date==date)
                .ToListAsync();

            bookings = RecountDeviceAmount(bookings);

            return bookings.IsNullOrEmpty() ? null : bookings;
        }
        public async Task<IEnumerable<Booking>?> GetStudentBookings(int studentId, DateOnly? dateFrom = null, DateOnly? dateTo = null)
        {
            dateFrom ??= DateOnly.FromDateTime(DateTime.Now);

            var query = _context.Bookings
                .Include(x => x.Schedule)
                .Include(x => x.Schedule!.Period)
                .Include(x => x.Student)
                .Include(x => x.Device)
                .Include(x => x.Device!.DeviceInfo)
                .Where(x => x.StudentId==studentId
                    && (dateTo==null ? (x.Date>=dateFrom) : (x.Date>=dateFrom&&x.Date<=dateTo)));

            var bookings = await query.ToListAsync();

            bookings = RecountDeviceAmount(bookings);

            return bookings.IsNullOrEmpty() ? null : bookings;
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

            if (subjects.IsNullOrEmpty()) return null;

            var bookings = await _context.Bookings
                .Include(x => x.Schedule)
                .Include(x => x.Schedule!.Period)
                .Include(x => x.Schedule!.Period!.DayMapping)
                .Include(x => x.Device)
                .Include(x => x.Device!.DeviceInfo)
                .Include(x => x.Student)
                .Where(x => EF.Property<DateTime>(x, "Date") >= DateTime.Now && x.Status==BookingStatus.Pending && subjects.Contains(x.Schedule!.SubjectId!.Value))
                .ToListAsync();

            if (bookings.IsNullOrEmpty())
                return null;

            bookings = RecountDeviceAmount(bookings);

            return bookings;
        }

        public async Task<(bool res, string? message)> RejectBooking(int teacherId, int bookingId)
        {
            List<int> subjects = await _context.Subjects
                .Where(x => x.TeacherId==teacherId)
                .Select(x => x.Id)
                .ToListAsync();

            var booking = await _context.Bookings
                .Include(x => x.Schedule)
                .FirstOrDefaultAsync(x => x.Id==bookingId);

            if (booking==null)
                return (false, "Not Found");

            if (booking.Date<DateOnly.FromDateTime(DateTime.Now))
                return (false, "Date is expired");

            if (!subjects.Contains(booking.Schedule!.SubjectId!.Value))
                return (false, "Teacher does not have access to this group");

            if (booking.Status == BookingStatus.Rejected)
                return (false, "Booking request already rejected");

            booking.Status = BookingStatus.Rejected;

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool res, string? message)> ApproveBooking(int teacherId, int bookingId)
        {
            List<int> subjects = await _context.Subjects
                .Where(x => x.TeacherId==teacherId)
                .Select(x => x.Id)
                .ToListAsync();

            var booking = await _context.Bookings
                .Include(x => x.Schedule)
                .FirstOrDefaultAsync(x => x.Id==bookingId);

            if (booking==null)
                return (false, "Not Found");

            if (booking.Date<DateOnly.FromDateTime(DateTime.Now))
                return (false, "Date is expired");

            if (!subjects.Contains(booking.Schedule!.SubjectId!.Value))
                return (false, "Teacher does not have access to this group");

            if (booking.Status == BookingStatus.Approved)
                return (false, "Booking request already approved");

            var res = await _deviceService.CheckIfDeviceAvailableAsync(booking.DeviceId!.Value, booking.Date, booking.ScheduleId!.Value);
            if (!res)
                return (false, "There are no available devices for this schedule");

            booking.Status = BookingStatus.Approved;

            await _context.SaveChangesAsync();

            return (true, null);
        }

        private List<Booking> RecountDeviceAmount(List<Booking> bookings)
        {
            var bookingsRealAmount = new List<Booking>();

            //count real amount of each devices
            foreach (var b in bookings)
            {
                var amountApprovedSameBookings = _context.Bookings
                    .Where(x => x.Date == b.Date && x.ScheduleId == b.ScheduleId && x.DeviceId==b.DeviceId && x.Status == BookingStatus.Approved)
                    .Count();

                var booking = _mapper.Map<Booking>(b);
                booking.Device!.Amount -= amountApprovedSameBookings;

                if (booking.Device!.Amount<0) booking.Device!.Amount=0;

                bookingsRealAmount.Add(booking);
            }

            return bookingsRealAmount;
        }
    }
}
