using System.ComponentModel.DataAnnotations;


namespace CCM.Models
{
    public class PatientLifestyle_WorkAndRelationship
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Occupation")]
        public string Occupation { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        // Workplace
        [Display(Name = "Home")]
        public bool Home { get; set; }

        [Display(Name = "Office")]
        public bool Office { get; set; }

        [Display(Name = "Field")]
        public bool Field { get; set; }
        //------------------------------

        // Living Situation
        [Display(Name = "Spouse")]
        public bool Spouse { get; set; }

        [Display(Name = "Partner")]
        public bool Partner { get; set; }

        [Display(Name = "Alone")]
        public bool Alone { get; set; }

        [Display(Name = "Children")]
        public bool Children { get; set; }

        [Display(Name = "Parents")]
        public bool Parents { get; set; }
        //---------------------------------
        //NCA
     


        // Pet
        [Display(Name = "Dog")]
        public bool Dog { get; set; }

        [Display(Name = "Cat")]
        public bool Cat { get; set; }

        [Display(Name = "Fish")]
        public bool Fish { get; set; }

        [Display(Name = "Bird")]
        public bool Bird { get; set; }

        [Display(Name = "Other")]
        public bool Other { get; set; }

        [Display(Name = "Other")]
        public string OtherPet { get; set; }
        // -----------------------------------

        [Display(Name = "Employment Status")]
        public int? Employment_StatusId { get; set; }

        [Display(Name = "Travel")]
        public int? TravelRequirementId { get; set; }

        [Display(Name = "Relationship Status")]
        public int? Relationship_StatusId { get; set; }


        ////Na , CA ,N , T , SW New Attributes
        //[Display(Name = "NCA")]
        //public bool NCA { get; set; }
        //[Display(Name = "Hours/Week")]
        //public double? NCAHrsweek { get; set; }

        //[Display(Name = "CA")]
        //public int? CAID { get; set; }
        //[Display(Name = "Hours/Week")]
        //public double? CAHrsweek { get; set; }

        //[Display(Name = "N")]
        //public int? NID { get; set; }
        //[Display(Name = "Hours/Week")]
        //public double? NHrsweek { get; set; }

        //[Display(Name = "T")]
        //public int? TID { get; set; }
        //[Display(Name = "Hours/Week")]
        //public double? THrsweek { get; set; }

        //[Display(Name = "SW")]
        //public int? SwID { get; set; }
        //[Display(Name = "Hours/Week")]
        //public double? SwHrsweek { get; set; }

        public virtual PatientLifestyle_WorkAndRelationship_EmploymentStatus Employment_Status { get; set; }
        public virtual PatientLifestyle_WorkAndRelationship_Travel TravelRequirement { get; set; }
        public virtual PatientLifestyle_WorkAndRelationship_RelationshipStatus Relationship_Status { get; set; }
    }

    public class PatientLifestyle_WorkAndRelationship_EmploymentStatus
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class PatientLifestyle_WorkAndRelationship_Travel
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class PatientLifestyle_WorkAndRelationship_RelationshipStatus
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class PatientLifestyle_DietAndHabit
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Dietary Restrictions")]
        public string DietaryRestrictions { get; set; }

        [Display(Name = "Breakfast")]
        public string Breakfast { get; set; }

        [Display(Name = "Lunch")]
        public string Lunch { get; set; }

        [Display(Name = "Dinner")]
        public string Dinner { get; set; }

        [Display(Name = "Do You Excercise?")]
        public bool? DoExcercise { get; set; }

        [Display(Name = "How Often?")]
        public string ExcerciseHowOften { get; set; }

        [Display(Name = "What Kind")]
        public string ExcerciseWhatKind { get; set; }

        [Display(Name = "Do You Use Tobacco Products?")]
        public bool? UseTobacco { get; set; }

        [Display(Name = "If Yes, How Often?")]
        public string TobaccoHowOften { get; set; }

        [Display(Name = "How Long Have You Used Them?")]
        public string TobaccoUseDuration { get; set; }

        // Drink Alcohol?
        [Display(Name = "Do You Drink Alcohol?")]
        public int? AlcoholId { get; set; }
        //----------------------------------

        [Display(Name = "Do You Use caffeine Products?")]
        public bool? UseCaffeine { get; set; }

        [Display(Name = "If Yes, How Much Per Day?")]
        public string CaffeineQuantity { get; set; }

        [Display(Name = "Do You Have Symptoms Of Hypoglycemia")]
        public bool? HaveHypoglycemia { get; set; }

        [Display(Name = "Are You Currently Following a Special Diet")]
        public bool? OnSpecialDiet { get; set; }

        public virtual PatientLifestyle_DietAndHabit_AlcoholFrequency Alcohol { get; set; }
    }

    public class PatientLifestyle_DietAndHabit_AlcoholFrequency
    {
        public int id { get; set; }
        public string Type { get; set; }
    }

    public class PatientLifestyle_LifeStress
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Work Related Stress")]
        public int? workStressId { get; set; }

        [Display(Name = "Personal Life Related Stress")]
        public int? LifeStressId { get; set; }

        [Display(Name = "Coping With Stress")]
        public int? Coping_StressId { get; set; }

        [Display(Name = "Main Causes Of Stress")]
        public string StressCauses { get; set; }

        public virtual PatientLifestyle_LifeStress_Stress WorkStress { get; set; }
        public virtual PatientLifestyle_LifeStress_Stress LifeStress { get; set; }
        public virtual PatientLifestyle_LifeStress_CopingStress Coping_Stress { get; set; }
    }

    public class PatientLifestyle_LifeStress_Stress
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class PatientLifestyle_LifeStress_CopingStress
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class PatientLifestyle_NutritionalSupplement
    {
        public int Id { get; set; }
        public int Cycle { get; set; }
        public int PatientId { get; set; }

        [Display(Name = "Vitamins")]
        public string Vitamins { get; set; }

        [Display(Name = "Date Started")]
        public string VitaminsStartDate { get; set; }

        [Display(Name = "Daily Dosage")]
        public string VitaminsDailyDosage { get; set; }

        [Display(Name = "Minerals")]
        public string Minerals { get; set; }

        [Display(Name = "Date Started")]
        public string MineralsStartDate { get; set; }

        [Display(Name = "Daily Dosage")]
        public string MineralsDailyDosage { get; set; }

        [Display(Name = "Herbs")]
        public string Herbs { get; set; }

        [Display(Name = "Date Started")]
        public string HerbsStartDate { get; set; }

        [Display(Name = "Daily Dosage")]
        public string HerbsDailyDosage { get; set; }

        [Display(Name = "Enzymes")]
        public string Enzymes { get; set; }

        [Display(Name = "Date Started")]
        public string EnzymesStartDate { get; set; }

        [Display(Name = "Daily Dosage")]
        public string EnzymesDailyDosage { get; set; }

        [Display(Name = "Nutrition/Protein Supplements")]
        public string Supplements { get; set; }

        [Display(Name = "Date Started")]
        public string SupplementsStartDate { get; set; }

        [Display(Name = "Daily Dosage")]
        public string SupplementsDailyDosage { get; set; }

        [Display(Name = "Others")]
        public string Others { get; set; }

        [Display(Name = "Date Started")]
        public string OthersStartDate { get; set; }

        [Display(Name = "Daily Dosage")]
        public string OthersDailyDosage { get; set; }
    }
}