using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCM.Models.ViewModels;
using CCM.Models;
using CCM.Models.DataModels;
using System.Threading.Tasks;
using CCM.Models.DataModels;
using System.Data.Entity;
using Rotativa;
using Rotativa.Options;
using CCM.Helpers;

namespace CCM.Controllers
{
    public class G0506Controller : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: G0506
        public ActionResult Index(int PatientId)
        {
            var primaryinsurance = new G0506_PrimaryInsurance();
            var secondaryinsurance = new G0506_SecondaryInsurance();
            var patientinfo = _db.G0506_PatientsInfo.Where(p => p.PatientId == PatientId).FirstOrDefault();
            if (patientinfo != null)
            {

            primaryinsurance = _db.G0506_PrimaryInsurance.Where(p => p.Id == patientinfo.G0506_PrimaryInsuranceId).FirstOrDefault();
           secondaryinsurance = _db.G0506_SecondaryInsurance.Where(p => p.Id == patientinfo.G0506_SecondaryInsuranceId).FirstOrDefault();

            }
           
          
            G0506ViewModel viewModel = new G0506ViewModel();
            viewModel.G0506_PatientsInfo = patientinfo;
            viewModel.G0506_PrimaryInsurance = primaryinsurance;
            viewModel.G0506_SecondaryInsurance = secondaryinsurance;

            

            ViewBag.PatientId = PatientId;
            var Category = "G0506 INITIAL VISIT";
            ViewBag.Message = Category;
            ViewBag.BillingReviewId = HelperExtensions.ReviewTimeGet(Category, PatientId, User.Identity.GetUserId(),BillingCodeHelper.G0506BillingCatagoryid);
            var G0506FormDataViewModel = new G0506FormDataViewModel();

         
            var PatientInfo = _db.G0506_PatientsInfo.Where(x => x.PatientId == PatientId).FirstOrDefault();
            if (patientinfo == null)
            {

                patientinfo = new G0506_PatientsInfo();
                patientinfo.PatientId = PatientId;
                patientinfo.CreatedBy = User.Identity.GetUserId();
                patientinfo.CreatedOn = DateTime.Now;
                _db.G0506_PatientsInfo.Add(patientinfo);
                _db.SaveChanges();
            }



            if (PatientInfo != null)
            {
                ViewBag.AdditionalProviders = _db.G0506_AdditionalProviders.Where(sd => sd.G0506_PatientsInfoId == PatientInfo.Id).ToList();
            }
            else
            {
                ViewBag.AdditionalProviders = new List<G0506_AdditionalProviders>();
            }


            ViewBag.G0506Status = CategoryCycleStatusHelper.GetPatientNewOrOldCycleStatusbyCategory(PatientId,BillingCodeHelper.G0506BillingCatagoryid,null);

            return View(viewModel);
        }


        public ActionResult _UpdateAdditionalProviders(int PatientId)
        {
            var patientinfo = _db.G0506_PatientsInfo.Where(p => p.PatientId == PatientId).FirstOrDefault();


            var G0506AdditionalProviders = _db.G0506_AdditionalProviders.Where(x=>x.G0506_PatientsInfoId==patientinfo.Id).ToList();

            var AdditionalProviders = _db.SecondaryDoctors.Where(sd => sd.PatientId == PatientId).ToList();
            foreach (var item in AdditionalProviders)
            {
                var exist = G0506AdditionalProviders.Find(x => x.NPI == item.NPI);
                if (exist == null)
                {
                    var model = new G0506_AdditionalProviders();
                    model.Address1 = model.Address1;
                    model.Address2 = item.Address2;
                    model.DoctorType = item.DoctorType;
                    model.Email = item.Email;
                    model.FullName = item.FullName;
                    model.isCCMProvider = item.isCCMProvider;
                    model.IsPrimaryCareProvider = false;
                    model.IsShareCarePlan = item.IsShareCarePlan;
                    model.LastVisit = item.LastVisit;
                    model.MainPhoneNumber = item.MainPhoneNumber ?? "";
                    model.MobilePhoneNumber = item.MobilePhoneNumber ?? "";
                    model.NextAppointment = item.NextAppointment;
                    model.NPI = item.NPI;
                    model.G0506_PatientsInfoId = patientinfo.Id;
                    model.Speciality = item.Speciality;
                    _db.G0506_AdditionalProviders.Add(model);
                    _db.SaveChanges();
                }

            }

           
            ViewBag.AdditionalProviders = _db.G0506_AdditionalProviders.Where(sd => sd.G0506_PatientsInfoId == patientinfo.Id).ToList();
         
            return View();

          
        }



     


