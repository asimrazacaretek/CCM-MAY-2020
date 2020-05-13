using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//have to get this from nuget Packager- it is not included in MVC.Net and you need it to do .OrderBy query
using System.Linq.Dynamic;
using CCM.Models;

namespace loginAPI.Controllers
{
    public class ServerSideProcessingController : Controller
    {
       // private readonly ApplicationdbContect db = new ApplicationdbContect();
        // GET: ServerSideProcessing
        public ActionResult Index()
        {
            return View();
        }


        //public ActionResult LoadDrugData()
        //{
        //    var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
        //    var start = Request.Form.GetValues("start")?.FirstOrDefault();
        //    var length = Request.Form.GetValues("length")?.FirstOrDefault();
        //    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
        //    var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
        //    string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();


        //    //Paging Size (10,20,50,100)    
        //    int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //    int skip = start != null ? Convert.ToInt32(start) : 0;
        //    int recordsTotal = 0;


        //    //This is getting the Data
        //    List<PatientDrugViewModel> AdminR = new List<PatientDrugViewModel>();
        //    var PassDate1 = DateTime.Now.AddMonths(-6);



        //    var AdminData = (from PatientData in db.Patients
        //                     join DrugData in db.DrugClaims on PatientData.PatientNo equals DrugData.PatientNo
        //                     join b in db.BodyParts on PatientData.PatientNo equals b.PatientNo
        //                     select new
        //                     {
        //                         PatientData.FirstName,
        //                         PatientData.PatientNo,
        //                         PatientData.LastName,
        //                         PatientData.BirthDate,
        //                         DrugData.RxNo,
        //                         DrugData.DrugName,
        //                         DrugData.Doctor,
        //                         DrugData.DateFilled,
        //                         DrugData.PtWantsRefills,
        //                         DrugData.DidYouGetNewRx,
        //                         DrugData.DrugRepId,
        //                         DrugData.BodyPartsAffected,
        //                         DrugData.QuantityCalculatedbyBodyPart,
        //                         DrugData.Quantity,
        //                         DrugData.NewRxFromBilling,
        //                         DrugData.Validated,
        //                         DrugData.ValidationDate

        //                     }).ToList();

        //    foreach (var item in AdminData)
        //    {
        //        PatientDrugViewModel PD = new PatientDrugViewModel();

        //        PD.PatientNo = item.PatientNo;
        //        PD.FirstName = item.FirstName;
        //        PD.LastName = item.LastName;
        //        PD.RxNo = item.RxNo;
        //        PD.BirthDate = item.BirthDate;
        //        PD.DateFilled = item.DateFilled;
        //        PD.Doctor = item.Doctor;
        //        PD.DrugName = item.DrugName;
        //        PD.RequestedRefill = item.PtWantsRefills;
        //        PD.DidYouGetRx = item.DidYouGetNewRx;
        //        PD.BodyPartsAffected = item.BodyPartsAffected;
        //        PD.Quantity = item.Quantity;
        //        PD.QuantityCalculated = item.QuantityCalculatedbyBodyPart;
        //        PD.NewRxFromBilling = item.NewRxFromBilling;
        //        PD.Validated = item.Validated;
        //        PD.ValidationDate = item.ValidationDate;
        //        AdminR.Add(PD);

        //    }
        //    var dataView = AdminR.Where(m =>
        //        m.RequestedRefill == "Yes" && m.DidYouGetRx == null && m.DateFilled >= PassDate1
        //        || m.NewRxFromBilling == "Yes"
        //        || m.DateFilled == DateTime.Now);
        //    var FilteredData = dataView.Where(p => p.Validated == null).GroupBy(m => m.RxNo).Select(x => x.FirstOrDefault()).ToList();



        //    //SORT
        //    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
        //    {
        //        FilteredData = FilteredData.OrderBy(sortColumn + " " + sortColumnDir).ToList();
        //    }



        //    //Search    
        //    if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
        //    {
        //        // Apply search   
        //        FilteredData = FilteredData.Where(p => p.FirstName.ToString().ToLower().Contains(searchValue.ToLower()) ||
        //                                               p.LastName.ToLower().Contains(searchValue.ToLower())
        //        ).ToList();
        //    }

        //    //total number of rows count     
        //    recordsTotal = FilteredData.Count();
        //    //Paging     
        //    var data = FilteredData.Skip(skip).Take(pageSize).ToList();
        //    //Returning Json Data    
        //    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, JsonRequestBehavior.AllowGet });




        //}




    }
}