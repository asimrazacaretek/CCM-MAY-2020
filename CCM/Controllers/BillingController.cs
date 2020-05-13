using CCM.Models;
using CCM.Models.CCMBILLINGS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CCM.Models.CCMBILLINGS.ViewModels;

namespace CCM.Controllers
{
    public class BillingController : BaseController
    {
        // GET: Billing
        //private Application_dbContect _db = new Application_dbContect();

        // GET: BillingCategories
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> CategoryIndex()
        {
            var model = new BillingCategory();
         var Categories   = await _db.BillingCategories.ToListAsync();
          //  var BillingPeriodsIds = _db.BillingCategories.Select(p => p.BillingPeriodsId).ToList().Distinct();

            ViewBag.Categories = Categories;

            var BillingPeriods = _db.BillingPeriods.ToList();  
                                 // _db.BillingPeriods.Where(p=>!BillingPeriodsIds.Contains(p.BillingPeriodsId)).Select(p=>p).ToList();
            ViewBag.BillingPeriod = BillingPeriods;
            return View(model);

        }

        // GET: BillingCategories/Details/5

        [Authorize(Roles ="Admin")]

        public async Task<ActionResult> CategoryDetails(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillingCategory billingCategory = await _db.BillingCategories.FindAsync(id);
            if (billingCategory == null)
            {
                return HttpNotFound();
            }
            return View(billingCategory);
        }

        // GET: BillingCategories/Create

