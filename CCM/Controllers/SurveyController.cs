using CCM.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Physician, PhysiciansGroup, Admin, QAQC, LiaisonGroup")]
    public class SurveyController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        //string apiurl = WebConfigurationManager.AppSettings["apiUrl"];
        //string apiSurvey = WebConfigurationManager.AppSettings["apiSurvey"];
        //string apiSurveyType = WebConfigurationManager.AppSettings["apiSurveyTypes"];
        //string apiSurveySections = WebConfigurationManager.AppSettings["apiSurveySection"];
        //// GET: Default
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> AddQuestions()
        {
            return View();
        }
        public List<List<T>> GetAllCombos<T>(List<T> list)
        {
            int comboCount = (int)Math.Pow(2, list.Count) - 1;
            List<List<T>> result = new List<List<T>>();
            for (int i = 1; i < comboCount + 1; i++)
            {
                // make each combo here
                result.Add(new List<T>());
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }
            return result.OrderBy(x => x.Count).ToList();
        }
        public async Task<PartialViewResult> SurveyQuestionMappings(int? SurveryId, int? SectionId, int? SurveryTypeId)
        {
            List<SurveyQuestionMappingViewModel> SurveyQuestions = new List<SurveyQuestionMappingViewModel>();
            List<SurveyQuestionSequenceMapping> SurveyQuestionSequenceMapping = new List<SurveyQuestionSequenceMapping>();
            string surveymappings = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //////////////////////////////////////////////////////////////////////////////////////////
                /////Get List of Questions According to Survey Id///////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////
                string apiParameters = "surveyId=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyTypeId=" + (SurveryTypeId ?? 0);

                HttpResponseMessage GetSurveyQuestionSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSurveyId + "?" + apiParameters);
                if (GetSurveyQuestionSurveyId.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = GetSurveyQuestionSurveyId.Content.ReadAsStringAsync().Result;
                    SurveyQuestions = JsonConvert.DeserializeObject<List<SurveyQuestionMappingViewModel>>(GetSurveyQuestionSurveyIdModel);
                }
                apiParameters = "";
                apiParameters = "Surveyid=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyType=" + (SurveryTypeId ?? 0);

                HttpResponseMessage GetSurveyQuestionMappingSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSequenceMappingBySurvey + "?" + apiParameters);
                if (GetSurveyQuestionMappingSurveyId.IsSuccessStatusCode)
                {
                    var results = GetSurveyQuestionMappingSurveyId.Content.ReadAsStringAsync().Result;
                    surveymappings = results;
                    SurveyQuestionSequenceMapping = JsonConvert.DeserializeObject<List<SurveyQuestionSequenceMapping>>(results);
                }
            }
            //foreach (var question in SurveyQuestions.Where(x => x.SurveyQuestionType.QuestionType == "Checkbox").ToList())
            //{
            //    question.surveyAnswers = question.surveyAnswers.Where(x => x.IsDeleted == false).ToList();
            //    question.surveyAnswersCombo = GetAllCombos(question.surveyAnswers);

            //}
            ViewBag.ChildQuestions = SurveyQuestions.Select(p => new SelectListItem { Value = p.Id.ToString(), Text =("Q: "+p.OrderBy.ToString()+" "+ p.QuestionText) });
            ViewBag.FirstOrDefaultQuestion = SurveyQuestionSequenceMapping.Where(p => p.IsFirstOrLast == 1).Select(p=>p.ParentQuestionID).FirstOrDefault();
            SurveyQuestionMappingAdminViewmodel surveyQuestionViewmodel = new SurveyQuestionMappingAdminViewmodel();
            surveyQuestionViewmodel.surveyQuestions = SurveyQuestions;
            surveyQuestionViewmodel.surveyQuestionSequenceMappings = SurveyQuestionSequenceMapping;
            return PartialView(surveyQuestionViewmodel);
        }
        public async Task<PartialViewResult> SurveyQuestions(int? SurveryId, int? SectionId, int? SurveryTypeId, int PatientId)
        {
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();
            List<SurveyQuestionSequenceMapping> SurveyQuestionSequenceMapping = new List<SurveyQuestionSequenceMapping>();
            string surveymappings = "";
            string apiParameters = "";            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ///////////////GET/api/GetSurveyQuestionSurveyId//////////////////////////////////////////
                apiParameters = "surveyId=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyTypeId=" + (SurveryTypeId ?? 0);
                HttpResponseMessage GetSurveyQuestionSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSurveyId + "?" + apiParameters);
                if (GetSurveyQuestionSurveyId.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = GetSurveyQuestionSurveyId.Content.ReadAsStringAsync().Result;
                    SurveyQuestions = JsonConvert.DeserializeObject<List<SurveyQuestion>>(GetSurveyQuestionSurveyIdModel);
                    if (SurveyQuestions.Count > 0)
                        ViewBag.TotalQ = SurveyQuestions.Count;
                }
                apiParameters = "";
                apiParameters = "Surveyid=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyType=" + (SurveryTypeId ?? 0);
                ////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////GET/api/GetSurveyQuestionSequenceMappingBySurveyId//////////////////
                HttpResponseMessage GetSurveyQuestionMappingSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSequenceMappingBySurvey + "?" + apiParameters);
                if (GetSurveyQuestionMappingSurveyId.IsSuccessStatusCode)
                {
                    var results = GetSurveyQuestionMappingSurveyId.Content.ReadAsStringAsync().Result;
                    surveymappings = results;
                    SurveyQuestionSequenceMapping = JsonConvert.DeserializeObject<List<SurveyQuestionSequenceMapping>>(results);
                }
            }
            SurveyQuestionViewmodel surveyQuestionViewmodel = new SurveyQuestionViewmodel();
            surveyQuestionViewmodel.surveyQuestions = SurveyQuestions;
            surveyQuestionViewmodel.surveyQuestionSequenceMappings = SurveyQuestionSequenceMapping;

            if (SurveyQuestionSequenceMapping.Count > 0)
            {
                var ParentQ = SurveyQuestionSequenceMapping.Where(p => p.ChildQuestionID == 0).Select(x => x.ParentQuestionID).ToList();
                ViewBag.LastQuestionID = ParentQ[0];
            }
            return PartialView(surveyQuestionViewmodel);


            ///Check If Patient Has Already Take Survey
            //HttpResponseMessage patientSurvey = await client.GetAsync(HelperExtensions.GetSurveyByPatientId + "?" + apiParameters+ "&patientId="+PatientId);


            /////////////////////////Check For Patient Take Survey////////////////////////////////////
            ///////////////////////GET/api/api/GetPatientSurveyByPatientId//////////////////
            //apiParameters = "patientId=" + PatientId + "&surveyId=" + (SurveryId ?? 0) + "&surveySectionId=" + (SectionId ?? 0) + "&surveyTypeId=" + (SurveryTypeId ?? 0);

            //HttpResponseMessage patientSurvey = await client.GetAsync(HelperExtensions.GetSurveyByPatientId + "?" + apiParameters);
            //apiParameters = "";
            //if (patientSurvey.IsSuccessStatusCode)
            //{
            //    var GetPatientSurvey = patientSurvey.Content.ReadAsStringAsync().Result;
            //    if (GetPatientSurvey.Length > 0)
            //        return PartialView("_AlreadySurvey");
            //}

            ////////////////////////////////////////////////////////////////////////////////////////////

        }

        public async Task<ActionResult> SurveyQuestionMappingsStart()
        {
            // ViewBag.PatientId = Id;
            List<SurveyType> SurveyType = new List<SurveyType>();
            List<SurveySection> Section = new List<SurveySection>();
            List<Survey> Survey = new List<Survey>();
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                }

                HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySections);
                if (respons1.IsSuccessStatusCode)
                {
                    var SurveySection = respons1.Content.ReadAsStringAsync().Result;
                    Section = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
                }

                HttpResponseMessage respons2 = await client.GetAsync(HelperExtensions.apiSurvey);
                if (respons2.IsSuccessStatusCode)
                {
                    var SurveyModel = respons2.Content.ReadAsStringAsync().Result;
                    Survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyModel);
                }


            }
            var _SurveyTypes = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName });
            var _SurveySections = Section.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName });
            var _Surveys = Survey.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SurveyName }).OrderBy(p=>p.Text);

            ViewBag.SurveyType = _SurveyTypes;
            ViewBag.SurveySections = _SurveySections;
            ViewBag.surveys = _Surveys;

            return View();
        }

        public async Task<ActionResult> SurveyStart(int Id)
        {
            ViewBag.PatientId = Id;
            var patient = _db.Patients.AsNoTracking().Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.PatientName =  patient.FirstName + " " + patient.LastName;
            ViewBag.DOB =  patient.BirthDate.ToString("d");
            ViewBag.Diagnosis = string.Join("\n", _db.Icd10Codes.AsNoTracking().Where(x => x.PatientId == Id).Select(x=>x.Code10).ToList());
            List<SurveyType> SurveyType = new List<SurveyType>();
            List<SurveySection> Section = new List<SurveySection>();
            List<Survey> Survey = new List<Survey>();
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();
            List<PatientSurvey> PatientSurveys = new List<PatientSurvey>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage GetSurveysbyPatientid = await client.GetAsync(HelperExtensions.GetSurveyByPatientIdList + "?patientId=" + Id);
                if (GetSurveysbyPatientid.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = GetSurveysbyPatientid.Content.ReadAsStringAsync().Result;
                    PatientSurveys = JsonConvert.DeserializeObject<List<PatientSurvey>>(GetSurveyQuestionSurveyIdModel);
                    PatientSurveys = PatientSurveys.OrderByDescending(x => x.CreatedOn).ToList();
                }
                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                }

                HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySections);
                if (respons1.IsSuccessStatusCode)
                {
                    var SurveySection = respons1.Content.ReadAsStringAsync().Result;
                    Section = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
                }

                HttpResponseMessage respons2 = await client.GetAsync(HelperExtensions.apiSurvey);
                if (respons2.IsSuccessStatusCode)
                {
                    var SurveyModel = respons2.Content.ReadAsStringAsync().Result;
                    Survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyModel);
                }
            }
            var _SurveyTypes = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName });
            var _SurveySections = Section.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName });
            var _Surveys = Survey.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SurveyName });

            ViewBag.SurveyType = _SurveyTypes;
            ViewBag.SurveySections = _SurveySections;
            ViewBag.surveys = _Surveys;

            return View(PatientSurveys);
        }
        public async Task<PartialViewResult> SurveyQuestionsFlowDiagram(int SurveryId, int SurveyTypeId, int SurveySectionId, int ChildQuestionID, int IsFirstOrLast)
        {
            List<SurveyFlowNextNode> lstsurveyFlowNextNode = new List<SurveyFlowNextNode>();
            SurveyFlowNextNode surveyFlowNextNode = new SurveyFlowNextNode();
            surveyFlowNextNode.SurveyId = SurveryId;
            surveyFlowNextNode.SurveyTypeId = SurveyTypeId;
            surveyFlowNextNode.SurveySectionId = SurveySectionId;
            surveyFlowNextNode.ChildQuestionID = ChildQuestionID;
            surveyFlowNextNode.IsFirstOrLast = IsFirstOrLast;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var _obj = new StringContent(JsonConvert.SerializeObject(surveyFlowNextNode), Encoding.UTF8, "application/json");
                HttpResponseMessage PostFlowDiagrom = await client.PostAsync(HelperExtensions.apiSurveyNextNodeByFlow, _obj);
                if (PostFlowDiagrom.IsSuccessStatusCode)
                {
                    var PostSurveyFlowNextNode = PostFlowDiagrom.Content.ReadAsStringAsync().Result;
                    lstsurveyFlowNextNode = JsonConvert.DeserializeObject<List<SurveyFlowNextNode>>(PostSurveyFlowNextNode);
                }
                return PartialView(lstsurveyFlowNextNode);
            }
        }

        public async Task<JsonResult> FlowNextNode(int SurveryId, int ChildQuestionID, int IsFirstOrLast)
        {
            List<SurveyFlowNextNode> lstsurveyFlowNextNode = new List<SurveyFlowNextNode>();
            SurveyFlowNextNode surveyFlowNextNode = new SurveyFlowNextNode();
            surveyFlowNextNode.SurveyId = SurveryId;
            surveyFlowNextNode.ChildQuestionID = ChildQuestionID;
            surveyFlowNextNode.IsFirstOrLast = IsFirstOrLast;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var _obj = new StringContent(JsonConvert.SerializeObject(surveyFlowNextNode), Encoding.UTF8, "application/json");
                HttpResponseMessage PostFlowDiagrom = await client.PostAsync(HelperExtensions.apiSurveyNextNodeByFlow, _obj);
                if (PostFlowDiagrom.IsSuccessStatusCode)
                {
                    var PostSurveyFlowNextNode = PostFlowDiagrom.Content.ReadAsStringAsync().Result;
                    lstsurveyFlowNextNode = JsonConvert.DeserializeObject<List<SurveyFlowNextNode>>(PostSurveyFlowNextNode);
                }

                return Json(lstsurveyFlowNextNode);
            }
        }

        public async Task<JsonResult> FlowAllNextNode(int? SurveryId, int? SectionId, int? SurveryTypeId)
        {
            List<SurveyQuestionMappingViewModel> SurveyQuestions = new List<SurveyQuestionMappingViewModel>();
            List<SurveyQuestionSequenceMapping> SurveyQuestionSequenceMapping = new List<SurveyQuestionSequenceMapping>();
            string surveymappings = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //////////////////////////////////////////////////////////////////////////////////////////
                /////Get List of Questions According to Survey Id///////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////
                string apiParameters = "surveyId=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyTypeId=" + (SurveryTypeId ?? 0);

                HttpResponseMessage GetSurveyQuestionSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSurveyId + "?" + apiParameters);
                if (GetSurveyQuestionSurveyId.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = GetSurveyQuestionSurveyId.Content.ReadAsStringAsync().Result;
                    SurveyQuestions = JsonConvert.DeserializeObject<List<SurveyQuestionMappingViewModel>>(GetSurveyQuestionSurveyIdModel);
                }
                apiParameters = "";
                apiParameters = "Surveyid=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyType=" + (SurveryTypeId ?? 0);

                HttpResponseMessage GetSurveyQuestionMappingSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSequenceMappingBySurvey + "?" + apiParameters);
                if (GetSurveyQuestionMappingSurveyId.IsSuccessStatusCode)
                {
                    var results = GetSurveyQuestionMappingSurveyId.Content.ReadAsStringAsync().Result;
                    surveymappings = results;
                    SurveyQuestionSequenceMapping = JsonConvert.DeserializeObject<List<SurveyQuestionSequenceMapping>>(results);
                }
            }
            foreach (var question in SurveyQuestions.Where(x => x.SurveyQuestionType.QuestionType == "Checkbox").ToList())
            {
                question.surveyAnswers = question.surveyAnswers.Where(x => x.IsDeleted == false).ToList();
                question.surveyAnswersCombo = GetAllCombos(question.surveyAnswers);

            }
            //ViewBag.ChildQuestions = SurveyQuestions.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = ("Q: " + p.OrderBy.ToString() + " " + p.QuestionText) });
            //ViewBag.FirstOrDefaultQuestion = SurveyQuestionSequenceMapping.Where(p => p.IsFirstOrLast == 1).Select(p => p.ParentQuestionID).FirstOrDefault();
            SurveyQuestionMappingAdminViewmodel surveyQuestionViewmodel = new SurveyQuestionMappingAdminViewmodel();
            surveyQuestionViewmodel.surveyQuestions = SurveyQuestions;
            surveyQuestionViewmodel.surveyQuestionSequenceMappings = SurveyQuestionSequenceMapping;
            return Json(surveyQuestionViewmodel);
        }

        //public async Task<PartialViewResult> SurveyQuestionsFlowDiagram(int? SurveryId, int? SectionId, int? SurveryTypeId)
        //    {
        //    //var result = Json(null);
        //    SurveyFlowView surveyFlowView = new SurveyFlowView();
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(HelperExtensions.apiurl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //Get Survey Flow Tree View Data
        //        string apiParameters = "surveyId=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyTypeId=" + (SurveryTypeId ?? 0);

        //        HttpResponseMessage GetFlowDiagrom = await client.GetAsync(HelperExtensions.GetSurveyQuestionFlowBySurveyId + "?" + apiParameters);
        //        if (GetFlowDiagrom.IsSuccessStatusCode)
        //        {
        //            var GetSurveyQuestionFlowModel = GetFlowDiagrom.Content.ReadAsStringAsync().Result;
        //            surveyFlowView = JsonConvert.DeserializeObject<SurveyFlowView>(GetSurveyQuestionFlowModel);

        //            //result = Json(surveyFlowView);
        //            //result.RecursionLimit = 5120;//1024;
        //            //result.MaxJsonLength = 41943040;// 8388608;
        //            //ViewBag._myjsonFlowData = result;
        //        }
        //        return PartialView(surveyFlowView);
        //    }
        //}
        public async Task<PartialViewResult> SurveyQuestionsFlowDiagram1(int? SurveryId, int? SectionId, int? SurveryTypeId)
        {
            //var result = Json(null);
            SurveyFlowView surveyFlowView = new SurveyFlowView();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Get Survey Flow Tree View Data
                string apiParameters = "surveyId=" + (SurveryId ?? 0) + "&SurveySectionId=" + (SectionId ?? 0) + "&SurveyTypeId=" + (SurveryTypeId ?? 0);

                HttpResponseMessage GetFlowDiagrom = await client.GetAsync(HelperExtensions.GetSurveyQuestionFlowBySurveyId + "?" + apiParameters);
                if (GetFlowDiagrom.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionFlowModel = GetFlowDiagrom.Content.ReadAsStringAsync().Result;
                    surveyFlowView = JsonConvert.DeserializeObject<SurveyFlowView>(GetSurveyQuestionFlowModel);

                    //result = Json(surveyFlowView);
                    //result.RecursionLimit = 5120;//1024;
                    //result.MaxJsonLength = 41943040;// 8388608;
                    //ViewBag._myjsonFlowData = result;
                }
                return PartialView(surveyFlowView);
            }
        }
        public async Task<PartialViewResult> _SurveyStart(int Id)
        {
            ViewBag.PatientId = Id;
             var patient = _db.Patients.AsNoTracking().Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            ViewBag.DOB = patient.BirthDate.ToString("d");
            ViewBag.Diagnosis = string.Join("\n", _db.Icd10Codes.AsNoTracking().Where(x => x.PatientId == Id).Select(x => x.Code10).ToList());
            List<SurveyType> SurveyType = new List<SurveyType>();
            List<SurveySection> Section = new List<SurveySection>();
            List<Survey> Survey = new List<Survey>();
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();
            List<PatientSurvey> PatientSurveys = new List<PatientSurvey>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {

                    HttpResponseMessage GetSurveysbyPatientid = await client.GetAsync(HelperExtensions.GetSurveyByPatientIdList + "?patientId=" + Id);
                    if (GetSurveysbyPatientid.IsSuccessStatusCode)
                    {
                        var GetSurveyQuestionSurveyIdModel = GetSurveysbyPatientid.Content.ReadAsStringAsync().Result;
                        PatientSurveys = JsonConvert.DeserializeObject<List<PatientSurvey>>(GetSurveyQuestionSurveyIdModel);
                        PatientSurveys = PatientSurveys.OrderByDescending(x => x.CreatedOn).ToList()
                            ;
                    }
                    HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                    if (respons.IsSuccessStatusCode)
                    {
                        var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                        SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                    }

                    HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySections);
                    if (respons1.IsSuccessStatusCode)
                    {
                        var SurveySection = respons1.Content.ReadAsStringAsync().Result;
                        Section = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
                    }

                    HttpResponseMessage respons2 = await client.GetAsync(HelperExtensions.apiSurvey);
                    if (respons2.IsSuccessStatusCode)
                    {
                        var SurveyModel = respons2.Content.ReadAsStringAsync().Result;
                        Survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyModel);
                    }


                    var _SurveyTypes = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName });
                    var _SurveySections = Section.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName });
                    var _Surveys = Survey.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SurveyName });

                    ViewBag.SurveyType = _SurveyTypes;
                    ViewBag.SurveySections = _SurveySections;
                    ViewBag.surveys = _Surveys;

                    return PartialView(PatientSurveys);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return PartialView();

            }
        }

        public async Task<JsonResult> GetjsonSequnceMapping(int SurveyId, int SurveySectionId,int SurveyTypeId)
        {
            List<SurveyQuestionSequenceMapping> SurveyQuestionSequenceMapping = new List<SurveyQuestionSequenceMapping>();
            string apiParameters = "";
            using (var _client = new HttpClient())
            {
                _client.BaseAddress = new Uri(HelperExtensions.apiurl);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                apiParameters = "Surveyid=" + SurveyId + "&SurveySectionId=" + SurveySectionId + "&SurveyType=" + SurveyTypeId;
                ///////////////////////////////////////////////////////////////////////////
                ///////////////////////GET/api/GetSurveyQuestionSequenceMappingBySurveyId//////////////////
                HttpResponseMessage GetSurveyQuestionMappingSurveyId = await _client.GetAsync(HelperExtensions.apiGetSurveyQuestionSequenceMappingBySurvey + "?" + apiParameters);
                if (GetSurveyQuestionMappingSurveyId.IsSuccessStatusCode)
                {
                    var results = GetSurveyQuestionMappingSurveyId.Content.ReadAsStringAsync().Result;

                    SurveyQuestionSequenceMapping = JsonConvert.DeserializeObject<List<SurveyQuestionSequenceMapping>>(results);
                    return Json(SurveyQuestionSequenceMapping, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("error");
        }

        [HttpGet]
        public async Task<PartialViewResult> _SurveyQuestionsEdit(int Id, int PatientId,int SurveyId,int SurveyTypeId,int SurveySectionId)
        {
            try
            {

           
            ViewBag.PatientSurveyID = Id;
            ViewBag.SurveyID = SurveyId;
            ViewBag.PatientSurveyTypeID = SurveyTypeId;
            ViewBag.PatientSurveySectionID = SurveySectionId;

            PatientSurveyQAReviewModel patientSurveyQAReviewModel = new PatientSurveyQAReviewModel();
            SurveyQuestionViewmodel surveyQuestionViewmodel = new SurveyQuestionViewmodel();
            PatientSurveryViewModel patientSurveryViewModel = new PatientSurveryViewModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Get List of Questions According to Survey Id
                string apiParameters = "";

                apiParameters = "Id=" + Id;
                HttpResponseMessage GetSurveyByPatientId = await client.GetAsync(HelperExtensions.PatientSurvey + "?" + apiParameters);
                
                if (GetSurveyByPatientId.IsSuccessStatusCode)
                {
                    var results = GetSurveyByPatientId.Content.ReadAsStringAsync().Result;
                    patientSurveyQAReviewModel.patientSurveryViewModels = JsonConvert.DeserializeObject<PatientSurveryViewModel>(results);

                    
                }

                /////////////////////////Start Questions & Mapping//////////////////////////////////////////////////////////
                List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();
                List<SurveyQuestionSequenceMapping> SurveyQuestionSequenceMapping = new List<SurveyQuestionSequenceMapping>();
                string surveymappings = "";
                apiParameters = "";
                using (var _client = new HttpClient())
                {
                    _client.BaseAddress = new Uri(HelperExtensions.apiurl);
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    ///////////////GET/api/GetSurveyQuestionSurveyId//////////////////////////////////////////
                    apiParameters = "surveyId=" + SurveyId + "&SurveySectionId=" + SurveySectionId + "&SurveyTypeId=" + SurveyTypeId;
                    HttpResponseMessage GetSurveyQuestionSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSurveyId + "?" + apiParameters);
                    if (GetSurveyQuestionSurveyId.IsSuccessStatusCode)
                    {
                        var GetSurveyQuestionSurveyIdModel = GetSurveyQuestionSurveyId.Content.ReadAsStringAsync().Result;
                        SurveyQuestions = JsonConvert.DeserializeObject<List<SurveyQuestion>>(GetSurveyQuestionSurveyIdModel);
                        if (SurveyQuestions.Count > 0)
                            ViewBag.TotalQ = SurveyQuestions.Count;
                    }
                    apiParameters = "";
                    apiParameters = "Surveyid=" + SurveyId + "&SurveySectionId=" + SurveySectionId + "&SurveyType=" + SurveyTypeId;
                   ///////////////////////////////////////////////////////////////////////////
                    ///////////////////////GET/api/GetSurveyQuestionSequenceMappingBySurveyId//////////////////
                    HttpResponseMessage GetSurveyQuestionMappingSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSequenceMappingBySurvey + "?" + apiParameters);
                    if (GetSurveyQuestionMappingSurveyId.IsSuccessStatusCode)
                    {
                        var results = GetSurveyQuestionMappingSurveyId.Content.ReadAsStringAsync().Result;
                        surveymappings = results;
                        SurveyQuestionSequenceMapping = JsonConvert.DeserializeObject<List<SurveyQuestionSequenceMapping>>(results);
                    }
                }


                if (SurveyQuestionSequenceMapping.Count > 0)
                {
                    var ParentQ = SurveyQuestionSequenceMapping.Where(p => p.ChildQuestionID == 0).Select(x => x.ParentQuestionID).ToList();
                    ViewBag.LastQuestionID = ParentQ[0];
                }

                surveyQuestionViewmodel.surveyQuestions = SurveyQuestions;
                surveyQuestionViewmodel.surveyQuestionSequenceMappings = SurveyQuestionSequenceMapping;

                patientSurveyQAReviewModel.surveyQuestionViewmodel = surveyQuestionViewmodel;
                ///////End Questions & Mapping//////////////////////////////////////////////////////////
                // var patient = _db.Patients.Where(x => x.Id == PatientId).FirstOrDefault();
                ////patientSurvey.PatientName = patient.FirstName + " " + patient.LastName;
                ////patientSurvey.DOB = patient.BirthDate.ToString("d");
                //return PartialView("SurveyResult", patientSurvey);

            }
            
            return PartialView("_SurveyQuestionsEdit", patientSurveyQAReviewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message ;
                return PartialView("_SurveyQuestionsEdit", new PatientSurveryQAViewModel());

            }
        }

        public async Task<ActionResult> PatientSurveys(int Id)
        {
            ViewBag.PatientId = Id;
            var patient = _db.Patients.AsNoTracking().Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            ViewBag.DOB = patient.BirthDate.ToString("d");
            ViewBag.Diagnosis = string.Join("\n", _db.Icd10Codes.AsNoTracking().Where(x => x.PatientId == Id).Select(x => x.Code10).ToList());
            List<SurveyType> SurveyType = new List<SurveyType>();
            List<SurveySection> Section = new List<SurveySection>();
            List<Survey> Survey = new List<Survey>();
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();
            List<PatientSurvey> PatientSurveys = new List<PatientSurvey>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage GetSurveysbyPatientid = await client.GetAsync(HelperExtensions.GetSurveyByPatientIdList + "?patientId=" + Id);
                if (GetSurveysbyPatientid.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = GetSurveysbyPatientid.Content.ReadAsStringAsync().Result;
                    PatientSurveys = JsonConvert.DeserializeObject<List<PatientSurvey>>(GetSurveyQuestionSurveyIdModel);
                    PatientSurveys = PatientSurveys.OrderByDescending(x => x.CreatedOn).ToList();
                }
                //HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                //if (respons.IsSuccessStatusCode)
                //{
                //    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                //    SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                //}

                //HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySections);
                //if (respons1.IsSuccessStatusCode)
                //{
                //    var SurveySection = respons1.Content.ReadAsStringAsync().Result;
                //    Section = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
                //}

                //HttpResponseMessage respons2 = await client.GetAsync(HelperExtensions.apiSurvey);
                //if (respons2.IsSuccessStatusCode)
                //{
                //    var SurveyModel = respons2.Content.ReadAsStringAsync().Result;
                //    Survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyModel);
                //}
            }
            //var _SurveyTypes = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName });
            //var _SurveySections = Section.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName });
            //var _Surveys = Survey.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SurveyName });

            //ViewBag.SurveyType = _SurveyTypes;
            //ViewBag.SurveySections = _SurveySections;
            //ViewBag.surveys = _Surveys;

            return View(PatientSurveys);
        }
        public async Task<ActionResult> surveyAdmin()
        {
            List<SurveyType> SurveyType = new List<SurveyType>();
            List<SurveySection> Section = new List<SurveySection>();
            List<Survey> Survey = new List<Survey>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                }

                HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySections);
                if (respons1.IsSuccessStatusCode)
                {
                    var SurveySection = respons1.Content.ReadAsStringAsync().Result;
                    Section = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
                }

                HttpResponseMessage respons2 = await client.GetAsync(HelperExtensions.apiSurvey);
                if (respons2.IsSuccessStatusCode)
                {
                    var SurveyModel = respons2.Content.ReadAsStringAsync().Result;
                    Survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyModel);
                }
            }
            var result = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName });
            var surveySectionResult = Section.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName });
            var surveys = Survey.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SurveyName });

            ViewBag.SurveyType = result;
            ViewBag.SurveySections = surveySectionResult;
            ViewBag.surveys = surveys;
            return View();
            //return Json(new { ServeyType=result,ServeySection=surveySectionResult },JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PatientsSurveyList()
        {
            //int Id = 16608;
            //ViewBag.PatientId = Id;
            //var patient = _db.Patients.AsNoTracking().Where(x => x.Id == Id).FirstOrDefault();
            //ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
            //ViewBag.DOB = patient.BirthDate.ToString("d");
            //ViewBag.Diagnosis = string.Join("\n", _db.Icd10Codes.AsNoTracking().Where(x => x.PatientId == Id).Select(x => x.Code10).ToList());
            List<SurveyType> SurveyType = new List<SurveyType>();
            List<SurveySection> Section = new List<SurveySection>();
            List<Survey> Survey = new List<Survey>();
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();
            List<PatientSurvey> PatientSurveys = new List<PatientSurvey>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage GetSurveysbyPatientid = await client.GetAsync(HelperExtensions.GetAllPatientsSurveysList);
                if (GetSurveysbyPatientid.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = GetSurveysbyPatientid.Content.ReadAsStringAsync().Result;
                    PatientSurveys = JsonConvert.DeserializeObject<List<PatientSurvey>>(GetSurveyQuestionSurveyIdModel);
                    PatientSurveys = PatientSurveys.OrderByDescending(x => x.CreatedOn).ToList();
                }

                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                }

                HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySections);
                if (respons1.IsSuccessStatusCode)
                {
                    var SurveySection = respons1.Content.ReadAsStringAsync().Result;
                    Section = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
                }

                HttpResponseMessage respons2 = await client.GetAsync(HelperExtensions.apiSurvey);
                if (respons2.IsSuccessStatusCode)
                {
                    var SurveyModel = respons2.Content.ReadAsStringAsync().Result;
                    Survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyModel);
                }
            }
            var _SurveyTypes = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName });
            var _SurveySections = Section.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName });
            var _Surveys = Survey.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SurveyName });

            ViewBag.SurveyType = _SurveyTypes;
            ViewBag.SurveySections = _SurveySections;
            ViewBag.surveys = _Surveys;
            var patientids = PatientSurveys.Select(x => x.PatientId).Distinct().ToList();

            var patients = _db.Patients.Where(x => patientids.Contains(x.Id)).Select(x => new { Id = x.Id, Name = x.FirstName + " " + x.LastName }).ToList();

            foreach (var item in PatientSurveys)
            {

                item.PatientName = patients.Where(x => x.Id == item.PatientId).FirstOrDefault()?.Name;
            }
            PatientSurveys = PatientSurveys.Where(x => !string.IsNullOrEmpty(x.PatientName)).ToList();
            return View(PatientSurveys);
        }
        public async Task<PartialViewResult> SurveyQAs(int surveyId, int? SecId, int? TypeId)
        {
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Get List of Questions According to Survey Id
                string apiParameters = "";
                if (surveyId > 0)
                {
                    apiParameters = "surveyId=" + surveyId + "&SurveySectionId=" + (SecId ?? 0) + "&SurveyTypeId=" + (TypeId ?? 0);
                    HttpResponseMessage GetSurveyByPatientId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSurveyId + "?" + apiParameters);
                    if (GetSurveyByPatientId.IsSuccessStatusCode)
                    {
                        var results = GetSurveyByPatientId.Content.ReadAsStringAsync().Result;
                        SurveyQuestions = JsonConvert.DeserializeObject<List<SurveyQuestion>>(results);
                    }
                }

            }
            return PartialView(SurveyQuestions);
        }
        public async Task<ActionResult> surveyQuestionsText(int id)
        {
            List<SurveyQuestion> surveyQuestions = new List<SurveyQuestion>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyQuestions);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    surveyQuestions = JsonConvert.DeserializeObject<List<SurveyQuestion>>(SurveyResponse);
                }

            }
            var surveyQuestion = surveyQuestions.Where(p => p.SurveyId == id).Select(p => p.QuestionText);
            return Json(surveyQuestion, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> surveyText()
        {
            List<Survey> survey = new List<Survey>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurvey);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyResponse);
                }

            }
            var survey1 = survey.Select(p => p.SurveyName);
            return Json(survey1, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> surveyTypeText()
        {
            List<SurveyType> surveyType = new List<SurveyType>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    surveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                }

            }
            var type = surveyType.Select(p => p.TypeName);
            return Json(type, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> surveySectionText()
        {
            List<SurveySection> surveySection = new List<SurveySection>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveySections);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    surveySection = JsonConvert.DeserializeObject<List<SurveySection>>(SurveyResponse);
                }

            }
            var section = surveySection.Select(p => p.SectionName);
            return Json(section, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> jsonddlbinding()
        {
            List<SurveyType> SurveyType = new List<SurveyType>();
            List<SurveySection> Section = new List<SurveySection>();
            List<Survey> Survey = new List<Survey>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyType);
                if (respons.IsSuccessStatusCode)
                {
                    var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
                    SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
                }

                HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySections);
                if (respons1.IsSuccessStatusCode)
                {
                    var SurveySection = respons1.Content.ReadAsStringAsync().Result;
                    Section = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
                }

                HttpResponseMessage respons2 = await client.GetAsync(HelperExtensions.apiSurvey);
                if (respons2.IsSuccessStatusCode)
                {
                    var SurveyModel = respons2.Content.ReadAsStringAsync().Result;
                    Survey = JsonConvert.DeserializeObject<List<Survey>>(SurveyModel);
                }
            }
            var surveyType = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName });
            var surveySectionResult = Section.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName }).OrderBy(p => p.Text);
            var surveys = Survey.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SurveyName }).OrderBy(p => p.Text);

            ViewBag.SurveyType = surveyType;
            ViewBag.SurveySections = surveySectionResult;
            ViewBag.surveys = surveys;
            //return View();
            return Json(new { ServeyType = surveyType, ServeySection = surveySectionResult, Surveys = surveys }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> addSurveyType(SurveyType surveyType)
        {
            using (var client = new HttpClient())
            {
                surveyType.CreatedOn = DateTime.Now;
                surveyType.CreatedBy = User.Identity.GetUserId();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(surveyType, Formatting.Indented);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //var stringContent = new StringContent(json);
                var respons = await client.PostAsync(HelperExtensions.apiSurveyType, content);



                if (respons.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return Json("error");
        }
        [HttpPost]
        public async Task<ActionResult> addSurvey(Survey survey)
        {
            using (var client = new HttpClient())
            {
                survey.CreatedOn = DateTime.Now;
                survey.CreatedBy = User.Identity.GetUserId();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(survey, Formatting.Indented);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //var stringContent = new StringContent(json);
                var respons = await client.PostAsync(HelperExtensions.apiSurvey, content);



                if (respons.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return Json("error");
        }

        [HttpPut]
        public async Task<ActionResult> editSurveyQuestion(int id, SurveyQuestion surveyQuestion, string[] ClinicalNotes)
        {
            for (int i = 0; i <= surveyQuestion.surveyAnswers.Count - 1; i++)
            {
                surveyQuestion.surveyAnswers[i].ClinicalNotes = ClinicalNotes[i];
                surveyQuestion.surveyAnswers[i].UpdatedOn = DateTime.Now;
                surveyQuestion.surveyAnswers[i].UpdatedBy = User.Identity.GetUserId();
            }
            using (var client = new HttpClient())
            {

                surveyQuestion.Id = id;
                surveyQuestion.UpdatedOn = DateTime.Now;
                surveyQuestion.UpdatedBy = User.Identity.GetUserId();

                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(surveyQuestion);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var json1 = JsonConvert.SerializeObject(id, Formatting.Indented);

                var SurveyQuestionId = new StringContent(json1, Encoding.UTF8, "application/json");


                var sid = "id=" + id;
                var respons = await client.PutAsync(HelperExtensions.apiSurveyQuestions + "?" + sid, content);

              

                if (respons.IsSuccessStatusCode)
                {
                    var contents = respons.Content.ReadAsStringAsync();
                    if (contents.Result != "")
                    {
                        return Json(contents.Result);
                    }
                    else
                    {
                        return Json("success");
                    }
                    
                }
            }

            return Json("error");
        }

        [HttpPut]
        public async Task<ActionResult> deleteSurveyQuestion(int id)
        {
            
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json1 = JsonConvert.SerializeObject(id, Formatting.Indented);

                var SurveyQuestionId = new StringContent(json1, Encoding.UTF8, "application/json");


                var sqid = "id=" + id;
                var respons = await client.DeleteAsync(HelperExtensions.apiSurveyQuestions + "?" + sqid);



                if (respons.IsSuccessStatusCode)
                {
                    var contents = respons.Content.ReadAsStringAsync();
                    if (contents.Result != "")
                    {
                        return Json(contents.Result);
                    }
                    else
                    {
                        return Json("success");
                    }

                }
            }

            return Json("error");
        }

        [HttpPut]
        public async Task<ActionResult> editSurvey(int id, Survey survey)
        {
            using (var client = new HttpClient())
            {
                
                survey.Id = id;
                survey.UpdatedOn = DateTime.Now;
                survey.UpdatedBy = User.Identity.GetUserId();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(survey);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var json1 = JsonConvert.SerializeObject(id, Formatting.Indented);

                var surveyid = new StringContent(json1, Encoding.UTF8, "application/json");


                var sid = "id=" + id;
                var respons = await client.PutAsync(HelperExtensions.apiSurvey+"?"+sid, content);



                if (respons.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return Json("error");
        }
        [HttpPut]
        public async Task<ActionResult> editSurveyType(int id, SurveyType surveyType)
        {
            using (var client = new HttpClient())
            {
                surveyType.Id = id;
                surveyType.UpdatedOn = DateTime.Now;
                surveyType.UpdatedBy = User.Identity.GetUserId();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(surveyType);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var json1 = JsonConvert.SerializeObject(id, Formatting.Indented);

                var surveyid = new StringContent(json1, Encoding.UTF8, "application/json");

                var sid = "id=" + id;
                var respons = await client.PutAsync(HelperExtensions.apiSurveyType+"?"+sid, content);



                if (respons.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return Json("error");
        }
        [HttpPut]
        public async Task<ActionResult> editSurveySection(int id, SurveySection surveySection)
        {
            using (var client = new HttpClient())
            {
                surveySection.Id = id;
                surveySection.UpdatedOn = DateTime.Now;
                surveySection.UpdatedBy = User.Identity.GetUserId();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(surveySection);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var json1 = JsonConvert.SerializeObject(id, Formatting.Indented);

                var surveyid = new StringContent(json1, Encoding.UTF8, "application/json");

                var sid = "id=" + id;
                var respons = await client.PutAsync(HelperExtensions.apiSurveySections+"?"+sid, content);



                if (respons.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return Json("error");
        }
        [HttpPost]
        public async Task<ActionResult> addSurveySection(SurveySection surveySection)
        {
            using (var client = new HttpClient())
            {
                surveySection.CreatedOn = DateTime.Now;
                surveySection.CreatedBy = User.Identity.GetUserId();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(surveySection, Formatting.Indented);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //var stringContent = new StringContent(json);
                var respons = await client.PostAsync(HelperExtensions.apiSurveySections, content);



                if (respons.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return Json("error");
        }
        [HttpPost]
        public async Task<ActionResult> addSurveyQuestion(SurveyQuestion surveyQuestion,string[] ClinicalNotes)
        {
            for(int i = 0; i <= surveyQuestion.surveyAnswers.Count - 1; i++)
            {
                surveyQuestion.surveyAnswers[i].CreatedOn = DateTime.Now;
                surveyQuestion.surveyAnswers[i].CreatedBy = User.Identity.GetUserId();
                surveyQuestion.surveyAnswers[i].ClinicalNotes = ClinicalNotes[i];
            }
            
            using (var client = new HttpClient())
            {
                surveyQuestion.CreatedOn = DateTime.Now;
                surveyQuestion.CreatedBy = User.Identity.GetUserId();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(surveyQuestion, Formatting.Indented);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //var stringContent = new StringContent(json);
                var respons = await client.PostAsync(HelperExtensions.apiSurveyQuestions, content);

                if (respons.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return Json("error");
        }

        public async Task<ActionResult> loadQuestions(int SurveryId, int SectionId, int SurveryTypeId)
        {
            var apiParameters = "";
            List<SurveyQuestion> SurveyQuestions = new List<SurveyQuestion>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                apiParameters = "surveyId=" + (SurveryId) + "&SurveySectionId=" + (SectionId) + "&SurveyTypeId=" + (SurveryTypeId);
                HttpResponseMessage GetSurveyQuestionSurveyId = await client.GetAsync(HelperExtensions.apiGetSurveyQuestionSurveyId + "?" + apiParameters);
                if (GetSurveyQuestionSurveyId.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = GetSurveyQuestionSurveyId.Content.ReadAsStringAsync().Result;
                    SurveyQuestions = JsonConvert.DeserializeObject<List<SurveyQuestion>>(GetSurveyQuestionSurveyIdModel);
                }
            }
            return Json("success");
        }

        [HttpPost]
        public async Task<bool> SubmitSurveyQuestionMappings(int Survey, string[] ParentQ, string[] Answers, string[] ChildQ,int[] FirstOrLast)
        {
            List<SurveyQuestionSequenceMapping> sequenceMappinglst = new List<SurveyQuestionSequenceMapping>();
            for (int i = 0; i < ParentQ.Length; i++)
            {
                sequenceMappinglst.Add(new SurveyQuestionSequenceMapping()
                {
                    CreatedOn = DateTime.Now,
                    CreatedBy = User.Identity.GetUserId(),
                    UpdatedOn = DateTime.Now,
                    UpdatedBy = User.Identity.GetUserId(),

                    ParentQuestionID = Convert.ToInt32(ParentQ[i]),
                    AnswerIds = Answers[i],
                    ChildQuestionID = Convert.ToInt32(ChildQ[i]),
                    IsFirstOrLast = Convert.ToInt32(FirstOrLast[i]),
                    SurveyId = Survey
                });
                //sequenceMapping.ParentQuestionID = Convert.ToInt32(ParentQ[i]);
                //sequenceMapping.AnswerIds = Answers[i];
                //sequenceMapping.ChildQuestionID = Convert.ToInt32(ChildQ[i]);
                //sequenceMapping.SurveyId = SurveyId;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(sequenceMappinglst), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(HelperExtensions.apisurveyQuestionSequenceMapping, content);
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        [HttpPost]
        public async Task<bool> DeleteSurveyQuestionMappings(int[] ids)
        {
            List<int> _IDs = new List<int>();
            var apiParameters = "";
            foreach (var id in ids)
            {
                _IDs.Add(id);
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                apiParameters = "DeletedBy=" + User.Identity.GetUserId();

                var content = new StringContent(JsonConvert.SerializeObject(_IDs), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(HelperExtensions.DeleteQuestionSequenceMapping + "?" + apiParameters, content);
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }


        [HttpPost]
        public async Task<bool> SurveySubmitAsync(int PatientSurveyID, int PatientID, int SurveyId, int? SectionID, int? SurveyTypeId, string[] Questions, string[] Answers, string[] AnswerTxt,int surveyCompletionPercent)
        {
            PatientSurvey pSurvey = new PatientSurvey();
            pSurvey.Id = PatientSurveyID;
            pSurvey.PatientId = PatientID;
            pSurvey.SurveyId = SurveyId;
            pSurvey.SurveySectionId = SectionID ?? 0;
            pSurvey.SurveyTypeId = SurveyTypeId ?? 0;
            
            pSurvey.IsCompleted = surveyCompletionPercent;
            if (PatientSurveyID != 0)
            {
                pSurvey.UpdatedBy = User.Identity.GetUserId();
                pSurvey.UpdatedOn = DateTime.Now;
            }
            else
            {
                pSurvey.CreatedBy = User.Identity.GetUserId();
                pSurvey.CreatedOn = DateTime.Now;
            }
            //pSurvey.UpdatedOn = DateTime.Now;

            // int[] nums = Questions.Split(',').Select(int.Parse).ToArray();
            pSurvey.patientSurveyQAs = new List<PatientSurveyQA>();
            for (int i = 0; i < Questions.Length; i++)
            {
                var q = Questions[i];
                var answers = Answers[i];
                pSurvey.patientSurveyQAs.Add(new PatientSurveyQA
                {
                    PatientSurveyId= PatientSurveyID,
                    SurveyAnswerId = answers,
                    SurveyQuestionId = Convert.ToInt32(q),
                    AnswerText = AnswerTxt[i]
                });
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HelperExtensions.apiurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(pSurvey), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(HelperExtensions.PatientSurvey, content);
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }


        [HttpPost]
        public async Task<PartialViewResult> AllSurveysList()
        {
            using (var client = new HttpClient())
            {
                List<SurveyDependent> lst_Sd = new List<SurveyDependent>();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var respons = await client.GetAsync(HelperExtensions.apiAllSurveysList);//.apiSurvey + "?" + sid, content

                if (respons.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = respons.Content.ReadAsStringAsync().Result;
                    lst_Sd = JsonConvert.DeserializeObject<List<SurveyDependent>>(GetSurveyQuestionSurveyIdModel);
                    return PartialView("SurveyDependent",lst_Sd);
                    //return Json("success");
                }
            }
            return PartialView("SurveyDependent", new List<SurveyDependent>());

        }

        [HttpPost]
        public async Task<ActionResult> GetSurveyTypeBySurveyId(int surveyId)
        {
            //var apiParameters = "";
            List<SurveyType> SurveyType = new List<SurveyType>();
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(HelperExtensions.apiurl);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    apiParameters = "SurveyId=" + (surveyId);
            //    HttpResponseMessage respons = await client.GetAsync(HelperExtensions.apiSurveyTypesBySurveyId + "?" + apiParameters);
            //    if (respons.IsSuccessStatusCode)
            //    {
            //        var SurveyResponse = respons.Content.ReadAsStringAsync().Result;
            //        SurveyType = JsonConvert.DeserializeObject<List<SurveyType>>(SurveyResponse);
            //    }
            //}
            using (var client = new HttpClient())
            {
                List<SurveyDependent> lst_Sd = new List<SurveyDependent>();
                //List<SurveyDependent> _lst_Sd = new List<SurveyDependent>();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var respons = await client.GetAsync(HelperExtensions.apiAllSurveysList);//.apiSurvey + "?" + sid, content

                if (respons.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = respons.Content.ReadAsStringAsync().Result;
                    lst_Sd = JsonConvert.DeserializeObject<List<SurveyDependent>>(GetSurveyQuestionSurveyIdModel);
                    var surveytypes = lst_Sd.Where(x => x.surveyId == surveyId).ToList();//.GroupBy(y=>y.surveyTypeId)
                    //foreach (var item in surveytypes)
                    //{
                    //    foreach (var item1 in item)
                    //    {
                    //        _lst_Sd.Add(new SurveyDependent { surveyId = item1.surveyId, survey = item1.survey, surveyTypeId = item1.surveyTypeId, surveyType = item1.surveyType, surveySectionId = item1.surveySectionId, surveySection = item1.surveySection });
                    //    }
                    //}
                    var SurveyType1 = surveytypes.Select(p => new SelectListItem { Value = p.surveyTypeId.ToString(), Text = p.surveyType }).ToList();//.Distinct().ToList()
                    var dataview2 = SurveyType1.ToList()
   .GroupBy(n => new { n.Value, n.Text })
   .Select(g => g.First())

   .ToList();
                    ViewBag.SurveyType = dataview2;
                    return Json(dataview2, JsonRequestBehavior.AllowGet);

                    //return Json("success");
                }
            }
            ViewBag.SurveyType = SurveyType;
            return Json(SurveyType, JsonRequestBehavior.AllowGet);
            //  var surveyType = SurveyType.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.TypeName }).ToList();

            // ViewBag.SurveyType = surveyType;
            //  return Json(surveyType, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GetSurveySectionBySurveyIdAndTypeID(int surveyId, int surveyTypeId)
        {
            //var apiParameters = "";
            //List<SurveySection> SectionSection = new List<SurveySection>();
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(HelperExtensions.apiurl);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    apiParameters = "SurveyId=" + (surveyId) + "&SurveyTypeId=" + (surveyTypeId);
            //    HttpResponseMessage respons1 = await client.GetAsync(HelperExtensions.apiSurveySectionsBySurveyAndType + "?" + apiParameters);
            //    if (respons1.IsSuccessStatusCode)
            //    {
            //        var SurveySection = respons1.Content.ReadAsStringAsync().Result;
            //        SectionSection = JsonConvert.DeserializeObject<List<SurveySection>>(SurveySection);
            //    }
            //}
            //var surveySectionResult = SectionSection.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.SectionName }).ToList();
            //ViewBag.SurveySections = surveySectionResult;
            //return Json( surveySectionResult , JsonRequestBehavior.AllowGet);


            using (var client = new HttpClient())
            {
                List<SurveyDependent> lst_Sd = new List<SurveyDependent>();
                client.BaseAddress = new Uri(HelperExtensions.apiurl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var respons = await client.GetAsync(HelperExtensions.apiAllSurveysList);

                if (respons.IsSuccessStatusCode)
                {
                    var GetSurveyQuestionSurveyIdModel = respons.Content.ReadAsStringAsync().Result;
                    lst_Sd = JsonConvert.DeserializeObject<List<SurveyDependent>>(GetSurveyQuestionSurveyIdModel);
                    var surveytypes = lst_Sd.Where(x => x.surveyId == surveyId && x.surveyTypeId == surveyTypeId ).ToList();
                    var SurveyType1 = surveytypes.Select(p => new SelectListItem { Value = p.surveySectionId.ToString(), Text = p.surveySection }).ToList();
                    ViewBag.SurveyType = SurveyType1;
                    return Json(SurveyType1, JsonRequestBehavior.AllowGet);
                }
            }
            ViewBag.SurveyType =new SurveyType();
            return Json(new SurveyType(), JsonRequestBehavior.AllowGet);
        }


    }
}