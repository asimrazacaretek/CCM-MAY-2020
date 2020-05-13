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
using CCM.Models.DataModels;

namespace CCM.Controllers
{
    public class QuestionBanksController : BaseController
    {
        //private ApplicationdbContect _db = new ApplicationdbContect();

        // GET: QuestionBanks
        public async Task<ActionResult> Index()
        {

           var model = new QuestionBank();
           var Question= await _db.QuestionBanks.ToListAsync();
            ViewBag.Question = Question;
            return View(model);
        }

        // GET: QuestionBanks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionBank questionBank = await _db.QuestionBanks.FindAsync(id);
            if (questionBank == null)
            {
                return HttpNotFound();
            }
            return View(questionBank);
        }

        // GET: QuestionBanks/Create

        public ActionResult Create()
        {

            return View();
        }

        // POST: QuestionBanks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind(Include = "Id,Question,Status,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")]
        public async Task<ActionResult> Create( QuestionBank questionBank)
        {
            var modal = new QuestionBank();
            
            if (ModelState.IsValid)
            {
                if (questionBank.Id==0)
                {

                    modal.Question = questionBank.Question;
                    modal.Status = questionBank.Status;
                    modal.CreatedOn = DateTime.Now;
                    _db.QuestionBanks.Add(modal);
                var save=await _db.SaveChangesAsync();
                    if(save>0)
                        return Json("added", JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Questionb = await _db.QuestionBanks.FirstOrDefaultAsync(x => x.Id == questionBank.Id);

                    Questionb.Question = questionBank.Question;
                    Questionb.Status = questionBank.Status;
                    Questionb.UpdatedOn = DateTime.Now;
                    _db.Entry(Questionb).State = EntityState.Modified;
                     var save = await _db.SaveChangesAsync();
                    if (save > 0)
                        return Json("updated", JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);

               }
               
            }
            
            return View("Index");
        }

        // GET: QuestionBanks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionBank questionBank = await _db.QuestionBanks.FindAsync(id);
            if (questionBank == null)
            {
                return HttpNotFound();
            }
            return View(questionBank);
        }

        // POST: QuestionBanks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Question,Status,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] QuestionBank questionBank)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(questionBank).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(questionBank);
        }

        // GET: QuestionBanks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionBank questionBank = await _db.QuestionBanks.FindAsync(id);
            if (questionBank == null)
            {
                return HttpNotFound();
            }
            return View(questionBank);
        }

        // POST: QuestionBanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            QuestionBank questionBank = await _db.QuestionBanks.FindAsync(id);
            _db.QuestionBanks.Remove(questionBank);
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
