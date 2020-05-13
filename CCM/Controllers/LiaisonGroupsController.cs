using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CCM.Models;
using Microsoft.AspNet.Identity.Owin;

namespace CCM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LiaisonGroupsController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();

        private ApplicationUserManager _userManager;

        public LiaisonGroupsController()
        {
        }

        public LiaisonGroupsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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


        // GET: LiaisonGroups
        public ActionResult Index()
        {
            return View(_db.liaisonGroups.ToList());
        }

        // GET: LiaisonGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiaisonGroup liaisonGroup = _db.liaisonGroups.Find(id);
            if (liaisonGroup == null)
            {
                return HttpNotFound();
            }
            return View(liaisonGroup);
        }

        // GET: LiaisonGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LiaisonGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "GroupName,MainPhoneNumber,Email")] LiaisonGroup liaisonGroup)
        {
            if (ModelState.IsValid)
            {
                var existingUser =await  UserManager.FindByNameAsync(liaisonGroup.Email);
                if (existingUser == null)
                {
                    _db.liaisonGroups.Add(liaisonGroup);
                   await  _db.SaveChangesAsync();


                    var user = new ApplicationUser
                    {
                        UserName = liaisonGroup.Email,
                        Email = liaisonGroup.Email
                    };
                    var password = "lgm" + liaisonGroup.MainPhoneNumber + "#LG1013"; // + physiciansGroup.Id;
                    var result = await UserManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        user.Role = "LiaisonGroup";
                        user.CCMid = liaisonGroup.Id;
                        user.FirstName = liaisonGroup.GroupName;
                        user.LastName = "";
                        user.PhoneNumber = liaisonGroup.MainPhoneNumber;

                        await UserManager.AddToRoleAsync(user.Id, "LiaisonGroup");
                        await _db.SaveChangesAsync();

                        ViewBag.Message = "Liaison Group Portal Created.";
                        ViewBag.Username = user.Email;
                        ViewBag.Password = password;
                        return RedirectToAction("Index");
                       // return View(liaisonGroup);
                    }

                    _db.liaisonGroups.Remove(liaisonGroup);
                    await _db.SaveChangesAsync();

                    ViewBag.Message = "Error: " + result.Errors.FirstOrDefault();
                    return View(liaisonGroup);
                }

                ViewBag.Message = "Email Already Exists! Liaison Manager Not Created!.";
            }

            return View(liaisonGroup);
        }

        // GET: LiaisonGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiaisonGroup liaisonGroup = _db.liaisonGroups.Find(id);
            if (liaisonGroup == null)
            {
                return HttpNotFound();
            }
            return View(liaisonGroup);
        }

        // POST: LiaisonGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GroupName,MainPhoneNumber,Email")] LiaisonGroup liaisonGroup)
        {
            if (ModelState.IsValid)
            {
                
                _db.Entry(liaisonGroup).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liaisonGroup);
        }

        // GET: LiaisonGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiaisonGroup liaisonGroup = _db.liaisonGroups.Find(id);
            if (liaisonGroup == null)
            {
                return HttpNotFound();
            }
            return View(liaisonGroup);
        }

        // POST: LiaisonGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LiaisonGroup liaisonGroup = _db.liaisonGroups.Find(id);
            _db.liaisonGroups.Remove(liaisonGroup);
            _db.SaveChanges();
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
