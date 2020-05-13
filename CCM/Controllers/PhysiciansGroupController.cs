using System.Net;
using System.Web;
using CCM.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;


namespace CCM.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Liaison, Admin, PhysiciansGroup, LiaisonGroup")]
    public class PhysiciansGroupController : BaseController
    {
       // private readonly ApplicationdbContect _db = new ApplicationdbContect();

        private ApplicationUserManager _userManager;

        public PhysiciansGroupController()
        {
        }

        public PhysiciansGroupController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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



        // GET: PhysiciansGroup
        public async Task<ActionResult> Index()
        {
            return View(await _db.PhysiciansGroup.ToListAsync());
        }


        // GET: PhysiciansGroup/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysiciansGroup physiciansGroup = await _db.PhysiciansGroup.FindAsync(id);
            if (physiciansGroup == null)
            {
                return HttpNotFound();
            }
            return View(physiciansGroup);
        }


        // GET: PhysiciansGroup/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PhysiciansGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PhysiciansGroup physiciansGroup)
        {
            if (ModelState.IsValid)
            {
                var existingUser  = await UserManager.FindByNameAsync(physiciansGroup.Email);
                if (existingUser == null)
                {
                    _db.PhysiciansGroup.Add(physiciansGroup);
                    await _db.SaveChangesAsync();


                    var user = new ApplicationUser
                    {
                        UserName = physiciansGroup.Email,
                        Email    = physiciansGroup.Email
                    };
                    var password = "npi" + physiciansGroup.NPI + "#PG1013"; // + physiciansGroup.Id;
                    var result   = await UserManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        user.Role        = "PhysiciansGroup";
                        user.CCMid       = physiciansGroup.Id;
                        user.FirstName   = physiciansGroup.GroupName;
                        user.LastName    = physiciansGroup.NPI.ToString();
                        user.PhoneNumber = physiciansGroup.MainPhoneNumber;

                        await UserManager.AddToRoleAsync(user.Id, "PhysiciansGroup");
                        await _db.SaveChangesAsync();

                        ViewBag.Message  = "Physician Group Portal Created.";
                        ViewBag.Username = user.Email;
                        ViewBag.Password = password;

                        return View(physiciansGroup);
                    }

                    _db.PhysiciansGroup.Remove(physiciansGroup);
                    await _db.SaveChangesAsync();

                    ViewBag.Message = "Error: " + result.Errors.FirstOrDefault();
                    return View(physiciansGroup);
                }

                ViewBag.Message = "Email Already Exists! Physician Group Portal Not Created!.";
            }

            return View(physiciansGroup);
        }


        // GET: PhysiciansGroup/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysiciansGroup physiciansGroup = await _db.PhysiciansGroup.FindAsync(id);
            if (physiciansGroup == null)
            {
                return HttpNotFound();
            }
            return View(physiciansGroup);
        }

        // POST: PhysiciansGroup/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PhysiciansGroup physiciansGroup)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(physiciansGroup).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(physiciansGroup);
        }


        // GET: PhysiciansGroup/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysiciansGroup physiciansGroup = await _db.PhysiciansGroup.FindAsync(id);
            if (physiciansGroup == null)
            {
                return HttpNotFound();
            }
            return View(physiciansGroup);
        }

        // POST: PhysiciansGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PhysiciansGroup physiciansGroup = await _db.PhysiciansGroup.FindAsync(id);
            _db.PhysiciansGroup.Remove(physiciansGroup);
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