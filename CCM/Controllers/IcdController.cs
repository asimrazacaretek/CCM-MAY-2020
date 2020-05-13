using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CCM.Models;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using CCM.Helpers;

namespace CCM.Controllers
{
    public class IcdController : BaseController
    {
        //private ApplicationdbContect _db = new ApplicationdbContect();

        // GET: Icd
        public async Task<ActionResult> Index(int? patientId)
        {
            ViewBag.patientId = patientId;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            var list = _db.Icd10Codes.Where(m => m.PatientId == patientId);

            return View(await list.ToListAsync());
        }

        public async Task<PartialViewResult> _Index(int? patientId)
        {
            ViewBag.patientId = patientId;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            var list = _db.Icd10Codes.Where(m => m.PatientId == patientId).OrderByDescending(x=>x.DateCreated);

            return PartialView(await list.ToListAsync());
        }


        // GET: Icd/Details/5
        public async Task<ActionResult> Details(int? id,int patientId)
        {
            ViewBag.patientId = patientId;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
            if (icd10Codes == null)
            {
                return HttpNotFound();
            }
            return View(icd10Codes);
        }

        public async Task<PartialViewResult> _Details(int? id, int patientId)
        {
            ViewBag.patientId = patientId;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            if (id == null)
            {
                return PartialView("_BadRequest");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
            if (icd10Codes == null)
            {
                return PartialView("_NotFound");
            }
            return PartialView(icd10Codes);
        }

        // GET: Icd/Create
        public ActionResult Create(int patientId)
        {
            ViewBag.patientId = patientId;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            var list = _db.Icd10Codes.Find(patientId);

            return View(new Icd10Codes { PatientId = patientId });
        }
       
        // POST: Icd/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code10,Code9,PatientId,DateCreated")] Icd10Codes icd10Codes)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(icd10Codes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(icd10Codes.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            if (ModelState.IsValid)
            {
                _db.Icd10Codes.Add(icd10Codes);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", new { patientId = icd10Codes.PatientId });
            }

            return View(icd10Codes);
        }

