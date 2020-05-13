

using System.Collections.Generic;

namespace CCM.Models
{
    public class LiaisonHourlyPerformance
    {
        public string HourName { get; set; }
        public int Hour { get; set; }

        public List<MinuteandType> minuteandTypes { get; set; }

        public List<MinuteandType> minuteandTypesPercent { get; set; }

    }
    
    public class LiasionCallHistory
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }

       public List<MonthNames> MonthNames { get; set; }

       

        
    }
    public class MonthNames
    {
        public string MonthName { get; set; }
        public string YearName { get; set; }
        public List<CallDurationandType> callDurationandTypes { get; set; }
    }
    public class GraphData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalCost { get; set; }
        public int TotalVisit { get; set; }
        public decimal Average { get; set; }

    }
    public class CallDurationandType
    {
        public string CallType { get; set; }
        public string CallDuration { get; set; } = "";

        public  string ClassName { get; set; }
    }
    public class ActiveWorkQueTotals
    {
        public string CycleType { get; set; }
        public string colorname { get; set; }

        public List<MinuteandType> MonthNames { get; set; } = new List<MinuteandType>();
        public int TotalCounts { get; set; }

       

    }
    public class MinuteandType
    {
        public int Minute { get; set; }
        public string Type { get; set; }
        public int MinutesOnly { get; set; }
        public string isCarePlanSubmitted { get; set; } = "";
    }
    public class NewDashBoardViewModel
    {
        public string StatusName { get; set; }
        public int Totalcount { get; set; }
        public int Totalcount1 { get; set; }
        public int BillingCategoryID { get; set; }

        public string  BillingCategoryName { get; set; }

        public List<NewDashBoardViewModel> SubStatuses { get; set; }

        public DashBoardViewModel dashBoardView { get; set; }
    }
    public class DashBoardViewModel
    {
        public string UserId { get; set; }
        public int TotalPatients { get; set; }
        public int TotalPatients1 { get; set; }

        // Initial Enrollment Status   
        public int NotEligible        { get; set; }
        public int NotEnrolled         { get; set; }
        public int InProgress          { get; set; }
        public int Refused             { get; set; }
        public int Deceased            { get; set; }
        public int NotQualified        { get; set; }
        public int InvalidPhoneNumber  { get; set; }
        public int PatientNotSeeingMd  { get; set; }
        public int LeftVoiceMessage    { get; set; }

        public int Assigned            { get; set; }
        public int NotAssigned         { get; set; }

        // After Enrollment - CCM Status
        public int InCcm               { get; set; }
        public int InClinicalSignOff   { get; set; }
        public int InClaimSubmission   { get; set; }
        public int InReconciliation    { get; set; }

        public decimal? TotalRevenue   { get; set; }
        public decimal? YearlyEarnings { get; set; }
        public int      CallsDueToday  { get; set; }
        public int    CallsDueTomorrow { get; set; }
        public int      PastCallDues   { get; set; }

        // Billing Codes
        public int Code99490           { get; set; }
        public int Code99487           { get; set; }
        public int Code99489           { get; set; }
        public string Datestr { get; set; }
        public List<NewDashBoardViewModel> newDashBoardViewModels { get; set; }

        //Language
        public string PreferredLanguage { get; set; }
    }
}