using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCM.Models
{
    public class CAccount
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime DateOfTransmission { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }
        public DateTime ReconciliationDate { get; set; }
    }

    public class CAccountViewModel
    {
        public int PatientId { get; set; }
        public int PhysicianId { get; set; }
        public int CPTId { get; set; }

        [Display(Name = "Name Of Patient")]
        public string PatientName{ get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime? DOB { get; set; }
        [Display(Name = "Date Of Service")]
        public DateTime? DOS { get; set; }
        public DateTime? NextAppointment { get; set; }

        public byte[] PatientPhoto { get; set; }
        public byte[] LiaisonPhoto { get; set; }
        public byte[] PhysicianPhoto { get; set; }

        [Display(Name = "Patient's Full Name")]
        public string PatientFullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Home Phone Number")]
        public string HomePhoneNumber { get; set; }

        [Display(Name = "Mobile Phone Number")]
        public string MobilePhoneNumber { get; set; }

        public FinalCarePlanNotes FinalCarePlan { get; set; }
        public List<SecondaryDoctor> SecondaryDoctors { get; set; }
        public PatientProfile_UrgencyContact UrgencyContact { get; set; }
        public List<PatientMedicalHistory_MedicationRx> MedicationRx { get; set; }
    }
}