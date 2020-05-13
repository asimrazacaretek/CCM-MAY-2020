using CCM.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System;
using System.Linq.Dynamic;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace CCM.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Physician, Admin, PhysiciansGroup, LiaisonGroup")]
    public class PhysicianPortalController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public async Task<ActionResult> Details(int? physicianId)
        {
            var physician = await _db.Physicians.FindAsync(physicianId);
            ViewBag.PatientsCount = await _db.Patients.CountAsync(p => p.PhysicianId == physician.Id);

            return View(physician);
        }


        public async Task<PartialViewResult> _PatientsPartial(int physicianId)
        {
            return PartialView(await _db.Patients.Where(p => p.PhysicianId == physicianId).ToListAsync());
        }
        [Authorize(Roles = "Physician, Admin, PhysiciansGroup, LiaisonGroup")]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> _PatientDetailsPartial(int patientId)
        {
            return PartialView(await _db.Patients.FirstOrDefaultAsync(p => p.Id == patientId) ?? new Patient());
        }
        [Authorize(Roles = "Physician, Admin, PhysiciansGroup, LiaisonGroup")]
        public ActionResult LoadFinalCarePlan()
        {
            var user = _db.Users.Find(User.Identity.GetUserId());

            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();


            //Paging Size (10,20,50,100)    
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            int CCMID = user.CCMid ?? 0;
            int physicianId = CCMID;
            List<int> physicianids = new List<int>();

            if (user.Role == "PhysiciansGroup")
            {
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
            }
            List<int> liasionids = new List<int>();
            if (user.Role == "LiaisonGroup")
            {
                liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
            }
            //var results = _db.Patients.Join(_db.FinalCarePlanNotes, //filter the pets
            //                  patient => patient.Id, //left side key for the join
            //                  fcp => fcp.PatientId, //right side key for the join
            //                  (patient, fcp) => patient).Join(_db.BillingCycles.Include(mbox=>mbox.BillingCycleDetails).Include(m=>m.BillingCycleComments), //filter the pets
            //                  patient => patient.Id, //left side key for the join
            //                  bc => bc.PatientId, //right side key for the join
            //                  (patient, bc) => patient).Select(new PatientName=;
            try
            {


                var alreadyaddedbilling = (from p in _db.Patients
                                               join bc in _db.BillingCycles on p.Id equals bc.PatientId 
                                              
                                               join fcp in _db.FinalCarePlanNotes on p.Id equals fcp.PatientId
                                               
                                           select new PhysicianFinalCarePlanViewModel
                                           {
                                               FirstName = p.FirstName + " " + p.LastName ?? "",
                                               LastName = p.LastName,
                                               PatientId = p.Id,
                                               PhyscianID = p.PhysicianId ?? 0,
                                               DOB = p.BirthDate.ToString() ?? ""
                                                
                                           }).Distinct().ToList();
                alreadyaddedbilling = user.Role == "Physician"
                           ? alreadyaddedbilling.Where(p => p.PhyscianID == user.CCMid).ToList()
                           : user.Role == "PhysiciansGroup" ? alreadyaddedbilling.Where(p => p.PhyscianID != null && physicianids.Contains((int)p.PhyscianID)).ToList()


                           : alreadyaddedbilling;

                if (sortColumn == "")
                {
                    sortColumn = "FirstName";
                }
                //SORT
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    try
                    {



                        if (1 == 1)
                        {
                            alreadyaddedbilling = alreadyaddedbilling.Where(p => p.FirstName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                                                                               p.LastName.ToLower().Contains(searchValue.ToLower()) ||
                                                                                 p.PatientId.ToString().Contains(searchValue.ToLower()) ||
                                                                               p.DOB.Contains(searchValue)






                                                ).ToList();
                        }
                        recordsTotal = alreadyaddedbilling.Count();
                        //Paging 
                        if (pageSize == -1)
                        {
                            pageSize = recordsTotal;
                        }
                        var data = alreadyaddedbilling.Skip(skip).Take(pageSize).ToList();
                        //Returning Json Data    
                        return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, JsonRequestBehavior.AllowGet });
                    }
                    catch (Exception ex)
                    {

                    }


                }
            }
            catch (Exception ex)
            {


            }
            recordsTotal = 0;
            //Paging  
            if (pageSize == -1)
            {
                pageSize = recordsTotal;
            }
            var results1 = new List<PhysicianFinalCarePlanViewModel>();
            var data1 = results1.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data1, JsonRequestBehavior.AllowGet });
            //total number of rows count     

        }
        [Authorize(Roles = "Physician, Admin, PhysiciansGroup, LiaisonGroup")]
        public PartialViewResult GetPatientFinalCarePlans(int PatientID)
        {
           var patient = _db.Patients.Where(x => x.Id == PatientID).FirstOrDefault();
            ViewBag.Patient = patient.FirstName + " " + patient.LastName;
           var billingcycles = _db.BillingCycles.Where(x => x.PatientId == PatientID).Select(x=>x.Cycle).Distinct().ToList();
            var finalcareplans = _db.FinalCarePlanNotes.Where(x => x.PatientId == PatientID).ToList();
           var results= finalcareplans.Where(p => billingcycles.Any(p2 => p2 == p.Cycle)).ToList();
            var careplanshared = _db.carePlanSharedHistories.AsNoTracking().Where(x => x.PatientId == PatientID && billingcycles.Contains(x.Cycle)).ToList();
            ViewBag.CarePlanShared = careplanshared;
            return PartialView("FinalCarePlanAll", results);


        }
        [Authorize(Roles = "Physician, Admin, PhysiciansGroup, LiaisonGroup")]
        public PartialViewResult FinalCarePlanComparison(int patientId, int[] cyclesforreivew)
        {
            var finalcareplans = _db.FinalCarePlanNotes.Where(x => cyclesforreivew.Contains(x.Cycle) && x.PatientId==patientId).ToList();
            return PartialView("CycleComparison", finalcareplans);

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