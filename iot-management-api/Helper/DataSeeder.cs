using iot_management_api.Entities;
using iot_management_api.Entities.common;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Helper
{
    public static class DataSeeder
    {
        public static void SeedData(ModelBuilder builder)
        {
            builder.Entity<DayMapping>().HasData(
                new DayMapping
                {
                    Id = 1,
                    SubjectNumber = 1,
                    StartTime = new TimeOnly(8, 30),
                    EndTime = new TimeOnly(10, 5),
                }, new DayMapping
                {
                    Id = 2,
                    SubjectNumber = 2,
                    StartTime = new TimeOnly(10, 25),
                    EndTime = new TimeOnly(12, 0),
                }, new DayMapping
                {
                    Id = 3,
                    SubjectNumber = 3,
                    StartTime = new TimeOnly(12, 20),
                    EndTime = new TimeOnly(13, 55),
                }, new DayMapping
                {
                    Id = 4,
                    SubjectNumber = 4,
                    StartTime = new TimeOnly(14, 15),
                    EndTime = new TimeOnly(16, 0),
                }, new DayMapping
                {
                    Id = 5,
                    SubjectNumber = 5,
                    StartTime = new TimeOnly(16, 10),
                    EndTime = new TimeOnly(18, 20),
                }
            );

            builder.Entity<Period>().HasData(
                new Period
                {
                    Id = 1,
                    Day = DayEnum.Monday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 1
                }, new Period
                {
                    Id = 2,
                    Day = DayEnum.Tuesday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 2
                }, new Period
                {
                    Id = 3,
                    Day = DayEnum.Wednesday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 4
                }, new Period
                {
                    Id = 4,
                    Day = DayEnum.Thursday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 3
                }, new Period
                {
                    Id = 5,
                    Day = DayEnum.Friday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 5
                }, new Period
                {
                    Id = 6,
                    Day = DayEnum.Saturday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 1
                }, new Period
                {
                    Id = 7,
                    Day = DayEnum.Monday,
                    Week = WeekEnum.Second,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 1
                }, new Period
                {
                    Id = 8,
                    Day = DayEnum.Tuesday,
                    Week = WeekEnum.Second,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 2
                }, new Period
                {
                    Id = 9,
                    Day = DayEnum.Wednesday,
                    Week = WeekEnum.Second,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 1
                }, new Period
                {
                    Id = 10,
                    Day = DayEnum.Thursday,
                    Week = WeekEnum.Second,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 4
                }, new Period
                {
                    Id = 11,
                    Day = DayEnum.Friday,
                    Week = WeekEnum.Second,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 3
                }, new Period
                {
                    Id = 12,
                    Day = DayEnum.Saturday,
                    Week = WeekEnum.Second,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 2
                }, new Period
                {
                    Id = 13,
                    Day = DayEnum.Tuesday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 5
                }, new Period
                {
                    Id = 14,
                    Day = DayEnum.Tuesday,
                    Week = WeekEnum.First,
                    Semester = SemesterEnum.First,
                    Year = DateTime.Now.Year,
                    Lable = "",
                    DayMappingId = 4
                }
            );

            builder.Entity<Group>().HasData(
                new Group
                {
                    Id = 1,
                    GroupCode = "TV-12",
                    Term = 3
                }, new Group
                {
                    Id = 2,
                    GroupCode = "TR-22",
                    Term = 4
                }, new Group
                {
                    Id = 3,
                    GroupCode = "IT-31",
                    Term = 1
                }, new Group
                {
                    Id = 4,
                    GroupCode = "TK-46",
                    Term = 2
                }, new Group
                {
                    Id = 5,
                    GroupCode = "IR-88",
                    Term = 1
                }, new Group
                {
                    Id = 6,
                    GroupCode = "RT-45",
                    Term = 1
                }
            );

            builder.Entity<Room>().HasData(
                new Room()
                {
                    Id = 1,
                    Number = 101,
                    Floor = 1
                }, new Room()
                {
                    Id = 2,
                    Number = 102,
                    Floor = 1
                }, new Room()
                {
                    Id = 3,
                    Number = 202,
                    Floor = 2
                }, new Room()
                {
                    Id = 4,
                    Number = 301,
                    Floor = 3
                }, new Room()
                {
                    Id = 5,
                    Number = 401,
                    Floor = 4
                }
            );

            builder.Entity<DeviceInfo>().HasData(
                new DeviceInfo()
                {
                    Id = 1,
                    Type = "Microcontroller",
                    Name = "Arduino",
                    Model = "RT-32",
                    Description = ""
                },
                new DeviceInfo()
                {
                    Id = 2,
                    Type = "Microcontroller",
                    Name = "Arduino",
                    Model = "RT-12",
                    Description = ""
                },
                new DeviceInfo()
                {
                    Id = 3,
                    Type = "Microcontroller",
                    Name = "Arduino",
                    Model = "RT-22",
                    Description = ""
                }, new DeviceInfo()
                {
                    Id = 4,
                    Type = "Diode",
                    Name = "Diod",
                    Model = "Q-23",
                    Description = ""
                }, new DeviceInfo()
                {
                    Id = 5,
                    Type = "Diode",
                    Name = "Diod",
                    Model = "B-56",
                    Description = ""
                }, new DeviceInfo()
                {
                    Id = 6,
                    Type = "Cable",
                    Name = "Сopper cable",
                    Model = "P-82",
                    Description = ""
                }, new DeviceInfo()
                {
                    Id = 7,
                    Type = "Speaker",
                    Name = "Sony",
                    Model = "VT-66",
                    Description = ""
                }
            );
            builder.Entity<Device>().HasData(
                new Device
                {
                    Id = 1,
                    Amount = 16,
                    DeviceInfoId = 1,
                    RoomId = 1
                }, new Device
                {
                    Id = 2,
                    Amount = 16,
                    DeviceInfoId = 1,
                    RoomId = 2
                }, new Device
                {
                    Id = 3,
                    Amount = 16,
                    DeviceInfoId = 2,
                    RoomId = 3
                }, new Device
                {
                    Id = 4,
                    Amount = 16,
                    DeviceInfoId = 3,
                    RoomId = 4
                }, new Device
                {
                    Id = 5,
                    Amount = 8,
                    DeviceInfoId = 7,
                    RoomId = 5
                }, new Device
                {
                    Id = 6,
                    Amount = 5,
                    DeviceInfoId = 6,
                    RoomId = 2
                }, new Device
                {
                    Id = 7,
                    Amount = 9,
                    DeviceInfoId = 5,
                    RoomId = 2
                }
            );
        }
    }
}
