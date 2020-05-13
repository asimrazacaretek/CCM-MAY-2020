using CCM.Models;
using CCM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Controllers
{
    public class TwilioTableController : BaseController
    {
       // private readonly ApplicationdbContect _db = new ApplicationdbContect();
        // GET: TwilioTable
        public ActionResult Index()
        {
            var twiliodata = _db.TwilioNumbersTable.ToList();
            List<TwilioNumberViewModel> TwilioNumberViewModellist = new List<TwilioNumberViewModel>();
            foreach (var item in twiliodata)
            {
                TwilioNumberViewModel TwilioNumberViewModel = new TwilioNumberViewModel();

                TwilioNumberViewModel.Id = item.Id;
                TwilioNumberViewModel.FriendlyPhoneNumer = item.FriendlyPhoneNumer;
                TwilioNumberViewModel.MobilePhoneNumber = item.MobilePhoneNumber;
                TwilioNumberViewModel.Status = item.Status;
                TwilioNumberViewModel.CreatedBy = item.CreatedBy;
                TwilioNumberViewModel.CreatedOn = item.CreatedOn;
                TwilioNumberViewModel.UpdatedBy = item.UpdatedBy;
                TwilioNumberViewModel.UpdatedOn = item.UpdatedOn;
                if (item.Status == true)
                {
                    TwilioNumberViewModel.Assignedto= _db.Liaisons.Where(p => p.TwilioNumbersTableId == item.Id).Select(p => p.FirstName + " " + p.LastName).FirstOrDefault();
                }

                TwilioNumberViewModellist.Add(TwilioNumberViewModel);
               

            }



            return View(TwilioNumberViewModellist);
        }
    }
}