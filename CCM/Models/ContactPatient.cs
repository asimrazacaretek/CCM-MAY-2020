using System;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CCM.Models
{
    public class TextHistory
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string   FromUser      { get; set; }
        public string   ToPhoneNumber { get; set; }
        public string   Message       { get; set; }
        public DateTime DateTime      { get; set; }

        public string   TwilioSid     { get; set; }
        public string   TwilioStatus  { get; set; }
        public string TwilioCallerId { get; set; }

     
    }

    public class EmailHistory
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string   FromUser         { get; set; }
        public string   ToEmail          { get; set; }
        public string   Message          { get; set; }
        public int      AttachmentsCount { get; set; }
        public DateTime DateTime         { get; set; }
        


    }
    public class CallHistoryViewModel
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string RecordingURL { get; set; }

        public string CallerId { get; set; }
        public string TwilioCallId { get; set; }

        public int PatientID { get; set; }

        public bool isVoiceMail { get; set; } = false;
    }
    public class CallHistory
    {
        public int       Id           { get; set; }
        public string    To           { get; set; }
        public string    From         { get; set; }
        public string    Status       { get; set; }
        public DateTime? StartTime    { get; set; }
        public DateTime? EndTime      { get; set; }
        public TimeSpan  Duration     { get; set; }
        public string    RecordingURL { get; set; }

        public string    CallerId     { get; set; }
        public string    TwilioCallId { get; set; }

        public int PatientID { get; set; }

        public string Direction { get; set; }

        public int LiaisonId { get; set; } = 0;

    }

    public class VoiceMailHistory
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string RecordingURL { get; set; }

        public string CallerId { get; set; }
        public string TwilioCallId { get; set; }

        public int PatientID { get; set; }

        public int LiaisonId { get; set; }
    }


    public class ContactPatientViewModel
    {
        public int?   PatientId { get; set; }
        public string FullName { get; set; }

        public PatientProfile_Contact PatientProfile_Contact { get; set; }

        public PatientProfile_UrgencyContact PatientProfile_UrgencyContact { get; set; }

        public List< PatientProfile_UrgencyContact> PatientProfile_UrgencyContacts { get; set; }

        public List<SecondaryDoctor> secondaryDoctors { get; set; }
        public string MobilePhoneNumber { get; set; }

        public string HomePhoneNumber { get; set; }
        public string WorkPhoneNumber { get; set; }
        public string EmergencyPhoneNumber { get; set; }
        public string CareTakerPhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string EmailTo { get; set; }

        [Required]
        public string EmailMessage { get; set; }

        public IEnumerable<HttpPostedFileBase> Attachments { get; set; }

        
    }
}