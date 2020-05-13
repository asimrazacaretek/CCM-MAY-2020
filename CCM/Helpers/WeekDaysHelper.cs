using CCM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public static class WeekDaysHelper
    {
        internal static List<string> GetWeekDaysName()
        {
            return new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        }
        public static string DateFormat = "MM/dd/yyyy";
        public static string TimeFormat = "HH:mm tt";
        public static string ClinicStartTime = "9:00 AM";
        public static string ClinicEndTime = "5:00 PM";
        internal static List<ClinicTiming> GetClinincalTiming()
        {
            List<ClinicTiming> clinicTimingList = new List<ClinicTiming>();
            foreach (var item in GetWeekDaysName())
            {
                ClinicTiming ct = new ClinicTiming()
                {
                    WeekDayName = item,
                    StartTime = Convert.ToDateTime(DateTime.Now.ToString(DateFormat) + " " + ClinicStartTime, CultureInfo.InvariantCulture),
                    EndTime = Convert.ToDateTime(DateTime.Now.ToString(DateFormat) + " " + ClinicEndTime, CultureInfo.InvariantCulture)
                };
                clinicTimingList.Add(ct);
            }
            return clinicTimingList;
        }

        internal static List<ClinicTimingViewModel> GetClinicalTimingWithViewModel(List<DoctorTiming> DoctorTiming)
        {
            List<ClinicTimingViewModel> clinicTimingViewModelsList = new List<ClinicTimingViewModel>();
            if (DoctorTiming!=null) {
                foreach (var item in DoctorTiming)
                {
                    ClinicTimingViewModel vm = new ClinicTimingViewModel();
                    vm.ID = item.ID;
                    vm.StartTime = item.StartTime.ToString(TimeFormat);
                    vm.EndTime = item.EndTime.ToString(TimeFormat);
                    vm.WeekDayName = item.WeekDayName;
                    vm.ClinicTimingStr = item.StartTime.ToString(TimeFormat) + "-" + item.EndTime.ToString(TimeFormat);

                    clinicTimingViewModelsList.Add(vm);
                }
               
            }
            var lstnotinlist = GetWeekDaysName().Where(p => !DoctorTiming.Any(p2 => p2.WeekDayName == p)).ToList();
            foreach (var item in lstnotinlist)
            {
                ClinicTimingViewModel vm = new ClinicTimingViewModel();
                vm.ID = 0;
                string one = DateTime.Now.ToString(DateFormat);
                vm.StartTime = Convert.ToDateTime(one + " " + ClinicStartTime).ToString(TimeFormat);
                vm.EndTime = Convert.ToDateTime(one + " " + ClinicEndTime).ToString(TimeFormat);
                vm.isHoliday = true;
                vm.WeekDayName = item;
                clinicTimingViewModelsList.Add(vm);
            }
            return clinicTimingViewModelsList;
        }

        
    }
}