        // POST: BillingCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> CategoryCreate(BillingCategory obj)
        {
            
                if (obj.BillingCategoryId == 0)
                {
                    var model = new BillingCategory();
                    model.Name = obj.Name;
                    if (obj.BillingPeriodsId != null)
                    {
                        model.BillingPeriodsId = obj.BillingPeriodsId;
                        model.BillingPeriods = obj.BillingPeriods;
                    }
                    else
                    {
                        return Json("error", JsonRequestBehavior.AllowGet);
                    }
                    model.MinimunMinutes = obj.MinimunMinutes;
                    _db.BillingCategories.Add(model);
                    var save = await _db.SaveChangesAsync();
                    if (save > 0)
                        return Json("added", JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var category = await _db.BillingCategories.FirstOrDefaultAsync(x => x.BillingCategoryId == obj.BillingCategoryId);
                
                    //if (obj.BillingPeriodsId != null)
                    //{
                    //    category.BillingPeriodsId = obj.BillingPeriodsId;
                    //    category.BillingPeriods = obj.BillingPeriods;
                    //}
                    //else
                    //{
                    //    return Json("error", JsonRequestBehavior.AllowGet);
                    //}
                    category.MinimunMinutes = obj.MinimunMinutes;
                    _db.Entry(category).State = EntityState.Modified;
                    var save = await _db.SaveChangesAsync();
                    if (save > 0)
                        return Json("updated", JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
           
        }

        
        // GET: BillingCategories/Delete/5
        //public async Task<ActionResult> CategoryDelete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BillingCategory billingCategory = await _db.BillingCategories.FindAsync(id);
        //    if (billingCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(billingCategory);
        //}

        // POST: BillingCategories/Delete/5
        //[HttpPost, ActionName("Delete")]
        //public async Task<JsonResult> CategoryDeleteConfirmed(int id)
        //{
        //    BillingCategory billingCategory = await _db.BillingCategories.FindAsync(id);
        //    _db.BillingCategories.Remove(billingCategory);
        //    var delete = await _db.SaveChangesAsync();
        //    if (delete > 0)
        //        return Json("deleted", JsonRequestBehavior.AllowGet);
        //    return Json("error", JsonRequestBehavior.AllowGet);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// ////////////////////////// Billing Periods///////////////////////////
        /// </summary>
        /// <returns></returns>
       
        
        //public async Task<ActionResult> BillingPeriods()
        //{
        //    var model = new BillingPeriods();
        //    ViewBag.BillingPeriods = await _db.BillingPeriods.ToListAsync();
        //    return View(model);
        //}

        // GET: BillingPeriods/Details/5
        //public async Task<ActionResult> BillingPeriodsDetails(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BillingPeriods billingPeriods = await _db.BillingPeriods.FindAsync(id);
        //    if (billingPeriods == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(billingPeriods);
        //}

        //// GET: BillingPeriods/Create
        //public ActionResult BillingPeriodsCreate()
        //{
        //    return View();
        //}

        // POST: BillingPeriods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
    
        //public async Task<ActionResult> BillingPeriodsCreate( BillingPeriods obj)
        //{
          
        //    var exist=await _db.BillingPeriods.Where(p => p.Name == obj.Name ).Select(p=>p).FirstOrDefaultAsync();
        //    var existdescription = await _db.BillingPeriods.Where(p => p.Description == obj.Description && p.BillingPeriodsId==obj.BillingPeriodsId).Select(p => p).FirstOrDefaultAsync();
           
        //        if (obj.BillingPeriodsId == 0)
        //        {
        //            if (exist == null)
        //            {
        //                var model = new BillingPeriods();
        //                model.Name = obj.Name;
        //                model.Description = obj.Description;
        //                try
        //                {
        //                    _db.BillingPeriods.Add(model);
        //                    var save = await _db.SaveChangesAsync();
        //                    if (save > 0)
        //                        return Json("added", JsonRequestBehavior.AllowGet);
        //                    return Json("error", JsonRequestBehavior.AllowGet);
        //                }
        //                catch (Exception e)
        //                {
        //                    return Json("error", JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                return Json("Exist", JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else 
        //        {
        //        var exist1 = await _db.BillingPeriods.Where(p => p.Name == obj.Name && p.BillingPeriodsId ==obj.BillingPeriodsId ).Select(p => p).FirstOrDefaultAsync();
        //        if (exist1 == null)
        //        {
        //            if(exist == null)
        //            {
                        
        //                    var periods = await _db.BillingPeriods.FirstOrDefaultAsync(x => x.BillingPeriodsId == obj.BillingPeriodsId);


        //                    periods.Name = obj.Name;
        //                    periods.Description = obj.Description;
        //                    _db.Entry(periods).State = EntityState.Modified;
        //                    var save = await _db.SaveChangesAsync();
        //                    if (save > 0)
        //                        return Json("updated", JsonRequestBehavior.AllowGet);
        //                    return Json("error", JsonRequestBehavior.AllowGet);
                      
        //            }
        //            else
        //            {
        //                return Json("Exist", JsonRequestBehavior.AllowGet);
        //            }
                
        //        }

        //        if (exist1 != null)
        //        {
        //            if (existdescription == null)
        //            {
        //                var periods = await _db.BillingPeriods.FirstOrDefaultAsync(x => x.BillingPeriodsId == obj.BillingPeriodsId);


        //                periods.Name = obj.Name;
        //                periods.Description = obj.Description;
        //                _db.Entry(periods).State = EntityState.Modified;
        //                var save = await _db.SaveChangesAsync();
        //                if (save > 0)
        //                    return Json("updated", JsonRequestBehavior.AllowGet);
        //                return Json("error", JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json("Description Exist", JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            return Json("Exist", JsonRequestBehavior.AllowGet);
        //        }
        //        }

         

          

        //}

        //// GET: BillingPeriods/Edit/5
        //public async Task<ActionResult> BillingPeriodsEdit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BillingPeriods billingPeriods = await _db.BillingPeriods.FindAsync(id);
        //    if (billingPeriods == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(billingPeriods);
        //}

        // POST: BillingPeriods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> BillingPeriodsEdit([Bind(Include = "BillingPeriodsId,Name")] BillingPeriods billingPeriods)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Entry(billingPeriods).State = EntityState.Modified;
        //        await _db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(billingPeriods);
        //}

        //// GET: BillingPeriods/Delete/5
        //public async Task<ActionResult> BillingPeriodsDelete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BillingPeriods billingPeriods = await _db.BillingPeriods.FindAsync(id);
        //    if (billingPeriods == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(billingPeriods);
        //}

    


        //[HttpPost]
        //public async Task<JsonResult> BillingPeriodsDeleteConfirmed(int id)
        //{
        //    BillingPeriods billingPeriods = await _db.BillingPeriods.FindAsync(id);
        //    _db.BillingPeriods.Remove(billingPeriods);
        //    var delete = await _db.SaveChangesAsync();
        //    if (delete > 0)
        //        return Json("deleted", JsonRequestBehavior.AllowGet);
        //    return Json("error", JsonRequestBehavior.AllowGet);
        //}

        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> BilingCodes()
        {
            var model = new BillingCodes();
            ViewBag.BilingCodes = await _db.BillingCodes.ToListAsync();
            ViewBag.BillingCategories= await _db.BillingCategories.ToListAsync();
            ViewBag.BillingPeriod = await _db.BillingPeriods.ToListAsync();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> AddUpdateBillingCode(BillingCodes obj)
        {
            var Exsist = await _db.BillingCategories.FirstOrDefaultAsync(x => x.Name == obj.Name);
            if (Exsist == null)
            {
                if (obj.Id == 0)
                {
                    
                    _db.BillingCodes.Add(obj);
                    var save = await _db.SaveChangesAsync();
                    if (save > 0)
                        return Json("added", JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var billingCode = await _db.BillingCodes.FirstOrDefaultAsync(x => x.Id == obj.Id);
                    billingCode.BillingCategoryId = obj.BillingCategoryId;
                    //billingCode.BillingPeriodsId = obj.BillingPeriodsId;
                    billingCode.MinimunMinutes = obj.MinimunMinutes;
                    billingCode.Name = obj.Name;
                    billingCode.Description = obj.Description;
                    _db.Entry(billingCode).State = EntityState.Modified;
                    var save = await _db.SaveChangesAsync();
                    if (save > 0)
                        return Json("updated", JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else return Json("exists", JsonRequestBehavior.AllowGet);

            return Json("",JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> DeleteBilingCode(int id)
        {
            var billingCode= await _db.BillingCodes.FindAsync(id);
            _db.BillingCodes.Remove(billingCode);
            var delete = await _db.SaveChangesAsync();
            if (delete > 0)
                return Json("deleted", JsonRequestBehavior.AllowGet);
            return Json("error", JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult GetBillingCodes(string Categories)
        {

            List<BillingCodes> billing = new List<BillingCodes>();
            var deserilizer = new JavaScriptSerializer();
            var categoriesselected = deserilizer.Deserialize<List<string>>(Categories).ToList();

            foreach (var catgoryid in categoriesselected)
            {
                int id = Convert.ToInt32(catgoryid);
                try
                {
                    billing.AddRange(_db.BillingCodes.Where(p => p.BillingCategoryId == id).ToList());

                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            List<BillingViewModel> BillingCodes = new List<BillingViewModel>();
            foreach (var item in billing)
            {
                BillingViewModel billcodes = new BillingViewModel();
                billcodes.Id = item.Id;
                billcodes.Name = item.Name;
                billcodes.Minutes = item.MinimunMinutes;
                billcodes.BillingCategory = item.BillingCategory.Name;
                BillingCodes.Add(billcodes);
            }
           // var jsonresponse = deserilizer.Serialize(billing);
            //var deserilizer = new JavaScriptSerializer();
            //var categoriesselected = deserilizer.Deserialize <List <string>>(Categories).ToArray();
            //for (int i = 0; i < categoriesselected.Length; i++)
            //{
            //billing =    _db.BillingCodes.Where(p => p.BillingCategoryId == Convert.ToInt32(categoriesselected[i])).ToList();


            //}


            return Json(BillingCodes, JsonRequestBehavior.AllowGet);
        }






    }
}