using CCM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;


namespace CCM.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Patient")]
    public class PatientPortalController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        public async Task<ActionResult> Details(int? patientId)
        {
            var patient  = await _db.Patients.FindAsync(patientId);
            ViewBag.MessagesCount = await _db.MessageNotifications.CountAsync(m => m.PatientId == patient.Id);

            return View(patient);
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