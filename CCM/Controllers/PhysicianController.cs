using CCM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Liaison, PhysiciansGroup, Admin, LiaisonGroup")]
    public class PhysicianController : BaseController
    {
        private ApplicationUserManager _userManager;

        public PhysicianController()
        {
        }

        public PhysicianController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public async Task<FileContentResult> UserPhotos(int userId)
        {
            if (1 == 1)
            {
                var physician = await _db.Physicians.FirstOrDefaultAsync(u => u.Id == userId);

                if (physician?.Photo != null && physician.Photo.Length > 0)
                    return new FileContentResult(physician.Photo, "image/jpeg");
            }

            //if there is no photo chosen then use a Stock (default) photo
            var fileName = HttpContext.Server.MapPath(@"~/dashboard/assets/img/MegaAidLogo.jpg");

            //convert imported image into byte file that can be read using FileStream and BinaryReader
            var fileInfo = new FileInfo(fileName);
            var imageSize = fileInfo.Length;
            var fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var bReader = new BinaryReader(fStream);
            var imageData = bReader.ReadBytes((int)imageSize);

            return File(imageData, "image/jpeg");
        }
        private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Index()
        {
            var group = _db.Users.Find(User.Identity.GetUserId());
            return View(User.IsInRole("PhysiciansGroup")
                ? await _db.Physicians.AsNoTracking().Where(p => p.MainPhoneNumber == group.PhoneNumber).ToListAsync()
                : await _db.Physicians.AsNoTracking().ToListAsync());
        }


        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var physician = await _db.Physicians.FindAsync(id);

            if (physician == null)
            {
                return HttpNotFound();
            }
            return View(physician);
        }


        // GET: Physicians/Create
        [Authorize(Roles = "Liaison, Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Physicians/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Exclude = "Photo")]Physician physician)
        {
        //    , decimal CPT99490Billing, decimal CPT99491Billing, decimal CPT99487Billing, decimal CPT99489Billing, decimal CPT99490Invoice, decimal CPT99491Invoice, decimal CPT99487Invoice, decimal CPT99489Invoice
            if (ModelState.IsValid)
            {
                var existingUser = await UserManager.FindByNameAsync(physician.Email);
                if (existingUser == null)
                {
                    physician.CreatedOn = DateTime.Now;
                    physician.CreatedBy = User.Identity.GetUserId();
                    var postedImageFile = Request.Files["Photo"];

                    if (postedImageFile?.ContentLength != 0 && postedImageFile?.InputStream != null)
                        using (var binary = new BinaryReader(postedImageFile.InputStream))
                        {
                            var imageData = binary.ReadBytes(postedImageFile.ContentLength);
                            if (imageData.Length > 0)
                                physician.Photo = imageData;
                        }
                    _db.Physicians.Add(physician);
                    _db.SaveChanges();
                    try
                    {


                        //Physician_CPTRates physician_CPTRatesCPT99490 = new Physician_CPTRates();
                        //physician_CPTRatesCPT99490.BillingCode = "CPT99490";
                        //physician_CPTRatesCPT99490.BillingRate = CPT99490Billing;
                        //physician_CPTRatesCPT99490.InvoiceRate = CPT99490Invoice;
                        //physician_CPTRatesCPT99490.CreatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99490.CreatedOn = DateTime.Now;
                        //physician_CPTRatesCPT99490.PhysicianId = physician.Id;
                        ////CPTpp491
                        //Physician_CPTRates physician_CPTRatesCPT99491 = new Physician_CPTRates();
                        //physician_CPTRatesCPT99491.BillingCode = "CPT99491";
                        //physician_CPTRatesCPT99491.BillingRate = CPT99491Billing;
                        //physician_CPTRatesCPT99491.InvoiceRate = CPT99491Invoice;
                        //physician_CPTRatesCPT99491.CreatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99491.CreatedOn = DateTime.Now;
                        //physician_CPTRatesCPT99491.PhysicianId = physician.Id;
                        ////CPT99487
                        //Physician_CPTRates physician_CPTRatesCPT99487 = new Physician_CPTRates();
                        //physician_CPTRatesCPT99487.BillingCode = "CPT99487";
                        //physician_CPTRatesCPT99487.BillingRate = CPT99487Billing;
                        //physician_CPTRatesCPT99487.InvoiceRate = CPT99487Invoice;
                        //physician_CPTRatesCPT99487.CreatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99487.CreatedOn = DateTime.Now;
                        //physician_CPTRatesCPT99487.PhysicianId = physician.Id;
                        ////CPT99489
                        //Physician_CPTRates physician_CPTRatesCPT99489 = new Physician_CPTRates();
                        //physician_CPTRatesCPT99489.BillingCode = "CPT99489";
                        //physician_CPTRatesCPT99489.BillingRate = CPT99489Billing;
                        //physician_CPTRatesCPT99489.InvoiceRate = CPT99489Invoice;
                        //physician_CPTRatesCPT99489.CreatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99489.CreatedOn = DateTime.Now;
                        //physician_CPTRatesCPT99489.PhysicianId = physician.Id;

                        //_db.Physician_CPTRates.Add(physician_CPTRatesCPT99490);
                        //_db.Physician_CPTRates.Add(physician_CPTRatesCPT99491);
                        //_db.Physician_CPTRates.Add(physician_CPTRatesCPT99487);
                        //_db.Physician_CPTRates.Add(physician_CPTRatesCPT99489);
                        //_db.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                    }




                    var user = new ApplicationUser { UserName = physician.Email, Email = physician.Email };
                    var password = physician.LastName.ToLower() + "#PH1013"; // + physician.Id;
                    var result = await UserManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        user.Role = "Physician";
                        user.CCMid = physician.Id;
                        user.FirstName = physician.FirstName;
                        user.LastName = physician.LastName;
                        user.PhoneNumber = physician.MobilePhoneNumber;

                        await UserManager.AddToRoleAsync(user.Id, "Physician");

                        _db.Entry(user).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                        ViewBag.Message = "Physician Portal Created.";
                        ViewBag.Username = user.Email;
                        ViewBag.Password = password;

                        return View(physician);
                    }

                    _db.Physicians.Remove(physician);
                    await _db.SaveChangesAsync();

                    ViewBag.Message = "Error: Unable To Create Physician Portal! Please, Try Again.";
                    return View(physician);
                }

                ViewBag.Message = "Email Already Exists! Physician Portal Not Created!.";
            }

            return View(physician);
        }


        // GET: Physicians/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var physician = await _db.Physicians.Include(m => m.PhysicianCPTRates).Where(m => m.Id == id).FirstOrDefaultAsync();
            try
            {


            //    ViewBag.CPT99490Billing = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99490").FirstOrDefault().BillingRate;
            //    ViewBag.CPT99491Billing = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99491").FirstOrDefault().BillingRate;
            //    ViewBag.CPT99487Billing = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99487").FirstOrDefault().BillingRate;
            //    ViewBag.CPT99489Billing = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99489").FirstOrDefault().BillingRate;
            //    ViewBag.CPT99490Invoice = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99490").FirstOrDefault().InvoiceRate;
            //    ViewBag.CPT99491Invoice = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99491").FirstOrDefault().InvoiceRate;
            //    ViewBag.CPT99487Invoice = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99487").FirstOrDefault().InvoiceRate;
            //    ViewBag.CPT99489Invoice = _db.Physician_CPTRates.Where(x => x.PhysicianId == id && x.BillingCode == "CPT99489").FirstOrDefault().InvoiceRate;
            }
            catch (Exception ex)
            {


            }
            if (physician == null)
            {
                return HttpNotFound();
            }

            return View(physician);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Exclude = "Photo")]Physician physician)
        {
            //, decimal CPT99490Billing, decimal CPT99491Billing, decimal CPT99487Billing, decimal CPT99489Billing, decimal CPT99490Invoice, decimal CPT99491Invoice, decimal CPT99487Invoice, decimal CPT99489Invoice
            if (ModelState.IsValid)
            {
                HttpPostedFileBase userPhoto = Request.Files["Photo"];

                if (userPhoto?.ContentLength != 0 && userPhoto?.InputStream != null)
                    using (var binary = new BinaryReader(userPhoto.InputStream))
                    {
                        var imageData = binary.ReadBytes(userPhoto.ContentLength);
                        physician.Photo = imageData;
                    }

                else
                {
                    var caller = await _db.Physicians.AsNoTracking().FirstOrDefaultAsync(l => l.Id == physician.Id);
                    if (caller.Photo != null)
                        physician.Photo = caller.Photo;
                }
                _db.Entry(physician).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                var physiciancptrateexists = _db.Physician_CPTRates.Where(x => x.PhysicianId == physician.Id).FirstOrDefault();
                if (physiciancptrateexists != null)
                {
                    try
                    {

                        ////CPT99490
                        //var physician_CPTRatesCPT99490 = _db.Physician_CPTRates.Where(x => x.PhysicianId == physician.Id && x.BillingCode == "CPT99490").FirstOrDefault();

                        //physician_CPTRatesCPT99490.BillingRate = CPT99490Billing;
                        //physician_CPTRatesCPT99490.InvoiceRate = CPT99490Invoice;
                        //physician_CPTRatesCPT99490.UpdatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99490.UpdatedOn = DateTime.Now;
                        ////CPT99491
                        //var physician_CPTRatesCPT99491 = _db.Physician_CPTRates.Where(x => x.PhysicianId == physician.Id && x.BillingCode == "CPT99491").FirstOrDefault();

                        //physician_CPTRatesCPT99491.BillingRate = CPT99491Billing;
                        //physician_CPTRatesCPT99491.InvoiceRate = CPT99491Invoice;
                        //physician_CPTRatesCPT99491.UpdatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99491.UpdatedOn = DateTime.Now;
                        ////CPT99487
                        //var physician_CPTRatesCPT99487 = _db.Physician_CPTRates.Where(x => x.PhysicianId == physician.Id && x.BillingCode == "CPT99487").FirstOrDefault();

                        //physician_CPTRatesCPT99487.BillingRate = CPT99487Billing;
                        //physician_CPTRatesCPT99487.InvoiceRate = CPT99487Invoice;
                        //physician_CPTRatesCPT99487.UpdatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99487.UpdatedOn = DateTime.Now;

                        ////CPT99489
                        //var physician_CPTRatesCPT99489 = _db.Physician_CPTRates.Where(x => x.PhysicianId == physician.Id && x.BillingCode == "CPT99489").FirstOrDefault();

                        //physician_CPTRatesCPT99489.BillingRate = CPT99489Billing;
                        //physician_CPTRatesCPT99489.InvoiceRate = CPT99489Invoice;
                        //physician_CPTRatesCPT99489.UpdatedBy = User.Identity.GetUserId();
                        //physician_CPTRatesCPT99489.UpdatedOn = DateTime.Now;


                        //_db.Entry(physician_CPTRatesCPT99490).State = EntityState.Modified;
                        //_db.Entry(physician_CPTRatesCPT99491).State = EntityState.Modified;
                        //_db.Entry(physician_CPTRatesCPT99487).State = EntityState.Modified;
                        //_db.Entry(physician_CPTRatesCPT99489).State = EntityState.Modified;
                        //_db.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                    }
                }
                else
                {
                    try
                    {


                    //    Physician_CPTRates physician_CPTRatesCPT99490 = new Physician_CPTRates();
                    //    physician_CPTRatesCPT99490.BillingCode = "CPT99490";
                    //    physician_CPTRatesCPT99490.BillingRate = CPT99490Billing;
                    //    physician_CPTRatesCPT99490.InvoiceRate = CPT99490Invoice;
                    //    physician_CPTRatesCPT99490.CreatedBy = User.Identity.GetUserId();
                    //    physician_CPTRatesCPT99490.CreatedOn = DateTime.Now;
                    //    physician_CPTRatesCPT99490.PhysicianId = physician.Id;
                    //    //CPT99491
                    //    Physician_CPTRates physician_CPTRatesCPT99491 = new Physician_CPTRates();
                    //    physician_CPTRatesCPT99491.BillingCode = "CPT99491";
                    //    physician_CPTRatesCPT99491.BillingRate = CPT99491Billing;
                    //    physician_CPTRatesCPT99491.InvoiceRate = CPT99491Invoice;
                    //    physician_CPTRatesCPT99491.CreatedBy = User.Identity.GetUserId();
                    //    physician_CPTRatesCPT99491.CreatedOn = DateTime.Now;
                    //    physician_CPTRatesCPT99491.PhysicianId = physician.Id;
                    //    //CPT99487
                    //    Physician_CPTRates physician_CPTRatesCPT99487 = new Physician_CPTRates();
                    //    physician_CPTRatesCPT99487.BillingCode = "CPT99487";
                    //    physician_CPTRatesCPT99487.BillingRate = CPT99487Billing;
                    //    physician_CPTRatesCPT99487.InvoiceRate = CPT99487Invoice;
                    //    physician_CPTRatesCPT99487.CreatedBy = User.Identity.GetUserId();
                    //    physician_CPTRatesCPT99487.CreatedOn = DateTime.Now;
                    //    physician_CPTRatesCPT99487.PhysicianId = physician.Id;
                    //    //CPT99489
                    //    Physician_CPTRates physician_CPTRatesCPT99489 = new Physician_CPTRates();
                    //    physician_CPTRatesCPT99489.BillingCode = "CPT99489";
                    //    physician_CPTRatesCPT99489.BillingRate = CPT99489Billing;
                    //    physician_CPTRatesCPT99489.InvoiceRate = CPT99489Invoice;
                    //    physician_CPTRatesCPT99489.CreatedBy = User.Identity.GetUserId();
                    //    physician_CPTRatesCPT99489.CreatedOn = DateTime.Now;
                    //    physician_CPTRatesCPT99489.PhysicianId = physician.Id;

                    //    _db.Physician_CPTRates.Add(physician_CPTRatesCPT99490);
                    //    _db.Physician_CPTRates.Add(physician_CPTRatesCPT99491);
                    //    _db.Physician_CPTRates.Add(physician_CPTRatesCPT99487);
                    //    _db.Physician_CPTRates.Add(physician_CPTRatesCPT99489);
                    //    _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                    }
                }

                return RedirectToAction("Index");
            }

            return View(physician);
        }


        // GET: Physicians/Delete/5
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Liaison, Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Physician physician = _db.Physicians.Find(id);
            if (physician == null)
            {
                return HttpNotFound();
            }
            return View(physician);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Liaison, Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Physician physician = _db.Physicians.Find(id);
            _db.Physicians.Remove(physician);
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