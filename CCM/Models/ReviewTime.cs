using CCM.Models.CCMBILLINGS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace CCM.Models
{
    public class LiaisonDashBoardViewModel
    {
        public string JsonData { get; set; }
        public List<LiaisonDashBoard> liaisonDashBoards { get; set; }
        public List<LiaisonDashBoard> liaisonDashBoards1 { get; set; }
        public List<LiaisonDashBoard> liaisonDashBoardsGetPatientReject { get; set; }

        public List<LiaisonDashBoard> liaisonDashBoardsTwilio { get; set; }
        public int LiasionID { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Datestr { get; set; }


        public string LiaisonName { get; set; }
    }
    public class LiaisonDashBoard
    {
        public string Page { get; set; }
        public string TotalTime { get; set; }

        //// just For patient 
        public string patientId { get; set; }
        public string rejectedCount { get; set; }
        //// end

        public string TotalHours { get; set; }

        public string DataType { get; set; }

        public int ShowHide { get; set; }
    }
    public class ReviewTimeCcm
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan ReviewTime { get; set; }
        public string Page { get; set; }

        public string Activity { get; set; }

        public int Cycle { get; set; } = 0;

        public string RollupFrom { get; set; }
        public string RolledupTo { get; set; }
        public bool IsLocked { get; set; }
        [ForeignKey("BillingCategory")]
        public int? BillingcategoryId { get; set; }    
        public BillingCategory BillingCategory { get; set; }
        public int? BillingCodeId { get; set; }


    }

    public class ReviewTimeViewModel
    {
        public int? PatientId { get; set; }
        public int? Cycle { get; set; }
        public List<ReviewTimeCcm> ReviewTimeCcm { get; set; }

        public TimeSpan TotalReviewTime  { get; set; }
        public TimeSpan ReviewTimeCycle1 { get; set; }
        public TimeSpan ReviewTimeCycle2 { get; set; }

        public string BillingCode1 { get; set; }
        public string BillingCode2 { get; set; }

        public List<ReviewTimeCycle> ReviewTimeCycles { get; set; }

        public List<BillingCycleDetails> BillingCycleDetails { get; set; }

        public List<BillingCycleComments> BillingCycleComments { get; set; }

        public List<BillingCycle> billingCycles { get; set; }
        public int? BillingCategoryId { get; set; }
        public int? BillingCodeId { get; set; }

    }

    public class ReviewTimeCycle
    {
        public int CycleId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<TimeSpan> ReviewTimes { get; set; }

        public List<ReviewTimeCcm> ReviewTimeCycleCcms { get; set; }

        public TimeSpan TotalReviewTime {
            get { return ReviewTimes.Any()
                ? ReviewTimes.Aggregate(TimeSpan.Zero, (sum, reviewTime) => sum + reviewTime)
                : TimeSpan.Zero;
            }
        }
    }

    public class ClinicTiming
    {
        public int ID { get; set; } = 0;
        public int ClinicID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string WeekDayName { get; set; }
        public bool isHoliday { get; set; } = false;
        public Boolean IsDeleted { get; set; }
        public string ClinicTimingStr { get; set; }
    }
    public class DoctorSchedule
    {

        public int ID { get; set; }
        public int? LiaisonID { get; set; }

        public int ClinicID { get; set; }

        public DateTime ScheduleValidTill { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime ScheduleValidFrom { get; set; }
        public List<DoctorScheduleDeatil> DoctorScheduleDeatils { get; set; }
        public virtual Liaison Liaison { get; set; }

        public int? SaleStaffID { get; set; }

        public virtual SaleStaff SaleStaff { get; set; }



    }
    public class Clinic
    {
        public int ID { get; set; } = 0;
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Web { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Photo { get; set; }
        public string NPI { get; set; }
        public string IntegraTaxID { get; set; }
        public Boolean IsDeleted { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string SecondaryContactName { get; set; }
        public string SecondaryContactNumber { get; set; }
    }
    public class PhysicianHolidays
    {

        public int ID { get; set; }

        public int? LiaisonID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: MM/dd/yyyy}")]
        public DateTime? HDate { get; set; }
        public int? ClinicID { get; set; }
        public string Remarks { get; set; }
        public Liaison Liaison { get; set; }
        public int? SaleStaffID { get; set; }

        public virtual SaleStaff SaleStaff { get; set; }
    }
    public class DoctorScheduleDeatil
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ScheduleID { get; set; }
        public string WeekDayName { get; set; }
        public Boolean IsDeleted { get; set; }
        public DoctorSchedule DoctorSchedule { get; set; }

    }
    public class PatientAppointment
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? LiaisonID { get; set; }
        public int? PatientID { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        public Boolean IsDeleted { get; set; }
        public int? ClinicID { get; set; }
        public string AptStatus { get; set; }
        public Patient Patient { get; set; }
        public Liaison Liaison { get; set; }

        [ForeignKey("BillingCategory")]
        public int? BillingCategoryId { get; set; }

        public BillingCategory BillingCategory { get; set; }
        public int? SaleStaffID { get; set; }

        public virtual SaleStaff SaleStaff { get; set; }
    }
    public class ClinicTimingViewModel
    {
        public int ID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int ClinicID { get; set; }
        public string WeekDayName { get; set; }
        public bool isHoliday { get; set; } = false;
        public string ClinicTimingStr { get; set; }
    }
    public class MapDoctorClinicViewModel
    {
        public int ClinicID { get; set; }
        public Clinic Clinic { get; set; }
        public List<Physician> Doctors { get; set; }
        public List<MapDoctorViewModel> SelectedDoctors { get; set; }


    }
    public class Doctorlist
    {
        public string FullName { get; set; }
        public int Value { get; set; }
    }
    public class MapDoctorViewModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NPI { get; set; }
        public bool IsSelected { get; set; }

    }
    public class PatientAppointmentViewModel
    {
        public int ID { get; set; }

        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PatientName { get; set; }
        public int DoctorID { get; set; }

        public int? PatientID { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        public Boolean IsDeleted { get; set; }
        public string AptStatus { get; set; }

        public bool isMigrated { get; set; } = false;

    }
    public class DoctorTiming
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? LiaisonID { get; set; }
        public string WeekDayName { get; set; }
        public Boolean IsDeleted { get; set; }
        public virtual Liaison Liaison { get; set; }
        public int? ClinicID { get; set; }
        
        public int? SaleStaffID { get; set; }

        public virtual SaleStaff SaleStaff { get; set; }
    }
    public class EditViewModel
    {
        public int SheduleId { get; set; }
        public string ScheduleValidTill { get; set; }
        public string ScheduleValidFrom { get; set; }

        public List<AddUpdateViewModel> list { get; set; }

    }
    public class AddUpdateViewModel
    {
        public int DetailId { get; set; }
        public int SheduleID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int ScheduleID { get; set; }
        public string WeekDayName { get; set; }
        public Boolean IsDeleted { get; set; }
        public int SheduleDetailID { get; set; }
        public int DoctorID { get; set; }
        public int ClinicID { get; set; }
        public DateTime ScheduleValidTill { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsHollyDay { get; set; }

        public string UpdatedBy { get; set; }

    }
}