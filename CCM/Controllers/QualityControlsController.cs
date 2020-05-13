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
using Microsoft.AspNet.Identity.Owin;

namespace CCM.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Admin")]
    public class QualityControlsController : BaseController
    {
        //private ApplicationdbContect _db = new ApplicationdbContect();
        private ApplicationUserManager _userManager;
        public QualityControlsController()
        {

        }
        public QualityControlsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: QualityControls
        public async Task<ActionResult> Index()
        {
            return View(await _db.QualityControls.ToListAsync());
        }

        // GET: QualityControls/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualityControl qualityControl = await _db.QualityControls.FindAsync(id);
            if (qualityControl == null)
            {
                return HttpNotFound();
            }
            return View(qualityControl);
        }

        // GET: QualityControls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QualityControls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,FirstName,LastName,Gender,MobilePhoneNumber,Email,Address,City")] QualityControl qualityControl)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await UserManager.FindByNameAsync(qualityControl.Email);
                if (existingUser != null)
                {
                    string sExistingId = existingUser.Id;
                    ViewBag.Message = "Email Already Exists: " + qualityControl.Email + "! Liaison Portal Not Created!.";
                    return View(qualityControl);
                    // await UserManager.DeleteAsync(existingUser);

                }

                ApplicationUser user = new ApplicationUser { UserName = qualityControl.Email, Email = qualityControl.Email ,FirstName=qualityControl.FirstName,LastName=qualityControl.LastName,Role="QAQC"};
                string password = qualityControl.LastName.ToLower() + "#QAQC1013"; // + liaisonId;
                var result = await UserManager.CreateAsync(user, password);





                if (result.Succeeded)
                {

                    await UserManager.AddToRoleAsync(user.Id, "QAQC");


                    qualityControl.UserId = user.Id;
                    qualityControl.CreatedOn = DateTime.Now;
                    qualityControl.CreatedBy = User.Identity.GetUserId();
                }
                    _db.QualityControls.Add(qualityControl);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(qualityControl);
        }

        // GET: QualityControls/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualityControl qualityControl = await _db.QualityControls.FindAsync(id);
            if (qualityControl == null)
            {
                return HttpNotFound();
            }
            return View(qualityControl);
        }

        // POST: QualityControls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,FirstName,LastName,Gender,MobilePhoneNumber,Email,Address,City")] QualityControl qualityControl)
        {
            if (ModelState.IsValid)
            {
                qualityControl.UpdatedBy = User.Identity.GetUserId();
                qualityControl.UpdatedOn = DateTime.Now;
                _db.Entry(qualityControl).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(qualityControl);
        }

        // GET: QualityControls/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QualityControl qualityControl = await _db.QualityControls.FindAsync(id);
            if (qualityControl == null)
            {
                return HttpNotFound();
            }
            return View(qualityControl);
        }

        // POST: QualityControls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            QualityControl qualityControl = await _db.QualityControls.FindAsync(id);
            var existingUser = await UserManager.FindByNameAsync(qualityControl.Email);
            if (existingUser != null)
            {
                //string sExistingId = existingUser.Id;
                //ViewBag.Message = "Email Already Exists: " + sExistingId + "! Liaison Portal Not Created!.";
                await UserManager.DeleteAsync(existingUser);

            }
          
            _db.QualityControls.Remove(qualityControl);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
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
