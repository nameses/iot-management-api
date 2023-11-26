using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using iot_management_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iot_xunit_tests.DeviceTests.Services
{
    public class DeviceServiceTests
    {


        // CheckIfDeviceAvailableAsync returns true if device is available on given date and schedule
        [Fact]
        public async Task test_check_if_device_available_returns_true()
        {
            // Arrange
            var context = new Mock<AppDbContext>();
            var deviceInfoService = new Mock<IDeviceInfoService>();
            var roomService = new Mock<IRoomService>();
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<DeviceService>>();

            var deviceService = new DeviceService(context.Object, deviceInfoService.Object, roomService.Object, mapper.Object, logger.Object);

            int deviceId = 1;
            DateOnly date = new DateOnly();
            int scheduleId = 1;

            var schedule = new Schedule { Id = scheduleId, RoomId = 1 };
            var device = new Device { Id = deviceId, RoomId = 1, Amount = 1 };
            var booking = new Booking { ScheduleId = scheduleId, DeviceId = deviceId, Date = date, Status = BookingStatus.Approved };

            context.Setup(c => c.Schedules.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == scheduleId)).ReturnsAsync(schedule);
            context.Setup(c => c.Devices.Include(x => x.DeviceInfo).FirstOrDefaultAsync(x => x.Id == deviceId)).ReturnsAsync(device);
            context.Setup(c => c.Bookings.Where(x => x.ScheduleId == schedule.Id && x.DeviceId == deviceId && x.Date == date && x.Status == BookingStatus.Approved).ToListAsync()).ReturnsAsync(new List<Booking> { booking });

            // Act
            var result = await deviceService.CheckIfDeviceAvailableAsync(deviceId, date, scheduleId);

            // Assert
            Assert.True(result);
        }

        // GetAvailableAsync returns list of available devices on given date and schedule
        [Fact]
        public async Task test_get_available_returns_list_of_devices()
        {
            // Arrange
            var context = new Mock<AppDbContext>();
            var deviceInfoService = new Mock<IDeviceInfoService>();
            var roomService = new Mock<IRoomService>();
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<DeviceService>>();

            var deviceService = new DeviceService(context.Object, deviceInfoService.Object, roomService.Object, mapper.Object, logger.Object);

            DateOnly date = new DateOnly();
            int scheduleId = 1;

            var schedule = new Schedule { Id = scheduleId, RoomId = 1 };
            var device1 = new Device { Id = 1, RoomId = 1, Amount = 2 };
            var device2 = new Device { Id = 2, RoomId = 1, Amount = 0 };
            var booking = new Booking { ScheduleId = scheduleId, DeviceId = 1, Date = date, Status = BookingStatus.Approved };

            context.Setup(c => c.Schedules.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == scheduleId)).ReturnsAsync(schedule);
            context.Setup(c => c.Devices.Include(x => x.DeviceInfo).Where(x => x.RoomId == schedule.RoomId).ToListAsync()).ReturnsAsync(new List<Device> { device1, device2 });
            context.Setup(c => c.Bookings.Where(x => x.ScheduleId == schedule.Id && x.Date == date && x.Status == BookingStatus.Approved).ToListAsync()).ReturnsAsync(new List<Booking> { booking });

            // Act
            var result = await deviceService.GetAvailableAsync(date, scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            Assert.Equal(device1.Id, result.First().Id);
        }

        // GetByIdAsync returns device entity if id is not null and device exists
        [Fact]
        public async Task test_get_by_id_returns_device_entity()
        {
            // Arrange
            var context = new Mock<AppDbContext>();
            var deviceInfoService = new Mock<IDeviceInfoService>();
            var roomService = new Mock<IRoomService>();
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<DeviceService>>();

            var deviceService = new DeviceService(context.Object, deviceInfoService.Object, roomService.Object, mapper.Object, logger.Object);

            int deviceId = 1;

            var device = new Device { Id = deviceId };

            context.Setup(c => c.Devices.Include(x => x.Room).Include(x => x.DeviceInfo).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == deviceId)).ReturnsAsync(device);

            // Act
            var result = await deviceService.GetByIdAsync
                  // Assert
            Assert.Equal(device, result);
        }

        // GetByRoomAsync returns list of devices if roomNumber is not null and room exists
        [Fact]
        public async Task test_get_by_room_returns_list_of_devices()
        {
            // Arrange
            var context = new Mock<AppDbContext>();
            var deviceInfoService = new Mock<IDeviceInfoService>();
            var roomService = new Mock<IRoomService>();
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<DeviceService>>();

            var deviceService = new DeviceService(context.Object, deviceInfoService.Object, roomService.Object, mapper.Object, logger.Object);

            int? roomNumber = 1;

            var devices = new List<Device>
    {
        new Device { Id = 1, RoomId = 1, Amount = 1 },
        new Device { Id = 2, RoomId = 1, Amount = 2 }
    };

            context.Setup(c => c.Devices.Include(x => x.DeviceInfo).Include(x => x.Room).Where(x => x.Room!.Number == roomNumber).ToListAsync()).ReturnsAsync(devices);

            // Act
            var result = await deviceService.GetByRoomAsync(roomNumber);

            // Assert
            Assert.Equal(devices, result);
        }

        // CreateAsync creates a new device entity with valid inputs
        [Fact]
        public async Task test_create_device_with_valid_inputs()
        {
            // Arrange
            var context = new Mock<AppDbContext>();
            var deviceInfoService = new Mock<IDeviceInfoService>();
            var roomService = new Mock<IRoomService>();
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<DeviceService>>();

            var deviceService = new DeviceService(context.Object, deviceInfoService.Object, roomService.Object, mapper.Object, logger.Object);

            var entity = new Device { Id = 1, Amount = 1 };
            var deviceInfo = new DeviceInfo { Id = 1 };
            int? roomNumber = 1;

            var room = new Room { Id = 1 };
            var dbDeviceInfo = new DeviceInfo { Id = 1 };

            context.Setup(c => c.Rooms.FindAsync(roomNumber)).ReturnsAsync(room);
            deviceInfoService.Setup(d => d.GetByDeviceInfoAsync(deviceInfo)).ReturnsAsync(dbDeviceInfo);
            context.Setup(c => c.Devices.AddAsync(entity)).Callback(() => entity.Id = 1);
            context.Setup(c => c.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await deviceService.CreateAsync(entity, deviceInfo, roomNumber);

            // Assert
            Assert.Equal(1, result);
        }

        // DeleteAsync deletes device entity with valid id
        [Fact]
        public async Task delete_async_deletes_device_entity_with_valid_id()
        {
            // Arrange
            var context = new Mock<AppDbContext>();
            var deviceInfoService = new Mock<IDeviceInfoService>();
            var roomService = new Mock<IRoomService>();
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<DeviceService>>();

            var deviceService = new DeviceService(context.Object, deviceInfoService.Object, roomService.Object, mapper.Object, logger.Object);

            int deviceId = 1;

            context.Setup(c => c.Devices.FirstOrDefaultAsync(x => x.Id == deviceId)).ReturnsAsync(new Device { Id = deviceId });

            // Act
            var result = await deviceService.DeleteAsync(deviceId);

            // Assert
            Assert.True(result);
        }

        // UpdateAsync updates device entity with valid inputs
        [Fact]
        public async Task test_update_device_entity_with_valid_inputs()
        {
            // Arrange
            var context = new Mock<AppDbContext>();
            var deviceInfoService = new Mock<IDeviceInfoService>();
            var roomService = new Mock<IRoomService>();
            var mapper = new Mock<IMapper>();
            var logger = new Mock<ILogger<DeviceService>>();

            var deviceService = new DeviceService(context.Object, deviceInfoService.Object, roomService.Object, mapper.Object, logger.Object);

            int deviceId = 1;
            var entity = new Device { Id = deviceId, Amount = 5 };
            var dbEntity = new Device { Id = deviceId, Amount = 10 };

            context.Setup(c => c.Devices.Include(x => x.DeviceInfo).Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == deviceId)).ReturnsAsync(dbEntity);
            deviceInfoService.Setup(d => d.UpdateAsync(deviceId, entity.DeviceInfo)).Returns(Task.CompletedTask);

            // Act
            var result = await deviceService.UpdateAsync(deviceId, entity);

            // Assert
            Assert.True(result);
            Assert.Equal(entity.Amount, dbEntity.Amount);
            deviceInfoService.Verify(d => d.UpdateAsync(deviceId, entity.DeviceInfo), Times.Once);
            context.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

    }
}
