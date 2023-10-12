using iot_management_api.Entities;
using iot_management_api.Entities.common;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Helper
{
    public class DataSeeder
    {
        private readonly Encrypter _encrypter;

        public DataSeeder(Encrypter encrypter)
        {
            _encrypter=encrypter;
        }

        public void SeedData(ModelBuilder builder)
        {
            var dayMappingList = new List<DayMapping>
            {
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
            };
            builder.Entity<DayMapping>().HasData(dayMappingList);

            var periods = new List<Period>();
            var year = DateTime.Now.Year;
            foreach (DayEnum day in Enum.GetValues(typeof(DayEnum)))
            {
                foreach (WeekEnum week in Enum.GetValues(typeof(WeekEnum)))
                {
                    for (int dayMappingId = 1; dayMappingId <= 5; dayMappingId++)
                    {
                        periods.Add(new Period
                        {
                            Id = periods.Count + 1,
                            Day = day,
                            Week = week,
                            Semester = SemesterEnum.First,
                            Year = year,
                            Lable = "",
                            DayMappingId = dayMappingId
                        });
                    }
                }
            }
            builder.Entity<Period>().HasData(periods);

            var groupList = new List<Group>
            {
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
            };
            builder.Entity<Group>().HasData(groupList);

            var roomList = new List<Room>
            {
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
                    Number = 201,
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
                    Floor = 1
                }, new Room()
                {
                    Id = 6,
                    Number = 103,
                    Floor = 1
                }, new Room()
                {
                    Id = 7,
                    Number = 104,
                    Floor = 1
                }, new Room()
                {
                    Id = 8,
                    Number = 202,
                    Floor = 2
                }, new Room()
                {
                    Id = 9,
                    Number = 203,
                    Floor = 2
                }, new Room()
                {
                    Id = 10,
                    Number = 204,
                    Floor = 2
                }
            };
            builder.Entity<Room>().HasData(roomList);

            var deviceInfoList = new List<DeviceInfo>
            {
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
            };
            builder.Entity<DeviceInfo>().HasData(deviceInfoList);

            var deviceList = new List<Device>
            {
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
                    RoomId = 6
                }, new Device
                {
                    Id = 7,
                    Amount = 9,
                    DeviceInfoId = 5,
                    RoomId = 7
                }, new Device
                {
                    Id = 8,
                    Amount = 6,
                    DeviceInfoId = 2,
                    RoomId = 8
                }, new Device
                {
                    Id = 9,
                    Amount = 11,
                    DeviceInfoId = 1,
                    RoomId = 9
                }, new Device
                {
                    Id = 10,
                    Amount = 9,
                    DeviceInfoId = 2,
                    RoomId = 8
                }, new Device
                {
                    Id = 11,
                    Amount = 15,
                    DeviceInfoId = 3,
                    RoomId = 7
                }, new Device
                {
                    Id = 12,
                    Amount = 12,
                    DeviceInfoId = 4,
                    RoomId = 6
                }
            };
            builder.Entity<Device>().HasData(deviceList);

            var teacherList = new List<Teacher>
            {
                new Teacher
                {
                    Id = 1,
                    Name = "Edvin",
                    Surname = "Brown",
                    Email = "edvin.brown@gmail.com",
                    Password = _encrypter.Encrypt("edvin.brown@gmail.com"),
                    CreatedAt = DateTime.Now
                }, new Teacher
                {
                    Id = 2,
                    Name = "Kilden",
                    Surname = "Egregor",
                    Email = "kildren.egregor@gmail.com",
                    Password = _encrypter.Encrypt("kildren.egregor@gmail.com"),
                    CreatedAt = DateTime.Now
                }, new Teacher
                {
                    Id = 3,
                    Name = "Gregor",
                    Surname = "Blue",
                    Email = "gregor.blue@gmail.com",
                    Password = _encrypter.Encrypt("gregor.blue@gmail.com"),
                    CreatedAt = DateTime.Now
                }, new Teacher
                {
                    Id = 4,
                    Name = "Mike",
                    Surname = "Peterson",
                    Email = "mike.peterson@gmail.com",
                    Password = _encrypter.Encrypt("mike.peterson@gmail.co"),
                    CreatedAt = DateTime.Now
                }, new Teacher
                {
                    Id = 5,
                    Name = "Peter",
                    Surname = "Lomakin",
                    Email = "peter.lomakin@gmail.com",
                    Password = _encrypter.Encrypt("peter.lomakin@gmail.com"),
                    CreatedAt = DateTime.Now
                }, new Teacher
                {
                    Id = 6,
                    Name = "Oleksandr",
                    Surname = "Dodokin",
                    Email = "oleksandr.dodokin@gmail.com",
                    Password = _encrypter.Encrypt("oleksandr.dodokin@gmail.com"),
                    CreatedAt = DateTime.Now
                }
            };
            builder.Entity<Teacher>().HasData(teacherList);

            var subjectList = new List<Subject>
            {
                new Subject
                {
                    Id=1,
                    TeacherId=1,
                    Name = "Introduction to Physics",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=2,
                    TeacherId=1,
                    Name = "Introduction to Physics",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=3,
                    TeacherId=2,
                    Name = "Functional programming",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=4,
                    TeacherId=2,
                    Name = "Functional programming",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=5,
                    TeacherId=3,
                    Name = "Introduction to Computer Science",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=6,
                    TeacherId=3,
                    Name = "Introduction to Computer Science",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=7,
                    TeacherId=4,
                    Name = "Algorithms and Data Structures",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=8,
                    TeacherId=4,
                    Name = "Algorithms and Data Structures",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=9,
                    TeacherId=5,
                    Name = "Advanced Algorithms",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=10,
                    TeacherId=5,
                    Name = "Advanced Algorithms",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=11,
                    TeacherId=1,
                    Name = "Advanced Physics",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=12,
                    TeacherId=1,
                    Name = "Advanced Physics",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=13,
                    TeacherId=2,
                    Name = "Advanced Functional programming",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=14,
                    TeacherId=2,
                    Name = "Advanced Functional programming",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=15,
                    TeacherId=3,
                    Name = "Computer Science",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=16,
                    TeacherId=3,
                    Name = "Computer Science",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=17,
                    TeacherId=4,
                    Name = "Data Science",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=18,
                    TeacherId=4,
                    Name = "Data Science",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=19,
                    TeacherId=5,
                    Name = "SQL Data Bases",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=20,
                    TeacherId=5,
                    Name = "SQL Data Bases",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=21,
                    TeacherId=6,
                    Name = "Async Programming",
                    Type= SubjectType.Lecture
                }, new Subject
                {
                    Id=22,
                    TeacherId=6,
                    Name = "Async Programming",
                    Type= SubjectType.Practice
                }, new Subject
                {
                    Id=23,
                    TeacherId=6,
                    Name = "English. Teachical skills",
                    Type= SubjectType.Practice
                }
            };
            builder.Entity<Subject>().HasData(subjectList);

            var groupSchedule = new List<GroupSchedule>();
            var schedules = new List<Schedule>();
            var random = new Random();
            int i = 1;
            foreach (var group in groupList)
            {
                var numToCreate = random.Next(20, 26);

                var groupPeriods = periods.OrderBy(p => random.Next())
                    .Take(numToCreate)
                    .ToList();

                //create rnd subject list
                var sList = subjectList.OrderBy(p => random.Next())
                    .Take(numToCreate)
                    .ToList();

                foreach ((var s, var p) in sList.Zip(groupPeriods, Tuple.Create))
                {
                    groupSchedule.Add(new GroupSchedule { Id=i, GroupId=group.Id, ScheduleId=i });
                    schedules.Add(new Schedule
                    {
                        Id = i++,
                        SubjectId = s.Id,
                        PeriodId = p.Id,
                        RoomId = roomList[random.Next(roomList.Count)].Id
                    });
                }
            }
            builder.Entity<Schedule>().HasData(schedules);
            builder.Entity<GroupSchedule>().HasData(groupSchedule);

        }
    }
}