        public async Task<ActionResult> GeneratePdfG0506Async(int? PatientId)
        {
            var patient = await _db.Patients.FindAsync(PatientId);
            if (patient != null)
            {
                var PatientsInfo = _db.G0506_PatientsInfo.Where(x => x.PatientId == patient.Id).FirstOrDefault();
                var PrimaryInsurance = PatientsInfo.G0506_PrimaryInsurance;
                var SecondaryInsurance =  PatientsInfo.G0506_SecondaryInsurance;
                var AdditionalProviders = PatientsInfo.AdditionalProvidersList;

                var G0506FormViewModal = new G0506FormDataViewModel();
                G0506FormViewModal.G0506_PrimaryInsurance = PatientsInfo.G0506_PrimaryInsurance;
                G0506FormViewModal.G0506_SecondaryInsurance = PatientsInfo.G0506_SecondaryInsurance;
                G0506FormViewModal.G0506_PrimaryInsurance = PatientsInfo.G0506_PrimaryInsurance;
                if (PatientsInfo.AdditionalProvidersList.Count>0)
                G0506FormViewModal.G0506_PatientsInfo.AdditionalProvidersList = PatientsInfo.AdditionalProvidersList;
                if (PatientsInfo.BirthDate != null) {
                G0506FormViewModal.G0506_PatientsInfo.BirthDate = PatientsInfo.BirthDate;
                }
                G0506FormViewModal.G0506_PatientsInfo.CreatedBy = PatientsInfo.CreatedBy;
                G0506FormViewModal.G0506_PatientsInfo.CreatedOn = PatientsInfo.CreatedOn;
                G0506FormViewModal.G0506_PatientsInfo.DateConsentcompleted = PatientsInfo.DateConsentcompleted;
                G0506FormViewModal.G0506_PatientsInfo.DesignatedCCMContact = PatientsInfo.DesignatedCCMContact;
                G0506FormViewModal.G0506_PatientsInfo.G0506_PrimaryInsurance = PatientsInfo.G0506_PrimaryInsurance;
                G0506FormViewModal.G0506_PatientsInfo.G0506_SecondaryInsurance = PatientsInfo.G0506_SecondaryInsurance;
                G0506FormViewModal.G0506_PatientsInfo.G0506_SecondaryInsuranceId = PatientsInfo.G0506_SecondaryInsuranceId;
                G0506FormViewModal.G0506_PatientsInfo.Id = PatientsInfo.Id;
                G0506FormViewModal.G0506_PatientsInfo.IsCCMConsentcompleted = PatientsInfo.IsCCMConsentcompleted;
                G0506FormViewModal.G0506_PatientsInfo.IsCurrentlyActiveinCCM = PatientsInfo.IsCurrentlyActiveinCCM;
                G0506FormViewModal.G0506_PatientsInfo.PatientId = PatientsInfo.PatientId;
                G0506FormViewModal.G0506_PatientsInfo.Patients = PatientsInfo.Patients;
                G0506FormViewModal.G0506_PatientsInfo.Status = PatientsInfo.Status;
                G0506FormViewModal.G0506_PatientsInfo.UpdatedBy = PatientsInfo.UpdatedBy;
                G0506FormViewModal.G0506_PatientsInfo.UpdatedOn = PatientsInfo.UpdatedOn;
                G0506FormViewModal.G0506_PatientsInfo.FullName = PatientsInfo.FullName;
                G0506FormViewModal.UrgencyContactList = await _db.PatientProfile_UrgencyContacts.AsNoTracking().Where(m => m.PatientId == patient.Id).ToListAsync();
                G0506FormViewModal.EvaluationViewModel = GetEvaluationForm(BillingCodeHelper.G0506BillingCatagoryid,patient.Id);
                var footer = "--footer-center \"Last Updated on " + DateTime.Now.ToString("MM/dd/yyyy @ hh:mmtt ") +
                             "- Page: [page]/[toPage]\"" + " --footer-font-size \"8\" --footer-spacing \"6\" --footer-font-name \"times\"";
                var generatePdf = new ViewAsPdf("G0506Report", G0506FormViewModal)
                {
                    FileName = "CCM_Health_" + patient.FirstName + "_" + patient.LastName + "_G0506.pdf",
                    //MinimumFontSize =10,
                    CustomSwitches = footer,
                    IsLowQuality = false,

                    PageOrientation = Rotativa.Options.Orientation.Portrait,

                    PageSize = Rotativa.Options.Size.A5,
                    PageMargins = new Margins(5, 5, 5, 5),
                    //PageMargins = new Margins(3, 3,3, 3),
                    //PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 }
                };
                var pdf= generatePdf.BuildPdf(ControllerContext);
                var ms = new System.IO.MemoryStream(pdf);
                return new FileStreamResult(ms, "application/pdf");
            }
            return View();
            //ViewBag.Message = "Patient Not Found!";
            //return View("Error");
        }
        public EvaluationViewModel GetEvaluationForm(int? BillingCategoryId, int? PatientId)
        {
            List<MainQuestionViewModal> mainQuestionViewModallist = new List<MainQuestionViewModal>();


            var formdata =_db.EvaluationFormData.Where(p => p.PatientId == PatientId).ToList();

            var evaluationformlist =_db.Evaluation.Where(p => p.BillingCategoryId == BillingCategoryId).ToList();

            if (evaluationformlist.Count() > 0)
            {
                var evaluationform = evaluationformlist.LastOrDefault();
                EvaluationViewModel evaluationViewModel = new EvaluationViewModel();

                evaluationViewModel.BillingCategoryId = evaluationform.BillingCategoryId.ToString();

                evaluationViewModel.Name = evaluationform.Name;
                evaluationViewModel.Id = evaluationform.Id.ToString();

                var EvaluationQuestions =_db.Evaluation_Questions.Where(p => p.EvaluationsId == evaluationform.Id).ToList();

                List<MainQuestionViewModal> MainQuestionViewModallist = new List<MainQuestionViewModal>();
                foreach (var questions in EvaluationQuestions)
                {

                    MainQuestionViewModal mainQuestionViewModal = new MainQuestionViewModal();


                    mainQuestionViewModal.QuestionId = questions.QuestionId.ToString();
                    mainQuestionViewModal.MainQuestion = questions.QuestionBank.Question;
                    mainQuestionViewModal.sortIndex = questions.Order.ToString();
                    mainQuestionViewModal.AnswerType = questions.Type;
                    mainQuestionViewModal.QuestionGUID = questions.QuestionGUID;
                    mainQuestionViewModal.haveDateTime = questions.HasDate == true ? "yes" : "no";
                    mainQuestionViewModal.haveSubQuestion = questions.HasSubtype == true ? "yes" : "no";
                    if (questions.HasSubtype == true)
                    {
                        List<SubQuestionsViewModal> subQuestionsViewModallist = new List<SubQuestionsViewModal>();


                        var Subquestions =_db.Evaluation_SubQuestions.Where(p => p.Evaluation_QuestionsId == questions.Id).ToList();
                        foreach (var subquestions in Subquestions)
                        {

                            SubQuestionsViewModal subQuestionsViewModal = new SubQuestionsViewModal();
                            subQuestionsViewModal.QuestionId = subquestions.QuestionId.ToString();
                            subQuestionsViewModal.Question = subquestions.Name;
                            subQuestionsViewModal.haveDateTime = subquestions.HasDate == true ? "yes" : "no";
                            subQuestionsViewModal.AnswerType = subquestions.Type;
                            subQuestionsViewModal.QuestionGUID = subquestions.QuestionGUID;
                            var subquestionguidlist = formdata.Where(p => p.MainQuestionGuid == subquestions.QuestionGUID).FirstOrDefault();
                            if (subquestions.Type != "text")
                            {
                                var subquestionAnswers =_db.Evaluation_SubQuestionAnswer.Where(p => p.SubQuestionId == subquestions.Id).ToList();
                                List<AnswersViewModal> answersViewModallist = new List<AnswersViewModal>();

                                foreach (var subanswers in subquestionAnswers)
                                {
                                    AnswersViewModal answersViewModal = new AnswersViewModal();
                                    answersViewModal.AnswerId = subanswers.QuestionId.ToString();
                                    answersViewModal.Answer = subanswers.Question;
                                    answersViewModal.haveDateTime = subquestions.HasDate == true ? "yes" : "no";


                                    if (subquestionguidlist != null)
                                    {
                                        if (answersViewModal.haveDateTime == "yes")
                                        {
                                            if (subquestionguidlist.Date != null)
                                            {
                                                DateTime time = (DateTime)subquestionguidlist.Date;
                                                subQuestionsViewModal.Date = time.Date.ToString("dd/MM/yyyy");
                                            }


                                        }
                                        if (subanswers.Type == "radio")
                                        {
                                            if (subanswers.QuestionGUID == subquestionguidlist.AnswerGuid)
                                            {
                                                answersViewModal.IsAnswer = "yes";
                                            }

                                        }
                                        else if (subanswers.Type == "checkbox")
                                        {
                                            var checkboxes =_db.EvaluationFormDataAnswersForCheckBox.Where(p => p.EvaluationFormDataId == subquestionguidlist.Id && p.Status == true).ToList();
                                            if (checkboxes.Count() > 0)
                                            {
                                                var checkans = checkboxes.Where(p => p.AnswerGuid.Contains(subanswers.QuestionGUID)).Count();
                                                if (checkans > 0)
                                                {
                                                    answersViewModal.IsAnswer = "yes";
                                                }

                                            }

                                        }




                                    }

                                    answersViewModal.type = subanswers.Type;
                                    answersViewModal.QuestionGUID = subanswers.QuestionGUID;




                                    answersViewModallist.Add(answersViewModal);

                                    subQuestionsViewModal.answers = answersViewModallist;


                                }
                            }

                            else if (subquestions.Type == "text")
                            {
                                if (subquestionguidlist != null)
                                    subQuestionsViewModal.CurrentAnswer = subquestionguidlist.Answer;


                            }

                            subQuestionsViewModallist.Add(subQuestionsViewModal);
                            mainQuestionViewModal.subQuestions = subQuestionsViewModallist;
                        }
                    }
                    else
                    {
                        var mainanswerlist = formdata.Where(p => p.MainQuestionGuid == questions.QuestionGUID).FirstOrDefault();
                        if (questions.Type != "text" && questions.Type != "none")
                        {
                            List<MainAnswerViewModal> mainAnswerViewModallist = new List<MainAnswerViewModal>();
                            var mainquestionsanswers =_db.Evaluation_MainQuestionAnswer.Where(p => p.MainQuestionId == questions.Id).ToList();
                            foreach (var mainanswers in mainquestionsanswers)
                            {
                                MainAnswerViewModal mainAnswerViewModal = new MainAnswerViewModal();
                                mainAnswerViewModal.AnswerId = mainanswers.QuestionId.ToString();
                                mainAnswerViewModal.Answer = mainanswers.Question;
                                mainAnswerViewModal.QuestionGUID = mainanswers.QuestionGUID;
                                if (mainanswerlist != null)
                                {
                                    if (mainQuestionViewModal.haveDateTime == "yes")
                                    {
                                        DateTime dateTime = (DateTime)mainanswerlist.Date;
                                        mainQuestionViewModal.Date = dateTime.ToString("MM/dd/yyyy");
                                    }
                                    if (mainQuestionViewModal.AnswerType == "radio")
                                    {
                                        if (mainanswers.QuestionGUID == mainanswerlist.AnswerGuid)
                                        {
                                            mainAnswerViewModal.IsAnswer = "yes";
                                        }
                                    }
                                    else if (mainQuestionViewModal.AnswerType == "checkbox")
                                    {
                                        var checkboxes =_db.EvaluationFormDataAnswersForCheckBox.Where(p => p.EvaluationFormDataId == mainanswerlist.Id).ToList();
                                        if (checkboxes.Count() > 0)
                                        {
                                            var checkans = checkboxes.Where(p => p.AnswerGuid.Contains(mainanswers.QuestionGUID)).Count();
                                            if (checkans > 0)
                                            {
                                                mainAnswerViewModal.IsAnswer = "yes";
                                            }
                                        }
                                    }
                                }
                                mainAnswerViewModallist.Add(mainAnswerViewModal);
                            }
                            mainQuestionViewModal.MainAnswer = mainAnswerViewModallist;
                        }
                        else if (questions.Type == "text")
                        {
                            if (mainanswerlist != null)
                            {
                                mainQuestionViewModal.CurrentAnswer = mainanswerlist.Answer;
                            }
                        }
                    }
                    mainQuestionViewModallist.Add(mainQuestionViewModal);
                }
                evaluationViewModel.MainQuestionViewModal = mainQuestionViewModallist;
                return evaluationViewModel;

            }
            else
            {
                EvaluationViewModel evaluationViewModel = new EvaluationViewModel();
                return evaluationViewModel;
            }

        }



        public TimeSpan TotalReviewTimeG0506(int patientId)
       {
            try
            {

                var BillingCategoryId = BillingCodeHelper.G0506BillingCatagoryid;
                var reviews = _db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.BillingcategoryId== BillingCategoryId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                var BillingCategoryId = BillingCodeHelper.G0506BillingCatagoryid;

                log.Error(Environment.NewLine + User.Identity.GetUserName() + "-------" + User.Identity.GetUserId() + Environment.NewLine + ex.Message + "-----" + ex.StackTrace);

                var reviews = _db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.BillingcategoryId == BillingCategoryId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;

            }
        }

    }
}