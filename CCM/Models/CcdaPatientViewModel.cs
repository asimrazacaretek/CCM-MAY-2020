using System;


namespace CCM.Models
{
    public class CcdaPatientViewModel
    {
        public Name Name { get; set; }
        public DateTime? Dob { get; set; }
        public string Gender { get; set; }
        public Address Address { get; set; }
        public Phone Phone { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public string SmokingStatus { get; set; }

        public int? PhysicianId { get; set; }

        public Allergy[] Allergies { get; set; }
        public Medication[] Medications { get; set; }
        public LabResult[] LabResults { get; set; }
        public Problem[] Problems { get; set; }
        public Procedure[] Procedures { get; set; }
        public Vital[] Vitals { get; set; }
    }

    public struct Vital
    {
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
    }

    public struct Procedure
    {
        public DateTime? Date { get; set; }
        public string Name { get; set; }
    }

    public struct Problem
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public struct LabResult
    {
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public string TestValue { get; set; }
    }

    public struct Medication
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string DoseValue { get; set; }
        public string DoseUnit { get; set; }
        public string RateValue { get; set; }
        public string RateUnit { get; set; }
        public string RxCui { get; set; }
        public string Route { get; set; }
        public string Reason { get; set; }
        public string DoseRepetitionTime { get; set; }
        public string PrescribeBy { get; set; }
        public string ForHowLong { get; set; }
    }

    public struct Allergy
    {
        public DateTime? StartDate { get; set; }
        public string Name { get; set; }
        public string Severity { get; set; }
    }

    public struct Name
    {
        public string Prefix { get; set; }
        public string[] Given { get; set; }
        public string Family { get; set; }
    }

    public struct Address
    {
        public string[] Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public struct Phone
    {
        public string Home { get; set; }
        public string Work { get; set; }
        public string Mobile { get; set; }
    }
}