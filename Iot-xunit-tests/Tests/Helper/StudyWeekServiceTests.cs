using iot_management_api.Entities.common;
using iot_management_api.Helper;

namespace Iot_xunit_tests.Tests.Helper
{
    public class StudyWeekServiceTests
    {
        [Theory]
        [InlineData("2023-11-06", "2023-10-30")]
        [InlineData("2023-11-09", "2023-10-30")]
        [InlineData("2023-11-13", "2023-11-13")]
        [InlineData("2023-11-20", "2023-11-13")]
        public void GetMondayOfFirstWeek_ReturnsCorrectWeekEnum(string inputDate, string expectedMonday)
        {
            // Arrange
            var date = DateOnly.Parse(inputDate);
            var service = new StudyWeekService();

            // Act
            var result = service.GetMondayOfFirstWeek(date);

            // Assert
            Assert.Equal(DateOnly.Parse(expectedMonday), result);
        }

        [Theory]
        [InlineData("2023-09-04", WeekEnum.First)]
        [InlineData("2023-09-10", WeekEnum.First)]
        [InlineData("2023-09-11", WeekEnum.Second)]
        [InlineData("2023-09-17", WeekEnum.Second)]
        [InlineData("2023-09-18", WeekEnum.First)]
        [InlineData("2023-09-24", WeekEnum.First)]
        [InlineData("2023-11-12", WeekEnum.Second)]
        public void GetCurrentWeek_ReturnsCorrectWeekEnum(string inputDate, WeekEnum expectedWeek)
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Parse(inputDate));
            var service = new StudyWeekService();

            // Act
            var result = service.GetCurrentWeek(date);

            // Assert
            Assert.Equal(expectedWeek, result);
        }

        [Theory]
        [InlineData("2023-11-06", WeekEnum.Second, DayEnum.Monday)]
        [InlineData("2023-11-11", WeekEnum.Second, DayEnum.Saturday)]
        [InlineData("2023-11-13", WeekEnum.First, DayEnum.Monday)]
        [InlineData("2023-11-14", WeekEnum.First, DayEnum.Tuesday)]
        [InlineData("2023-11-26", WeekEnum.Second, DayEnum.Sunday)]
        public void GetWeekDateEnums_ReturnsCorrectWeekEnum(string inputDate, WeekEnum expectedWeek, DayEnum expectedDay)
        {
            // Arrange
            var date = DateOnly.Parse(inputDate);
            var service = new StudyWeekService();

            // Act
            var result = service.GetWeekDateEnums(date);

            // Assert
            Assert.Equal((expectedWeek, expectedDay), result);
        }

        [Theory]
        [InlineData(2023, 9, 4)]
        [InlineData(2022, 9, 5)]
        [InlineData(2021, 9, 6)]
        public void GetFirstMondayOfSeptember_ReturnsCorrectDate(int year, int expectedMonth, int expectedDay)
        {
            // Arrange
            var service = new StudyWeekService();

            // Act
            var result = service.GetFirstMondayOfSeptember(year);

            // Assert
            Assert.Equal(new DateTime(year, expectedMonth, expectedDay), result);
        }
    }
}
