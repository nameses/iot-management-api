using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Controllers;
using iot_management_api.Entities;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iot_management_api.Controllers.DeviceController;
using Iot_xunit_tests.DeviceTests.FakeServices;

namespace Iot_xunit_tests.DeviceTests.Controllers
{
    public class DeviceControllerTests
    {
        [Fact]
        public async Task GetAvailableReturnsListOfDevices()
        {
            // Arrange
            var deviceServiceMock = new Mock<IDeviceService>();
            var fakeScheculeService = new Mock<FakeScheduleService>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<DeviceController>>();

            var controller = new DeviceController(
                deviceServiceMock.Object,
                fakeScheculeService.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            var date = new DateOnly(2024, 1, 1);
            var scheduleId = 1;

            var expectedEntities = new List<Device>();
            var expectedDeviceModels = new List<DeviceModel>();

            deviceServiceMock.Setup(s => s.GetAvailableAsync(date, scheduleId))
                .ReturnsAsync(expectedEntities);
            mapperMock.Setup(m => m.Map<IEnumerable<DeviceModel>>(expectedEntities))
                .Returns(expectedDeviceModels);

            // Act
            var result = await controller.GetAvailable(date, scheduleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDeviceModels = Assert.IsAssignableFrom<IEnumerable<DeviceModel>>(okResult.Value);
            Assert.Equal(expectedDeviceModels, actualDeviceModels);
        }

        [Fact]
        public async Task GetAvailableReturnsBadRequestDateExpired()
        {
            // Arrange
            var deviceServiceMock = new Mock<IDeviceService>();
            var fakeScheculeService = new Mock<FakeScheduleService>();

            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<DeviceController>>();

            var controller = new DeviceController(
                deviceServiceMock.Object,
                fakeScheculeService.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            var date = new DateOnly(2023, 11, 24);
            var scheduleId = 1;

            var expectedEntities = new List<Device>();
            var expectedDeviceModels = new List<DeviceModel>();

            deviceServiceMock.Setup(s => s.GetAvailableAsync(date, scheduleId))
                .ReturnsAsync(expectedEntities);
            mapperMock.Setup(m => m.Map<IEnumerable<DeviceModel>>(expectedEntities))
                .Returns(expectedDeviceModels);

            // Act
            var badRequestResult = await controller.GetAvailable(date, scheduleId);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(badRequestResult);
            var message = badResult.Value; // Get the message from the bad request result
            Assert.Equal("Date is expired", message);
        }

        [Fact]
        public async Task GetByIdWhenDeviceExistsReturnsOkResult()
        {
            // Arrange
            var deviceServiceMock = new Mock<IDeviceService>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<DeviceController>>();

            var deviceId = 1;
            var device = new Device { Id = deviceId, Amount = 1 };
            var deviceModel = new DeviceModel { Id = deviceId, Amount = 1 };

            deviceServiceMock.Setup(service => service.GetByIdAsync(deviceId))
                .ReturnsAsync(device);
            mapperMock.Setup(mapper => mapper.Map<DeviceModel>(device))
                .Returns(deviceModel);

            var controller = new DeviceController(deviceServiceMock.Object, null, mapperMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetById(deviceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDeviceModel = Assert.IsType<DeviceModel>(okResult.Value);
            Assert.Equal(deviceId, returnedDeviceModel.Id);
        }

        [Fact]
        public async Task GetByIdWhenDeviceDoesNotExistReturnsNotFoundResult()
        {
            // Arrange
            var deviceServiceMock = new Mock<IDeviceService>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<DeviceController>>();

            var deviceId = 1;

            deviceServiceMock.Setup(service => service.GetByIdAsync(deviceId))
                .ReturnsAsync((Device)null);

            var controller = new DeviceController(deviceServiceMock.Object, null, mapperMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetById(deviceId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByRoomReturnsOkResultWhenEntitiesExist()
        {
            // Arrange
            var roomNumber = 123;
            var expectedEntities = new List<Device>();

            var deviceServiceMock = new Mock<IDeviceService>();
            deviceServiceMock.Setup(service => service.GetByRoomAsync(roomNumber))
        .ReturnsAsync(expectedEntities);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<IEnumerable<DeviceModel>>(expectedEntities))
                .Returns(new List<DeviceModel>());

            var controller = new DeviceController(deviceServiceMock.Object, null, mapperMock.Object, null);

            // Act
            var result = await controller.GetByRoom(roomNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var entities = Assert.IsAssignableFrom<IEnumerable<DeviceModel>>(okResult.Value);
            Assert.Empty(entities);
        }

        [Fact]
        public async Task GetByRoomReturnsOkResultWhenNoEntitiesExist()
        {
            // Arrange
            var roomNumber = 123;

            var deviceServiceMock = new Mock<IDeviceService>();
            deviceServiceMock.Setup(service => service.GetByRoomAsync(roomNumber))
                .ReturnsAsync((List<Device>)null);

            var controller = new DeviceController(deviceServiceMock.Object, null, null, null);

            // Act
            var result = await controller.GetByRoom(roomNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var entities = Assert.IsAssignableFrom<IEnumerable<DeviceModel>>(okResult.Value);
            Assert.Empty(entities);
        }

        // Creates a new device with valid input data
        [Fact]
        public async Task TestCreateDeviceWithValidInputData()
        {
            // Arrange
            var deviceReq = new DeviceReq
            {
                Type = "Type",
                Name = "Name",
                Model = "Model",
                Description = "Description",
                Amount = 1,
                RoomNumber = 1
            };

            var mapperMock = new Mock<IMapper>();
            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(x => x.CreateAsync(It.IsAny<Device>(), It.IsAny<DeviceInfo>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            var controller = new DeviceController(mockDeviceService.Object, null, mapperMock.Object, null);

            // Act
            var result = await controller.Create(deviceReq);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var type = okResult.Value.GetType();
            var prop = type.GetProperty("CreatedId");
            var createdId = Assert.IsType<int>(prop.GetValue(okResult.Value));
            Assert.Equal(1, createdId);
        }

        // Returns a BadRequest status code when the input data is invalid
        [Fact]
        public async Task TestCreateReturnsBadRequestWithInvalidInputData()
        {
            // Arrange
            var deviceReq = new DeviceReq
            {
                Type = null, // Invalid input data
                Name = "Name",
                Model = "Model",
                Description = "Description",
                Amount = 1,
                RoomNumber = 1
            };

            var mapperMock = new Mock<IMapper>();
            var mockDeviceService = new Mock<IDeviceService>();

            var controller = new DeviceController(mockDeviceService.Object, null, mapperMock.Object, null);

            // Act
            var result = await controller.Create(deviceReq);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        // Returns a BadRequest status code when service give error
        [Fact]
        public async Task TestCreateReturnsBadRequestWhenDateIsExpired()
        {
            // Arrange
            var deviceReq = new DeviceReq
            {
                Type = "Type",
                Name = "Name",
                Model = "Model",
                Description = "Description",
                Amount = 1,
                RoomNumber = 1
            };

            var mapperMock = new Mock<IMapper>();


            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(x => x.CreateAsync(It.IsAny<Device>(), It.IsAny<DeviceInfo>(), It.IsAny<int>())).ReturnsAsync((int?)null);

            var controller = new DeviceController(mockDeviceService.Object, null, mapperMock.Object, null);

            // Act
            var result = await controller.Create(deviceReq);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        // Returns 200 OK when given a valid id and DeviceUpdateReq object
        [Fact]
        public async Task TestUpdateReturns200OkWithValidIdAndDeviceUpdateReq()
        {
            // Arrange
            int id = 1;
            var req = new DeviceUpdateReq {
                Id = id,
                Type = "Type",
                Name = "Name",
                Model = "Model",
                Description = "Description",
                Amount = 1,
                RoomNumber = 1
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(service => service.UpdateAsync(id, It.IsAny<Device>())).ReturnsAsync(true);

            var mockMapper = new Mock<IMapper>();

            var controller = new DeviceController(mockDeviceService.Object, null, mockMapper.Object, null);

            // Act
            var result = await controller.Update(id, req);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // Returns 404 Not Found when given an invalid id
        [Fact]
        public async Task TestUpdateReturns404NotFoundWithInvalidId()
        {
            // Arrange
            int id = 1;
            var req = new DeviceUpdateReq
            {
                Id = id,
                Type = "Type",
                Name = "Name",
                Model = "Model",
                Description = "Description",
                Amount = 1,
                RoomNumber = 1
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(service => service.UpdateAsync(id, It.IsAny<Device>())).ReturnsAsync(false);

            var mockMapper = new Mock<IMapper>();

            var controller = new DeviceController(mockDeviceService.Object, null, mockMapper.Object, null);

            // Act
            var result = await controller.Update(id, req);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Returns 400 Bad Request when id in DeviceUpdateReq object does not match id in route
        [Fact]
        public async Task TestUpdateReturns400BadRequestWhenIdNotMatchInDeviceUpdateReq()
        {
            // Arrange
            int id = 1;
            var req = new DeviceUpdateReq
            {
                Id = 2,
                Type = "Type",
                Name = "Name",
                Model = "Model",
                Description = "Description",
                Amount = 1,
                RoomNumber = 1
            }; ;

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(service => service.UpdateAsync(id, It.IsAny<Device>())).ReturnsAsync(true);

            var mockMapper = new Mock<IMapper>();

            var controller = new DeviceController(mockDeviceService.Object, null, mockMapper.Object, null);

            // Act
            var result = await controller.Update(id, req);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        // Deletes a device with a valid id and returns 200 OK.
        [Fact]
        public async Task DeleteValidIdReturns200Ok()
        {
            // Arrange
            int id = 1;
            var deviceServiceMock = new Mock<IDeviceService>();
            deviceServiceMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);
            var controller = new DeviceController(deviceServiceMock.Object, null, null, null);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // Returns 404 Not Found when trying to delete a device with an invalid id.
        [Fact]
        public async Task DeleteInvalidIdReturns404NotFound()
        {
            // Arrange
            int id = 1;
            var deviceServiceMock = new Mock<IDeviceService>();
            deviceServiceMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);
            var controller = new DeviceController(deviceServiceMock.Object, null, null, null);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
