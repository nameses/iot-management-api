using iot_management_api.Entities.common;

namespace iot_management_api.Helper
{
    public class StudyWeekService
    {
        public DateOnly GetMondayOfFirstWeek(DateOnly date)
        {
            var startYear = date.Year;
            if (date.Month<9)
                startYear--;
            var startDate = GetFirstMondayOfSeptember(startYear);

            //make date point to monday of this week
            if (date.DayOfWeek!=DayOfWeek.Monday)
            {
                date = date.AddDays(-7);
                while (date.DayOfWeek != DayOfWeek.Monday)
                    date = date.AddDays(1);
            }

            //find out current week enum
            WeekEnum? currentWeek = null;
            if (((date.ToDateTime(TimeOnly.MinValue)-startDate).Days/7)%2==0)
                currentWeek = WeekEnum.First;
            else currentWeek = WeekEnum.Second;

            //make date point to monday of first week
            if (currentWeek==WeekEnum.Second)
                date = date.AddDays(-7);

            return date;
        }

        public WeekEnum GetCurrentWeek()
        {
            var startYear = DateTime.Now.Year;
            if (DateTime.Now.Month<9)
                startYear--;
            var startDate = GetFirstMondayOfSeptember(startYear);

            var now = DateTime.Now;
            //make now point to monday of this week
            if (now.DayOfWeek!=DayOfWeek.Monday)
            {
                now = now.AddDays(-7);
                while (now.DayOfWeek != DayOfWeek.Monday)
                    now = now.AddDays(1);
            }
            //find out current week enum
            if (((now-startDate).Days/7)%2==0)
                return WeekEnum.First;
            else
                return WeekEnum.Second;
        }

        public DateTime GetFirstMondayOfSeptember(int year)
        {
            var septemberFirst = new DateTime(year, 9, 1);
            while (septemberFirst.DayOfWeek != DayOfWeek.Monday)
                septemberFirst = septemberFirst.AddDays(1);

            return septemberFirst;
        }

        public (WeekEnum, DayEnum) GetWeekDateEnums(DateOnly date)
        {
            var firstMonday = GetMondayOfFirstWeek(date);
            WeekEnum currentWeek = WeekEnum.First;
            DayEnum currentDay = DayEnum.Monday;
            int days = ((date.ToDateTime(TimeOnly.MinValue)-firstMonday.ToDateTime(TimeOnly.MinValue))).Days;

            if (days/7==1)
            {
                days-=7;
                currentWeek++;// = WeekEnum.Second;
            }
            if (days>7) throw new Exception("Exception during convertion from date to week and date enums");
            while (days>0)
            {
                currentDay++;
                days--;
            }

            return (currentWeek, currentDay);
        }
    }
}
