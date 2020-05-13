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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace CCM.Controllers
{
    [Authorize(Roles = "Liaison, PhysiciansGroup, Admin, LiaisonGroup")]
    public class PhysicianGroupPhysicianMappingController : BaseController
    {
       // private ApplicationdbContect _db = new ApplicationdbContect();
        private ApplicationUserManager _userManager;
        public PhysicianGroupPhysicianMappingController()
        {

        }
        public async Task<JsonResult> SaveDoctorClinicMapping(int ClinicID, List<int> DoctorID)
        {
            var results = _db.physicianGroup_Physician_Mappings.Where(x => x.PhysiciansGroupId == ClinicID).ToList();
            _db.physicianGroup_Physician_Mappings.RemoveRange(results);
            _db.SaveChanges();
            foreach(var item in DoctorID)
            {
                PhysicianGroup_Physician_Mapping physicianGroup_Physician_Mapping = new PhysicianGroup_Physician_Mapping();
                physicianGroup_Physician_Mapping.PhysicianId = item;
                physicianGroup_Physician_Mapping.PhysiciansGroupId = ClinicID;
                physicianGroup_Physician_Mapping.CreatedBy = User.Identity.GetUserId();
                physicianGroup_Physician_Mapping.CreatedOn = DateTime.Now;
                _db.physicianGroup_Physician_Mappings.Add(physicianGroup_Physician_Mapping);

            }
            await _db.SaveChangesAsync();
            return Json(true);
        }
            public PhysicianGroupPhysicianMappingController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            var physicianGroup_Physician_Mappings = _db.physicianGroup_Physician_Mappings.Include(p => p.Physician).Include(p => p.PhysiciansGroup);
            return View(await physicianGroup_Physician_Mappings.ToListAsync());
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
            var billingcodes = _db.BillingCodes.ToList();
            ViewBag.codeList = billingcodes;


            var physicianmapping = _db.physicianGroup_Physician_Mappings.Select(p => p.PhysicianId).ToList().Distinct();
            var physicians = _db.Physicians.Where(p => !physicianmapping.Contains(p.Id)).Select(p => p).Include(p=>p.PhysicianCPTRates).ToList();
           
            

            ViewBag.PhysicianId = physicians;
            var physiciansrates = _db.Physician_CPTRates.ToList();
            var physiciangroupmapping = _db.physicianGroup_Physician_Mappings.Select(p => p.PhysiciansGroupId).ToList().Distinct();
            ViewBag.PhysiciansGroupId = _db.PhysiciansGroup.Where(p => !physiciangroupmapping.Contains(p.Id)).Select(p => p).ToList();
            return View();
        }

        // POST: PhysicianGroupPhysicianMapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,PhysicianId,PhysiciansGroupId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] PhysicianGroup_Physician_Mapping physicianGroup_Physician_Mapping)
        {
            if (ModelState.IsValid)
            {
                _db.physicianGroup_Physician_Mappings.Add(physicianGroup_Physician_Mapping);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var physicians = _db.Physicians.ToList().Select(x => new { Id = x.Id, Name = x.FirstName + " " + x.LastName }).ToList();

            ViewBag.PhysicianId = new SelectList(physicians, "Id", "Name");
            ViewBag.PhysiciansGroupId = new SelectList(_db.PhysiciansGroup, "Id", "GroupName");
         
            return View(physicianGroup_Physician_Mapping);
        }

        // GET: PhysicianGroupPhysicianMapping/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var billingcodes = _db.BillingCodes.ToList();
            ViewBag.codeList = billingcodes;
            var physicians = (from phy in _db.Physicians
                             where !(from c in _db.physicianGroup_Physician_Mappings where c.PhysiciansGroupId!=id select c.PhysicianId).Contains(phy.Id)
                             select phy).ToList();

            ViewBag.physiciansgroupmapped = _db.physicianGroup_Physician_Mappings.Include(p => p.Physician).Where(x => x.PhysiciansGroupId == id).Select(x => x.Physician).ToList();

            ViewBag.PhysicianId = physicians;
            ViewBag.PhysiciansGroupId = _db.PhysiciansGroup.Where(x=>x.Id==id).ToList();
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

        // GET: PhysicianGroupPhysicianMap 
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //  List<PhysicianGroup_Physician_Mapping> physicianGroup_Physician_Mapping =  _db.physicianGroup_Physician_Mappings.Where(x=>x.PhysiciansGroupId==id).ToList();
        //    if (physicianGroup_Physician_Mapping == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(physicianGroup_Physician_Mapping);
        //}

        // POST: PhysicianGroupPhysicianMapping/Delete/5

        
        public async Task<JsonResult> Delete(int id)
        {
            var results = _db.physicianGroup_Physician_Mappings.Where(x => x.PhysiciansGroupId == id).ToList();
            _db.physicianGroup_Physician_Mappings.RemoveRange(results);
            await _db.SaveChangesAsync();
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }
        public async Task<string> UpdatePhysicianCptCodes(string[] CptCodes)
        {

            string response = "true";

            var billingvalue1 = CptCodes[0].Split(',', '-');
           
            int physicianId = Convert.ToInt32(billingvalue1[2]);
            try
            {
                var exist = _db.Physician_CPTRates.Where(x => x.PhysicianId == physicianId).FirstOrDefault();

                if (exist == null)
                {

                    for (int i = 0; i <= CptCodes.Length - 1; i = i + 2)
                    {
                        Physician_CPTRates physicianrate = new Physician_CPTRates();

                        var values = CptCodes[i].Split(',', '-');
                        string payrate = values[0];
                        string billingid = values[1];
                        string physicianid = values[2];
                        decimal? billingrate = null;

                        if (payrate != "")
                        {
                            billingrate = Convert.ToInt32(payrate);
                        }
                        physicianrate.CreatedOn = DateTime.Now;
                        physicianrate.CreatedBy = User.Identity.GetUserId();
                        physicianrate.BillingCodeId = Convert.ToInt32(billingid);

                        physicianrate.BillingRate = billingrate;
                        physicianrate.PhysicianId = Convert.ToInt32(physicianid);

                        values = CptCodes[i + 1].Split(',', '-');
                        string invoicepayrate = values[0];
                        decimal? invoicerate = null;
                        if (invoicepayrate != "") {

                            invoicerate = Convert.ToInt32(invoicepayrate);
                        }
                        physicianrate.InvoiceRate = invoicerate;
                        _db.Physician_CPTRates.Add(physicianrate);
                        var save = _db.SaveChanges();
                        if (save == 0)
                            response = "false";




                    }
                }
                else
                {
                    for (int i = 0; i <= CptCodes.Length - 1; i = i + 2)
                    {


                        var values = CptCodes[i].Split(',', '-');
                        string payrate = values[0];
                        int? billingid = Convert.ToInt32(values[1]);
                        int? physicianid = Convert.ToInt32(values[2]);
                        decimal? billingrate = null;
                        string invoicepayrate = "";
                        decimal? invoicerate = null;

                        var save = 0;
                        var PhysicianCPT = _db.Physician_CPTRates.Where(p => p.BillingCodeId == billingid && p.PhysicianId == physicianid).FirstOrDefault();

                        if (PhysicianCPT == null)
                        {

                            Physician_CPTRates physicianrate = new Physician_CPTRates();



                            physicianrate.CreatedOn = DateTime.Now;
                            physicianrate.CreatedBy = User.Identity.GetUserId();
                            physicianrate.BillingCodeId = Convert.ToInt32(billingid);
                            if (payrate != "")
                            {
                                billingrate = Convert.ToInt32(payrate);
                            }
                            physicianrate.BillingRate = billingrate;
                            physicianrate.PhysicianId = Convert.ToInt32(physicianid);

                            values = CptCodes[i + 1].Split(',', '-');
                            invoicepayrate = values[0];
                            if (invoicepayrate != "")
                            {

                                invoicerate = Convert.ToInt32(invoicepayrate);
                            }
                            physicianrate.InvoiceRate = invoicerate;
                            _db.Physician_CPTRates.Add(physicianrate);
                            save = _db.SaveChanges();
                            if (save == 0)
                                response = "false";





                        }
                        else
                        {
                            PhysicianCPT.CreatedOn = DateTime.Now;
                            PhysicianCPT.CreatedBy = User.Identity.GetUserId();
                            PhysicianCPT.BillingCodeId = Convert.ToInt32(billingid);
                            if (payrate != "")
                            {
                                billingrate = Convert.ToInt32(payrate);
                            }
                            PhysicianCPT.BillingRate = billingrate;
                            PhysicianCPT.PhysicianId = Convert.ToInt32(physicianid);

                            values = CptCodes[i + 1].Split(',', '-');
                            invoicepayrate = values[0];
                            if (invoicepayrate != "")
                            {

                                invoicerate = Convert.ToInt32(invoicepayrate);
                            }
                            PhysicianCPT.InvoiceRate = invoicerate;
                            _db.Entry(PhysicianCPT).State = EntityState.Modified;
                            save = _db.SaveChanges();
                            if (save == 0)
                                response = "false";



                        }
                    }
                }

                return response;
            }
            catch(Exception ex)
            {
                return response;
            }

           
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


    public class CptInviceRatesViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }
}
