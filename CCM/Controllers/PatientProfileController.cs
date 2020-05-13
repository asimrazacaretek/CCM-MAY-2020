using System;
using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Net;
using System.Collections.Generic;
using System.Web;
using CCM.Helpers;
using CCM.Models.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, Admin, QAQC, PhysiciansGroup, LiaisonGroup,Sales")]
    public class PatientProfileController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<PartialViewResult> LaboratoryResult(int patientId)
        {
            ViewBag.PatientId = patientId;
            var patientLaboratoryFiles = await _db.patient_Images.Where(x => x.PatientId == patientId).FirstOrDefaultAsync();
            if (patientLaboratoryFiles == null)
            {
                patientLaboratoryFiles = new Patient_Images();
                patientLaboratoryFiles.PatientId = patientId;
            }
            var patientLaboratorylist = _db.patient_Images.Where(x => x.PatientId == patientId && x.IsDelete == false && x.ImgType == "Lab Report").ToList();
            if (patientLaboratorylist.Count > 0)
            {
                for (int i = 0; i < patientLaboratorylist.Count; i++)
                {
                    //string baseval = HelperExtensions.Decrypt(patientLaboratorylist[i].FilePath);
                    string baseval = patientLaboratorylist[i].FilePath;

                    string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                    patientLaboratorylist[i].FilePath = files;
                }
            }
            ViewBag.MyList = patientLaboratorylist;


            var patient = await _db.Patients.FindAsync(patientId);


            return PartialView(patient);
        }


     

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> LaboratoryResult([Bind(Exclude = "Photo,PhotoEmrRecords1,PhotoEmrRecords2,PhotoEmrRecords3,PhotoEmrRecords4,PhotoEmrRecords5,PhotoEmrRecords6,PhotoEmrRecords7,PhotoEmrRecords8,PhotoEmrRecords9,PhotoEmrRecords10,PhotoEmrRecords11,PhotoEmrRecords12,PhotoEmrRecords13,PhotoEmrRecords14,PhotoEmrRecords15,EnrollmentStatus,EnrollmentSubStatus,EnrollmentSubStatusReason")] Patient patient)
        {
            int patientId = patient.Id;

            Patient_Images laboratoryResult = new Patient_Images();
            if (HelperExtensions.isAllowedforEditingorAdd(patientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            string [] arr =new string[14];
            for (int i = 1; i <=arr.Count(); i++)
            {
                string temp = "";
              
                  temp = "PhotoEmrRecords"+i;
                arr[i-1]=temp;
                
            }

            var lastid = _db.patient_Images.Where(y => y.PatientId == patientId).Count();
           
            for (int i = 0; i <= arr.Count() - 1; i++)
            {
                string base64StringData = ""; // Your base 64 string data
                var postedfile= Request.Files[arr[i]];
                if (postedfile?.ContentLength != 0 && postedfile?.InputStream != null)
                {
                    using (var binaryEmr = new BinaryReader(postedfile.InputStream))
                    {
                        var emrImageData = binaryEmr.ReadBytes(postedfile.ContentLength);
                        if (emrImageData.Length > 0)
                        {
                            base64StringData = Convert.ToBase64String(emrImageData);
                        }
                    }


                    int id = lastid == null ? 1 + i : lastid + i == 0 ? 1 : lastid + i + 1;
                    string fileName = id + "_LR_" + DateTime.Now.ToString("MMddyyyy") + "_" + patientId;

                    string filepath = HelperExtensions.fileDirectory() + fileName;

                    string cleandata = "";
                    string mimtype = "";
                    if (base64StringData.Contains("data:image/jpeg") || base64StringData.Contains("data:image/jpg"))
                    {
                        cleandata = base64StringData.Replace("data:image/jpeg;base64,", "");
                        mimtype = "image/jpeg";
                    }
                    else if (base64StringData.Contains("data:image/png"))
                    {
                        cleandata = base64StringData.Replace("data:image/png;base64,", "");
                        mimtype = "image/png";
                    }
                    else
                        cleandata = base64StringData;

                    Patient_Images patient_Laboratory = new Patient_Images();
                    patient_Laboratory.PatientId = patientId;
                    patient_Laboratory.FileName = fileName;
                    //patient_Laboratory.FilePath =  HelperExtensions.Encrypt(filepath);
                    patient_Laboratory.FilePath = filepath;

                    patient_Laboratory.ImgType = "Lab Report";
                    patient_Laboratory.MimeType = mimtype;
                    patient_Laboratory.IsDelete = false;
                    patient_Laboratory.CreatedOn = DateTime.Now;
                    patient_Laboratory.CreatedBy = User.Identity.GetUserId();
                    patient_Laboratory.UpdatedOn = DateTime.Now;

                    patient_Laboratory.UpdatedBy = User.Identity.GetUserId();

                    _db.patient_Images.Add(patient_Laboratory);
                    //After added into list
                    byte[] data = System.Convert.FromBase64String(cleandata);

                    System.IO.File.WriteAllBytes(filepath, data);
                }

                //********************Save image into folder ***********////
                //MemoryStream ms = new MemoryStream(data);
                //System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                //img.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                //********************Save image into folder ***********////
            }
           var SAVE= await _db.SaveChangesAsync();
            if (SAVE>0)
            {
                return "True";
            }
            return "False";
            //else
            //{
            //    var errorList = ModelState.Values.SelectMany(m => m.Errors)
            //                     .Select(e => e.ErrorMessage)
            //                     .ToList();
            //    var errorstr = string.Join(",", errorList);
            //    return errorstr;
            //}
        }
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _DeleteLabRes(int id)
        {
            var LabRes = await _db.patient_Images.FindAsync(id);
            if (LabRes != null)
            {
                LabRes.IsDelete = true;
                LabRes.UpdatedBy = User.Identity.GetUserId();
                LabRes.UpdatedOn = DateTime.Now;
                _db.Entry(LabRes).State = EntityState.Modified;
                //_db.patient_Images.Remove(LabRes);
                await _db.SaveChangesAsync();
                return "True";
            }

            return "False";
        }


        public async Task<ActionResult> Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var patientProfile = patient?.ProfileId != null
                               ? await _db.PatientProfiles.FindAsync(patient.ProfileId)
                               : new PatientProfile
                               {
                                   PatientId = patientId,
                                   Prefix = patient?.Prefix,
                                   FirstName = patient?.FirstName,
                                   MiddleName = patient?.MiddleName,
                                   LastName = patient?.LastName,
                                   Suffix = patient?.Suffix,
                                   BirthDate = patient?.BirthDate,
                                   Gender = patient?.Gender,
                                   OtherLanguage = patient?.OtherLanguage,
                                   PreferredLanguage = patient?.PreferredLanguage
                               };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Profile", patient?.Id, User.Identity.GetUserId());

            return View(patientProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create(PatientProfile profile)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(profile.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(profile.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
            }
            var patient = await _db.Patients.FindAsync(profile.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.Prefix = profile.Prefix;
                patient.MiddleName = profile.MiddleName;
                patient.Suffix = profile.Suffix;
                patient.Gender = profile.Gender;
                patient.PreferredLanguage = profile.PreferredLanguage;
                patient.OtherLanguage = profile.OtherLanguage;
                patient.FirstName = profile.FirstName;
                patient.LastName = profile.LastName;
                patient.BirthDate = profile.BirthDate.Value;
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;

                if (patient.ProfileId != null)
                    _db.Entry(profile).State = EntityState.Modified;

                else
                {
                    _db.PatientProfiles.Add(profile);
                    await _db.SaveChangesAsync();

                    patient.ProfileId = profile.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Create", "PatientContact", new { patientId = patient.Id });
            }


            var sameProfile = new PatientProfile
            {
                PatientId = profile.PatientId,
                Prefix = patient?.Prefix,
                FirstName = patient?.FirstName,
                MiddleName = patient?.MiddleName,
                LastName = patient?.LastName,
                Suffix = patient?.Suffix,
                BirthDate = patient?.BirthDate,
                Gender = patient?.Gender,
                OtherLanguage = patient?.OtherLanguage,
                PreferredLanguage = patient?.PreferredLanguage
            };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            return View(sameProfile);
        }

        public async Task<PartialViewResult> _Create(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var patientProfile = patient?.ProfileId != null
                               ? await _db.PatientProfiles.FindAsync(patient.ProfileId)
                               : new PatientProfile
                               {
                                   PatientId = patientId,
                                   Prefix = patient?.Prefix,
                                   FirstName = patient?.FirstName,
                                   MiddleName = patient?.MiddleName,
                                   LastName = patient?.LastName,
                                   Suffix = patient?.Suffix,
                                   BirthDate = patient?.BirthDate,
                                   Gender = patient?.Gender,
                                   OtherLanguage = patient?.OtherLanguage,
                                   PreferredLanguage = patient?.PreferredLanguage
                               };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Profile", patient?.Id, User.Identity.GetUserId());

            return PartialView(patientProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create(PatientProfile profile)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(profile.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(profile.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                return "Cycle is locked.";

            }
            var patient = await _db.Patients.FindAsync(profile.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patient.Prefix = profile.Prefix;
                patient.MiddleName = profile.MiddleName;
                patient.Suffix = profile.Suffix;
                patient.Gender = profile.Gender;
                patient.PreferredLanguage = profile.PreferredLanguage;
                patient.OtherLanguage = profile.OtherLanguage;
                patient.FirstName = profile.FirstName;
                patient.LastName = profile.LastName;
                patient.BirthDate = profile.BirthDate.Value;
                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;

                if (patient.ProfileId != null)
                    _db.Entry(profile).State = EntityState.Modified;

                else
                {
                    _db.PatientProfiles.Add(profile);
                    await _db.SaveChangesAsync();

                    patient.ProfileId = profile.Id;
                }

                patient.UpdatedBy = User.Identity.GetUserId();
                patient.UpdatedOn = DateTime.Now;
                _db.Entry(patient).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return "True";
                //return RedirectToAction("Create", "PatientContact", new { patientId = patient.Id });
            }


            var sameProfile = new PatientProfile
            {
                PatientId = profile.PatientId,
                Prefix = patient?.Prefix,
                FirstName = patient?.FirstName,
                MiddleName = patient?.MiddleName,
                LastName = patient?.LastName,
                Suffix = patient?.Suffix,
                BirthDate = patient?.BirthDate,
                Gender = patient?.Gender,
                OtherLanguage = patient?.OtherLanguage,
                PreferredLanguage = patient?.PreferredLanguage
            };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            return "False";
            // return View(sameProfile);
        }

        [ValidateInput(false)]
        [HttpPost]
        public void ReviewTimePost(int reviewId, string activity)
        {
            if (/*User.IsInRole("Liaison") || User.IsInRole("Sales") &&*/ reviewId > 0)
            {

                var review = _db.ReviewTimeCcms.Find(reviewId);

                //var ccmcyclestatus=_db.CCMCycleStatuses.Where(x => x.PatientId == review.PatientId && x.Cycle == review.Cycle).FirstOrDefault();
                //if(ccmcyclestatus.CCMStatus=="Enrolled" || ccmcyclestatus.CCMStatus=="In Progress")
                if (1 == 1)
                {
                    if (review != null)
                    {
                        review.BillingcategoryId = BillingCodeHelper.cmmBillingCatagoryid;
                        review.BillingCategory= _db.BillingCategories.Where(p => p.BillingCategoryId == review.BillingcategoryId).Select(p => p).FirstOrDefault();
                        review.EndTime = DateTime.Now;
                        review.ReviewTime = DateTime.Now - review.StartTime;
                        if (activity == null || activity == "")
                        {
                            activity = "Idle";
                        }
                        review.Activity = activity;
                        if (review.PatientId.Value > 0)
                        {
                            review.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(review.PatientId.Value, BillingCodeHelper.cmmBillingCatagoryid);
                        }

                        if (review.ReviewTime.TotalSeconds < 15 && activity == "Idle")
                        {
                            _db.ReviewTimeCcms.Remove(review);
                            _db.Entry(review).State = EntityState.Deleted;
                            _db.SaveChanges();
                            return;
                        }

                        _db.Entry(review).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }

            }
        }
        [HttpPost]
        public  TimeSpan GetTotalReviewTime(int? patientId, int? BillingCategoryId)
        {

            try
            {

                var reviews = _db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.BillingcategoryId == BillingCategoryId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;
            }
            catch (Exception ex)
            {


                var reviews = _db.ReviewTimeCcms.Where(r => r.PatientId == patientId && r.BillingcategoryId == BillingCategoryId).AsNoTracking().ToList();

                return reviews.Any()
                    ? reviews.Aggregate(TimeSpan.Zero, (sum, review) => sum + review.ReviewTime)
                    : TimeSpan.Zero;

            }
        }
        public void ReviewTimePostOtherCategories(int? reviewId, string activity)
        {

                List<ReviewTimeActivityViewModel> ReviewTimeActivityList = JsonConvert.DeserializeObject<List<ReviewTimeActivityViewModel>>(activity);


                foreach (var ReviewTimeActivity in ReviewTimeActivityList)
            {
                reviewId = null;
               reviewId = Convert.ToInt32(ReviewTimeActivity.ReviewId);
                    if (reviewId != null && reviewId!=0)
                    {
                    var review = _db.ReviewTimeCcms.Find(reviewId);
                    review.BillingcategoryId = Convert.ToInt32(ReviewTimeActivity.BillingCategoryId);
                        review.BillingCategory = _db.BillingCategories.Where(p => p.BillingCategoryId == review.BillingcategoryId).Select(p => p).FirstOrDefault();
                        review.EndTime = DateTime.Now;
                        review.ReviewTime = DateTime.Now - review.StartTime;
                        if (ReviewTimeActivity.Activity == null || ReviewTimeActivity.Activity == "")
                        {
                            ReviewTimeActivity.Activity = "Idle";
                        }
                        review.Activity = ReviewTimeActivity.Activity;
                        if (review.PatientId.Value > 0)
                        {
                            review.Cycle = CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(review.PatientId.Value, BillingCodeHelper.cmmBillingCatagoryid);
                        }

                        if (review.ReviewTime.TotalSeconds < 15 && ReviewTimeActivity.Activity == "Idle")
                        {
                            _db.ReviewTimeCcms.Remove(review);
                            _db.Entry(review).State = EntityState.Deleted;
                            _db.SaveChanges();
                            return;
                        }

                        _db.Entry(review).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }



        }
        public async Task<PartialViewResult> _ContactPreferences(int patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            var patientProfile = patient?.ProfileId != null
                               ? await _db.PatientProfiles.FindAsync(patient.ProfileId)
                               : new PatientProfile
                               {
                                   PatientId = patientId,
                                   Prefix = patient?.Prefix,
                                   FirstName = patient?.FirstName,
                                   MiddleName = patient?.MiddleName,
                                   LastName = patient?.LastName,
                                   Suffix = patient?.Suffix,
                                   BirthDate = patient?.BirthDate,
                                   Gender = patient?.Gender,
                                   OtherLanguage = patient?.OtherLanguage,
                                   PreferredLanguage = patient?.PreferredLanguage
                               };

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;

            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("Profile", patient?.Id, User.Identity.GetUserId());

            return PartialView(patientProfile);
        }

        public async Task<PartialViewResult> _PatientProfileConsent(int? patientId)
        {
            ViewBag.EnrolledCcmCategories = _db.Patients_BillingCategories.Where(x => x.PatientId == patientId && x.Status == true).Select(p => p.BillingCategory).Distinct().ToList();
            ViewBag.PatientConcents = _db.PatientProfile_Consent.Where(p => p.PatientId == patientId).ToList();

            if (patientId != null)
            {

                var patientconsethistory = _db.Database.SqlQuery<PatientProfile_Consent>("USP_GetPatientProfile_Consent_Histrory @pidd", new SqlParameter("pidd", patientId));
  
                ViewBag.PatientConsetHistory = patientconsethistory.Reverse();
                var patientConsent = await _db.PatientProfile_Consent.Where(x => x.PatientId == patientId).FirstOrDefaultAsync();
                if (patientConsent != null)
                {
                    if (patientConsent.filePath != null)
                    {

                    string baseval = patientConsent.filePath;
                    string files = "data:image/png;base64," + HelperExtensions.convertbase64(baseval);
                   ViewBag.filepath = files;
                    }
                    return PartialView(patientConsent);
                }
                else
                {
                    //var ConsentTemplate = await _db.PatientProfile_ConsentTemplate.FirstAsync();

                    return PartialView(new PatientProfile_Consent { PatientId = patientId.Value/*, Consent = ConsentTemplate.ConsentTemplate*/ });
                }
            }
            else {
                return PartialView(new PatientProfile_Consent());
            }
        }
        [HttpPost]
        public PartialViewResult _PatientProfileConsentPatrial(int? patientId)
        {
            if (patientId != null)
            {
                var patientconsethistory = _db.Database.SqlQuery<PatientProfile_Consent>("USP_GetPatientProfile_Consent_Histrory @pidd", new SqlParameter("pidd", patientId));
                //ViewBag.PatientConsetHistory = patientconsethistory.OrderByDescending(n=>n.CreatedOn).ThenByDescending(n=>n.UpdatedOn);
                //ViewBag.PatientConsetHistory = patientconsethistory.OrderByDescending(n => n.UpdatedBy);
                ViewBag.PatientConsetHistory = patientconsethistory.Reverse();
            }
                return PartialView();
        }

        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        //,PatientProfile_Consent profile2
        public async Task<string> _PatientProfileConsent(FormCollection fc, PatientProfile_Consent profile)
         {
          
            var val = fc["Signature"].ToString();
            //PatientProfile_Consent profile = new PatientProfile_Consent();
            profile.PatientId = Convert.ToInt32(fc["PatientId"]);
            profile.Id = Convert.ToInt32(fc["Id"]);
            profile.Consent = Convert.ToString(fc["Consent"]);
            profile.Signature = fc["Signature"].Contains("data:image/png") == true ? Convert.FromBase64String(fc["Signature"].Replace("data:image/png;base64,", "")) : Convert.FromBase64String(fc["Signature"]);
            profile.CreatedBy = Convert.ToString(fc["CreatedBy"]);
            profile.CreatedOn = Convert.ToDateTime(fc["CreatedOn"]);
            profile.Note = fc["Note"];
            profile.BillingCategoryId = Convert.ToInt32( fc["BillingCategoryId"]);
            //////////************************************************For Images Saving*************************////////////////
            string base64StringData = ""; // Your base 64 string data
            var postedfile = Request.Files["uploadFiles"];


            if (postedfile?.ContentLength != 0 && postedfile?.InputStream != null)
            {


                using (var binaryEmr = new BinaryReader(postedfile.InputStream))
                {
                    var emrImageData = binaryEmr.ReadBytes(postedfile.ContentLength);
                    if (emrImageData.Length > 0)
                    {
                        base64StringData = Convert.ToBase64String(emrImageData);
                    }
                }


                string fileName = Guid.NewGuid() + "_UDT_" + DateTime.Now.ToString("MMddyyyy") + "_" + profile.Id;

                string filepath = HelperExtensions.fileDirectoryForPatientConcent() + fileName;

                string cleandata = "";
                string mimtype = "";
                if (base64StringData.Contains("data:image/jpeg") || base64StringData.Contains("data:image/jpg"))
                {
                    cleandata = base64StringData.Replace("data:image/jpeg;base64,", "");
                    mimtype = "image/jpeg";
                }
                else if (base64StringData.Contains("data:image/png"))
                {
                    cleandata = base64StringData.Replace("data:image/png;base64,", "");
                    mimtype = "image/png";
                }
                else
                    cleandata = base64StringData;

                profile.fileName = fileName;
                profile.filePath = filepath;




                byte[] data = System.Convert.FromBase64String(cleandata);

                System.IO.File.WriteAllBytes(filepath, data);

            }

            //if(profile.fileName==null || profile.filePath == null)
            //{
            //    if (profile.Id != 0)
            //    {
            //    var profiles = _db.PatientProfile_Consent.Where(p => p.Id == profile.Id).FirstOrDefault();

            //    profile.fileName = profiles.fileName;
            //    profile.filePath = profiles.filePath;
            //}

            //}
            //profile.PatientId = Convert.ToInt32(fc["PatientId"]);
            //profile.PatientId = Convert.ToInt32(fc["PatientId"]);
            var patient = await _db.Patients.FindAsync(profile.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (profile.Id > 0)
                {
                    try
                    {

                    profile.UpdatedBy = User.Identity.GetUserId();
                    profile.UpdatedOn = DateTime.Now;
                    _db.Entry(profile).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                    catch(Exception ex)
                    {

                    }
                    }
                else
                {

                    try
                    {

                    profile.CreatedBy = User.Identity.GetUserId();
                    profile.CreatedOn = DateTime.Now;
                    profile.UpdatedBy = User.Identity.GetUserId();
                    profile.UpdatedOn = DateTime.Now;
                    _db.PatientProfile_Consent.Add(profile);
                    await _db.SaveChangesAsync();
                    }
                    catch(Exception ex)
                    {
                        return "false";
                    }
                }





                return "True";
            }

            return "False";
        }



        public PartialViewResult _PatientProfile_AddHospitalDetails(int patientId)
        {

            //ViewBag.physcians = new SelectList(_db.Physicians.Select(P =>new string{ P.FirstName+" "+P.LastName).ToList());
            //ViewBag.department = new SelectList(_db.Departments.Select(Q => Q.DepartmentName).ToList());

            ViewBag.hospitals = _db.Hospitals.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.HospitalName
            }).OrderBy(p => p.Text);


            //ViewBag.physcians = _db.Physicians.Select(p => new SelectListItem
            //{
            //    Value = p.Id.ToString(),
            //    Text = p.FirstName + " " + p.LastName
            //});
            //ViewBag.department = _db.Departments.Select(p => new SelectListItem
            //{
            //    Value = p.Id.ToString(),
            //    Text = p.DepartmentName
            //});

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> PatientProfile_AddHospitalDetails(PatientProfile_HospitalDetails hospitalDetails)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(hospitalDetails.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(hospitalDetails.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                return "Cycle is locked.";
            }
            hospitalDetails.HospitalName = _db.Hospitals.Where(x => x.Id == hospitalDetails.HospitalsId).FirstOrDefault().HospitalName;
            //hospitalDetails.Doctors = _db.Physicians.Where(y => y.Id == hospitalDetails.PhysicianId).FirstOrDefault().FirstName;
            //hospitalDetails.Department = _db.Departments.Where(y => y.Id == hospitalDetails.DepartmentId).FirstOrDefault().DepartmentName;
            hospitalDetails.CreatedBy = User.Identity.GetUserId();
            hospitalDetails.CreatedOn = DateTime.Now;
            hospitalDetails.UpdatedOn = DateTime.Now;
            var patient = _db.Patients.Find(hospitalDetails.PatientId);

            if (patient != null && ModelState.IsValid)
            {
                _db.PatientProfile_HospitalDetails.Add(hospitalDetails);
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                }
                return "True";
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Hospital Visits";

            // return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
            return "False";
        }

        public string HospitalDetails(int hospitalId)
        {
            var hospital = _db.Hospitals.Where(p => p.Id == hospitalId).Select(p => new SelectListItem
            {
                Text = p.City + "/" + p.Country + "/" + p.State
            }).FirstOrDefault().Text;
            return hospital;
        }

        /////////////////////////////Edit Hospital Detials
        public PartialViewResult _EditHospitalDetails(int hospitalId)
        {

            ViewBag.hospitals = _db.Hospitals.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.HospitalName
            });
            //var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            //ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text).ToList(), "Text", "Text");//, patient.PhysicianId

            PatientProfile_HospitalDetails model = _db.PatientProfile_HospitalDetails.Where(m => m.Id == hospitalId).First();

            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _EditHospitalDetails(PatientProfile_HospitalDetails patientProfile_Hospital)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(patientProfile_Hospital.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientProfile_Hospital.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";

            }
            var patient = _db.Patients.Find(patientProfile_Hospital.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                patientProfile_Hospital.HospitalName = _db.Hospitals.Where(x => x.Id == patientProfile_Hospital.HospitalsId).FirstOrDefault().HospitalName;
                patientProfile_Hospital.UpdatedBy = User.Identity.GetUserId();
                patientProfile_Hospital.UpdatedOn = DateTime.Now;

                _db.Entry(patientProfile_Hospital).State = EntityState.Modified;

                await _db.SaveChangesAsync();
                return "True";
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                var errorstr = string.Join(",", errorList);
                return errorstr;
            }
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";

            //return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
        }

        public async Task<PartialViewResult> _HospitalDetailList(int patientId)
        {
            var currentdate = DateTime.Now;
            var startdate = currentdate.AddDays(-365);

            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);

            var Last365days = 0; var JanToCurrent = 0; var countLast365days = 0;
            var hospitaldetails = _db.PatientProfile_HospitalDetails.Where(z => z.PatientId == patientId).ToList().OrderByDescending(x => x.DischargeDate).ToList();
            if (hospitaldetails.Count > 0)
            {


                Last365days = hospitaldetails.Where(x => x.AdmitDate <= currentdate).Where(x => x.AdmitDate >= startdate).Select(x => x.TotalDays).Sum();
                countLast365days = hospitaldetails.Where(x => x.AdmitDate <= currentdate).Where(x => x.AdmitDate >= startdate).Select(x => x.TotalDays).Count();
                JanToCurrent = hospitaldetails.Where(x => x.AdmitDate <= currentdate).Where(x => x.AdmitDate >= firstDay).Select(x => x.TotalDays).Sum();

                //if (date1.Date<DateTime.Now.Date)
                //{
                //    var diffrer = DateTime.Now - date1;
                //    var newdate = date1.AddDays(diffrer.TotalDays);
                //    enddateforoneyear = date1.AddDays(-365);

                //    //var totalcostforoneyear=hospitaldetailforoneyear.Sum(x=>x.Rate*x.)
                //}
                //else
                //{

                //}
            }
            ViewBag.yearlCost = Last365days;
            ViewBag.JanToNow = JanToCurrent;
            ViewBag.TotalAdmofYear = countLast365days;
            return PartialView(hospitaldetails);
        }


        //////////////////////////Updated Patient Profile Hospital Visit///////////////////////
        public async Task<PartialViewResult> _PatientProfile_HospitalVisit(int patientId)
        {
            var currentdate = DateTime.Now;
            var startdate = currentdate.AddDays(-365);

            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);

            var Last365days = 0; var JanToCurrent = 0; var countLast365days = 0;
            var hospitaldetails = _db.patientProfile_Hospitalvisits.Where(z => z.PatientId == patientId).Include(p=>p.HospitalReasons).ToList().OrderByDescending(x => x.Id).ToList();
            if (hospitaldetails.Count > 0)
            {
                Last365days = hospitaldetails.Where(x => x.AdmitDate <= currentdate).Where(x => x.AdmitDate >= startdate).Select(x => x.TotalDays).Sum();
                countLast365days = hospitaldetails.Where(x => x.AdmitDate <= currentdate).Where(x => x.AdmitDate >= startdate).Select(x => x.TotalDays).Count();
                JanToCurrent = hospitaldetails.Where(x => x.AdmitDate <= currentdate).Where(x => x.AdmitDate >= firstDay).Select(x => x.TotalDays).Sum();
            }
            ViewBag.yearlCost = Last365days;
            ViewBag.JanToNow = JanToCurrent;
            ViewBag.TotalAdmofYear = countLast365days;
            ViewBag.PatientId = patientId;
            return PartialView(hospitaldetails);
        }

        public PartialViewResult _PatientProfile_CreateHospitalVisit(int patientId)
        {
            ViewBag.HospitalReasons = _db.HospitalReasons.ToList();
            ViewBag.HospitalDepartments = _db.HospitalDepartments.ToList();
            ViewBag.Procedures = _db.HospitalProcedures.ToList();
            //ViewBag.hospitals = _db.Hospitals.Select(p => new SelectListItem
            //{
            //    Value = p.Id.ToString(),
            //    Text = p.HospitalName
            //}).OrderBy(p => p.Text);

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _PatientProfile_CreateHospitalVisit(PatientProfile_Hospitalvisits hospitalDetails)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(hospitalDetails.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(hospitalDetails.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

                return "Cycle is locked.";
            }
            //hospitalDetails.HospitalName = _db.Hospitals.Where(x => x.Id == hospitalDetails.HospitalsId).FirstOrDefault().HospitalName;
            //hospitalDetails.Doctors = _db.Physicians.Where(y => y.Id == hospitalDetails.PhysicianId).FirstOrDefault().FirstName;
            //hospitalDetails.Department = _db.Departments.Where(y => y.Id == hospitalDetails.DepartmentId).FirstOrDefault().DepartmentName;
            hospitalDetails.CreatedBy = User.Identity.GetUserId();
            hospitalDetails.CreatedOn = DateTime.Now;
            hospitalDetails.UpdatedOn = DateTime.Now;
            var patient = _db.Patients.Find(hospitalDetails.PatientId);

            if (patient != null && ModelState.IsValid)
            {
                _db.patientProfile_Hospitalvisits.Add(hospitalDetails);
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                }
                return "True";
            }

            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Hospital Visits";

            // return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
            return "False";
        }

        public PartialViewResult _PatientProfile_EditHospitalVisit(int hospitalId)
        {
            ViewBag.HospitalReasons = _db.HospitalReasons.ToList();
            ViewBag.HospitalDepartments = _db.HospitalDepartments.ToList();
            ViewBag.Procedures = _db.HospitalProcedures.ToList();
            //ViewBag.hospitals = _db.Hospitals.Select(p => new SelectListItem
            //{
            //    Value = p.Id.ToString(),
            //    Text = p.HospitalName
            //});
            //var physicians = _db.Physicians.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FirstName + " " + p.LastName });
            //ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text).ToList(), "Text", "Text");//, patient.PhysicianId

            PatientProfile_Hospitalvisits model = _db.patientProfile_Hospitalvisits.Where(m => m.Id == hospitalId).First();

            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _PatientProfile_EditHospitalVisit(PatientProfile_Hospitalvisits patientProfile_Hospital)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(patientProfile_Hospital.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(patientProfile_Hospital.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";

            }
            var patient = _db.Patients.Find(patientProfile_Hospital.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                //patientProfile_Hospital.HospitalName = _db.Hospitals.Where(x => x.Id == patientProfile_Hospital.HospitalsId).FirstOrDefault().HospitalName;
                patientProfile_Hospital.UpdatedBy = User.Identity.GetUserId();
                patientProfile_Hospital.UpdatedOn = DateTime.Now;

                _db.Entry(patientProfile_Hospital).State = EntityState.Modified;

                await _db.SaveChangesAsync();
                return "True";
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                var errorstr = string.Join(",", errorList);
                return errorstr;
            }
            ViewBag.PatientName = patient?.FirstName + " " + patient?.LastName;
            ViewBag.PatientId = patient?.Id;
            ViewBag.CcmStatus = patient?.CcmStatus;
            ViewBag.CurrentPage = "Medical History";

            //return RedirectToAction("Patient", "CurrentMedication", new { patientId = medicationRx.PatientId });
        }


        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _DeleteHospitalDetails(int id)
        {
            var hospitalDetails = await _db.patientProfile_Hospitalvisits.FindAsync(id);
            if (hospitalDetails != null)
            {
                _db.patientProfile_Hospitalvisits.Remove(hospitalDetails);
                await _db.SaveChangesAsync();
                return "True";
            }
            return "False";
        }

        [Authorize(Roles = "Liaison, Admin,Sales")]
        public PartialViewResult _PatientEnrollmentCategories(string patientId)
        {



          List<string> Data = patientId.Split('-').ToList<string>();
           var Category= Data[1];
            ViewBag.Message = Category;
            int PatientId = Convert.ToInt32(Data[0]);

            ViewBag.BillingReviewId = HelperExtensions.ReviewTimeGet(Category, PatientId, User.Identity.GetUserId());



            return PartialView();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}