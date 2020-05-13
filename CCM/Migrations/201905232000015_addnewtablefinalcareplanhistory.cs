namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablefinalcareplanhistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinalCarePlanNotes_History",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        CarePlanCreatedOn = c.DateTime(),
                        CarePlanCreatedBy = c.String(),
                        CarePlanEditedBy = c.String(),
                        CarePlanEditedOn = c.DateTime(),
                        Version = c.Guid(nullable: false),
                        Cycle = c.Int(nullable: false),
                        PatientSummary = c.String(nullable: false),
                        HealthConcerns = c.String(),
                        DrugUtillizationReview = c.String(),
                        NutritionGoals = c.String(),
                        NutritionInterventions = c.String(),
                        NutritionReview = c.String(),
                        PhysicalActivityGoals = c.String(),
                        PhysicalActivityInterventions = c.String(),
                        PhysicalActivityReview = c.String(),
                        SleepGoals = c.String(),
                        SleepInterventions = c.String(),
                        SleepReview = c.String(),
                        MentalResilienceGoals = c.String(),
                        MentalResilienceInterventions = c.String(),
                        MentalResilienceReview = c.String(),
                        ManagingMedicationGoals = c.String(),
                        ManagingMedicationInterventions = c.String(),
                        ManagingMedicationReview = c.String(),
                        BadHabitsCessationGoals = c.String(),
                        BadHabitsCessationInterventions = c.String(),
                        BadHabitsCessationReview = c.String(),
                        SideEffectsHistory = c.String(),
                        Allergies = c.String(),
                        LastHospitalized = c.String(),
                        HospitalizedReason = c.String(),
                        LastErVisit = c.String(),
                        ErReason = c.String(),
                        HealthRating = c.String(),
                        PerformTasksRating = c.String(),
                        VerbalEducationToPatient = c.String(),
                        EducationSent = c.String(),
                        PatientComplianceManagement = c.String(),
                        PharmacyTreatmentGoals = c.String(),
                        HospitalizationLast30Days = c.String(),
                        AbilityPerformIADL = c.String(),
                        AssessmentVisionHearingSpeech = c.String(),
                        BloodPressureGlucoseReading = c.String(),
                        LastBloodPressureGlucoseReading = c.String(),
                        FallAmbulationAssessment = c.String(),
                        MedicationReconciliation = c.String(),
                        DiabeticScreening = c.String(),
                        TobaccoAlcoholScreening = c.String(),
                        ColonCancerScreenDate = c.DateTime(),
                        MammogramScreenDate = c.DateTime(),
                        UrinaryIncontinence = c.String(),
                        Immunization = c.String(),
                        CriticalChestPain = c.Boolean(nullable: false),
                        CriticalBreathShortness = c.Boolean(nullable: false),
                        NewWound = c.Boolean(nullable: false),
                        Bleedinggums = c.Boolean(nullable: false),
                        Nosebleed = c.Boolean(nullable: false),
                        Bloodinurine = c.Boolean(nullable: false),
                        Isurinemalodorous = c.Boolean(nullable: false),
                        Suddenincreasedswelling = c.Boolean(nullable: false),
                        SkinAssesment = c.String(maxLength: 5, unicode: false),
                        Diaperchange = c.String(maxLength: 5, unicode: false),
                        DMEsupply = c.String(),
                        isPatientBedbound = c.String(maxLength: 5, unicode: false),
                        CriticalSeverePain = c.Boolean(nullable: false),
                        CriticalMoodChange = c.Boolean(nullable: false),
                        CriticalSpeechChange = c.Boolean(nullable: false),
                        CriticalHeadache = c.Boolean(nullable: false),
                        CriticalBloodSugar = c.Boolean(nullable: false),
                        CriticalBloodPressure = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FinalCarePlanNotes_History");
        }
    }
}
