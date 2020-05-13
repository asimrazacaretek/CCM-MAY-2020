using CCM.Models;
using CCM.Models.DataModels;
using CCM.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class EvaluationController : BaseController
    {
        //public static List<PreviewFormViewModel> PreviewQuestionList = new List<PreviewFormViewModel>(); 
       // private Application_dbContect _db = new Application_dbContect();
        // GET: Evaluation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.BillingCategories = _db.BillingCategories.Select(x => new
            {
                Id = x.BillingCategoryId,
                Name = x.Name
            });
           
            ViewBag.Questions = _db.QuestionBanks.ToList();
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public int getEvaluationFormId(int BillingCategoryId,string Name)
        {
            var EvauationForm = new Evaluation();
            EvauationForm.Name = Name;
            EvauationForm.BillingCategoryId = BillingCategoryId;
            _db.Evaluation.Add(EvauationForm);
           var save= _db.SaveChanges();
            if (save > 0)
            {
                return EvauationForm.Id;
            }
            else
            {
                return 0;
            }
        }

        [HttpPost]
        public ActionResult SaveEvaluationFormData(string FormData,
            string G0506_PatientsInfo_FullName,
            string G0506_PatientsInfo_BirthDate,
            string G0506_PatientsInfo_IsCurrentlyActiveinCCM, 
            string G0506_PatientsInfo_IsCCMConsentcompleted,
            string G0506_PatientsInfo_DateConsentcompleted,
            string G0506_PrimaryInsurance_Name,
            string G0506_PrimaryInsurance_PlanName, 
            string G0506_PrimaryInsurance_InsuranceId,
            string G0506_PrimaryInsurance_GroupNumber,
            string G0506_PrimaryInsurance_RxBin,
            string G0506_PrimaryInsurance_RxPCN,
            string G0506_PrimaryInsurance_RxGroup,
            string G0506_PrimaryInsurance_RelatedtoInsurance ,
            string G0506_SecondaryInsurance_MedicareNo,
            string G0506_SecondaryInsurance_MedicaidNo,
            string G0506_SecondaryInsurance_OtherInsuranceNo,
            string PrimaryCareProviderId,
            string G0506_PatientsInfo_DesignatedCCMContact ,
            string G0506_PrimaryInsurance_PlanCode)
        {
            EvaluationViewModel EvaluationFormData = JsonConvert.DeserializeObject<EvaluationViewModel>(FormData);

            int? FormId = Convert.ToInt32(EvaluationFormData.Id);
                int? patientId= Convert.ToInt32(EvaluationFormData.PatientId);



            var checkpatientinfo = _db.G0506_PatientsInfo.Where(p => p.PatientId == patientId).FirstOrDefault();

            //update
            if (checkpatientinfo != null)
            {


                var primaryInsurance = _db.G0506_PrimaryInsurance.Where(p => p.Id == checkpatientinfo.G0506_PrimaryInsuranceId).FirstOrDefault(); ;
                if (primaryInsurance != null)
                {

                    primaryInsurance.Name = G0506_PrimaryInsurance_Name;
                    primaryInsurance.PlanName = G0506_PrimaryInsurance_PlanName;
                    if (G0506_PrimaryInsurance_InsuranceId != "")
                    {
                        int? insuranceid = Convert.ToInt32(G0506_PrimaryInsurance_InsuranceId);
                        primaryInsurance.InsuranceId = insuranceid;
                    }
                    if (G0506_PrimaryInsurance_GroupNumber != "")
                    {
                        int? groupno = Convert.ToInt32(G0506_PrimaryInsurance_GroupNumber);
                        primaryInsurance.GroupNumber = groupno;

                    }


                    primaryInsurance.PlanCode = G0506_PrimaryInsurance_PlanCode;
                    primaryInsurance.RxBin = G0506_PrimaryInsurance_RxBin;
                    primaryInsurance.RxPCN = G0506_PrimaryInsurance_RxPCN;
                    primaryInsurance.RxGroup = G0506_PrimaryInsurance_RxGroup;
                    primaryInsurance.RelatedtoInsurance = G0506_PrimaryInsurance_RelatedtoInsurance;
                    primaryInsurance.Status = true;
                    primaryInsurance.UpdatedBy = User.Identity.GetUserId();
                    primaryInsurance.UpdatedOn = DateTime.Now;
                    _db.Entry(primaryInsurance).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {

                    primaryInsurance = new G0506_PrimaryInsurance();
                    primaryInsurance.Name = G0506_PrimaryInsurance_Name;
                    primaryInsurance.PlanName = G0506_PrimaryInsurance_PlanName;
                    if (G0506_PrimaryInsurance_InsuranceId != "")
                    {
                        int? insuranceid = Convert.ToInt32(G0506_PrimaryInsurance_InsuranceId);
                        primaryInsurance.InsuranceId = insuranceid;

                    }
                    if (G0506_PrimaryInsurance_GroupNumber != "")
                    {
                        int? groupno = Convert.ToInt32(G0506_PrimaryInsurance_GroupNumber);
                        primaryInsurance.GroupNumber = groupno;

                    }


                    primaryInsurance.PlanCode = G0506_PrimaryInsurance_PlanCode;
                    primaryInsurance.RxBin = G0506_PrimaryInsurance_RxBin;
                    primaryInsurance.RxPCN = G0506_PrimaryInsurance_RxPCN;
                    primaryInsurance.RxGroup = G0506_PrimaryInsurance_RxGroup;
                    primaryInsurance.RelatedtoInsurance = G0506_PrimaryInsurance_RelatedtoInsurance;
                    primaryInsurance.Status = true;
                    primaryInsurance.UpdatedBy = User.Identity.GetUserId();
                    primaryInsurance.UpdatedOn = DateTime.Now;
                    _db.G0506_PrimaryInsurance.Add(primaryInsurance);
                    _db.SaveChanges();


                }


                var secondaryInsurance = _db.G0506_SecondaryInsurance.Where(p => p.Id == checkpatientinfo.G0506_SecondaryInsuranceId).FirstOrDefault();
                if (secondaryInsurance != null)
                {
                    if (G0506_SecondaryInsurance_MedicareNo != "")
                    {
                        int? MedicareNo = Convert.ToInt32(G0506_SecondaryInsurance_MedicareNo);
                        secondaryInsurance.MedicareNo = MedicareNo;

                    }
                    if (G0506_SecondaryInsurance_MedicaidNo != "")
                    {
                        int? MedicaidNo = Convert.ToInt32(G0506_SecondaryInsurance_MedicaidNo);
                        secondaryInsurance.MedicaidNo = MedicaidNo;

                    }
                    if (G0506_SecondaryInsurance_OtherInsuranceNo != "")
                    {
                        int? OtherInsuranceNo = Convert.ToInt32(G0506_SecondaryInsurance_OtherInsuranceNo);
                        secondaryInsurance.OtherInsuranceNo = OtherInsuranceNo;

                    }


                    secondaryInsurance.Status = true;
                    secondaryInsurance.UpdatedOn = DateTime.Now;
                    secondaryInsurance.UpdatedBy = User.Identity.GetUserId();
                    _db.Entry(secondaryInsurance).State = EntityState.Modified;
                    _db.SaveChanges();

                }
                else
                {

                    secondaryInsurance = new G0506_SecondaryInsurance();
                    if (G0506_SecondaryInsurance_MedicareNo != "")
                    {
                        int? MedicareNo = Convert.ToInt32(G0506_SecondaryInsurance_MedicareNo);
                        secondaryInsurance.MedicareNo = MedicareNo;
                    }
                    if (G0506_SecondaryInsurance_MedicaidNo != "")
                    {
                        int? MedicaidNo = Convert.ToInt32(G0506_SecondaryInsurance_MedicaidNo);
                        secondaryInsurance.MedicaidNo = MedicaidNo;
                    }
                    if (G0506_SecondaryInsurance_OtherInsuranceNo != "")
                    {
                        int? OtherInsuranceNo = Convert.ToInt32(G0506_SecondaryInsurance_OtherInsuranceNo);
                        secondaryInsurance.OtherInsuranceNo = OtherInsuranceNo;

                    }


                    secondaryInsurance.Status = true;
                    secondaryInsurance.UpdatedOn = DateTime.Now;
                    secondaryInsurance.UpdatedBy = User.Identity.GetUserId();
                    _db.G0506_SecondaryInsurance.Add(secondaryInsurance);
                    _db.SaveChanges();


                }


                checkpatientinfo.PatientId = patientId;
                checkpatientinfo.FullName = G0506_PatientsInfo_FullName;
                if (G0506_PatientsInfo_BirthDate != "")
                {
                    var date = Convert.ToDateTime(DateTime.ParseExact(G0506_PatientsInfo_BirthDate, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                    checkpatientinfo.BirthDate = Convert.ToDateTime(date);
                }
                checkpatientinfo.IsCurrentlyActiveinCCM = Convert.ToBoolean(G0506_PatientsInfo_IsCurrentlyActiveinCCM);
                checkpatientinfo.IsCCMConsentcompleted = Convert.ToBoolean(G0506_PatientsInfo_IsCCMConsentcompleted);
                if (G0506_PatientsInfo_DateConsentcompleted != "")
                {

                    checkpatientinfo.DateConsentcompleted = Convert.ToDateTime(DateTime.ParseExact(G0506_PatientsInfo_DateConsentcompleted, "MM/dd/yyyy", CultureInfo.InvariantCulture)); 
                }

                checkpatientinfo.DesignatedCCMContact = G0506_PatientsInfo_DesignatedCCMContact;
                checkpatientinfo.G0506_PrimaryInsuranceId = primaryInsurance.Id;
                checkpatientinfo.G0506_SecondaryInsuranceId = secondaryInsurance.Id;

                _db.Entry(checkpatientinfo).State = EntityState.Modified;
                _db.SaveChanges();

                if (PrimaryCareProviderId != "")
                {
                    int? primarycareid = Convert.ToInt32(PrimaryCareProviderId);

                    if (primarycareid != null)
                    {
                        var additionalproviderslist = _db.G0506_AdditionalProviders.ToList();
                        if (additionalproviderslist.Count() > 0)
                        {
                            var additionalpro = additionalproviderslist.Where(p => p.Id == primarycareid).FirstOrDefault();
                            if (additionalpro != null)
                            {
                                additionalpro.IsPrimaryCareProvider = false;

                                _db.Entry(additionalpro).State = EntityState.Modified;
                                _db.SaveChanges();
                            }


                            var additionalproviders = additionalproviderslist.Where(p => p.Id == primarycareid).FirstOrDefault();
                            additionalproviders.IsPrimaryCareProvider = true;
                            additionalproviders.G0506_PatientsInfoId = checkpatientinfo.Id;

                            _db.Entry(additionalproviders).State = EntityState.Modified;
                            _db.SaveChanges();




                        }
                    }
                }
            }
            //save 
            else
            {
                G0506_PrimaryInsurance primaryInsurance = new G0506_PrimaryInsurance();
                primaryInsurance.Name = G0506_PrimaryInsurance_Name;
                primaryInsurance.PlanName = G0506_PrimaryInsurance_PlanName;
                if (G0506_PrimaryInsurance_InsuranceId != "")
                {
                    int? insuranceid = Convert.ToInt32(G0506_PrimaryInsurance_InsuranceId);
                    primaryInsurance.InsuranceId = insuranceid;
                }
                if (G0506_PrimaryInsurance_GroupNumber != "")
                {
                    int? groupno = Convert.ToInt32(G0506_PrimaryInsurance_GroupNumber);
                    primaryInsurance.GroupNumber = groupno;
                }


                primaryInsurance.PlanCode = G0506_PrimaryInsurance_PlanCode;
                primaryInsurance.RxBin = G0506_PrimaryInsurance_RxBin;
                primaryInsurance.RxPCN = G0506_PrimaryInsurance_RxPCN;
                primaryInsurance.RxGroup = G0506_PrimaryInsurance_RxGroup;
                primaryInsurance.RelatedtoInsurance = G0506_PrimaryInsurance_RelatedtoInsurance;
                primaryInsurance.Status = true;
                primaryInsurance.CreatedBy = User.Identity.GetUserId();
                primaryInsurance.CreatedOn = DateTime.Now;
                _db.G0506_PrimaryInsurance.Add(primaryInsurance);
                _db.SaveChanges();
                G0506_SecondaryInsurance secondaryInsurance = new G0506_SecondaryInsurance();
                if (G0506_SecondaryInsurance_MedicareNo != "")
                {
                    int? MedicareNo = Convert.ToInt32(G0506_SecondaryInsurance_MedicareNo);
                    secondaryInsurance.MedicareNo = MedicareNo;
                }
                if (G0506_SecondaryInsurance_MedicaidNo != "")
                {
                    int? MedicaidNo = Convert.ToInt32(G0506_SecondaryInsurance_MedicaidNo);
                    secondaryInsurance.MedicaidNo = MedicaidNo;
                }

                if (G0506_SecondaryInsurance_OtherInsuranceNo != "")
                {
                    int? OtherInsuranceNo = Convert.ToInt32(G0506_SecondaryInsurance_OtherInsuranceNo);
                    secondaryInsurance.OtherInsuranceNo = OtherInsuranceNo;
                }

                secondaryInsurance.Status = true;
                secondaryInsurance.CreatedOn = DateTime.Now;
                secondaryInsurance.CreatedBy = User.Identity.GetUserId();

                _db.G0506_SecondaryInsurance.Add(secondaryInsurance);
                _db.SaveChanges();

                G0506_PatientsInfo _PatientsInfo = new G0506_PatientsInfo();

                _PatientsInfo.PatientId = patientId;
                _PatientsInfo.FullName = G0506_PatientsInfo_FullName;
                if (G0506_PatientsInfo_BirthDate != "")
                {
                    _PatientsInfo.BirthDate = Convert.ToDateTime(DateTime.ParseExact(G0506_PatientsInfo_BirthDate, "MM/dd/yyyy", CultureInfo.InvariantCulture));                 }
                _PatientsInfo.IsCurrentlyActiveinCCM = Convert.ToBoolean(G0506_PatientsInfo_IsCurrentlyActiveinCCM);
                _PatientsInfo.IsCCMConsentcompleted = Convert.ToBoolean(G0506_PatientsInfo_IsCCMConsentcompleted);
                if (G0506_PatientsInfo_DateConsentcompleted != "")
                {
                    _PatientsInfo.DateConsentcompleted = Convert.ToDateTime(DateTime.ParseExact(G0506_PatientsInfo_DateConsentcompleted, "MM/dd/yyyy", CultureInfo.InvariantCulture)); 
                }
                _PatientsInfo.DesignatedCCMContact = G0506_PatientsInfo_DesignatedCCMContact;
                _PatientsInfo.G0506_PrimaryInsuranceId = primaryInsurance.Id;
                _PatientsInfo.G0506_SecondaryInsuranceId = secondaryInsurance.Id;

                _db.G0506_PatientsInfo.Add(_PatientsInfo);
                _db.SaveChanges();
                if (PrimaryCareProviderId != "")
                {
                    int? primarycareid = Convert.ToInt32(PrimaryCareProviderId);

                    if (primarycareid != null)
                    {
                        var additionalproviderslist = _db.G0506_AdditionalProviders.ToList();
                        if (additionalproviderslist.Count() > 0)
                        {
                            var additionalproviders = additionalproviderslist.Where(p => p.Id == primarycareid).FirstOrDefault();
                            additionalproviders.IsPrimaryCareProvider = true;
                            additionalproviders.G0506_PatientsInfoId = _PatientsInfo.Id;
                            _db.Entry(additionalproviders).State = EntityState.Modified;
                            _db.SaveChanges();

                        }
                    }
                }


            }

            //G0506_FormSubmit            


            //update
            var UpdateCheck = _db.EvaluationFormData.Where(p => p.PatientId == patientId && p.EvaluationFormId == FormId).Count();

            if (UpdateCheck > 0)
            {
              var formdata=  _db.EvaluationFormData.Where(p => p.PatientId == patientId).ToList();
                foreach (var Questions in EvaluationFormData.MainQuestionViewModal)
                {
                    EvaluationFormData evaluationFormData = new EvaluationFormData();
                    var mainquestion = formdata.Where(p => p.MainQuestionGuid.Contains(Questions.QuestionGUID)).FirstOrDefault();

                    if (Questions.haveSubQuestion != "yes")
                    {
                


                        if (mainquestion != null)
                        {
                            mainquestion.MainQuestionGuid = Questions.QuestionGUID;
                            mainquestion.EvaluationFormId = FormId;
                            mainquestion.PatientId = patientId;
                            mainquestion.Status = true;
                            mainquestion.CreatedOn = DateTime.Now;
                            mainquestion.CreatedBy = User.Identity.GetUserId();
                            if (Questions.haveDateTime == "yes")
                            {
                                mainquestion.HasDate = true;
                                if (Questions.Date != "")
                                {
                                    mainquestion.Date = Convert.ToDateTime(DateTime.ParseExact(Questions.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                                }


                            }
                            else
                            {
                                mainquestion.HasDate = false;
                            }

                            _db.Entry(mainquestion).State = EntityState.Modified;
                            _db.SaveChanges();

                        }
                        else
                        {

                        evaluationFormData.MainQuestionGuid = Questions.QuestionGUID;
                        evaluationFormData.EvaluationFormId = FormId;
                        evaluationFormData.PatientId = patientId;
                        evaluationFormData.Status = true;
                        evaluationFormData.CreatedOn = DateTime.Now;
                        evaluationFormData.CreatedBy = User.Identity.GetUserId();
                        if (Questions.haveDateTime == "yes")
                        {
                            evaluationFormData.HasDate = true;
                            if (Questions.Date != "")
                            {
                                evaluationFormData.Date = Convert.ToDateTime(DateTime.ParseExact(Questions.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                            }


                        }
                        else
                        {
                            evaluationFormData.HasDate = false;
                        }
                        _db.EvaluationFormData.Add(evaluationFormData);
                        _db.SaveChanges();
                        }
                    }
                    else
                    if (Questions.haveSubQuestion == "yes")
                    {


                        foreach (var subQuestions in Questions.subQuestions)
                        {
                            EvaluationFormData evaluationFormData2 = new EvaluationFormData();
                            var subquestion = formdata.Where(p => p.MainQuestionGuid.Contains(subQuestions.QuestionGUID)).FirstOrDefault();
                            if (subquestion != null)
                            {

                                subquestion.MainQuestionGuid = subQuestions.QuestionGUID;
                                subquestion.EvaluationFormId = FormId;
                                subquestion.PatientId = patientId;
                                subquestion.Status = true;
                                subquestion.CreatedOn = DateTime.Now;
                                subquestion.CreatedBy = User.Identity.GetUserId();
                                if (subQuestions.haveDateTime == "yes")
                                {
                                    subquestion.HasDate = true;
                                    if (subQuestions.Date != "")
                                    {

                                        var date = Convert.ToDateTime(DateTime.ParseExact(subQuestions.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                                        subquestion.Date = date;
                                        //evaluationFormData2.Date = DateTime.ParseExact(subQuestions.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
                                    }


                                }
                                else
                                {
                                    subquestion.HasDate = false;
                                }

                                _db.Entry(subquestion).State = EntityState.Modified;
                                _db.SaveChanges();

                            }
                            else
                            {

                           
                            evaluationFormData2.MainQuestionGuid = subQuestions.QuestionGUID;
                            evaluationFormData2.EvaluationFormId = FormId;
                            evaluationFormData2.PatientId = patientId;
                            evaluationFormData2.Status = true;
                            evaluationFormData2.CreatedOn = DateTime.Now;
                            evaluationFormData2.CreatedBy = User.Identity.GetUserId();
                            if (subQuestions.haveDateTime == "yes")
                            {
                                evaluationFormData2.HasDate = true;
                                if (subQuestions.Date != "")
                                {

                                    var date = Convert.ToDateTime(DateTime.ParseExact(subQuestions.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                                    evaluationFormData2.Date = date;
                                    //evaluationFormData2.Date = DateTime.ParseExact(subQuestions.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
                                }


                            }
                            else
                            {
                                evaluationFormData2.HasDate = false;
                            }
                            _db.EvaluationFormData.Add(evaluationFormData2);
                            _db.SaveChanges();
                            }
                            if (subquestion != null)
                            {
                                if (subQuestions.AnswerType == "text")
                                {
                                    subquestion.Answer = subQuestions.CurrentAnswer;
                                    _db.Entry(subquestion).State = EntityState.Modified;
                                    _db.SaveChanges();

                                }
                                else if (subQuestions.AnswerType == "radio")
                                {
                                    foreach (var subAnswer in subQuestions.answers)
                                    {

                                        subquestion.AnswerGuid = subAnswer.QuestionGUID;

                                        _db.Entry(subquestion).State = EntityState.Modified;
                                        _db.SaveChanges();

                                    }



                                }
                                else if (subQuestions.AnswerType == "checkbox")
                                {
                                    var checkbox = _db.EvaluationFormDataAnswersForCheckBox.Where(x => x.EvaluationFormDataId == subquestion.Id && x.Status == true).ToList();

                                    if (checkbox.Count() > 0)
                                    {
                                        foreach (var item in checkbox)
                                        {
                                            item.UpdatedBy = User.Identity.GetUserId();
                                            item.UpdatedOn = DateTime.Now;
                                            item.Status = false;
                                            _db.Entry(item).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    foreach (var subAnswer in subQuestions.answers)
                                    {


                                        if (subAnswer.isChecked == "yes")
                                        {

                                        EvaluationFormDataAnswersForCheckBox forCheckBox = new EvaluationFormDataAnswersForCheckBox();
                                        forCheckBox.AnswerGuid = subAnswer.QuestionGUID;
                                        forCheckBox.Answer = subAnswer.Answer;
                                        forCheckBox.HasDate = false;
                                        forCheckBox.Status = true;
                                        forCheckBox.Date = null;
                                        forCheckBox.CreatedOn = DateTime.Now;
                                        forCheckBox.CreatedBy = User.Identity.GetUserId();
                                        forCheckBox.EvaluationFormDataId = subquestion.Id;
                                        _db.EvaluationFormDataAnswersForCheckBox.Add(forCheckBox);
                                        _db.SaveChanges();
                                        }



                                    }


                                }

                            }
                            else
                            {

                            if (subQuestions.AnswerType == "text")
                            {
                                evaluationFormData2.Answer = subQuestions.CurrentAnswer;
                                _db.Entry(evaluationFormData2).State = EntityState.Modified;
                                _db.SaveChanges();

                            }
                            else if (subQuestions.AnswerType == "radio")
                            {
                                foreach (var subAnswer in subQuestions.answers)
                                {

                                    evaluationFormData2.AnswerGuid = subAnswer.QuestionGUID;

                                    _db.Entry(evaluationFormData2).State = EntityState.Modified;
                                    _db.SaveChanges();

                                }



                            }
                            else if (subQuestions.AnswerType == "checkbox")
                            {
                                foreach (var subAnswer in subQuestions.answers)
                                {
                                        if (subAnswer.isChecked == "yes")
                                        {

                                        EvaluationFormDataAnswersForCheckBox forCheckBox = new EvaluationFormDataAnswersForCheckBox();
                                    forCheckBox.AnswerGuid = subAnswer.QuestionGUID;
                                    forCheckBox.Answer = subAnswer.Answer;
                                    forCheckBox.HasDate = false;
                                    forCheckBox.Status = true;
                                    forCheckBox.Date = null;
                                    forCheckBox.CreatedOn = DateTime.Now;
                                    forCheckBox.CreatedBy = User.Identity.GetUserId();
                                    forCheckBox.EvaluationFormDataId = evaluationFormData2.Id;
                                    _db.EvaluationFormDataAnswersForCheckBox.Add(forCheckBox);
                                    _db.SaveChanges();

                                        }


                                }


                            }

                            }



                        }




                    }
                    
                    

                    if (mainquestion != null)
                    {
                        if (Questions.AnswerType == "text")
                        {
                            mainquestion.Answer = Questions.CurrentAnswer;
                            _db.Entry(mainquestion).State = EntityState.Modified;
                            _db.SaveChanges();

                        }
                        else if (Questions.AnswerType == "radio")
                        {
                            foreach (var mainAnswer in Questions.MainAnswer)
                            {

                                mainquestion.AnswerGuid = mainAnswer.QuestionGUID;

                                _db.Entry(mainquestion).State = EntityState.Modified;
                                _db.SaveChanges();

                            }



                        }
                        else if (Questions.AnswerType == "checkbox")
                        {
                                var checkbox = _db.EvaluationFormDataAnswersForCheckBox.Where(x => x.EvaluationFormDataId == mainquestion.Id).ToList();

                                if (checkbox.Count() > 0)
                                {
                                    foreach (var item in checkbox)
                                    {
                                        item.UpdatedBy = User.Identity.GetUserId();
                                        item.UpdatedOn = DateTime.Now;
                                        item.Status = false;
                                        _db.Entry(item).State = EntityState.Modified;
                                        _db.SaveChanges();
                                    }
                                }
                            foreach (var mainAnswer in Questions.MainAnswer)
                            {
                                if (mainAnswer.isChecked == "yes")
                                {
                                EvaluationFormDataAnswersForCheckBox forCheckBox = new EvaluationFormDataAnswersForCheckBox();
                                forCheckBox.AnswerGuid = mainAnswer.QuestionGUID;
                                forCheckBox.Answer = mainAnswer.Answer;
                                forCheckBox.HasDate = false;
                                forCheckBox.Status = true;
                                forCheckBox.Date = null;
                                forCheckBox.CreatedOn = DateTime.Now;
                                forCheckBox.CreatedBy = User.Identity.GetUserId();
                                forCheckBox.EvaluationFormDataId = mainquestion.Id;
                                _db.EvaluationFormDataAnswersForCheckBox.Add(forCheckBox);
                                _db.SaveChanges();


                                }

                            }


                        }
                    }
                    else
                    {

                    if (Questions.AnswerType == "text")
                    {
                        evaluationFormData.Answer = Questions.CurrentAnswer;
                        _db.Entry(evaluationFormData).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                    else if (Questions.AnswerType == "radio")
                    {
                        foreach (var mainAnswer in Questions.MainAnswer)
                        {

                            evaluationFormData.AnswerGuid = mainAnswer.QuestionGUID;

                            _db.Entry(evaluationFormData).State = EntityState.Modified;
                            _db.SaveChanges();

                        }



                    }
                    else if (Questions.AnswerType == "checkbox")
                    {
                        foreach (var mainAnswer in Questions.MainAnswer)
                        {
                            EvaluationFormDataAnswersForCheckBox forCheckBox = new EvaluationFormDataAnswersForCheckBox();
                            forCheckBox.AnswerGuid = mainAnswer.QuestionGUID;
                            forCheckBox.Answer = mainAnswer.Answer;
                            forCheckBox.HasDate = false;
                            forCheckBox.Status = true;
                            forCheckBox.Date = null;
                            forCheckBox.CreatedOn = DateTime.Now;
                            forCheckBox.CreatedBy = User.Identity.GetUserId();
                            forCheckBox.EvaluationFormDataId = evaluationFormData.Id;
                            _db.EvaluationFormDataAnswersForCheckBox.Add(forCheckBox);
                            _db.SaveChanges();


                        }


                    }


                    }
                 








                }



            }





            //Save

            else
            {

            foreach (var Questions in EvaluationFormData.MainQuestionViewModal)
            {
                EvaluationFormData evaluationFormData = new EvaluationFormData();


                if (Questions.haveSubQuestion != "yes")
                {


                    evaluationFormData.MainQuestionGuid = Questions.QuestionGUID;
                evaluationFormData.EvaluationFormId = FormId;
                evaluationFormData.PatientId = patientId;
                evaluationFormData.Status = true;
                evaluationFormData.CreatedOn = DateTime.Now;
                evaluationFormData.CreatedBy = User.Identity.GetUserId();
                    if (Questions.haveDateTime == "yes")
                    {
                        evaluationFormData.HasDate = true;
                        if (Questions.Date != "") {
                        evaluationFormData.Date = Convert.ToDateTime(DateTime.ParseExact(Questions.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                            }


                    }
                    else
                    {
                        evaluationFormData.HasDate = false;
                    }
                _db.EvaluationFormData.Add(evaluationFormData);
                _db.SaveChanges();
                }
               else              
                if (Questions.haveSubQuestion == "yes")
                {


                    foreach (var subQuestions in Questions.subQuestions)
                    {
                        EvaluationFormData evaluationFormData2 = new EvaluationFormData();
                        evaluationFormData2.MainQuestionGuid = subQuestions.QuestionGUID;
                        evaluationFormData2.EvaluationFormId = FormId;
                        evaluationFormData2.PatientId = patientId;
                        evaluationFormData2.Status = true;
                        evaluationFormData2.CreatedOn = DateTime.Now;
                        evaluationFormData2.CreatedBy = User.Identity.GetUserId();
                        if (subQuestions.haveDateTime == "yes")
                        {
                            evaluationFormData2.HasDate = true;
                            if (subQuestions.Date != "")
                            {
                               
                            var date = Convert.ToDateTime( DateTime.ParseExact(subQuestions.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture)) ;
                                evaluationFormData2.Date = date;
                            //evaluationFormData2.Date = DateTime.ParseExact(subQuestions.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
                            }


                        }
                        else
                        {
                            evaluationFormData2.HasDate = false;
                        }
                        _db.EvaluationFormData.Add(evaluationFormData2);
                        _db.SaveChanges();
                        if (subQuestions.AnswerType == "text")
                        {
                            evaluationFormData2.Answer = subQuestions.CurrentAnswer;
                            _db.Entry(evaluationFormData2).State = EntityState.Modified;
                            _db.SaveChanges();

                        }
                        else if (subQuestions.AnswerType == "radio")
                        {
                            foreach (var subAnswer in subQuestions.answers)
                            {

                                evaluationFormData2.AnswerGuid = subAnswer.QuestionGUID;

                                _db.Entry(evaluationFormData2).State = EntityState.Modified;
                                _db.SaveChanges();

                            }



                        }
                        else if (subQuestions.AnswerType == "checkbox")
                        {
                            foreach (var subAnswer in subQuestions.answers)
                            {
                                    if (subAnswer.isChecked == "Yes")
                                    {
                                        EvaluationFormDataAnswersForCheckBox forCheckBox = new EvaluationFormDataAnswersForCheckBox();
                                        forCheckBox.AnswerGuid = subAnswer.QuestionGUID;
                                        forCheckBox.Answer = subAnswer.Answer;
                                        forCheckBox.HasDate = false;
                                        forCheckBox.Status = true;
                                        forCheckBox.Date = null;
                                        forCheckBox.CreatedOn = DateTime.Now;
                                        forCheckBox.CreatedBy = User.Identity.GetUserId();
                                        forCheckBox.EvaluationFormDataId = evaluationFormData2.Id;
                                        _db.EvaluationFormDataAnswersForCheckBox.Add(forCheckBox);
                                        _db.SaveChanges();
                                    }

                            }


                        }




                    }



                    
                }
              if(Questions.AnswerType=="text")
                {
                    evaluationFormData.Answer = Questions.CurrentAnswer;
                    _db.Entry(evaluationFormData).State = EntityState.Modified;
                    _db.SaveChanges();

                }
                 else   if(Questions.AnswerType == "radio")
                    {
                        foreach (var mainAnswer in Questions.MainAnswer)
                        {

                            evaluationFormData.AnswerGuid = mainAnswer.QuestionGUID;

                        _db.Entry(evaluationFormData).State = EntityState.Modified;
                        _db.SaveChanges();

                    }



                    }
                    else if(Questions.AnswerType=="checkbox")
                    {
                        foreach (var mainAnswer in Questions.MainAnswer)
                        {
                            if (mainAnswer.isChecked == "yes")
                            {
                                EvaluationFormDataAnswersForCheckBox forCheckBox = new EvaluationFormDataAnswersForCheckBox();
                                forCheckBox.AnswerGuid = mainAnswer.QuestionGUID;
                                forCheckBox.Answer = mainAnswer.Answer;
                                forCheckBox.HasDate = false;
                                forCheckBox.Status = true;
                                forCheckBox.Date = null;
                                forCheckBox.CreatedOn = DateTime.Now;
                                forCheckBox.CreatedBy = User.Identity.GetUserId();
                                forCheckBox.EvaluationFormDataId = evaluationFormData.Id;
                                _db.EvaluationFormDataAnswersForCheckBox.Add(forCheckBox);
                                _db.SaveChanges();
                            }

                        }


                    }



                





            }

            }






            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public string SaveEvaluationForm(int BillingCategoryId, string formName,string formData,int?formId,int?patientId)
        {
            List<MainQuestionViewModal> EvaluationInfo = JsonConvert.DeserializeObject<List<MainQuestionViewModal>>(formData);
            Evaluation evaluation = new Evaluation();

           
            evaluation.BillingCategoryId = BillingCategoryId;
            evaluation.Name = formName;
            evaluation.Status = true;
            evaluation.CreatedOn = DateTime.Now;
            evaluation.CreatedBy = User.Identity.GetUserId();
            _db.Evaluation.Add(evaluation);
            _db.SaveChanges();
            foreach (var Questions in EvaluationInfo)
            {
                Evaluation_Questions evaluation_Questions = new Evaluation_Questions();
                int? QuestionId = Convert.ToInt32(Questions.QuestionId);
                int? Order = Convert.ToInt32(Questions.sortIndex);


            evaluation_Questions.EvaluationsId = evaluation.Id;
                evaluation_Questions.QuestionGUID = HelperExtensions.GenerateGUID();
                evaluation_Questions.QuestionId = QuestionId;
                evaluation_Questions.Status = true;
                if (Questions.haveSubQuestion == "yes")
                {
                evaluation_Questions.HasSubtype = true;

                }
                else 
                {
                    evaluation_Questions.HasSubtype = false;
                }

                evaluation_Questions.Type = Questions.AnswerType;
                evaluation_Questions.Order = Order;
                if (Questions.haveDateTime == "yes")
                {

                    evaluation_Questions.HasDate = true;
                }
                else
                {
                    evaluation_Questions.HasDate = false;

                }

                evaluation_Questions.CreatedOn = DateTime.Now;
                evaluation_Questions.CreatedBy = User.Identity.GetUserId();

                _db.Evaluation_Questions.Add(evaluation_Questions);
                _db.SaveChanges();

                if (evaluation_Questions.HasSubtype == true)
                {
                    foreach (var SubQuestion in Questions.subQuestions)
                    {
                        Evaluation_SubQuestions evaluation_SubQuestions = new Evaluation_SubQuestions();
                        int? SubQuestionId = Convert.ToInt32(SubQuestion.QuestionId);

                        evaluation_SubQuestions.Name = SubQuestion.Question;
                        evaluation_SubQuestions.QuestionId = SubQuestionId;
                        evaluation_SubQuestions.QuestionGUID = HelperExtensions.GenerateGUID();

                        if (SubQuestion.haveDateTime == "yes")
                        {

                        evaluation_SubQuestions.HasDate = true;
                        }
                        else
                        {
                            evaluation_SubQuestions.HasDate = false;

                        }
                        evaluation_SubQuestions.Status = true;
                        evaluation_SubQuestions.Evaluation_QuestionsId = evaluation_Questions.Id;
                       
                        evaluation_SubQuestions.Type = SubQuestion.AnswerType;

                        evaluation_SubQuestions.CreatedOn = DateTime.Now;
                        evaluation_SubQuestions.CreatedBy = User.Identity.GetUserId();
                        _db.Evaluation_SubQuestions.Add(evaluation_SubQuestions);
                        _db.SaveChanges();

                        if (evaluation_SubQuestions.Type != "text")
                        {
                            foreach (var SubAnswers in SubQuestion.answers)
                            {
                                Evaluation_SubQuestionAnswer evaluation_SubQuestionAnswer = new Evaluation_SubQuestionAnswer();
                                int SubAnswerId=Convert.ToInt32( SubAnswers.AnswerId);

                                evaluation_SubQuestionAnswer.SubQuestionId = evaluation_SubQuestions.Id;
                                evaluation_SubQuestionAnswer.QuestionGUID = HelperExtensions.GenerateGUID();


                                evaluation_SubQuestionAnswer.QuestionId = SubAnswerId;
                                evaluation_SubQuestionAnswer.Question = SubAnswers.Answer;
                                evaluation_SubQuestionAnswer.Type = SubAnswers.type;
                                if (SubAnswers.haveDateTime == "yes")
                                {

                                evaluation_SubQuestionAnswer.HaveDateTime = true;
                                }
                                else
                                {
                                    evaluation_SubQuestionAnswer.HaveDateTime = false;

                                }
                                
                                evaluation_SubQuestionAnswer.CreatedOn = DateTime.Now;
                                evaluation_SubQuestionAnswer.CreatedBy = User.Identity.GetUserId();
                                evaluation_SubQuestionAnswer.Status = true;
                                _db.Evaluation_SubQuestionAnswer.Add(evaluation_SubQuestionAnswer);
                                _db.SaveChanges();

                            }



                        }



                    }



                }
                else
                {
                    if (evaluation_Questions.Type != "text")
                    {

                        foreach (var MainQuestionAnswer in Questions.MainAnswer)
                        {
                            Evaluation_MainQuestionAnswer evaluation_MainQuestionAnswer = new Evaluation_MainQuestionAnswer();

                            int? MainQuestionAnswerId =Convert.ToInt32( MainQuestionAnswer.AnswerId);
                            evaluation_MainQuestionAnswer.MainQuestionId = evaluation_Questions.Id;
                            evaluation_MainQuestionAnswer.Evaluation_Questions = evaluation_Questions;
                            evaluation_MainQuestionAnswer.QuestionGUID = HelperExtensions.GenerateGUID();

                            evaluation_MainQuestionAnswer.QuestionId = MainQuestionAnswerId;
                            evaluation_MainQuestionAnswer.Status = true;
                            evaluation_MainQuestionAnswer.Question = MainQuestionAnswer.Answer;
                            evaluation_MainQuestionAnswer.CreatedOn = DateTime.Now;
                            evaluation_MainQuestionAnswer.CreatedBy = User.Identity.GetUserId();
                            _db.Evaluation_MainQuestionAnswer.Add(evaluation_MainQuestionAnswer);
                            _db.SaveChanges();



                        }


                    }



                }




            }





            return "";
        }



        public JsonResult GetEvaluationForm (int? BillingCategoryId ,int? PatientId)
        {
            List<MainQuestionViewModal> mainQuestionViewModallist = new List<MainQuestionViewModal>();


            var formdata = _db.EvaluationFormData.Where(p => p.PatientId == PatientId).ToList();

            var evaluationformlist = _db.Evaluation.Where(p => p.BillingCategoryId == BillingCategoryId).ToList();

            if (evaluationformlist.Count() > 0)
            {
                var evaluationform = evaluationformlist.LastOrDefault();
                EvaluationViewModel evaluationViewModel = new EvaluationViewModel();

                evaluationViewModel.BillingCategoryId = evaluationform.BillingCategoryId.ToString();

                evaluationViewModel.Name = evaluationform.Name;
                evaluationViewModel.Id = evaluationform.Id.ToString();

                var EvaluationQuestions = _db.Evaluation_Questions.Where(p => p.EvaluationsId == evaluationform.Id).ToList();

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


                        var Subquestions = _db.Evaluation_SubQuestions.Where(p => p.Evaluation_QuestionsId == questions.Id).ToList();
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
                                var subquestionAnswers = _db.Evaluation_SubQuestionAnswer.Where(p => p.SubQuestionId == subquestions.Id).ToList();
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
                                                subQuestionsViewModal.Date = time.Date.ToString("MM/dd/yyyy");
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
                                            var checkboxes = _db.EvaluationFormDataAnswersForCheckBox.Where(p => p.EvaluationFormDataId == subquestionguidlist.Id && p.Status == true).ToList();
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


                            var mainquestionsanswers = _db.Evaluation_MainQuestionAnswer.Where(p => p.MainQuestionId == questions.Id).ToList();
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
                                        if (mainanswerlist.Date != null)
                                        {
                                        DateTime dateTime = (DateTime)mainanswerlist.Date;

                                        mainQuestionViewModal.Date = dateTime.ToString("MM/dd/yyyy");

                                        }

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
                                        var checkboxes = _db.EvaluationFormDataAnswersForCheckBox.Where(p => p.EvaluationFormDataId == mainanswerlist.Id && p.Status==true).ToList();
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






                return Json(evaluationViewModel, JsonRequestBehavior.AllowGet);

            }
            else
            {
                EvaluationViewModel evaluationViewModel = new EvaluationViewModel();
                return Json(evaluationViewModel, JsonRequestBehavior.AllowGet);
            }

        }


        //    [HttpPost]
        //    public JsonResult getEvaluationFormPreview(int FormId, string QuestionList)
        //    {
        //       PreviewFormViewModelRaw NewQuestion = JsonConvert.DeserializeObject<PreviewFormViewModelRaw>(QuestionList);
        //        int QuestionId = Convert.ToInt32(NewQuestion.QuestionId);
        //        var model = NewQuestion;

        //        var exist = PreviewQuestionList.FirstOrDefault(x => x.QuestionId == QuestionId);
        //        if (exist!= null)
        //        {
        //            PreviewQuestionList.Remove(exist);
        //        }
        //        if(model.haveSubQuestion=="no" && model.MainAnswer == "")
        //        {
        //            PreviewFormViewModel NewQuestiontoAdd = new PreviewFormViewModel();
        //            NewQuestiontoAdd.QuestionId = QuestionId;
        //            NewQuestiontoAdd.Question= _db.QuestionBanks.Where(x => x.Id == QuestionId).FirstOrDefault().Question;
        //            NewQuestiontoAdd.haveSubQuestion = NewQuestion.haveSubQuestion;
        //            NewQuestiontoAdd.AnswerType = NewQuestion.AnswerType;
        //            PreviewQuestionList.Add(NewQuestiontoAdd);
        //        }


        //        return Json(PreviewQuestionList,JsonRequestBehavior.AllowGet);
        //    }

        //}
        //public class PreviewFormViewModelRaw
        //{
        //    public string QuestionId { get; set; }
        //    public string haveSubQuestion { get; set; }
        //    public string AnswerType { get; set; }
        //    public string MainAnswer { get; set; }
        //}
        //public class PreviewFormViewModel
        //{
        //    public int QuestionId { get; set; }
        //    public string Question { get; set; }
        //    public string haveSubQuestion { get; set; }
        //    public string AnswerType { get; set; }
        //    public List<SubQuestionFormViewModel> SubQuestions { get; set; }
        //}
        //public class  SubQuestionFormViewModel
        //{
        //    public int QuestionId { get; set; }
        //    public string Question { get; set; }

    //}
  

}
}