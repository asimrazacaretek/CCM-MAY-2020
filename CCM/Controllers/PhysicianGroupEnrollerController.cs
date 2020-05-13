using CCM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{

    [Authorize(Roles = "Liaison, PhysiciansGroup, Admin, LiaisonGroup")]
    public class PhysicianGroupEnrollerController : BaseController
    {
        //private ApplicationdbContect _db = new ApplicationdbContect();
        private ApplicationUserManager _userManager;
        public PhysicianGroupEnrollerController()
        {

        }
        public async Task<JsonResult> SaveSaleStaffMapping(List<int> PhyGrpID, int SalesStaffIDs)
        {
            var results = _db.physicianGroup_SalesStaff_Mappings.Where(x => x.SaleStaffId == SalesStaffIDs).ToList();
            _db.physicianGroup_SalesStaff_Mappings.RemoveRange(results);
            _db.SaveChanges();
            foreach (var item in PhyGrpID)
            {
                PhysicianGroup_SalesStaff_Mapping physicianGroup_SalesStaff_Mapping = new PhysicianGroup_SalesStaff_Mapping();
                physicianGroup_SalesStaff_Mapping.PhysiciansGroupId = item;

                physicianGroup_SalesStaff_Mapping.SaleStaffId = SalesStaffIDs;
                physicianGroup_SalesStaff_Mapping.CreatedBy = User.Identity.GetUserId();
                physicianGroup_SalesStaff_Mapping.CreatedOn = DateTime.Now;
                _db.physicianGroup_SalesStaff_Mappings.Add(physicianGroup_SalesStaff_Mapping);

            }
            await _db.SaveChangesAsync();
            return Json(true);
        }
        public PhysicianGroupEnrollerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: PhysicianGroupPhysicianMapping
        public async Task<ActionResult> Index()
        {
            var physicianGroup_SalesStaff_Mappings = _db.physicianGroup_SalesStaff_Mappings.Include(p => p.SaleStaff).Include(p => p.PhysiciansGroup);
           
            return View(await physicianGroup_SalesStaff_Mappings.ToListAsync());
        }

        // GET: PhysicianGroupPhysicianMapping/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysicianGroup_Physician_Mapping physicianGroup_Physician_Mapping = await _db.physicianGroup_Physician_Mappings.FindAsync(id);
            if (physicianGroup_Physician_Mapping == null)
            {
                return HttpNotFound();
            }
            return View(physicianGroup_Physician_Mapping);
        }

        // GET: PhysicianGroupPhysicianMapping/Create
        public ActionResult Create()
        {
            var saleStaffs = _db.saleStaffs.AsNoTracking().ToList();
            try
            {
                foreach (var item in saleStaffs)
                {
                    item.FirstName = item.FirstName + " " + item.LastName;
                }
            }
            catch (Exception ex)
            {


            }
            ViewBag.SalesStaff = saleStaffs.ToList().OrderBy(y=>y.FirstName).ToList();
           
            ViewBag.PhysiciansGroupId = _db.PhysiciansGroup.ToList();
            return View();
        }

        // POST: PhysicianGroupPhysicianMapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        // GET: PhysicianGroupPhysicianMapping/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var saleStaffs = _db.saleStaffs.AsNoTracking().Where(x => x.Id == id).ToList();
            ViewBag.physiciansgroupmapped = _db.physicianGroup_SalesStaff_Mappings.Include(p => p.SaleStaff).Where(x => x.SaleStaffId == id).Select(x => x.PhysiciansGroup).ToList();
            try
            {


                foreach (var item in saleStaffs)
                {
                    item.FirstName = item.FirstName + " " + item.LastName;
                }
            }
            catch (Exception ex)
            {


            }
            ViewBag.SalesStaffId = saleStaffs;
            ViewBag.PhysiciansGroupId = _db.PhysiciansGroup.AsNoTracking().ToList();
            return View();
        }

        // POST: PhysicianGroupPhysicianMapping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,PhysicianId,PhysiciansGroupId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] PhysicianGroup_Physician_Mapping physicianGroup_Physician_Mapping)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(physicianGroup_Physician_Mapping).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PhysicianId = new SelectList(_db.Physicians, "Id", "ContactId", physicianGroup_Physician_Mapping.PhysicianId);
            ViewBag.PhysiciansGroupId = new SelectList(_db.PhysiciansGroup, "Id", "GroupName", physicianGroup_Physician_Mapping.PhysiciansGroupId);
            return View(physicianGroup_Physician_Mapping);
        }

        // GET: PhysicianGroupPhysicianMapping/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysicianGroup_Physician_Mapping physicianGroup_Physician_Mapping = await _db.physicianGroup_Physician_Mappings.FindAsync(id);
            if (physicianGroup_Physician_Mapping == null)
            {
                return HttpNotFound();
            }
            return View(physicianGroup_Physician_Mapping);
        }

        // POST: PhysicianGroupPhysicianMapping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var results = _db.physicianGroup_SalesStaff_Mappings.Where(x => x.PhysiciansGroupId == id).ToList();
            _db.physicianGroup_SalesStaff_Mappings.RemoveRange(results);
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