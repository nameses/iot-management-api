using iot_management_api.Entities.common;
using iot_management_api.Models;
using iot_management_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iot_xunit_tests.DeviceTests.FakeServices
{
    public class FakeScheduleService : IScheduleService
    {
        async Task<bool> IScheduleService.CheckDateSchedule(DateOnly date, int scheduleId)
        {
            return true;
        }

        Task<bool> IScheduleService.CheckUserAssignmentToSchedule(UserRole userRole, int userId, int scheduleId)
        {
            throw new NotImplementedException();
        }

        Task<Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>>?> IScheduleService.GetFullAsync(UserRole userRole, int userId, DateOnly date)
        {
            throw new NotImplementedException();
        }
    }
}
