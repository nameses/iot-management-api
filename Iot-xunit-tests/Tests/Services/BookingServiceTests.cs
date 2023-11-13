using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using iot_management_api.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Iot_xunit_tests.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<AppDbContext> _mockDbContext = new Mock<AppDbContext>();
        private readonly Mock<IDeviceService> _mockDeviceService = new Mock<IDeviceService>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<ILogger<BookingService>> _mockLogger = new Mock<ILogger<BookingService>>();

        [Fact]
        public async Task GetBookingsForTeacher_ShouldReturnBookings()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;
            var teacherId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var bookings = new List<Booking> { new Booking { Date = date, ScheduleId = scheduleId } };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(bookings);

            // Act
            var result = await service.GetBookingsForTeacher(date, scheduleId, teacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookings, result);
        }

        [Fact]
        public async Task GetBookingsForTeacher_ShouldReturnNullWhenNoBookings()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;
            var teacherId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            // No bookings in the database

            // Act
            var result = await service.GetBookingsForTeacher(date, scheduleId, teacherId);

            // Assert
            Assert.Null(result);
        }

        // Add more tests for GetBookingsForTeacher method if needed

        [Fact]
        public async Task GetBookingsForTeacher_ShouldReturnNullWhenNoMatchingSubjects()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;
            var teacherId = 1;

            var subjects = new List<Subject> { new Subject { Id = 2, TeacherId = 2 } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            // Act
            var result = await service.GetBookingsForTeacher(date, scheduleId, teacherId);

            // Assert
            Assert.Null(result);
        }

        // Add more tests for GetBookingsForTeacher method if needed

        [Fact]
        public async Task GetBookingsForTeacher_ShouldReturnNullWhenNoMatchingSchedule()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;
            var teacherId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var bookings = new List<Booking> { new Booking { Date = date, ScheduleId = 2 } };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(bookings);

            // Act
            var result = await service.GetBookingsForTeacher(date, scheduleId, teacherId);

            // Assert
            Assert.Null(result);
        }

        // Add more tests for GetBookingsForTeacher method if needed

        [Fact]
        public async Task GetBookingsForTeacher_ShouldReturnEmptyListWhenNoMatchingBookings()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;
            var teacherId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var bookings = new List<Booking> { new Booking { Date = date.AddDays(1), ScheduleId = scheduleId } };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(bookings);

            // Act
            var result = await service.GetBookingsForTeacher(date, scheduleId, teacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // Add more tests for GetBookingsForTeacher method if needed

        [Fact]
        public async Task GetBookingsForTeacher_ShouldReturnBookingsWithinDateRange()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var dateFrom = DateOnly.FromDateTime(DateTime.Now);
            var dateTo = dateFrom.AddDays(5);

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var bookings = new List<Booking>
            {
                new Booking { Date = dateFrom, ScheduleId = 1 },
                new Booking { Date = dateTo, ScheduleId = 2 },
                new Booking { Date = dateFrom.AddDays(10), ScheduleId = 3 }
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(bookings);

            // Act
            var result = await service.GetBookingsForTeacher(teacherId, dateFrom, dateTo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        // Add more tests for GetBookingsForTeacher method if needed

        [Fact]
        public async Task GetBookingsForTeacher_ShouldReturnBookingsWithinDateRange_DefaultToCurrentDate()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var dateFrom = DateOnly.FromDateTime(DateTime.Now);
            var dateTo = dateFrom.AddDays(5);

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var bookings = new List<Booking>
            {
                new Booking { Date = dateFrom, ScheduleId = 1 },
                new Booking { Date = dateTo, ScheduleId = 2 },
                new Booking { Date = dateFrom.AddDays(10), ScheduleId = 3 }
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(bookings);

            // Act
            var result = await service.GetBookingsForTeacher(teacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetStudentBookings_ShouldReturnBookings()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;
            var studentId = 1;

            var bookings = new List<Booking> { new Booking { Date = date, ScheduleId = scheduleId } };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(bookings);

            // Act
            var result = await service.GetStudentBookings(date, scheduleId, studentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookings, result);
        }

        [Fact]
        public async Task GetStudentBookings_ShouldReturnNullWhenNoBookings()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;
            var studentId = 1;

            // No bookings in the database

            // Act
            var result = await service.GetStudentBookings(date, scheduleId, studentId);

            // Assert
            Assert.Null(result);
        }

        // Add more tests for GetStudentBookings method if needed

        [Fact]
        public async Task ShowStudentsRequestsForTeacherAsync_ShouldReturnPendingBookings()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var userId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = userId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var bookings = new List<Booking>
            {
                new Booking { Status = BookingStatus.Pending, Schedule = new Schedule { SubjectId = 1 } },
                new Booking { Status = BookingStatus.Approved, Schedule = new Schedule { SubjectId = 2 } },
                new Booking { Status = BookingStatus.Pending, Schedule = new Schedule { SubjectId = 1 } }
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(bookings);

            // Act
            var result = await service.ShowStudentsRequestsForTeacherAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, b => Assert.Equal(BookingStatus.Pending, b.Status));
        }

        // Add more tests for ShowStudentsRequestsForTeacherAsync method if needed

        [Fact]
        public async Task RejectBooking_ShouldRejectBooking()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var bookingId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var booking = new Booking { Id = bookingId, Status = BookingStatus.Pending, Schedule = new Schedule { SubjectId = 1 } };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(new List<Booking> { booking });

            // Act
            var result = await service.RejectBooking(teacherId, bookingId);

            // Assert
            Assert.True(result.res);
            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }
        [Fact]
        public async Task ApproveBooking_ShouldApproveBooking()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var bookingId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;

            var booking = new Booking
            {
                Id = bookingId,
                Status = BookingStatus.Pending,
                Date = date,
                Schedule = new Schedule { SubjectId = 1, Id = scheduleId },
                DeviceId = 1,
                ScheduleId = scheduleId
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(new List<Booking> { booking });

            _mockDbContext.Setup(db => db.Bookings)
                .ReturnsDbSet(new List<Booking> { new Booking { Date = date, ScheduleId = scheduleId, Status = BookingStatus.Approved } });

            _mockDeviceService.Setup(x => x.CheckIfDeviceAvailableAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await service.ApproveBooking(teacherId, bookingId);

            // Assert
            Assert.True(result.res);
            Assert.Null(result.message);
            Assert.Equal(BookingStatus.Approved, booking.Status);
        }

        [Fact]
        public async Task ApproveBooking_ShouldNotApproveBookingWhenAlreadyApproved()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var bookingId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;

            var booking = new Booking
            {
                Id = bookingId,
                Status = BookingStatus.Approved,
                Date = date,
                Schedule = new Schedule { SubjectId = 1, Id = scheduleId },
                DeviceId = 1,
                ScheduleId = scheduleId
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(new List<Booking> { booking });

            // Act
            var result = await service.ApproveBooking(teacherId, bookingId);

            // Assert
            Assert.False(result.res);
            Assert.Equal("Booking request already approved", result.message);
            Assert.Equal(BookingStatus.Approved, booking.Status);
        }

        [Fact]
        public async Task ApproveBooking_ShouldNotApproveBookingWhenDateIsExpired()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var bookingId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
            var scheduleId = 1;

            var booking = new Booking
            {
                Id = bookingId,
                Status = BookingStatus.Pending,
                Date = date,
                Schedule = new Schedule { SubjectId = 1, Id = scheduleId },
                DeviceId = 1,
                ScheduleId = scheduleId
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(new List<Booking> { booking });

            // Act
            var result = await service.ApproveBooking(teacherId, bookingId);

            // Assert
            Assert.False(result.res);
            Assert.Equal("Date is expired", result.message);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact]
        public async Task ApproveBooking_ShouldNotApproveBookingWhenNoAccessToGroup()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var bookingId = 1;

            var subjects = new List<Subject> { new Subject { Id = 2, TeacherId = 2 } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;

            var booking = new Booking
            {
                Id = bookingId,
                Status = BookingStatus.Pending,
                Date = date,
                Schedule = new Schedule { SubjectId = 1, Id = scheduleId },
                DeviceId = 1,
                ScheduleId = scheduleId
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(new List<Booking> { booking });

            // Act
            var result = await service.ApproveBooking(teacherId, bookingId);

            // Assert
            Assert.False(result.res);
            Assert.Equal("Teacher does not have access to this group", result.message);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact]
        public async Task ApproveBooking_ShouldNotApproveBookingWhenNoAvailableDevices()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var teacherId = 1;
            var bookingId = 1;

            var subjects = new List<Subject> { new Subject { Id = 1, TeacherId = teacherId } };
            _mockDbContext.Setup(db => db.Subjects).ReturnsDbSet(subjects);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;

            var booking = new Booking
            {
                Id = bookingId,
                Status = BookingStatus.Pending,
                Date = date,
                Schedule = new Schedule { SubjectId = 1, Id = scheduleId },
                DeviceId = 1,
                ScheduleId = scheduleId
            };
            _mockDbContext.Setup(db => db.Bookings).ReturnsDbSet(new List<Booking> { booking });

            _mockDeviceService.Setup(x => x.CheckIfDeviceAvailableAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await service.ApproveBooking(teacherId, bookingId);

            // Assert
            Assert.False(result.res);
            Assert.Equal("There are no available devices for this schedule", result.message);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact]
        public void RecountDeviceAmount_ShouldRecountDeviceAmount()
        {
            // Arrange
            var service = new BookingService(
                _mockDbContext.Object,
                _mockDeviceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );

            var date = DateOnly.FromDateTime(DateTime.Now);
            var scheduleId = 1;

            var bookings = new List<Booking>
            {
                new Booking { Date = date, ScheduleId = scheduleId, Device = new Device { Id = 1, Amount = 3 } },
                new Booking { Date = date, ScheduleId = scheduleId, Device = new Device { Id = 2, Amount = 2 } },
                new Booking { Date = date, ScheduleId = scheduleId, Device = new Device { Id = 1, Amount = 1 } }
            };

            // Act
            service.RecountDeviceAmount(ref bookings);

            // Assert
            Assert.Equal(1, bookings.Count);
            Assert.Equal(2, bookings[0].Device.Amount);
        }
    }
}
