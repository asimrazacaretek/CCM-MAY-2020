using CCM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class PatientSignatureController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();
        // GET: PatientSignature
        public async Task<ActionResult> Index(string patientIdstr)
        {
            int patientId = Convert.ToInt32(HelperExtensions.Decrypt(patientIdstr));
            var patientConsent = await _db.PatientProfile_Consent.Where(x => x.PatientId == patientId).FirstOrDefaultAsync();
            if (patientConsent != null)
            {
                return PartialView(patientConsent);
            }
            else
            {
                var ConsentTemplate = await _db.PatientProfile_ConsentTemplate.FirstAsync();

                return View(new PatientProfile_Consent { PatientId = patientId, Consent = ConsentTemplate.ConsentTemplate });
            }
        }
        public ActionResult Thanks()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> PatientProfileConsent(PatientProfile_Consent profile)
        {
            var patient = await _db.Patients.FindAsync(profile.PatientId);
            if (patient != null && ModelState.IsValid)
            {
                if (profile.Id > 0)
                {

                    profile.UpdatedBy = User.Identity.GetUserId();
                    profile.UpdatedOn = DateTime.Now;
                    _db.Entry(profile).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                else
                {

                    profile.CreatedBy = User.Identity.GetUserId();
                    profile.CreatedOn = DateTime.Now;
                    profile.UpdatedBy = User.Identity.GetUserId();
                    profile.UpdatedOn = DateTime.Now;
                    _db.PatientProfile_Consent.Add(profile);
                    await _db.SaveChangesAsync();
                }




              return  RedirectToAction("Thanks");
            
            }
            return RedirectToAction("Thanks");

        }
    }
}