        [HttpGet]
        public async Task<PartialViewResult> _Create(int patientId)
        {
            ViewBag.patientId = patientId;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            var list = _db.Icd10Codes.Find(patientId);

            return PartialView(new Icd10Codes { PatientId = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Create([Bind(Include = "Id,Code10,Code9,PatientId,DateCreated")] Icd10Codes icd10Codes1,string[] ICD10Codes,string[] ICD9Codes,string[] DiseaseState,string[] DiseaseType,DateTime[] DateCreated,string[] DiseaseHistory)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(icd10Codes1.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(icd10Codes1.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            if (ModelState.IsValid)
            {
               
                var alreadycodes = _db.Icd10Codes.AsNoTracking().Where(x => x.PatientId == icd10Codes1.PatientId).ToList();
                for(int i = 0; i <= ICD10Codes.Count() - 1; i++)
                {
                    var alreadyitem = alreadycodes.Where(x => x.Code10 == ICD10Codes[i]).FirstOrDefault();
                    if (alreadyitem == null)
                    {
                        Icd10Codes icd10Codesnew = new Icd10Codes();
                        icd10Codesnew.Code10 = ICD10Codes[i];
                        icd10Codesnew.Code9 = ICD9Codes[i];
                        icd10Codesnew.PatientId = icd10Codes1.PatientId;
                        icd10Codesnew.DateCreated = DateCreated[i];
                        icd10Codesnew.DiseaseState = DiseaseState[i];
                        icd10Codesnew.DiseaseType = DiseaseType[i];
                        icd10Codesnew.DiseaseHistory = DiseaseHistory[i];
                        _db.Icd10Codes.Add(icd10Codesnew);
                    }
                }
                //foreach (var item in ICD10Codes)
                //{
                //    var alreadyitem = alreadycodes.Where(x => x.Code10 == item).FirstOrDefault();
                //    if (alreadyitem == null)
                //    {
                //        Icd10Codes icd10Codesnew = new Icd10Codes();
                //        icd10Codesnew.Code10 = item;
                //        icd10Codesnew.Code9 = icd10Codes.Code9;
                //        icd10Codesnew.PatientId = icd10Codes.PatientId;
                //        icd10Codesnew.DateCreated = icd10Codes.DateCreated;
                //        _db.Icd10Codes.Add(icd10Codesnew);
                //    }
                   

                //}

              
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
            //return "False";
        }


        // GET: Icd/Edit/5
        public async Task<ActionResult> Edit(int? id, int patientId)
        {
            ViewBag.patientId = patientId;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
            if (icd10Codes == null)
            {
                return HttpNotFound();
            }
            return View(icd10Codes);
        }

        // POST: Icd/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code10,Code9,PatientId,DateCreated")] Icd10Codes icd10Codes)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(icd10Codes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(icd10Codes.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            if (ModelState.IsValid)
            {
                _db.Entry(icd10Codes).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", new { patientId = icd10Codes.PatientId });
            }
            return View(icd10Codes);
        }

        /////////////////***Partia View***/////////////////////////
        public async Task<PartialViewResult> _Edit(int? id, int patientId)
        {
            ViewBag.patientId = patientId;
            //if (User.IsInRole("Liaison"))
            //    ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return PartialView("_BadRequest");
            }
            Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
            if (icd10Codes == null)
            {
                return PartialView("_NotFound");
                //return HttpNotFound();
            }
            return PartialView(icd10Codes);
        }

        // POST: Icd/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Edit([Bind(Include = "Id,Code10,Code9,DiseaseState,DiseaseType,DiseaseHistory,PatientId,DateCreated")] Icd10Codes icd10Codes)
        {
            if (HelperExtensions.isAllowedforEditingorAdd(icd10Codes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(icd10Codes.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return "Cycle is locked.";
            }
            if (ModelState.IsValid)
            {
                               
                _db.Entry(icd10Codes).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return "True";
                //return RedirectToAction("Index", new { patientId = icd10Codes.PatientId });
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();
                var errorstr = string.Join(",", errorList);
                return errorstr;
            }
        }

        // GET: Icd/Delete/5
        public async Task<ActionResult> Delete(int? id, int patientId)
        {
            ViewBag.patientId = patientId;
            if (User.IsInRole("Liaison"))
                ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
            if (icd10Codes == null)
            {
                return HttpNotFound();
            }
            return View(icd10Codes);
        }
       
        //public async Task<PartialViewResult> _Delete(int? id, int patientId)
        //{
        //    ViewBag.patientId = patientId;
        //    if (User.IsInRole("Liaison"))
        //        ViewBag.ReviewId = HelperExtensions.ReviewTimeGet("ICD 10 Codes", patientId, User.Identity.GetUserId());
        //    if (id == null)
        //    {
        //        return PartialView("_BadRequest");
        //        //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
        //    if (icd10Codes == null)
        //    {
        //        return PartialView("_NotFound");
        //        //return HttpNotFound();
        //    }
        //    return PartialView(icd10Codes);
        //}

        // POST: Icd/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Liaison, Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            
            Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
            if (HelperExtensions.isAllowedforEditingorAdd(icd10Codes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(icd10Codes.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });

            }
            _db.Icd10Codes.Remove(icd10Codes);
            await _db.SaveChangesAsync();
            return RedirectToAction("_Index", new { patientId = icd10Codes.PatientId });
        }

        [Authorize(Roles = "Liaison, Admin,Sales")]
        public async Task<string> _Delete(int id)
        {
            Icd10Codes icd10Codes = await _db.Icd10Codes.FindAsync(id);
            if (HelperExtensions.isAllowedforEditingorAdd(icd10Codes.PatientId, CategoryCycleStatusHelper.GetPatientNewOrOldCycleByCategory(icd10Codes.PatientId, BillingCodeHelper.cmmBillingCatagoryid), User.Identity.GetUserId()) == false)
            {
                //return RedirectToAction("Index", "CcmStatus", new { status = HelperExtensions.GetStatusRedirectionbyUser(User.Identity.GetUserId()), Message = "Cycle is locked." });
                return "Cycle is locked.";
            }
            _db.Icd10Codes.Remove(icd10Codes);
            await _db.SaveChangesAsync();
            return "True";
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
