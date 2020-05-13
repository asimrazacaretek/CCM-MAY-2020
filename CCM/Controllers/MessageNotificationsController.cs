using System;
using CCM.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;


namespace CCM.Controllers
{
    [Authorize(Roles = "Patient")]
    public class MessageNotificationsController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();


        // GET: MessageNotifications
        public async Task<PartialViewResult> _MessagesPartial(int patientId)
        {
            return PartialView(await _db.MessageNotifications.Where(m => m.PatientId == patientId).ToListAsync());
        }
        

        // GET: MessageNotifications/Create
        public PartialViewResult _NewMessagePartial()
        {
            var user      = _db.Users.Find(User.Identity.GetUserId());
            var patientId = user.CCMid ?? 0;

            return PartialView(new NewMessageViewModel { PatientId = patientId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMessage(NewMessageViewModel newMessage)
        {
            if (ModelState.IsValid)
            {
                var patient = await _db.Patients.FindAsync(newMessage.PatientId);

                if (patient != null)
                {
                    var messageNotification = new MessageNotification()
                    {
                        SendDateTime = DateTime.Now,
                        MessageBody  = newMessage.MessageBody,
                        PatientId    = newMessage.PatientId,
                        LiaisonId    = patient.LiaisonId
                    };

                    _db.MessageNotifications.Add(messageNotification);
                    await _db.SaveChangesAsync();


                    var body = "Hello " + patient.Liaison.FirstName + ",<br /><br />" +

                               "Following is a new message from your CCM Patient:<br /> " + 
                               "Name: " + patient.FirstName + " " + patient.LastName + "<br />" +
                               "Cell Phone Number: " + patient.MobilePhoneNumber + "<br />" +
                               "Home Phone Number: " + patient.HomePhoneNumber + "<br />" +
                               "CCM Enrollment Status: " + patient.EnrollmentStatus + "<br />" +
                               "CCM Status: " + patient.CcmStatus + "<br /><br>" +

                               newMessage.MessageBody + "<br /><br />" +

                               "<small>This email is system generated and is not monitored. Please, do not reply to this email.</small>";

                    var ems = new EmailService();
                    await ems.SendAsync(new IdentityMessage
                    {
                        Destination = patient.Liaison.Email,
                        Subject = "New message from CCM patient " + patient.FirstName + " " + patient.LastName,
                        Body = body
                    });
                }
            }

            return RedirectToAction("Details", "PatientPortal", new { patientId = newMessage.PatientId });
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
