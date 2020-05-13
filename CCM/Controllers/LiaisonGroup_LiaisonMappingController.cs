using CCM.Models;
using CCM.Models.CCMBILLINGS;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class LiaisonGroupLiaisonMappingController : BaseController
    {
        //private ApplicationdbContect _db = new ApplicationdbContect();
        private ApplicationUserManager _userManager;
        public LiaisonGroupLiaisonMappingController()
        {

        }
        public async Task<JsonResult> SaveDoctorClinicMapping(int ClinicID, List<int> DoctorID)
        {
            var results = _db.LiaisonGroup_Liaison_Mappings.Where(x => x.LiaisonGroupId == ClinicID).ToList();
            _db.LiaisonGroup_Liaison_Mappings.RemoveRange(results);
            _db.SaveChanges();
            foreach (var item in DoctorID)
            {
                LiaisonGroup_Liaison_Mapping physicianGroup_Physician_Mapping = new LiaisonGroup_Liaison_Mapping();
                physicianGroup_Physician_Mapping.LiaisonId = item;
                physicianGroup_Physician_Mapping.LiaisonGroupId = ClinicID;
                physicianGroup_Physician_Mapping.CreatedBy = User.Identity.GetUserId();
                physicianGroup_Physician_Mapping.CreatedOn = DateTime.Now;
                _db.LiaisonGroup_Liaison_Mappings.Add(physicianGroup_Physician_Mapping);

            }
            await _db.SaveChangesAsync();
            return Json(true);
        }
        public LiaisonGroupLiaisonMappingController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: LiaisonGroup_LiaisonMapping

        public async Task<ActionResult> Index()
        {
            var LiaisonGroup_LiaisonMapping = _db.LiaisonGroup_Liaison_Mappings.Include(p => p.Liaison).Include(p => p.LiaisonGroup);
            return View(await LiaisonGroup_LiaisonMapping.ToListAsync());
        }

        // GET: LiaisonGroup_LiaisonMapping/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiaisonGroup_Liaison_Mapping LiaisonGroup_LiaisonMapping = await _db.LiaisonGroup_Liaison_Mappings.FindAsync(id);
            if (LiaisonGroup_LiaisonMapping == null)
            {
                return HttpNotFound();
            }
            return View(LiaisonGroup_LiaisonMapping);
        }

        // GET: LiaisonGroup_LiaisonMapping/Create
        public ActionResult Create()
        {
            var billingcodes = _db.BillingCodes.ToList();

            ViewBag.codeList = billingcodes;




            var laisonmapping = _db.LiaisonGroup_Liaison_Mappings.Select(p => p.LiaisonId).ToList().Distinct();
            var laisons = _db.Liaisons.Where(p => !laisonmapping.Contains(p.Id)).Include(p => p.LiaisonCPTRates).Select(p => p).ToList();



            //var physicians = _db.Liaisons.Where(x => x.Id != _db.LiaisonGroup_Liaison_Mappings.FirstOrDefault(y => y.LiaisonId == x.Id).LiaisonId).ToList();
            ViewBag.PhysicianId = laisons;
            var laisongroupmapping = _db.LiaisonGroup_Liaison_Mappings.Select(p => p.LiaisonGroupId).ToList().Distinct();
            var PhysiciansGroupId = _db.liaisonGroups.Where(p => !laisongroupmapping.Contains(p.Id)).Select(p => p).OrderBy(y => y.GroupName).ToList();

            ViewBag.PhysiciansGroupId = PhysiciansGroupId;

            return View();
        }

        // POST: LiaisonGroup_LiaisonMapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,LiaisonId,LiaisonGroupId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LiaisonGroup_Liaison_Mapping LiaisonGroup_Liaison_Mapping)
        {
            if (ModelState.IsValid)
            {
                _db.LiaisonGroup_Liaison_Mappings.Add(LiaisonGroup_Liaison_Mapping);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var physicians = _db.Liaisons.ToList().Select(x => new { Id = x.Id, Name = x.FirstName + " " + x.LastName }).ToList();

            ViewBag.PhysicianId = new SelectList(physicians, "Id", "Name").OrderBy(p => p.Text);
            ViewBag.PhysiciansGroupId = new SelectList(_db.liaisonGroups, "Id", "GroupName");

            return View(LiaisonGroup_Liaison_Mapping);
        }

        // GET: LiaisonGroup_LiaisonMapping/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var billingcodes = _db.BillingCodes.ToList();
            ViewBag.codeList = billingcodes;
            var laisonmapping = _db.LiaisonGroup_Liaison_Mappings.Select(p => p.LiaisonId).ToList().Distinct();
            var laisons = _db.Liaisons.Where(p => !laisonmapping.Contains(p.Id)).Include(p => p.LiaisonCPTRates).Select(p => p).ToList();
            var laisonsexist = _db.LiaisonGroup_Liaison_Mappings.Include(p => p.Liaison).Where(x => x.LiaisonGroupId == id).Select(x => x.Liaison).ToList();
            var alllaisons = new List<Liaison>(laisonsexist);

            alllaisons.AddRange(laisons);
            //var physicians = _db.Liaisons.Where(x => x.Id != _db.LiaisonGroup_Liaison_Mappings.FirstOrDefault(y => y.LiaisonId == x.Id).LiaisonId).ToList();
            ViewBag.PhysicianId = alllaisons;
            //var physicians = _db.Liaisons.ToList();
            ViewBag.physiciansgroupmapped = laisonsexist;


            ViewBag.PhysiciansGroupId = _db.liaisonGroups.Where(x => x.Id == id).ToList();
            return View();
        }

        // POST: LiaisonGroup_LiaisonMapping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LiaisonId,LiaisonGroupId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LiaisonGroup_Liaison_Mapping LiaisonGroup_Liaison_Mapping)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(LiaisonGroup_Liaison_Mapping).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PhysicianId = new SelectList(_db.Physicians, "Id", "ContactId", LiaisonGroup_Liaison_Mapping.LiaisonId);
            ViewBag.PhysiciansGroupId = new SelectList(_db.PhysiciansGroup, "Id", "GroupName", LiaisonGroup_Liaison_Mapping.LiaisonGroupId);
            return View(LiaisonGroup_Liaison_Mapping);
        }

        // GET: LiaisonGroup_LiaisonMapping/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiaisonGroup_Liaison_Mapping LiaisonGroup_Liaison_Mapping = await _db.LiaisonGroup_Liaison_Mappings.FindAsync(id);
            if (LiaisonGroup_Liaison_Mapping == null)
            {
                return HttpNotFound();
            }
            return View(LiaisonGroup_Liaison_Mapping);
        }

        // POST: LiaisonGroup_LiaisonMapping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var results = _db.LiaisonGroup_Liaison_Mappings.Where(x => x.LiaisonGroupId == id).ToList();
            _db.LiaisonGroup_Liaison_Mappings.RemoveRange(results);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<string> _DeleteGroup(int id)
        {
            var results = _db.LiaisonGroup_Liaison_Mappings.Where(x => x.LiaisonGroupId == id).ToList();
            if (results != null)
            {
                _db.LiaisonGroup_Liaison_Mappings.RemoveRange(results);
                await _db.SaveChangesAsync();
                return "True";
            }
            return "False";
        }
        public async Task<string> UpdateLiasinCptCodes(string[] CptCodes)
        {
            var billingvalue1 = CptCodes[0].Split(',', '-');
            string laisonid1 = billingvalue1[2];
            int lisasonId1 = Convert.ToInt32(laisonid1);

            try
            {

                var exist = _db.Liaison_CPTRates.Where(x => x.LiaisonId == lisasonId1).FirstOrDefault();
                if (exist == null)
                {
                    foreach (var item in CptCodes)
                    {
                        Liaison_CPTRates laisonrates = new Liaison_CPTRates();
                        var billingvalue = item.Split(',', '-');
                        string payrate = billingvalue[0];
                        decimal? salary = null;
                        string billingId = billingvalue[1];

                        string laisonid = billingvalue[2];
                        var timenow = DateTime.Now;
                        int lisasonId = Convert.ToInt32(laisonid);
                        if (payrate != "")
                        {
                            salary = Convert.ToDecimal(payrate);
                        }
                        int? code = Convert.ToInt32(billingId);


                        laisonrates.CreatedOn = DateTime.Now;
                        laisonrates.CreatedBy = User.Identity.GetUserId();
                        laisonrates.UpdatedOn = null;
                        laisonrates.UpdatedBy = "";
                        laisonrates.LiaisonId = lisasonId;
                        laisonrates.BillingCode = "";
                        laisonrates.SalaryRate = salary;
                        laisonrates.BillingCodeId = code;



                        _db.Liaison_CPTRates.Add(laisonrates);
                        _db.SaveChanges();
                    }

                    return "Saved";

                }
                else
                {
                    foreach (var item in CptCodes)
                    {

                        var billingvalue = item.Split(',', '-');
                        string payrate = billingvalue[0];
                        string billingId = billingvalue[1];
                        decimal? salary = null;
                        string laisonid = billingvalue[2];
                        var timenow = DateTime.Now;
                        int lisasonId = Convert.ToInt32(laisonid);
                        if (payrate != "")
                        {
                            salary = Convert.ToDecimal(payrate);
                        }
                        int? code = Convert.ToInt32(billingId);
                        var liaisonCPT = _db.Liaison_CPTRates.Where(p => p.BillingCodeId == code && p.LiaisonId == lisasonId).FirstOrDefault();
                        if (liaisonCPT == null)
                        {

                            Liaison_CPTRates laisonrates = new Liaison_CPTRates();



                            laisonrates.CreatedOn = DateTime.Now;
                            laisonrates.CreatedBy = User.Identity.GetUserId();
                            laisonrates.UpdatedOn = null;
                            laisonrates.UpdatedBy = "";
                            laisonrates.LiaisonId = lisasonId;
                            laisonrates.BillingCode = "";
                            laisonrates.SalaryRate = salary;
                            laisonrates.BillingCodeId = code;

                            _db.Liaison_CPTRates.Add(laisonrates);
                            _db.SaveChanges();

                        }
                        else
                        {



                            liaisonCPT.UpdatedOn = DateTime.Now;
                            liaisonCPT.UpdatedBy = User.Identity.GetUserId();

                            liaisonCPT.BillingCode = "";
                            liaisonCPT.SalaryRate = salary;
                            liaisonCPT.BillingCodeId = code;


                            _db.Entry(liaisonCPT).State = EntityState.Modified;
                            _db.SaveChanges();

                        }


                    }

                    return "Updated";
                }
            }
            catch (Exception ex)
            {
                return "Error";
            }







            return "";
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

    public class CptCodesViewModel
    {
        public int Id { get; set; }
        public int LiasonId { get; set; }
        public string CodeName { get; set; }
        public string InvoiceCodeName { get; internal set; }

    }
    public class BillingCodesViewModel
    {
        public int Id { get; set; }

        public string CodeName { get; set; }



    }
    public class BillingCodesViewModel2
    {
        public List<BillingCodes> BillingCodes { get; set; }
        public List<decimal> Salaryrates { get; set; }





    }


}