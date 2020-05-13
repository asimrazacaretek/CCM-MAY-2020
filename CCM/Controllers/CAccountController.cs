using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class CAccountController : BaseController
    {
        // GET: CAccount
        public ActionResult Index()
        {
            return View();
        }

        // GET: CAccount/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CAccount/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CAccount/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CAccount/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CAccount/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CAccount/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
