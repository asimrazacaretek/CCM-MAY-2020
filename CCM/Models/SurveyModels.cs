using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCM.Models
{

    public class Survey
    {
        [Key]
        public int Id { get; set; }
        public int SurveySectionId { get; set; }
        public SurveySection SurveySection { get; set; }
        public int SurveyTypeId { get; set; }
        public SurveyType SurveyType { get; set; }

        //Name of Survey
        [Required]
        [Display(Name = "Survey Name")]
        [StringLength(100)]
        public string SurveyName { get; set; }
        //Survey Created Date
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SurveyQuestion
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int SurveyQuestionTypeId { get; set; }

        public SurveyQuestionType SurveyQuestionType { get; set; }

        public Survey Survey { get; set; }
        public int SurveySectionId { get; set; }
        public int SurveyTypeId { get; set; }
        public List<SurveyAnswer> surveyAnswers { get; set; }
        [Required]
        [Display(Name = "Question")]
        [StringLength(250)]
        public string QuestionText { get; set; }
        //Survey Created Date
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int OrderBy { get; set; }
    }

    public class SurveyAnswer
    {
        [Key]
        public int Id { get; set; }
        public int SurveyQuestionId { get; set; }
        public string AnswerText { get; set; }
        //Survey Created Date
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string ClinicalNotes { get; set; }

    }
    public class SurveyQuestionSequenceMapping
    {
        [Key]
        public int Id { get; set; }
        public string AnswerIds { get; set; }
        public int ChildQuestionID { get; set; }
        public int ParentQuestionID { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }
        public int IsFirstOrLast { get; set; } //For IsFirst=1 and IsLast=2 and IsMiddle =0
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SurveySection
    {
        [Key]
        public int Id { get; set; }
        //Survey Section i.e. Cough, Diabaties, Cancer
        [Required]
        [Display(Name = "Survey Section")]
        [StringLength(100)]
        public string SectionName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SurveyType
    {
        [Key]
        public int Id { get; set; }
        //Survey Type i.e. Monthly, Yearly, Weekly
        [Required]
        [Display(Name = "Survey Type")]
        [StringLength(100)]
        public string TypeName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class SurveyQuestionType
    {
        [Key]
        public int Id { get; set; }
        //Survey Question Type i.e. Check Box, Radio Button, Text Box, Text Area
        [Required]
        [Display(Name = "Question Type")]
        [StringLength(100)]
        public string QuestionType { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class PatientSurvey
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }
        public int SurveySectionId { get; set; }
        public SurveySection SurveySection { get; set; }
        public int SurveyTypeId { get; set; }
        public SurveyType SurveyType { get; set; }
        public List<PatientSurveyQA> patientSurveyQAs { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int IsCompleted { get; set; } //Fully Completed or Partially Completed

        public string PatientName { get; set; }
    }

    public class PatientSurveyQA
    {
        public int Id { get; set; }
        public int PatientSurveyId { get; set; }
        public int SurveyQuestionId { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }
        public string SurveyAnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class PatientSurveryViewModel
    {
        public string PatientName { get; set; }
        public string DOB { get; set; }
        public string SurveyType { get; set; }
        public string SurveySection { get; set; }
        public string SurveryName { get; set; }
        public List<PatientSurveryQAViewModel> patientSurveryQA { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false; 
    }
    public class AnswerTextandClass
    {
        public string Answer { get; set; }
        public string ClassName { get; set; }
    }
    public class PatientSurveryQAViewModel
    {
        public string Question { get; set; }
        public List<AnswerTextandClass> Answer { get; set; }
        public string AnswerGivenId { get; set; }
        public List<string> AnswerGiven { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
    public class SurveyFlowView
    {
        public string name { get; set; }
        public int QId { get; set; }
        public string AnswerIds { get; set; }
        public List<SurveyFlowView> children { get; set; } = new List<SurveyFlowView>();
    }
    public class SurveyFlowNextNode
    {
        public int SurveyId { get; set; }
        public int SurveyTypeId { get; set; }
        public int SurveySectionId { get; set; }
        public int ChildQuestionID { get; set; }
        public int ParentQuestionID { get; set; }
        public int IsFirstOrLast { get; set; }
        public string AnswerIds { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
    }

    public class SurveyDependent
    {
        public int surveyId { get; set; }
        public string survey { get; set; }
        public int surveyTypeId { get; set; }
        public string surveyType { get; set; }
        public int surveySectionId { get; set; }
        public string surveySection { get; set; }
    }

    public class SurveyQuestionMappingViewModel
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int SurveyQuestionTypeId { get; set; }

        public SurveyQuestionType SurveyQuestionType { get; set; }

        public Survey Survey { get; set; }
        public List<SurveyAnswer> surveyAnswers { get; set; }
        public List<List<SurveyAnswer>> surveyAnswersCombo { get; set; }
        [Required]
        [Display(Name = "Question")]
        [StringLength(250)]
        public string QuestionText { get; set; }
        //Survey Created Date
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }
        public int OrderBy { get; set; }
    }

    public class SurveyQuestionMappingAdminViewmodel
    {
        public List<SurveyQuestionMappingViewModel> surveyQuestions { get; set; }
        public List<SurveyQuestionSequenceMapping> surveyQuestionSequenceMappings { get; set; }
    }

    public class PatientSurveyQAReviewModel
    {
        public SurveyQuestionViewmodel surveyQuestionViewmodel { get; set; }
        public PatientSurveryViewModel patientSurveryViewModels { get; set; }
    }

    public class SurveyQuestionViewmodel
    {
        public List<SurveyQuestion> surveyQuestions { get; set; }


        public List<SurveyQuestionSequenceMapping> surveyQuestionSequenceMappings { get; set; }
    }
    
}