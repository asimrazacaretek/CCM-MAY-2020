using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CCM.Models.CCMBILLINGS;
using CCM.Models.CCMBILLINGS.ViewModels;
using CCM.Models.DataModels;
using CCM.Models.DataModels.Devices;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace CCM.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public int? CCMid { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim("FirstName", FirstName));
            userIdentity.AddClaim(new Claim("LastName", LastName));

            return userIdentity;
        }
    }


    public class ApplicationdbContect : IdentityDbContext<ApplicationUser>
    {
        public ApplicationdbContect() : base("myCCMhealthDB", throwIfV1Schema: false)
        {

            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }
        public ApplicationdbContect(string connectionstring) : base(connectionstring, throwIfV1Schema: false)
        {

            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        public static ApplicationdbContect Create()
        {
            return new ApplicationdbContect();
        }


        // Liaison
        public DbSet<Liaison> Liaisons { get; set; }
        public DbSet<ReviewTimeCcm> ReviewTimeCcms { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }

        public DbSet<Liaison_CPTRates> Liaison_CPTRates { get; set; }

        // Physician
        public DbSet<Physician> Physicians { get; set; }
        public DbSet<PhysiciansGroup> PhysiciansGroup { get; set; }

        public DbSet<Physician_CPTRates> Physician_CPTRates { get; set; }

        // Patient
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Patient_ChronicCondition1> Patient_ChronicCondition1s { get; set; }
        public DbSet<Patient_ChronicCondition2> Patient_ChronicCondition2s { get; set; }
        public DbSet<DoctorVisit> DoctorVisits { get; set; }
        public DbSet<SecondaryDoctor> SecondaryDoctors { get; set; }
        public DbSet<CarePlan> CarePlans { get; set; }
        public DbSet<FinalCarePlanNotes> FinalCarePlanNotes { get; set; }
        public DbSet<FinalCarePlanNotes_History> FinalCarePlanNotes_Histories { get; set; }

        public DbSet<MessageNotification> MessageNotifications { get; set; }
        public DbSet<TextHistory> TextHistories { get; set; }
        public DbSet<EmailHistory> EmailHistories { get; set; }
        public DbSet<CallHistory> CallHistories { get; set; }

        // CCDA Tables
        public DbSet<CcdaLabResult> CcdaLabResults { get; set; }
        public DbSet<CcdaAllergy> CcdaAllergies { get; set; }
        public DbSet<CcdaProblem> CcdaProblems { get; set; }
        public DbSet<CcdaVital> CcdaVitals { get; set; }
        public DbSet<CcdaProcedure> CcdaProcedures { get; set; }

        // Patient Profile
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<PatientProfile_Contact> PatientProfile_Contact { get; set; }
        public DbSet<PatientProfile_Address> PatientProfile_Addresses { get; set; }
        public DbSet<PatientProfile_UrgencyContact> PatientProfile_UrgencyContacts { get; set; }
        public DbSet<PatientProfile_Insurance> PatientProfile_Insurance { get; set; }
        public DbSet<CCM.Models.PatientProfile_ProfesionalCareProvider> PatientProfile_ProfesionalCareProvider { get; set; }
        public DbSet<CCM.Models.PatientProfile_DieseaseState> PatientProfile_DieseaseState { get; set; }
        public DbSet<CCM.Models.PatientProfile_Consent> PatientProfile_Consent { get; set; }
        public DbSet<CCM.Models.Patient_Images> patient_Images { get; set; }
        public DbSet<CCM.Models.PatientProfile_ConsentTemplate> PatientProfile_ConsentTemplate { get; set; }


        // Patient Medical History
        public DbSet<PatientMedicalHistory_MedicalStatus> PatientMedicalHistory_MedicalStatuses { get; set; }
        public DbSet<PatientMedicalHistory_MedicalCondition> PatientMedicalHistory_MedicalConditions { get; set; }
        public DbSet<PatientMedicalHistory_FamilyHistory> PatientMedicalHistory_FamilyHistories { get; set; }
        public DbSet<PatientMedicalHistory_Allergy> PatientMedicalHistory_Allergies { get; set; }
        public DbSet<PatientMedicalHistory_MedicationOTC> PatientMedicalHistory_MedicationOTCs { get; set; }
        public DbSet<PatientMedicalHistory_MedicationRx> PatientMedicalHistory_MedicationRxes { get; set; }
        public DbSet<PatientMedicalHistory_GeneralCondition> PatientMedicalHistory_GeneralConditions { get; set; }

        // Patient Lifestyle 
        public DbSet<PatientLifestyle_WorkAndRelationship> PatientLifestyle_WorkAndRelationships { get; set; }
        public DbSet<PatientLifestyle_DietAndHabit> PatientLifestyle_DietAndHabits { get; set; }
        public DbSet<PatientLifestyle_LifeStress> PatientLifestyle_LifeStresses { get; set; }
        public DbSet<PatientLifestyle_NutritionalSupplement> PatientLifestyle_NutritionalSupplements { get; set; }

        // Reference Tables for PatientProfile_Lifestyle
        public DbSet<PatientLifestyle_WorkAndRelationship_EmploymentStatus> PatientLifestyle_WorkAndRelationship_EmploymentStatuses { get; set; }
        public DbSet<PatientLifestyle_WorkAndRelationship_Travel> PatientLifestyle_WorkAndRelationship_Travels { get; set; }
        public DbSet<PatientLifestyle_WorkAndRelationship_RelationshipStatus> PatientLifestyle_WorkAndRelationship_RelationshipStatuses { get; set; }
        public DbSet<PatientLifestyle_DietAndHabit_AlcoholFrequency> PatientLifestyle_DietAndHabit_AlcoholFrequencies { get; set; }
        public DbSet<PatientLifestyle_LifeStress_Stress> PatientLifestyle_LifeStress_Stresses { get; set; }
        public DbSet<PatientLifestyle_LifeStress_CopingStress> PatientLifestyle_LifeStress_CopingStresses { get; set; }

        public System.Data.Entity.DbSet<CCM.Models.Icd10Codes> Icd10Codes { get; set; }
        public System.Data.Entity.DbSet<CCM.Models.EnrollmentStatus> EnrollmentStatuss { get; set; }
        public System.Data.Entity.DbSet<CCM.Models.EnrollmentSubStatus> EnrollmentSubStatuss { get; set; }
        public System.Data.Entity.DbSet<CCM.Models.EnrollmentSubstatusReason> EnrollmentSubstatusReasons { get; set; }
        public DbSet<CCM.Models.MedicationDoseRepetitionTime> MedicationDoseRepetitionTimes { get; set; }
        public DbSet<CCM.Models.MedicationRoute> MedicationRoutes { get; set; }

        public System.Data.Entity.DbSet<CCM.Models.PatientMeidcareMedicaidEligibility> PatientMeidcareMedicaidEligibilities { get; set; }

        public DbSet<BillingCycle> BillingCycles { get; set; }
        public DbSet<BillingCycleDetails> BillingCycleDetails { get; set; }

        public DbSet<BillingCycleComments> BillingCycleComments { get; set; }

        public DbSet<CCMCycleStatus> CCMCycleStatuses { get; set; }

        public DbSet<CCMCycleStatusRejectionHistory> CCMCycleStatusRejectionHistory { get; set; }

        public DbSet<QualityControl> QualityControls { get; set; }

        public DbSet<Reasons> Reasons { get; set; }

        public DbSet<PhysicianGroup_Physician_Mapping> physicianGroup_Physician_Mappings { get; set; }

        public DbSet<DoctorTiming> doctorTimings { get; set; }

        public DbSet<DoctorSchedule> doctorSchedules { get; set; }
        public DbSet<DoctorScheduleDeatil> doctorScheduleDeatils { get; set; }

        public DbSet<PhysicianHolidays> PhysicianHolidays { get; set; }

        public DbSet<PatientAppointment> patientAppointments { get; set; }

        public DbSet<LiaisonGroup> liaisonGroups { get; set; }

        public DbSet<LiaisonGroup_Liaison_Mapping> LiaisonGroup_Liaison_Mappings { get; set; }

        public DbSet<Patients_History> patients_Histories { get; set; }

        public DbSet<VoiceMailHistory> voiceMailHistories { get; set; }

        public DbSet<PatientNotes> PatientNotes { get; set; }

        public DbSet<CarePlanSharedHistory> carePlanSharedHistories { get; set; }

        public DbSet<SaleStaff> saleStaffs { get; set; }

        public DbSet<PhysicianGroup_SalesStaff_Mapping> physicianGroup_SalesStaff_Mappings { get; set; }

        //Chat AND Ticket
        public DbSet<PrivateChat> privateChats { get; set; }
        public DbSet<GroupChats> GroupsChats { get; set; }
        public DbSet<GroupChatsDetails> GroupChatsDetails { get; set; }
        public DbSet<GroupChatMessages> GroupChatMessages { get; set; }
        public DbSet<GroupChatsParticipants> GroupChatsParticipants { get; set; }

        public DbSet<UserTicketGeneration> UserTicketGeneration { get; set; }
        public DbSet<AssigneeTicket> AssigneeTicket { get; set; }
        public DbSet<TicketComment> TicketComment { get; set; }


        public DbSet<TicketAttachment> TicketAttachment { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Priority> Priority { get; set; }
        public DbSet<Type> Type { get; set; }
        public DbSet<TicketGeneration> TicketGeneration { get; set; }
        public DbSet<TicketResolution> ticketResolution { get; set; }


        public DbSet<OnlineUser> onlineUsers { get; set; }

        public DbSet<GroupChat> groupChats { get; set; }
        public DbSet<GroupChatParticipants> GroupChatParticipants { get; set; }
        public DbSet<GroupChatDetails> GroupChatDetails { get; set; }

        public DbSet<GroupChatNewMessage> groupChatNewMessages { get; set; }

        public DbSet<Hospitals> Hospitals { get; set; }
        public DbSet<PatientProfile_HospitalDetails> PatientProfile_HospitalDetails { get; set; }


        public DbSet<Department> Departments { get; set; }

        public DbSet<PatientProfile_Hospitalvisits> patientProfile_Hospitalvisits { get; set; }
        public DbSet<AutomaticCycleStatusEntry> automaticCycleStatusEntries { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<BillingPeriods> BillingPeriods { get; set; }
        public DbSet<BillingCategory> BillingCategories { get; set; }
        public DbSet<BillingCodes> BillingCodes { get; set; }
        public DbSet<Patients_BillingCategories> Patients_BillingCategories { get; set; } 
        public DbSet<Liaisons_BillingCategories> Liaisons_BillingCategories { get; set; }   
        public DbSet<Patients_PreLiaisons> Patients_PreLiaisons { get; set; }  
        public DbSet<HospitalReasons> HospitalReasons { get; set; }  
        public DbSet<HospitalDepartments> HospitalDepartments { get; set; }
        public DbSet<HospitalProcedures> HospitalProcedures { get; set; }
        public DbSet<QuestionBank> QuestionBanks { get; set; }
        public DbSet<Evaluation> Evaluation { get; set; }  
        public DbSet<Evaluation_Questions> Evaluation_Questions { get; set; }  
        public DbSet<Evaluation_SubQuestions> Evaluation_SubQuestions { get; set; }  
        public DbSet<G0506_PatientsInfo> G0506_PatientsInfo { get; set; }  
        public DbSet<G0506_AdditionalProviders> G0506_AdditionalProviders { get; set; }  
        public DbSet<TwilioNumbersTable> TwilioNumbersTable { get; set; }
        public DbSet<PatientProfile_Hospitalvisits_History> PatientProfile_Hospitalvisits_History { get; set; }
        public DbSet<Patient_HospitalVisit_NotAdmitted> Patient_HospitalVisit_NotAdmitted { get; set; }
        public DbSet<Evaluation_MainQuestionAnswer> Evaluation_MainQuestionAnswer { get; set; }
        public DbSet<Evaluation_SubQuestionAnswer> Evaluation_SubQuestionAnswer { get; set; }
        public DbSet<G0506_PrimaryInsurance> G0506_PrimaryInsurance { get; set; }
        public DbSet<G0506_SecondaryInsurance> G0506_SecondaryInsurance { get; set; }
        public DbSet<EvaluationFormData> EvaluationFormData { get; set; }
        public DbSet<EvaluationFormDataAnswersForCheckBox> EvaluationFormDataAnswersForCheckBox { get; set; }
        public DbSet<CategoriesStatuses> CategoriesStatuses { get; set; }
        public DbSet<TicketNotificationHistory> TicketNotificationHistory { get; set; }
        public DbSet<Patient_NoAdditionalProvider> Patient_NoAdditionalProvider { get; set; }
        public DbSet<BulkChangesLog> BulkChangesLogs { get; set; }
        public DbSet<BulkChange> BulkChangeses { get; set; }
        public DbSet<RPMService> RPMServices { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<Patients_Services>  Patients_Services { get; set; }



        public DbSet<Devices_Brand> Devices_Brands { get; set; }
        public DbSet<Device_Vendor> Device_Vendors { get; set; }
        public DbSet<Device_BrandModel> Device_BrandModels { get; set; }
        public DbSet<DeviceMappingHistory> DeviceMappingHistory { get; set; }
        public DbSet<Rpm_DeviceMappingAttachments> Rpm_DeviceMappingAttachments { get; set; }
        public DbSet<Rpm_PatientDeviceReading> Rpm_PatientDeviceReading { get; set; }


    }
}