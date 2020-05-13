using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Models.ViewModels
{

    #region "RPM Blood Sugar View Model Start"

    public class PatientBloodSugarReadingViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PatientId { get; set; }
        public List<PatientBloodSugarReadingLogBook> PatientLogBookList { get; set; }
        public PatientBloodSugarReadingDashboard DeviceReadingPatientDashboard { get; set; }
    }

    public class PatientBloodSugarReadingDashboard
    {
        public decimal? TotalTest { get; set; }
        public decimal? TotalDays { get; set; }
        public decimal? AverageTestPerDay { get; set; }
        public decimal? AverageGlucoseLevel { get; set; }
        public decimal? LowestGlucoseLevel { get; set; }
        public decimal? HighestGlucoseLevel { get; set; }



    }

    public class PatientBloodSugarReadingLogBook
    {
        public DateTime? Date_recorded { get; set; }
        public double Morning_12_to_10_AM { get; set; }
        public double Morning_10_to_3_PM { get; set; }
        public double Morning_3_to_9_PM { get; set; }
        public double Morning_9_to_12_AM { get; set; }
        public bool Before_meal { get; set; }

    }

    #endregion


}