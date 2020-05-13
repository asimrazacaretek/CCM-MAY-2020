using CCM.Helpers;
using CCM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CCM.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HomeController : BaseController
    {
        //private readonly ApplicationdbContect _db = new ApplicationdbContect();



        public ActionResult Index()
        {
            //Helpers.DataManipulation.CategoryStatusManipulation();
            //DataManipulation.Mapdata(User.Identity.GetUserId());
            //var item_T = new SelectListItem() { Text= "No Assigned", Value = "-1T" }; // Item for Translator
            //var item_L = new SelectListItem() { Text = "No Assigned", Value = "-1L" }; // Item for Liasion
            //var item_P = new SelectListItem() { Text = "No Assigned", Value = "-1P" }; // Item fro Physicion
            //var item_PG = new SelectListItem() { Text = "No Assigned", Value = "-1PG" }; // Item fro Physicion Group
            HelperExtensions.InitTelCodes();
            //var results = HelperExtensions.TelCodes;
            //var countrycode = results.Where(x => x.Iso == "US").FirstOrDefault().Pfx;
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
                return View();

            if (User.IsInRole("Admin"))
            {
                var liaisons = _db.Liaisons.AsNoTracking().Where(x => x.IsTranslator == false).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var translators = _db.Liaisons.AsNoTracking().Where(x => x.IsTranslator == true).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });

                var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                    Text = p.FirstName + " " + p.LastName
                });

                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "PhysiciansGroup").Id,
                    Text = p.GroupName
                });
                var translators1 = translators.ToList();
                // translators1.Insert(0, item_T);
                ViewBag.Tranlators = new SelectList(translators1.OrderBy(l => l.Text), "Value", "Text");
                var liaisons1 = liaisons.ToList();
                // liaisons1.Insert(0, item_L);
                ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text");
                var physicians1 = physicians.ToList();
                // physicians1.Insert(0, item_P);
                ViewBag.Physicians = new SelectList(physicians1.OrderBy(p => p.Text), "Value", "Text");
                var physiciansGroups1 = physiciansGroups.ToList();
                // physiciansGroups1.Insert(0, item_PG);
                ViewBag.physiciansGroups = new SelectList(physiciansGroups1.OrderBy(p => p.Text), "Value", "Text");

                return View();
            }
            var user = _db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("PhysiciansGroup"))
            {


                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var liaisons1 = liaisons.ToList();
                //  liaisons1.Insert(0, item_L);
                ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text");
                var translatorids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.TranslatorId != null).Select(x => x.TranslatorId).Distinct().ToList();
                var translators = _db.Liaisons.AsNoTracking().Where(p => translatorids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var translators1 = translators.ToList();
                // translators1.Insert(0, item_T);
                ViewBag.Tranlators = new SelectList(translators1.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text");
                var physicians1 = physicians.ToList();
                //  physicians1.Insert(0, item_P);
                ViewBag.Physicians = new SelectList(physicians1, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Sales"))
            {
                List<int> physicianids = new List<int>();
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();

                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var liaisons1 = liaisons.ToList();
                //  liaisons1.Insert(0, item_L);
                ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text");
                var translatorids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.TranslatorId != null).Select(x => x.TranslatorId).Distinct().ToList();
                var translators = _db.Liaisons.AsNoTracking().Where(p => translatorids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var translators1 = translators.ToList();
                // translators1.Insert(0, item_T);
                ViewBag.Tranlators = new SelectList(translators1.OrderBy(l => l.Text), "Value", "Text");
                var physicians1 = physicians.ToList();
                // physicians1.Insert(0, item_P);
                ViewBag.Physicians = new SelectList(physicians1, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Sales"))
            {
                List<int> physicianids = new List<int>();
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();

                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var liaisons1 = liaisons.ToList();
                // liaisons1.Insert(0, item_L);
                ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text");
                var translatorids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.TranslatorId != null).Select(x => x.TranslatorId).Distinct().ToList();
                var translators = _db.Liaisons.AsNoTracking().Where(p => translatorids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var translators1 = translators.ToList();
                // translators1.Insert(0, item_T);
                ViewBag.Tranlators = new SelectList(translators1.OrderBy(l => l.Text), "Value", "Text");
                var physicians1 = physicians.ToList();
                // physicians1.Insert(0, item_P);
                ViewBag.Physicians = new SelectList(physicians1, "Value", "Text");
                return View();
            }
            if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == false).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                var translators = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id) && p.IsTranslator == true).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var translators1 = translators.ToList();
                //  translators1.Insert(0, item_T);
                ViewBag.Tranlators = new SelectList(translators1.OrderBy(l => l.Text), "Value", "Text");
                var liaisons1 = liaisons.ToList();
                //  liaisons1.Insert(0, item_L);
                ViewBag.Liaisons = new SelectList(liaisons1.OrderBy(l => l.Text), "Value", "Text");
                var physicians1 = physicians.ToList();
                // physicians1.Insert(0, item_P);
                ViewBag.Physicians = new SelectList(physicians1, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Physician"))
            {
                ViewBag.PhysicianId = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                return View();
            }


            return RedirectToAction("Details", "PatientPortal", new { patientId = user.CCMid });
        }

        public ActionResult Chat()
        {
            ViewBag.Message = "Your chat page";
            return View();
        }
        public ActionResult NewDashBoard()
        {
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
                return View();

            if (User.IsInRole("Admin"))
            {
                var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : " (Counsler)")
                });

                var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                    Text = p.FirstName + " " + p.LastName
                });

                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "PhysiciansGroup").Id,
                    Text = p.GroupName
                });

                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

                return View();
            }
            var user = _db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("PhysiciansGroup"))
            {


                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : " (Counsler)")
                });
                var physcicangroupids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(p => physicianids.Contains(p.PhysicianId)).Select(p => p.PhysiciansGroupId).Distinct().ToList();
                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Where(p => physcicangroupids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "PhysiciansGroup").Id,
                    Text = p.GroupName
                });
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : " (Counsler)")
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var physcicangroupids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(p => physicianids.Contains(p.PhysicianId)).Select(p => p.PhysiciansGroupId).Distinct().ToList();
                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Where(p => physcicangroupids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "PhysiciansGroup").Id,
                    Text = p.GroupName
                });
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Physician"))
            {
                ViewBag.PhysicianId = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                return View();
            }


            return RedirectToAction("Details", "PatientPortal", new { patientId = user.CCMid });
        }

        public ActionResult LiasionDashBoard()
        {
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
                return View();

            if (User.IsInRole("Admin"))
            {
                var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : " (Counsler)")
                });

                var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                    Text = p.FirstName + " " + p.LastName
                });

                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "PhysiciansGroup").Id,
                    Text = p.GroupName
                });

                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

                return View();
            }
            var user = _db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("PhysiciansGroup"))
            {


                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Sales"))
            {



                List<int> physicianids = new List<int>();
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();

                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });


                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Physician"))
            {
                ViewBag.PhysicianId = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                return View();
            }


            return RedirectToAction("Details", "PatientPortal", new { patientId = user.CCMid });
        }
        public ActionResult LiasionHourlyReport()
        {
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
            {
                var user123 = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(x => x.Id == user123.CCMid).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                return View();
            }
            if (User.IsInRole("Admin"))
            {
                var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });

                var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                    Text = p.FirstName + " " + p.LastName
                });

                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "PhysiciansGroup").Id,
                    Text = p.GroupName
                });

                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

                return View();
            }
            var user = _db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("PhysiciansGroup"))
            {
                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Sales"))
            {
                List<int> physicianids = new List<int>();
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();

                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });


                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Physician"))
            {
                ViewBag.PhysicianId = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                return View();
            }


            return RedirectToAction("Details", "PatientPortal", new { patientId = user.CCMid });
        }
        public ActionResult LiasionDashBoardAll()
        {
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
                return View();

            if (User.IsInRole("Admin"))
            {
                var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });

                var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                    Text = p.FirstName + " " + p.LastName
                });

                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "PhysiciansGroup").Id,
                    Text = p.GroupName
                });

                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

                return View();
            }
            var user = _db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("PhysiciansGroup"))
            {


                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Sales"))
            {



                List<int> physicianids = new List<int>();
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();

                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });


                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Physician"))
            {
                ViewBag.PhysicianId = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                return View();
            }


            return RedirectToAction("Details", "PatientPortal", new { patientId = user.CCMid });
        }

        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup")]
        public async Task<PartialViewResult> _DashBoardPartial(string userId)
        {
            var user = string.IsNullOrEmpty(userId)
                          ? _db.Users.Find(User.Identity.GetUserId())
                          : _db.Users.Find(userId);

            var patients = user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == false
                          ? _db.Patients.AsNoTracking().Where(p => p.Liaison.UserId == user.Id)
                          : user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == true
                           ? _db.Patients.AsNoTracking().Where(p => p.TranslatorId == user.CCMid)
                          : user.Role == "Physician"
                          ? _db.Patients.AsNoTracking().Where(p => p.PhysicianId == user.CCMid)
                          : user.Role == "PhysiciansGroup"
                          ? _db.Patients.AsNoTracking().Where(p => p.Physician.MainPhoneNumber == user.PhoneNumber)
                          : _db.Patients.AsNoTracking();

            var liaison = user.Role == "Liaison" ? _db.Liaisons.FirstOrDefault(l => l.UserId == user.Id) : null;
            var dashboard = await PopulateDashBoardAsync(patients);

            //
            dashboard.UserId = userId;
            dashboard.TotalRevenue = user.Role == "Liaison"
                                     ? dashboard.Code99490 * liaison?.PayRate99490 + dashboard.Code99487 * liaison?.PayRate99487 + dashboard.Code99489 * liaison?.PayRate99489
                                     : user.Role == "Physician" || user.Role == "PhysiciansGroup"
                                     ? dashboard.InClaimSubmission * 12
                                     : await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission") * 47;
            dashboard.YearlyEarnings = dashboard.TotalRevenue * 12;

            return PartialView(dashboard);
        }
        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup, Sales")]
        public async Task<PartialViewResult> _DashBoardPartialPrePost(string userId = "", string language = "",string type="")
        {



            var user = string.IsNullOrEmpty(userId)
                          ? _db.Users.Find(User.Identity.GetUserId())
                          : _db.Users.Find(userId);

            var LiaisonId = 0;
            if (user.Id != "")
            {
                var Lid = _db.Liaisons.Where(p => p.UserId == user.Id).Select(p => p.Id).FirstOrDefault();
                if (Lid != 0)
                {
                    LiaisonId = Lid;
                }
            }
            List<int> physicianids = new List<int>();
            if (user.Role == "Translator")
            {
                var physiciangroupIds = _db.physicianGroup_Physician_Mappings.Where(x => x.PhysicianId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangroupIds.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
            }
            if (user.Role == "PhysiciansGroup")
            {
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
            }
            List<int> liasionids = new List<int>();
            if (user.Role == "LiaisonGroup")
            {
                liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
            }
            if (user.Role == "Sales")
            {
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
            }
            var patients = user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == false
                           ? _db.Patients.AsNoTracking().Where(p => p.Liaison.UserId == user.Id || p.Patients_PreLiaisonsId == (p.Patients_PreLiaisonsId != 0 ? (p.Patients_PreLiaisons.LiaisonId == LiaisonId && p.Patients_PreLiaisons.Status == true) ? p.Patients_PreLiaisonsId : 0 : 0)).AsQueryable()
                           : user.Role == "Liaison" && HelperExtensions.isTranslator(user.Id) == true
                           ? _db.Patients.AsNoTracking().Where(p => p.TranslatorId == user.CCMid || p.Patients_PreLiaisonsId == (p.Patients_PreLiaisonsId != 0 ? (p.Patients_PreLiaisons.TranslatorId == user.CCMid && p.Patients_PreLiaisons.Status == true) ? p.Patients_PreLiaisonsId : 0 : 0)).AsQueryable()
                           : user.Role == "Physician"
                           ? _db.Patients.AsNoTracking().Where(p => p.PhysicianId == user.CCMid).AsQueryable()
                           : user.Role == "PhysiciansGroup" || user.Role == "Sales"
                           ? _db.Patients.AsNoTracking().Where(p => physicianids.Contains(p.PhysicianId.Value)).AsQueryable()
                           : user.Role == "LiaisonGroup"
                           ? _db.Patients.AsNoTracking().Where(p => liasionids.Contains(p.LiaisonId.Value)).AsQueryable()
                           : user.Role == "Translator"
                           ? _db.Patients.AsNoTracking().Where(p => physicianids.Contains(p.PhysicianId.Value)).AsQueryable()
                           : _db.Patients.AsNoTracking();
            if (!string.IsNullOrEmpty(language))
            {
                patients = patients.Where(x => x.PreferredLanguage == language).AsQueryable();
            }
            var liaison = user.Role == "Liaison" ? _db.Liaisons.AsNoTracking().FirstOrDefault(l => l.UserId == user.Id) : null;
            var dashboard = await PopulateDashBoardAsync(patients);
            //if (userId == "-1T")
            //{

            //}
            //if (userId == "-1L")
            //{

            //}
            //if (userId == "-1P")
            //{ }
            //if (userId == "-1PG")
            //{ }
            //New Statuses data
            List<NewDashBoardViewModel> lstStatusCounts = new List<NewDashBoardViewModel>();
            var enrollmnetstatuses = _db.EnrollmentStatuss.OrderBy(x => x.OrderBy).ToList();

            ViewBag.TotalPaitents = patients.Count();
            foreach (var enrollmentstatus in enrollmnetstatuses)
            {
                NewDashBoardViewModel newDashBoardViewModel = new NewDashBoardViewModel();
                newDashBoardViewModel.StatusName = enrollmentstatus.Name;
                newDashBoardViewModel.Totalcount = patients.Count(p => p.EnrollmentStatus == enrollmentstatus.Name);
                newDashBoardViewModel.SubStatuses = new List<NewDashBoardViewModel>();
                var substatuses = _db.EnrollmentSubStatuss.Where(x => x.EnrollmentStatusID == enrollmentstatus.Id).OrderBy(item => item.OrderBy).ToList();
                foreach (var substatus in substatuses)
                {
                    NewDashBoardViewModel newDashBoardViewModelSub = new NewDashBoardViewModel();
                    newDashBoardViewModelSub.StatusName = substatus.Name;
                    newDashBoardViewModelSub.Totalcount = patients.Count(p => p.EnrollmentSubStatus == substatus.Name);

                    if (substatus.Name == "In-Active Enrolled")
                    {
                        var substatusreasons = _db.EnrollmentSubstatusReasons.ToList();
                        newDashBoardViewModelSub.SubStatuses = new List<NewDashBoardViewModel>();
                        foreach (var substatusreason in substatusreasons)
                        {
                            NewDashBoardViewModel objsubstatusreason = new NewDashBoardViewModel();
                            objsubstatusreason.StatusName = substatusreason.Name;
                            objsubstatusreason.Totalcount = patients.Count(p => p.EnrollmentSubStatusReason == substatusreason.Name);
                            newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                        }
                    }
                    newDashBoardViewModel.SubStatuses.Add(newDashBoardViewModelSub);
                }


                lstStatusCounts.Add(newDashBoardViewModel);
            }
            var patientids = patients.Select(x => x.Id).ToList();
            var cyclestatuses = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CCMStatus != "In Progress" && x.CCMStatus != "Reconciliation" && x.CCMStatus != "Expired").Select(x => x.CCMStatus).Distinct().ToList();
            NewDashBoardViewModel newDashBoardViewModel123 = new NewDashBoardViewModel();
            newDashBoardViewModel123.StatusName = "Active Enrolled Ques Division";
            newDashBoardViewModel123.Totalcount = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.Cycle > 0 && x.CCMStatus != "In Progress" && x.CCMStatus != "Reconciliation" && x.CCMStatus != "Expired" && patientids.Contains(x.PatientId)).AsQueryable().Count();
            newDashBoardViewModel123.SubStatuses = new List<NewDashBoardViewModel>();
            foreach (var item in cyclestatuses)
            {

                NewDashBoardViewModel newDashBoardViewModelSub = new NewDashBoardViewModel();
                newDashBoardViewModelSub.StatusName = item;
                newDashBoardViewModelSub.Totalcount = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.CCMStatus == item && x.Cycle > 0 && patientids.Contains(x.PatientId)).AsQueryable().Count();
                if (item == "Enrolled")
                {

                    newDashBoardViewModelSub.SubStatuses = new List<NewDashBoardViewModel>();

                    NewDashBoardViewModel objsubstatusreason = new NewDashBoardViewModel();
                    objsubstatusreason.StatusName = "Active Work Que (Counselor)";
                    var patientidswithcounserlsonly = patients.Where(x => x.TranslatorId == null).Select(x => x.Id).ToList();
                    objsubstatusreason.Totalcount = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.Cycle > 0 && x.CCMStatus != "In Progress" && x.CCMStatus != "Reconciliation" && x.CCMStatus != "Expired" && patientidswithcounserlsonly.Contains(x.PatientId) && x.CCMStatus == "Enrolled").AsQueryable().Count();
                    newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                    objsubstatusreason = new NewDashBoardViewModel();
                    objsubstatusreason.StatusName = "Active Work Que (Translators)";
                    patientidswithcounserlsonly = patients.Where(x => x.TranslatorId != null).Select(x => x.Id).ToList();
                    objsubstatusreason.Totalcount = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.Cycle > 0 && x.CCMStatus != "In Progress" && x.CCMStatus != "Reconciliation" && x.CCMStatus != "Expired" && patientidswithcounserlsonly.Contains(x.PatientId) && x.CCMStatus == "Enrolled").AsQueryable().Count();
                    newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                }
                newDashBoardViewModel123.SubStatuses.Add(newDashBoardViewModelSub);
            }
            ////Expired but active enrolled
            //NewDashBoardViewModel newDashBoardViewModelSub1 = new NewDashBoardViewModel();
            //newDashBoardViewModelSub1.StatusName = "Expired";
            //newDashBoardViewModelSub1.Totalcount = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.CCMStatus == item && x.Cycle > 0 && patientids.Contains(x.PatientId)).AsQueryable().Count();
            //newDashBoardViewModel123.SubStatuses.Add(newDashBoardViewModelSub1);
            ////
            newDashBoardViewModel123.SubStatuses = newDashBoardViewModel123.SubStatuses.OrderBy(x => x.StatusName).ToList();
            lstStatusCounts.Add(newDashBoardViewModel123);
            dashboard.newDashBoardViewModels = lstStatusCounts;
            //
            dashboard.UserId = userId;
            dashboard.TotalRevenue = user.Role == "Liaison"
                                     ? dashboard.Code99490 * liaison?.PayRate99490 + dashboard.Code99487 * liaison?.PayRate99487 + dashboard.Code99489 * liaison?.PayRate99489
                                     : user.Role == "Physician" || user.Role == "PhysiciansGroup"
                                     ? dashboard.InClaimSubmission * 12
                                     : await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission") * 47;
            dashboard.YearlyEarnings = dashboard.TotalRevenue * 12;

            return PartialView(dashboard);
        }
        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup, Sales")]
        public async Task<PartialViewResult> _DashBoardPartial1(string userId = "",string UserType="", string language = "")
        {



            var user = string.IsNullOrEmpty(userId)
                          ? _db.Users.Find(User.Identity.GetUserId())
                          : _db.Users.Find(userId);

            var LiaisonId = 0;
            if (user.Id != "")
            {
                var Lid=_db.Liaisons.Where(p => p.UserId == user.Id).Select(p => p.Id).FirstOrDefault();
                if (Lid != 0)
                {
                    LiaisonId = Lid;
                }
            }
            List<int> physicianids = new List<int>();
            var patinetidsfilter = new List<int>() ;
            var Translatorcheck = HelperExtensions.isTranslator(user.Id);


            if (user.Role == "Translator")
            {
                var physiciangroupIds = _db.physicianGroup_Physician_Mappings.Where(x => x.PhysicianId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangroupIds.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
            }
            if (user.Role == "PhysiciansGroup")
            {
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
            }
            List<int> liasionids = new List<int>();
            if (user.Role == "LiaisonGroup")
            {
                liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                patinetidsfilter = (from p in _db.Patients
                                    join pbc in _db.Patients_BillingCategories on p.Id equals pbc.PatientId
                                    where liasionids.Contains(pbc.LiaisonId.Value)
                                    select p.Id
                                  ).AsQueryable().Distinct().ToList();
            }
            if (user.Role == "Sales")
            {
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();
            }
           ;
            if (user.Role == "Liaison" && Translatorcheck == false)
            {
                patinetidsfilter = (from p in _db.Patients
                                    join pbc in _db.Patients_BillingCategories on p.Id equals pbc.PatientId
                                    where pbc.LiaisonId == user.CCMid
                                    select p.Id
                                  ).AsQueryable().Distinct().ToList();
            }
            else if(user.Role == "Liaison" && Translatorcheck == true)
            {
            patinetidsfilter=    (from p in _db.Patients
                 join pbc in _db.Patients_BillingCategories on p.Id equals pbc.PatientId
                 where pbc.TranslatorId == user.CCMid
                 select p.Id
                                  ).AsQueryable().Distinct().ToList();
            }
                var patients = user.Role == "Liaison" && Translatorcheck == false
                           ? _db.Patients.AsNoTracking().Where(p => patinetidsfilter.Contains(p.Id) ||p.Patients_PreLiaisonsId== (p.Patients_PreLiaisonsId != 0?(p.Patients_PreLiaisons.LiaisonId==LiaisonId && p.Patients_PreLiaisons.Status==true)?p.Patients_PreLiaisonsId:0:0) ).AsQueryable()
                           : user.Role == "Liaison" && Translatorcheck == true
                           ? _db.Patients.AsNoTracking().Where(p => patinetidsfilter.Contains(p.Id) || p.Patients_PreLiaisonsId == (p.Patients_PreLiaisonsId != 0 ? (p.Patients_PreLiaisons.TranslatorId == user.CCMid && p.Patients_PreLiaisons.Status == true) ? p.Patients_PreLiaisonsId : 0 : 0)).AsQueryable()
                           : user.Role == "Physician"
                           ? _db.Patients.AsNoTracking().Where(p => p.PhysicianId == user.CCMid).AsQueryable()
                           : user.Role == "PhysiciansGroup" || user.Role == "Sales"
                           ? _db.Patients.AsNoTracking().Where(p => physicianids.Contains(p.PhysicianId.Value)).AsQueryable()
                           : user.Role == "LiaisonGroup"
                           ? _db.Patients.AsNoTracking().Where(p => patinetidsfilter.Contains(p.Id)).AsQueryable()
                           : user.Role == "Translator"
                           ? _db.Patients.AsNoTracking().Where(p => physicianids.Contains(p.PhysicianId.Value)).AsQueryable()
                           : _db.Patients.AsNoTracking().AsQueryable();


            if (UserType == "Pre" && userId!="")
            {
                if (Translatorcheck == false)
                {
                    patients = patients.Where(p => p.Patients_PreLiaisonsId == (p.Patients_PreLiaisonsId != 0 ? (p.Patients_PreLiaisons.LiaisonId == LiaisonId && p.Patients_PreLiaisons.Status == true) ? p.Patients_PreLiaisonsId : 0 : 0)).AsQueryable();

                }
                else
                {
                    patients = patients.Where(p =>  p.Patients_PreLiaisonsId == (p.Patients_PreLiaisonsId != 0 ? (p.Patients_PreLiaisons.TranslatorId == user.CCMid && p.Patients_PreLiaisons.Status == true) ? p.Patients_PreLiaisonsId : 0 : 0)).AsQueryable();
                }

            }
            if (UserType == "Post" && userId != "")
            {
               
                   
                if (Translatorcheck == false)
                {
                    patinetidsfilter = (from p in _db.Patients
                                        join pbc in _db.Patients_BillingCategories on p.Id equals pbc.PatientId
                                        where pbc.LiaisonId == user.CCMid
                                        select p.Id
                                 ).AsQueryable().Distinct().ToList();
                    patients = patients.Where(p => patinetidsfilter.Contains(p.Id) && p.EnrollmentSubStatus=="Active Enrolled").AsQueryable();
                }
                else
                {
                    patinetidsfilter = (from p in _db.Patients
                                        join pbc in _db.Patients_BillingCategories on p.Id equals pbc.PatientId
                                        where pbc.TranslatorId == user.CCMid
                                        select p.Id
                                 ).AsQueryable().Distinct().ToList();
                    patients = patients.Where(p => patinetidsfilter.Contains(p.Id)).AsQueryable();
                }

            }
            if (!string.IsNullOrEmpty(language))
            {
                patients = patients.Where(x => x.PreferredLanguage == language).AsQueryable();
            }


            var liaison = user.Role == "Liaison" ? _db.Liaisons.AsNoTracking().FirstOrDefault(l => l.UserId == user.Id) : null;
            //var dashboard = await PopulateDashBoardAsync(patients);


            var today = DateTime.Today.Date;
            var tomorrow = today.AddDays(1);
            var dashboard = new DashBoardViewModel();
            try
            {
                dashboard.TotalPatients= patients.Count();
                dashboard.CallsDueToday = patients.Where(x => x.AppointmentDate != null && DbFunctions.TruncateTime(x.AppointmentDate)==today.Date).Count();
                dashboard.CallsDueTomorrow = patients.Where(x => x.AppointmentDate != null && DbFunctions.TruncateTime(x.AppointmentDate) == tomorrow.Date).Count();
              
            }
            catch (Exception e)
            {

                throw;
            }
         
            //if (userId == "-1T")
            //{

            //}
            //if (userId == "-1L")
            //{

            //}
            //if (userId == "-1P")
            //{ }
            //if (userId == "-1PG")
            //{ }
            //New Statuses data
            List<NewDashBoardViewModel> lstStatusCounts = new List<NewDashBoardViewModel>();
            var enrollmnetstatuses = _db.EnrollmentStatuss.OrderBy(x => x.OrderBy).ToList();

            ViewBag.TotalPaitents = patients.Count();
            foreach (var enrollmentstatus in enrollmnetstatuses)
            {
                NewDashBoardViewModel newDashBoardViewModel = new NewDashBoardViewModel();
                newDashBoardViewModel.StatusName = enrollmentstatus.Name;
                newDashBoardViewModel.Totalcount = patients.Count(p => p.EnrollmentStatus == enrollmentstatus.Name);
                newDashBoardViewModel.SubStatuses = new List<NewDashBoardViewModel>();
                var substatuses = _db.EnrollmentSubStatuss.Where(x => x.EnrollmentStatusID == enrollmentstatus.Id).OrderBy(item => item.OrderBy).ToList();
                foreach (var substatus in substatuses)
                {
                    NewDashBoardViewModel newDashBoardViewModelSub = new NewDashBoardViewModel();
                    newDashBoardViewModelSub.StatusName = substatus.Name;
                    newDashBoardViewModelSub.Totalcount = patients.Count(p => p.EnrollmentSubStatus == substatus.Name);

                    if (substatus.Name == "In-Active Enrolled")
                    {
                        var substatusreasons = _db.EnrollmentSubstatusReasons.Where(p=>p.BillingCategoryId==null).ToList();
                        newDashBoardViewModelSub.SubStatuses = new List<NewDashBoardViewModel>();
                        foreach (var substatusreason in substatusreasons)
                        {
                            NewDashBoardViewModel objsubstatusreason = new NewDashBoardViewModel();
                            objsubstatusreason.StatusName = substatusreason.Name;
                            objsubstatusreason.Totalcount = patients.Count(p => p.EnrollmentSubStatusReason == substatusreason.Name);
                            newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                        }
                    }
                    newDashBoardViewModel.SubStatuses.Add(newDashBoardViewModelSub);
                }


                lstStatusCounts.Add(newDashBoardViewModel);
            }
            var patientids = patients.Select(x => x.Id).ToList();
           var cyclestatuses = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CCMStatus != "In Progress" && x.CCMStatus != "Reconciliation" && x.CCMStatus !="Expired").Select(x => x.CCMStatus).Distinct().ToList();
            NewDashBoardViewModel newDashBoardViewModel123 = new NewDashBoardViewModel();
            newDashBoardViewModel123.StatusName =  _db.BillingCategories.Where(x=>x.BillingCategoryId==BillingCodeHelper.cmmBillingCatagoryid).FirstOrDefault()?.Name+ " Active Enrolled Ques" ;
            var totalpaientsbycategory= (from ccmcycle in _db.CCMCycleStatuses
                                         join pbc in _db.Patients_BillingCategories on ccmcycle.PatientId equals pbc.PatientId
                                         where pbc.BillingCategoryId == BillingCodeHelper.cmmBillingCatagoryid && pbc.Status == true && pbc.PatientId != null
                                         && ccmcycle.CreatedOn.Value.Month == DateTime.Now.Month && ccmcycle.CreatedOn.Value.Year == DateTime.Now.Year && ccmcycle.Cycle > 0 && patientids.Contains(ccmcycle.PatientId)
                                         select new
                                         {
                                             ccmcycle.PatientId,
                                             ccmcycle.CCMStatus,
                                             pbc.TranslatorId
                                         }
                                                      ).AsQueryable().Distinct();
            newDashBoardViewModel123.Totalcount = totalpaientsbycategory.Count();
            newDashBoardViewModel123.SubStatuses = new List<NewDashBoardViewModel>();

            foreach (var item in cyclestatuses)
            {

                NewDashBoardViewModel newDashBoardViewModelSub = new NewDashBoardViewModel();
                newDashBoardViewModelSub.StatusName = item;
                newDashBoardViewModelSub.BillingCategoryID = BillingCodeHelper.cmmBillingCatagoryid;

                newDashBoardViewModelSub.Totalcount = totalpaientsbycategory.Where(x => x.CCMStatus == item).Count();
               // _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.CCMStatus == item && x.Cycle > 0 && patientids.Contains(x.PatientId)).AsQueryable().Count();
                if (item == "Enrolled")
                {
                   
                    newDashBoardViewModelSub.SubStatuses = new List<NewDashBoardViewModel>();
                  
                    NewDashBoardViewModel objsubstatusreason = new NewDashBoardViewModel();
                    objsubstatusreason.StatusName = "Active Work Que (Counselors)";
                    objsubstatusreason.BillingCategoryID = BillingCodeHelper.cmmBillingCatagoryid;
                    objsubstatusreason.Totalcount = totalpaientsbycategory.Where(x=>x.CCMStatus==item && x.TranslatorId==null).Count();
                   
                    newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                    objsubstatusreason = new NewDashBoardViewModel();
                    objsubstatusreason.StatusName = "Active Work Que (Translators)";
                    objsubstatusreason.BillingCategoryID = BillingCodeHelper.cmmBillingCatagoryid;
                    objsubstatusreason.Totalcount = totalpaientsbycategory.Where(x => x.CCMStatus == item && x.TranslatorId != null).Count();
                    newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                }
                newDashBoardViewModel123.SubStatuses.Add(newDashBoardViewModelSub);
            }
            newDashBoardViewModel123.SubStatuses = newDashBoardViewModel123.SubStatuses.OrderBy(x => x.StatusName).ToList();
            lstStatusCounts.Add(newDashBoardViewModel123);
            ///Categories
            var cyclestatusesrpm = _db.CategoriesStatuses.AsNoTracking().Where(x => x.Status != "In Progress" && x.Status != "Reconciliation" && x.Status != "Expired").Select(x => x.Status).Distinct().ToList();
            foreach (var billingcategory in _db.BillingCategories.AsNoTracking().Where(x => x.BillingCategoryId ==BillingCodeHelper.RPMBillingCatagoryid ))
            {
                NewDashBoardViewModel newDashBoardViewModelCat = new NewDashBoardViewModel();
                newDashBoardViewModelCat.StatusName =  billingcategory.Name+ " Active Enrolled Ques";
                newDashBoardViewModelCat.BillingCategoryID = billingcategory.BillingCategoryId;
                var totalpatientbycategory = (from ccmcycle in _db.CategoriesStatuses
                                              join pbc in _db.Patients_BillingCategories on ccmcycle.PatientId equals pbc.PatientId
                                              where pbc.BillingCategoryId == billingcategory.BillingCategoryId && pbc.Status == true && pbc.PatientId != null
                                              && ccmcycle.BillingCategoryId==billingcategory.BillingCategoryId && ccmcycle.CreatedOn.Value.Month == DateTime.Now.Month && ccmcycle.CreatedOn.Value.Year == DateTime.Now.Year && ccmcycle.Cycle > 0 && patientids.Contains(ccmcycle.PatientId.Value)
                                              select new
                                              {
                                                  ccmcycle.PatientId,
                                                  ccmcycle.Status,
                                                  pbc.TranslatorId
                                              }
                                                    ).AsQueryable().Distinct();
                newDashBoardViewModelCat.Totalcount = totalpatientbycategory.Count();
                newDashBoardViewModelCat.SubStatuses = new List<NewDashBoardViewModel>();
                foreach (var item in cyclestatusesrpm)
                {

                    NewDashBoardViewModel newDashBoardViewModelSub = new NewDashBoardViewModel();
                    newDashBoardViewModelSub.StatusName = item;
                    newDashBoardViewModelSub.BillingCategoryID = billingcategory.BillingCategoryId;
                    newDashBoardViewModelSub.Totalcount = totalpatientbycategory.Where(x=>x.Status==item).Distinct().Count();

                   // _db.CategoriesStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.Status == item && x.Cycle > 0 && patientids.Contains(x.PatientId.Value) && x.BillingCategoryId == billingcategory.BillingCategoryId).AsQueryable().Count();
                    if (item == "Enrolled")
                    {

                        newDashBoardViewModelSub.SubStatuses = new List<NewDashBoardViewModel>();

                        NewDashBoardViewModel objsubstatusreason = new NewDashBoardViewModel();
                        objsubstatusreason.StatusName = "Active Work Que (Counselors)";
                        objsubstatusreason.BillingCategoryID = billingcategory.BillingCategoryId;
                        objsubstatusreason.Totalcount = totalpatientbycategory.Where(x => x.Status == item && x.TranslatorId==null).Distinct().Count();


                        newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                        objsubstatusreason = new NewDashBoardViewModel();
                        objsubstatusreason.StatusName = "Active Work Que (Translators)";
                        objsubstatusreason.BillingCategoryID = billingcategory.BillingCategoryId;
                        objsubstatusreason.Totalcount = totalpatientbycategory.Where(x => x.Status == item && x.TranslatorId != null).Distinct().Count();
                        newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                    }
                    newDashBoardViewModelCat.SubStatuses.Add(newDashBoardViewModelSub);
                }
                newDashBoardViewModelCat.SubStatuses = newDashBoardViewModelCat.SubStatuses.OrderBy(x => x.StatusName).ToList();
                lstStatusCounts.Add(newDashBoardViewModelCat);
            }
           
            foreach (var billingcategory in _db.BillingCategories.AsNoTracking().Where(x => x.BillingCategoryId == BillingCodeHelper.G0506BillingCatagoryid))
            {
                NewDashBoardViewModel newDashBoardViewModelCat = new NewDashBoardViewModel();
                newDashBoardViewModelCat.StatusName = billingcategory.Name + " Active Enrolled Ques";
                newDashBoardViewModelCat.BillingCategoryID = billingcategory.BillingCategoryId;
                var totalg050patients = (from ccmcycle in _db.CategoriesStatuses
                                         join pbc in _db.Patients_BillingCategories on ccmcycle.PatientId equals pbc.PatientId
                                         where pbc.BillingCategoryId == billingcategory.BillingCategoryId && pbc.Status == true && pbc.PatientId != null
                                          && ccmcycle.BillingCategoryId == billingcategory.BillingCategoryId && ccmcycle.Cycle > 0 && patientids.Contains(ccmcycle.PatientId.Value)
                                         select new { ccmcycle.PatientId,ccmcycle.Status,pbc.TranslatorId}
                                                    ).AsQueryable().Distinct();
                newDashBoardViewModelCat.Totalcount = totalg050patients.Count();
                newDashBoardViewModelCat.SubStatuses = new List<NewDashBoardViewModel>();
                foreach (var item in cyclestatusesrpm)
                {

                    NewDashBoardViewModel newDashBoardViewModelSub = new NewDashBoardViewModel();
                    newDashBoardViewModelSub.StatusName = item;
                    newDashBoardViewModelSub.BillingCategoryID = billingcategory.BillingCategoryId;
                    newDashBoardViewModelSub.Totalcount =totalg050patients.Where(x=>x.Status==item).Count();

                   // _db.CategoriesStatuses.AsNoTracking().Where(x =>x.Status == item && x.Cycle > 0 && patientids.Contains(x.PatientId.Value) && x.BillingCategoryId == billingcategory.BillingCategoryId).AsQueryable().Count();
                    if (item == "Enrolled")
                    {

                        newDashBoardViewModelSub.SubStatuses = new List<NewDashBoardViewModel>();

                        NewDashBoardViewModel objsubstatusreason = new NewDashBoardViewModel();
                        objsubstatusreason.StatusName = "Active Work Que (Counselors)";
                        objsubstatusreason.BillingCategoryID = billingcategory.BillingCategoryId;
                        objsubstatusreason.Totalcount =  totalg050patients.Where(x => x.Status == item && x.TranslatorId==null).Count();


                        newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                        objsubstatusreason = new NewDashBoardViewModel();
                        objsubstatusreason.StatusName = "Active Work Que (Translators)";
                        objsubstatusreason.BillingCategoryID = billingcategory.BillingCategoryId;
                        objsubstatusreason.Totalcount = totalg050patients.Where(x => x.Status == item && x.TranslatorId != null).Count();
                        newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                    }
                    newDashBoardViewModelCat.SubStatuses.Add(newDashBoardViewModelSub);
                }
                newDashBoardViewModelCat.SubStatuses = newDashBoardViewModelCat.SubStatuses.OrderBy(x => x.StatusName).ToList();
                lstStatusCounts.Add(newDashBoardViewModelCat);
            }

            //

            ////Expired but active enrolled
            //NewDashBoardViewModel newDashBoardViewModelSub1 = new NewDashBoardViewModel();
            //newDashBoardViewModelSub1.StatusName = "Expired";
            //newDashBoardViewModelSub1.Totalcount = _db.CCMCycleStatuses.AsNoTracking().Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.CreatedOn.Value.Year == DateTime.Now.Year && x.CCMStatus == item && x.Cycle > 0 && patientids.Contains(x.PatientId)).AsQueryable().Count();
            //newDashBoardViewModel123.SubStatuses.Add(newDashBoardViewModelSub1);
            ////

            dashboard.newDashBoardViewModels = lstStatusCounts;
            //
            dashboard.UserId = userId;
            

            return PartialView(dashboard);
        }
        public ActionResult LiaisonCallsHistory()
        {
            ViewBag.EnrollmentSubStatus = new SelectList(_db.EnrollmentSubStatuss.ToList(), "Name", "Name").ToList().OrderBy(y=>y.Value);
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
            {
                var user123 = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(x => x.Id == user123.CCMid).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                return View();
            }


            if (User.IsInRole("Admin"))
            {
                var liaisons = _db.Liaisons.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName + (p.isActive == true ? "(Active)" : "") + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                }).OrderBy(x => x.Text.Contains("Active"));

                var physicians = _db.Physicians.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                });

                var physiciansGroups = _db.PhysiciansGroup.AsNoTracking().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.GroupName
                });

                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians.OrderBy(p => p.Text), "Value", "Text");
                ViewBag.physiciansGroups = new SelectList(physiciansGroups.OrderBy(p => p.Text), "Value", "Text");

                return View();
            }
            var user = _db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("PhysiciansGroup"))
            {


                List<int> physicianids = new List<int>();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => x.PhysiciansGroupId == user.CCMid).Select(x => x.PhysicianId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = p.Id.ToString(),
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName + (p.isActive == true ? "(Active)" : "") + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Sales"))
            {



                List<int> physicianids = new List<int>();
                var physiciangrpids = _db.physicianGroup_SalesStaff_Mappings.AsNoTracking().Where(x => x.SaleStaffId == user.CCMid).Select(x => x.PhysiciansGroupId).ToList();
                physicianids = _db.physicianGroup_Physician_Mappings.AsNoTracking().Where(x => physiciangrpids.Contains(x.PhysiciansGroupId)).Select(x => x.PhysicianId).ToList();

                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = _db.Users.FirstOrDefault(u => u.CCMid == p.Id && u.Role == "Physician").Id,
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });
                var liasionids = _db.Patients.AsNoTracking().Where(x => physicianids.Contains(x.PhysicianId.Value) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().ToList();
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FirstName + " " + p.LastName + (p.isActive == true ? "(Active)" : "") + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("LiaisonGroup"))
            {


                List<int> physicianids = new List<int>();
                var liasionids = _db.LiaisonGroup_Liaison_Mappings.AsNoTracking().Where(x => x.LiaisonGroupId == user.CCMid).Select(x => x.LiaisonId).ToList();
                var group = _db.Users.Find(User.Identity.GetUserId());
                var liaisons = _db.Liaisons.AsNoTracking().Where(p => liasionids.Contains(p.Id)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName + (p.isActive == true ? "(Active)" : "") + (p.IsTranslator == true ? " (Translator)" : "(Counsler)")
                });
                physicianids = _db.Patients.AsNoTracking().Where(x => liasionids.Contains(x.LiaisonId.Value) && x.LiaisonId != null).Select(x => x.PhysicianId.Value).Distinct().ToList();
                var physicians = _db.Physicians.AsNoTracking().Where(p => physicianids.Contains(p.Id))
                                                              .Select(p => new SelectListItem
                                                              {
                                                                  Value = p.Id.ToString(),
                                                                  Text = p.FirstName + " " + p.LastName
                                                              });


                ViewBag.Liaisons = new SelectList(liaisons.OrderBy(l => l.Text), "Value", "Text");
                ViewBag.Physicians = new SelectList(physicians, "Value", "Text");
                return View();
            }
            if (User.IsInRole("Physician"))
            {
                ViewBag.PhysicianId = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                return View();
            }

            return View();
        }
        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup")]
        public async Task<PartialViewResult> _LiaisonCallHistoryData(string LiasionIDs, string PhysiciansIDs, string PhyGroupIDs, DateTime DateFrom, DateTime DateTo, string EnrollmentSubStatus, string AppointmentStatus, string CarePlanStatus, int pageS, int pageNo, string text)
        {
            try
            {


                var datediff = DateFrom - DateTo;
                int startmonth = DateFrom.Month;
                int endmonth = DateTo.Month;
                int startyear = DateFrom.Year;
                int endyear = DateTo.Year;
                if (endyear > startyear)
                {
                    endmonth = 12;
                }
                List<Patient> totalPatients = new List<Patient>();
                //Patient p;
                var totalmonths = Math.Abs((DateFrom.Month - DateTo.Month) + 12 * (DateFrom.Year - DateTo.Year));
                var patientids = new List<int>();
                if (!string.IsNullOrEmpty(LiasionIDs) && LiasionIDs != "null" && LiasionIDs != "undefined")
                {
                    string[] Liaisons = LiasionIDs.Split(',');
                    foreach (var item in Liaisons)
                    {
                        var liasionid = Convert.ToInt32(item);
                        var istranslator = _db.Liaisons.AsNoTracking().Where(x => x.Id == liasionid).FirstOrDefault().IsTranslator;
                        if (EnrollmentSubStatus == "")
                        {
                            if (text.Trim() == "")
                            {
                                if (istranslator == false)
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid).Select(x => x.Id).ToList());
                                }
                                else
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid).Select(x => x.Id).ToList());
                                }
                                

                            }
                            else
                            {
                                if (istranslator == false)
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                                }
                                else
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                                }
                                
                            }
                        }
                        else
                        {
                            if (text.Trim() == "")
                            {
                                if (istranslator == false)
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                                }
                                else
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                                }
                                
                            }
                            else
                            {
                                if (istranslator == false)
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                                }
                                else
                                {
                                    patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                                }
                                
                            }
                        }

                    }
                }
                if (!string.IsNullOrEmpty(PhysiciansIDs) && PhysiciansIDs != "null" && PhysiciansIDs != "undefined")
                {
                    string[] Physicians = PhysiciansIDs.Split(',');
                    foreach (var item in Physicians)
                    {
                        var physiid = Convert.ToInt32(item);
                        if (EnrollmentSubStatus == "")
                        {
                            if (text.Trim() == "")
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid && x.FirstName.Contains(text) && x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                        }
                        else
                        {
                            if (text.Trim() == "")
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid && x.EnrollmentSubStatus == EnrollmentSubStatus && x.FirstName.Contains(text) && x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                        }

                    }
                }
                if (!string.IsNullOrEmpty(PhyGroupIDs) && PhyGroupIDs != "null" && PhyGroupIDs != "undefined")
                {
                    string[] PhyGroups = PhyGroupIDs.Split(',');
                    foreach (var item in PhyGroups)
                    {
                        var phygrpid = Convert.ToInt32(item);
                        var physcianids = _db.physicianGroup_Physician_Mappings.Where(x => x.PhysiciansGroupId == phygrpid).Select(x => x.PhysicianId).ToList();
                        if (EnrollmentSubStatus == "")
                        {
                            if (text.Trim() == "")
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value)).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value) && x.FirstName.Contains(text) && x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                        }
                        else
                        {
                            if (text.Trim() == "")
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value) && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value) && x.EnrollmentSubStatus == EnrollmentSubStatus && x.FirstName.Contains(text) && x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                        }

                    }
                }
                if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
                {
                    var liasionid = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                    var istranslator = _db.Liaisons.AsNoTracking().Where(x => x.Id == liasionid).FirstOrDefault().IsTranslator;
                    //patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId != null && x.LiaisonId == liasionid).Select(x => x.Id).ToList());
                    if (EnrollmentSubStatus == "")
                    {
                        if (text.Trim() == "")
                        {
                            if (istranslator == false)
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid).Select(x => x.Id).ToList());
                            }
                            
                        }
                        else
                        {
                            if (istranslator == false)
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                            
                        }
                    }
                    else
                    {
                        if (text.Trim() == "")
                        {
                            if (istranslator == false)
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                            }
                            
                        }
                        else
                        {
                            if (istranslator == false)
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                            else
                            {
                                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.TranslatorId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus && x.FirstName.Contains(text) || x.LastName.Contains(text)).Select(x => x.Id).ToList());
                            }
                            
                        }
                    }
                }
                patientids = patientids.Distinct().ToList();

                var patients = _db.Patients.AsNoTracking().Where(x => patientids.Contains(x.Id)).Select(x => new { PatientName = x.FirstName + " " + x.LastName, Patientid = x.Id, LiaisonID = x.LiaisonId, AppointmentDate = x.AppointmentDate }).AsQueryable();
                //if (AppointmentStatus == "NoAppointment")
                //{
                //    patients.Where(x => x.AppointmentDate?.Year)
                //}
                //else
                //{
                //    if (AppointmentStatus == "AppointmentPassDo")
                //    {

                //    }
                //    else
                //    {
                //        if (AppointmentStatus == "AppointmentCompleted")
                //        {

                //        }
                //        else
                //        {

                //        }
                //    }
                //}
                var liasionids = _db.Patients.AsNoTracking().Where(x => patientids.Contains(x.Id) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().AsQueryable();
                var liaisions = _db.Liaisons.AsNoTracking().Where(x => liasionids.Contains(x.Id)).Select(x => new { LiaisionName = x.FirstName + " " + x.LastName, Liaisionid = x.Id }).AsQueryable();
                var CallHistories = new List<CallHistory>();

                if (startyear == endyear)
                {

                    CallHistories = _db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Year == startyear && x.EndTime != null && x.Direction == "Outgoing").ToList();
                }
                else
                {
                    for (int i = startyear; i <= endyear; i++)
                    {
                        CallHistories.AddRange(_db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Year == i && x.EndTime != null && x.Direction == "Outgoing").ToList());
                    }

                }

                var ReviewTimes = new List<ReviewTimeCcm>();
                if (startyear == endyear)
                {
                    ReviewTimes = _db.ReviewTimeCcms.AsNoTracking().Where(x => patientids.Contains(x.PatientId.Value)).ToList().Where(x => x.StartTime.Year == startyear && x.EndTime != null).ToList();
                }
                else
                {
                    for (int i = startyear; i <= endyear; i++)
                    {
                        ReviewTimes.AddRange(_db.ReviewTimeCcms.AsNoTracking().Where(x => patientids.Contains(x.PatientId.Value)).ToList().Where(x => x.StartTime.Year == i && x.EndTime != null).ToList());
                    }
                }
                var Careplansubmitted = new List<CCMCycleStatus>();
                if (startyear == endyear)
                {

                    Careplansubmitted = _db.CCMCycleStatuses.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CcmClinicalSignOffDate != null).ToList().Where(x => x.CcmClinicalSignOffDate.Value.Year == startyear).ToList();


                }
                else
                {
                    for (int i = startyear; i <= endyear; i++)
                    {
                        Careplansubmitted.AddRange(_db.CCMCycleStatuses.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CcmClinicalSignOffDate != null).ToList().Where(x => x.CcmClinicalSignOffDate.Value.Year == i).ToList());
                    }

                }
                var Appointmentdates = _db.patientAppointments.AsNoTracking().Where(x => patientids.Contains(x.PatientID.Value) && x.PatientID!=null).ToList().Where(x => x.StartTime.Year >= startyear && x.StartTime.Year <= endyear).ToList();
                var BillingCycles = _db.BillingCycles.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CreatedOn.Value.Year >= startyear && x.CreatedOn.Value.Year <= endyear).ToList();
                List<string> lstMonths = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                List<LiasionCallHistory> liasionCallHistories = new List<LiasionCallHistory>();
                LiasionCallHistory liasionCallHistory = new LiasionCallHistory();
                liasionCallHistory.MonthNames = new List<MonthNames>();
                liasionCallHistory.PatientName = "";
                for (int ii = startyear; ii <= endyear; ii++)
                {
                    if (ii > startyear && ii == endyear)
                    {
                        startmonth = 1;
                        endmonth = DateTo.Month;
                    }
                    else
                    {
                        if (ii > startyear && ii < endyear)
                        {
                            startmonth = 1;
                            endmonth = 12;
                        }
                    }
                    for (int jj = startmonth; jj <= endmonth; jj++)
                    {
                        MonthNames monthNames = new MonthNames();

                        monthNames.MonthName = lstMonths[jj - 1] + "-" + ii.ToString();

                        liasionCallHistory.MonthNames.Add(monthNames);
                    }
                }

                liasionCallHistories.Add(liasionCallHistory);
                int totalPage, totalRecord, pageSize;
                if (pageS == -1)
                {
                    pageSize = patientids.Count();
                }
                else if (pageS == 0)
                {
                    pageSize = 10;
                }
                else
                {
                    pageSize = pageS;
                }

                totalRecord = patientids.Count();
                totalPage = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);

                ViewBag.totalPage = totalPage;
                ViewBag.totalRecord = totalRecord;
                ViewBag.pageSize = pageSize;
                ViewBag.pageNo = pageNo;
                var pageids = patientids.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                foreach (var patientitem in pageids)
                {

                    LiasionCallHistory minuteandType = new LiasionCallHistory();
                    minuteandType.PatientId = patientitem;
                    minuteandType.PatientName = patients.Where(x => x.Patientid == patientitem).FirstOrDefault().PatientName;
                    try
                    {
                        var liaisonidforthis = patients.Where(x => x.Patientid == patientitem).FirstOrDefault().LiaisonID;
                        minuteandType.PatientName += " (" + liaisions.Where(x => x.Liaisionid == liaisonidforthis).FirstOrDefault().LiaisionName + ")";
                    }
                    catch (Exception ex)
                    {


                    }
                    minuteandType.MonthNames = new List<MonthNames>();
                    startmonth = DateFrom.Month;
                    endmonth = DateTo.Month;

                    if (endyear > startyear)
                    {
                        endmonth = 12;
                    }
                    for (int ii = startyear; ii <= endyear; ii++)
                    {
                        if (ii > startyear && ii == endyear)
                        {
                            startmonth = 1;
                            endmonth = DateTo.Month;
                        }
                        else
                        {
                            if (ii > startyear && ii < endyear)
                            {
                                startmonth = 1;
                                endmonth = 12;
                            }
                        }
                        for (int jj = startmonth; jj <= endmonth; jj++)
                        {



                            MonthNames monthNames = new MonthNames();
                            monthNames.MonthName = jj.ToString();
                            monthNames.YearName = ii.ToString();
                            monthNames.callDurationandTypes = new List<CallDurationandType>();
                            var calldata = CallHistories.Where(x => x.StartTime.Value.Month == jj && x.StartTime.Value.Year == ii && x.PatientID == patientitem).ToList();
                            CallDurationandType callDurationandType = new CallDurationandType();
                            callDurationandType.CallType = "Completed";
                            callDurationandType.ClassName = "fas fa-phone-volume greenclrcal";
                            var minutes = calldata.Where(x => x.Status != null && x.Status == "completed" && x.Duration != null).Select(x => x.Duration).ToList().Aggregate
                       (TimeSpan.Zero,
                       (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
                            callDurationandType.CallDuration = (minutes.Days * 24 * 60 +
                                                          minutes.Hours * 60 +
                                                          minutes.Minutes).ToString() + "";
                            monthNames.callDurationandTypes.Add(callDurationandType);
                            //
                            callDurationandType = new CallDurationandType();
                            callDurationandType.CallType = "Attempts";
                            callDurationandType.ClassName = "fas fa-headset redclrcal";

                            callDurationandType.CallDuration = calldata.Where(x => x.Status != null && x.Status != "completed" && x.Duration != null).ToList().Count().ToString();
                            monthNames.callDurationandTypes.Add(callDurationandType);
                            callDurationandType = new CallDurationandType();
                            callDurationandType.CallType = "Total Time Spent ";
                            callDurationandType.ClassName = "far fa-clock blueclrcal";
                            minutes = ReviewTimes.Where(x => x.PatientId == patientitem && x.StartTime.Month == jj && x.StartTime.Year == ii).Select(x => x.ReviewTime).ToList().Aggregate
                      (TimeSpan.Zero,
                      (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
                            callDurationandType.CallDuration = (minutes.Days * 24 * 60 +
                                                          minutes.Hours * 60 +
                                                          minutes.Minutes).ToString();
                            monthNames.callDurationandTypes.Add(callDurationandType);

                            callDurationandType = new CallDurationandType();
                            callDurationandType.CallType = "Care Plan Submitted";
                            callDurationandType.ClassName = "fas fa-thumbs-down redclrcal";
                            callDurationandType.CallDuration = "X";
                            var totalcounts = Careplansubmitted.Where(x => x.PatientId == patientitem && x.CcmClinicalSignOffDate != null && x.CcmClinicalSignOffDate.Value.Month == jj && x.CcmClinicalSignOffDate.Value.Year == ii).ToList();
                            if (totalcounts.Count() > 0)
                            {
                                var cycleforpatient = totalcounts.FirstOrDefault().Cycle;
                                var cptcode = BillingCycles.Where(x => x.PatientId == patientitem && x.Cycle == cycleforpatient).FirstOrDefault();
                                if (cptcode != null)
                                {
                                    if (cptcode.BillingCode1 == "CPT99490" && cptcode.BillingCode2 == "")
                                    {
                                        callDurationandType.CallDuration = "20";
                                        callDurationandType.ClassName = "fas fa-thumbs-up twentymincolor";
                                    }
                                    else
                                    {
                                        if (cptcode.BillingCode1 == "CPT99487" && cptcode.BillingCode2 == "")
                                        {
                                            callDurationandType.CallDuration = "60";
                                            callDurationandType.ClassName = "fas fa-thumbs-up sixtymincolor";
                                        }
                                        else
                                        {
                                            if (cptcode.BillingCode2 == "CPT99489")
                                            {
                                                callDurationandType.CallDuration = "60+30";
                                                callDurationandType.ClassName = "fas fa-thumbs-up nintymincolor";
                                            }
                                        }
                                    }
                                }


                            }
                            else
                            {
                                callDurationandType.ClassName = "fas fa-thumbs-down redclrcal";

                            }
                            monthNames.callDurationandTypes.Add(callDurationandType);
                            callDurationandType = new CallDurationandType();
                            callDurationandType.CallType = "Appointment";


                            var appointment = Appointmentdates.Where(X => X.PatientID == patientitem && X.StartTime.Month == jj && X.StartTime.Year == ii).FirstOrDefault();
                            if (appointment == null)
                            {
                                callDurationandType.ClassName = "fas fa-calendar-alt redclrcal";
                            }
                            else
                            {
                                if (appointment.AptStatus == "Pending")
                                {
                                    callDurationandType.ClassName = "fas fa-calendar-alt blueclrcal";
                                    callDurationandType.CallDuration = appointment.StartTime.ToString();
                                }
                                else
                                {
                                    callDurationandType.ClassName = "fas fa-calendar-alt greenclrcal";
                                    callDurationandType.CallDuration = appointment.StartTime.ToString();
                                }
                            }
                            monthNames.callDurationandTypes.Add(callDurationandType);
                            minuteandType.MonthNames.Add(monthNames);

                        }
                    }
                    liasionCallHistories.Add(minuteandType);
                }
                //var pageNumber = pageNo;
                //var onePageOfPosts = .ToPagedList(pageNumber, 10);
                //ViewBag.OnePageOfPosts = onePageOfPosts;
                return PartialView(liasionCallHistories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return PartialView(new List<LiasionCallHistory>());

            }
            //var reviewtime = ReviewTimes.Where(x => (x.StartTime.Hour == i && x.StartTime.Minute == j) || (x.EndTime.Value.Hour == i && x.EndTime.Value.Minute == j)).FirstOrDefault();
            //var callhistory = CallHistories.Where(x => (x.StartTime.Value.Hour == i && x.StartTime.Value.Minute == j) || (x.EndTime.Value.Hour == i && x.EndTime.Value.Minute == j)).FirstOrDefault();
            //var
        }
        [HttpPost]
        public ActionResult AjaxLiaisonCallHistoryData(string LiasionIDs, string PhysiciansIDs, string PhyGroupIDs, DateTime DateFrom, DateTime DateTo, string EnrollmentSubStatus, string AppointmentStatus, string CarePlanStatus)
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var datediff = DateFrom - DateTo;
            int startmonth = DateFrom.Month;
            int endmonth = DateTo.Month;
            int startyear = DateFrom.Year;
            int endyear = DateTo.Year;
            if (endyear > startyear)
            {
                endmonth = 12;
            }
            var totalmonths = Math.Abs((DateFrom.Month - DateTo.Month) + 12 * (DateFrom.Year - DateTo.Year));
            var patientids = new List<int>();
            if (!string.IsNullOrEmpty(LiasionIDs) && LiasionIDs != "null" && LiasionIDs != "undefined")
            {
                string[] Liaisons = LiasionIDs.Split(',');
                foreach (var item in Liaisons)
                {
                    var liasionid = Convert.ToInt32(item);
                    if (EnrollmentSubStatus == "")
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid).Select(x => x.Id).ToList());
                    }
                    else
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                    }

                }
            }
            if (!string.IsNullOrEmpty(PhysiciansIDs) && PhysiciansIDs != "null" && PhysiciansIDs != "undefined")
            {
                string[] Physicians = PhysiciansIDs.Split(',');
                foreach (var item in Physicians)
                {
                    var physiid = Convert.ToInt32(item);
                    if (EnrollmentSubStatus == "")
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid).Select(x => x.Id).ToList());
                    }
                    else
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                    }

                }
            }
            if (!string.IsNullOrEmpty(PhyGroupIDs) && PhyGroupIDs != "null" && PhyGroupIDs != "undefined")
            {
                string[] PhyGroups = PhyGroupIDs.Split(',');
                foreach (var item in PhyGroups)
                {
                    var phygrpid = Convert.ToInt32(item);
                    var physcianids = _db.physicianGroup_Physician_Mappings.Where(x => x.PhysiciansGroupId == phygrpid).Select(x => x.PhysicianId).ToList();
                    if (EnrollmentSubStatus == "")
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value)).Select(x => x.Id).ToList());
                    }
                    else
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value) && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                    }

                }
            }
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
            {
                var liasionid = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId != null && x.LiaisonId == liasionid).Select(x => x.Id).ToList());
            }
            patientids = patientids.Distinct().ToList();

            var patients = _db.Patients.AsNoTracking().Where(x => patientids.Contains(x.Id)).Select(x => new { PatientName = x.FirstName + " " + x.LastName, Patientid = x.Id, LiaisonID = x.LiaisonId, AppointmentDate = x.AppointmentDate }).AsQueryable();

            var liasionids = _db.Patients.AsNoTracking().Where(x => patientids.Contains(x.Id) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().AsQueryable();
            var liaisions = _db.Liaisons.AsNoTracking().Where(x => liasionids.Contains(x.Id)).Select(x => new { LiaisionName = x.FirstName + " " + x.LastName, Liaisionid = x.Id }).AsQueryable();
            var CallHistories = new List<CallHistory>();

            if (startyear == endyear)
            {

                CallHistories = _db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Year == startyear && x.EndTime != null && x.Direction == "Outgoing").ToList();
            }
            else
            {
                for (int i = startyear; i <= endyear; i++)
                {
                    CallHistories.AddRange(_db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Year == i && x.EndTime != null && x.Direction == "Outgoing").ToList());
                }

            }

            var ReviewTimes = new List<ReviewTimeCcm>();
            if (startyear == endyear)
            {
                ReviewTimes = _db.ReviewTimeCcms.AsNoTracking().Where(x => patientids.Contains(x.PatientId.Value)).ToList().Where(x => x.StartTime.Year == startyear && x.EndTime != null).ToList();
            }
            else
            {
                for (int i = startyear; i <= endyear; i++)
                {
                    ReviewTimes.AddRange(_db.ReviewTimeCcms.AsNoTracking().Where(x => patientids.Contains(x.PatientId.Value)).ToList().Where(x => x.StartTime.Year == i && x.EndTime != null).ToList());
                }
            }
            var Careplansubmitted = new List<CCMCycleStatus>();
            if (startyear == endyear)
            {

                Careplansubmitted = _db.CCMCycleStatuses.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CcmClinicalSignOffDate != null).ToList().Where(x => x.CcmClinicalSignOffDate.Value.Year == startyear).ToList();


            }
            else
            {
                for (int i = startyear; i <= endyear; i++)
                {
                    Careplansubmitted.AddRange(_db.CCMCycleStatuses.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CcmClinicalSignOffDate != null).ToList().Where(x => x.CcmClinicalSignOffDate.Value.Year == i).ToList());
                }

            }
            var Appointmentdates = _db.patientAppointments.AsNoTracking().Where(x => patientids.Contains(x.PatientID.Value) && x.PatientID !=null).ToList().Where(x => x.StartTime.Year >= startyear && x.StartTime.Year <= endyear).ToList();
            var BillingCycles = _db.BillingCycles.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CreatedOn.Value.Year >= startyear && x.CreatedOn.Value.Year <= endyear).ToList();
            List<string> lstMonths = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            List<LiasionCallHistory> liasionCallHistories = new List<LiasionCallHistory>();
            LiasionCallHistory liasionCallHistory = new LiasionCallHistory();
            liasionCallHistory.MonthNames = new List<MonthNames>();
            liasionCallHistory.PatientName = "";
            for (int ii = startyear; ii <= endyear; ii++)
            {
                if (ii > startyear && ii == endyear)
                {
                    startmonth = 1;
                    endmonth = DateTo.Month;
                }
                else
                {
                    if (ii > startyear && ii < endyear)
                    {
                        startmonth = 1;
                        endmonth = 12;
                    }
                }
                for (int jj = startmonth; jj <= endmonth; jj++)
                {
                    MonthNames monthNames = new MonthNames();

                    monthNames.MonthName = lstMonths[jj - 1] + "-" + ii.ToString();

                    liasionCallHistory.MonthNames.Add(monthNames);
                }
            }

            liasionCallHistories.Add(liasionCallHistory);
            var count = liasionCallHistories.Count();
            var p = patientids.Skip(skip).Take(pageSize).ToList();
            foreach (var patientitem in p)
            {

                LiasionCallHistory minuteandType = new LiasionCallHistory();
                minuteandType.PatientId = patientitem;
                minuteandType.PatientName = patients.Where(x => x.Patientid == patientitem).FirstOrDefault().PatientName;
                try
                {
                    var liaisonidforthis = patients.Where(x => x.Patientid == patientitem).FirstOrDefault().LiaisonID;
                    minuteandType.PatientName += " (" + liaisions.Where(x => x.Liaisionid == liaisonidforthis).FirstOrDefault().LiaisionName + ")";
                }
                catch (Exception ex)
                {


                }
                minuteandType.MonthNames = new List<MonthNames>();
                startmonth = DateFrom.Month;
                endmonth = DateTo.Month;

                if (endyear > startyear)
                {
                    endmonth = 12;
                }
                for (int ii = startyear; ii <= endyear; ii++)
                {
                    if (ii > startyear && ii == endyear)
                    {
                        startmonth = 1;
                        endmonth = DateTo.Month;
                    }
                    else
                    {
                        if (ii > startyear && ii < endyear)
                        {
                            startmonth = 1;
                            endmonth = 12;
                        }
                    }
                    for (int jj = startmonth; jj <= endmonth; jj++)
                    {



                        MonthNames monthNames = new MonthNames();
                        monthNames.MonthName = jj.ToString();
                        monthNames.YearName = ii.ToString();
                        monthNames.callDurationandTypes = new List<CallDurationandType>();
                        var calldata = CallHistories.Where(x => x.StartTime.Value.Month == jj && x.StartTime.Value.Year == ii && x.PatientID == patientitem).ToList();
                        CallDurationandType callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Completed";
                        callDurationandType.ClassName = "fas fa-phone-volume greenclrcal";
                        var minutes = calldata.Where(x => x.Status != null && x.Status == "completed" && x.Duration != null).Select(x => x.Duration).ToList().Aggregate
                   (TimeSpan.Zero,
                   (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
                        callDurationandType.CallDuration = (minutes.Days * 24 * 60 +
                                                      minutes.Hours * 60 +
                                                      minutes.Minutes).ToString() + "";
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        //
                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Attempts";
                        callDurationandType.ClassName = "fas fa-headset redclrcal";

                        callDurationandType.CallDuration = calldata.Where(x => x.Status != null && x.Status != "completed" && x.Duration != null).ToList().Count().ToString();
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Total Time Spent ";
                        callDurationandType.ClassName = "far fa-clock blueclrcal";
                        minutes = ReviewTimes.Where(x => x.PatientId == patientitem && x.StartTime.Month == jj && x.StartTime.Year == ii).Select(x => x.ReviewTime).ToList().Aggregate
                  (TimeSpan.Zero,
                  (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
                        callDurationandType.CallDuration = (minutes.Days * 24 * 60 +
                                                      minutes.Hours * 60 +
                                                      minutes.Minutes).ToString();
                        monthNames.callDurationandTypes.Add(callDurationandType);

                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Care Plan Submitted";
                        callDurationandType.ClassName = "fas fa-thumbs-down redclrcal";
                        callDurationandType.CallDuration = "X";
                        var totalcounts = Careplansubmitted.Where(x => x.PatientId == patientitem && x.CcmClinicalSignOffDate != null && x.CcmClinicalSignOffDate.Value.Month == jj && x.CcmClinicalSignOffDate.Value.Year == ii).ToList();
                        if (totalcounts.Count() > 0)
                        {
                            var cycleforpatient = totalcounts.FirstOrDefault().Cycle;
                            var cptcode = BillingCycles.Where(x => x.PatientId == patientitem && x.Cycle == cycleforpatient).FirstOrDefault();
                            if (cptcode != null)
                            {
                                if (cptcode.BillingCode1 == "CPT99490" && cptcode.BillingCode2 == "")
                                {
                                    callDurationandType.CallDuration = "20";
                                    callDurationandType.ClassName = "fas fa-thumbs-up twentymincolor";
                                }
                                else
                                {
                                    if (cptcode.BillingCode1 == "CPT99487" && cptcode.BillingCode2 == "")
                                    {
                                        callDurationandType.CallDuration = "60";
                                        callDurationandType.ClassName = "fas fa-thumbs-up sixtymincolor";
                                    }
                                    else
                                    {
                                        if (cptcode.BillingCode2 == "CPT99489")
                                        {
                                            callDurationandType.CallDuration = "60+30";
                                            callDurationandType.ClassName = "fas fa-thumbs-up nintymincolor";
                                        }
                                    }
                                }
                            }


                        }
                        else
                        {
                            callDurationandType.ClassName = "fas fa-thumbs-down redclrcal";

                        }
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Appointment";


                        var appointment = Appointmentdates.Where(X => X.PatientID == patientitem && X.StartTime.Month == jj && X.StartTime.Year == ii).FirstOrDefault();
                        if (appointment == null)
                        {
                            callDurationandType.ClassName = "fas fa-calendar-alt redclrcal";
                        }
                        else
                        {
                            if (appointment.AptStatus == "Pending")
                            {
                                callDurationandType.ClassName = "fas fa-calendar-alt blueclrcal";
                                callDurationandType.CallDuration = appointment.StartTime.ToString();
                            }
                            else
                            {
                                callDurationandType.ClassName = "fas fa-calendar-alt greenclrcal";
                                callDurationandType.CallDuration = appointment.StartTime.ToString();
                            }
                        }
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        minuteandType.MonthNames.Add(monthNames);

                    }
                }
                liasionCallHistories.Add(minuteandType);
            }
            return PartialView("_LiaisonCallHistoryData", liasionCallHistories);
            //var jsonResult= Json(liasionCallHistories,JsonRequestBehavior.AllowGet);
            //.MaxJsonLength = int.MaxValue;
            // return jsonResult;
            //var reviewtime = ReviewTimes.Where(x => (x.StartTime.Hour == i && x.StartTime.Minute == j) || (x.EndTime.Value.Hour == i && x.EndTime.Value.Minute == j)).FirstOrDefault();
            //var callhistory = CallHistories.Where(x => (x.StartTime.Value.Hour == i && x.StartTime.Value.Minute == j) || (x.EndTime.Value.Hour == i && x.EndTime.Value.Minute == j)).FirstOrDefault();
            //var
        }
        public async Task<PartialViewResult> _partialViewReport(string LiasionIDs, string PhysiciansIDs, string PhyGroupIDs, DateTime DateFrom, DateTime DateTo, string EnrollmentSubStatus, string AppointmentStatus, string CarePlanStatus, int pageS, int pageNo, string searchText)
        {
            var datediff = DateFrom - DateTo;
            int startmonth = DateFrom.Month;
            int endmonth = DateTo.Month;
            int startyear = DateFrom.Year;
            int endyear = DateTo.Year;
            if (endyear > startyear)
            {
                endmonth = 12;
            }
            var totalmonths = Math.Abs((DateFrom.Month - DateTo.Month) + 12 * (DateFrom.Year - DateTo.Year));
            var patientids = new List<int>();
            if (!string.IsNullOrEmpty(LiasionIDs) && LiasionIDs != "null" && LiasionIDs != "undefined")
            {
                string[] Liaisons = LiasionIDs.Split(',');
                foreach (var item in Liaisons)
                {
                    var liasionid = Convert.ToInt32(item);
                    if (EnrollmentSubStatus == "")
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.FirstName.Contains(searchText) || x.LastName.Contains(searchText)).Select(x => x.Id).ToList());
                    }
                    else
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId == liasionid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                    }

                }
            }
            if (!string.IsNullOrEmpty(PhysiciansIDs) && PhysiciansIDs != "null" && PhysiciansIDs != "undefined")
            {
                string[] Physicians = PhysiciansIDs.Split(',');
                foreach (var item in Physicians)
                {
                    var physiid = Convert.ToInt32(item);
                    if (EnrollmentSubStatus == "")
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid && x.FirstName.Contains(searchText) || x.LastName.Contains(searchText)).Select(x => x.Id).ToList());
                    }
                    else
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId == physiid && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                    }

                }
            }
            if (!string.IsNullOrEmpty(PhyGroupIDs) && PhyGroupIDs != "null" && PhyGroupIDs != "undefined")
            {
                string[] PhyGroups = PhyGroupIDs.Split(',');
                foreach (var item in PhyGroups)
                {
                    var phygrpid = Convert.ToInt32(item);
                    var physcianids = _db.physicianGroup_Physician_Mappings.Where(x => x.PhysiciansGroupId == phygrpid).Select(x => x.PhysicianId).ToList();
                    if (EnrollmentSubStatus == "")
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value)).Select(x => x.Id).ToList());
                    }
                    else
                    {
                        patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.PhysicianId != null && physcianids.Contains(x.PhysicianId.Value) && x.EnrollmentSubStatus == EnrollmentSubStatus).Select(x => x.Id).ToList());
                    }

                }
            }
            if (!HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison") || (HelperExtensions.isTranslator(User.Identity.GetUserId()) && User.IsInRole("Liaison")))
            {
                var liasionid = _db.Users.Find(User.Identity.GetUserId()).CCMid;
                patientids.AddRange(_db.Patients.AsNoTracking().Where(x => x.LiaisonId != null && x.LiaisonId == liasionid && x.FirstName.Contains(searchText) || x.LastName.Contains(searchText)).Select(x => x.Id).ToList());
            }
            patientids = patientids.Distinct().ToList();

            var patients = _db.Patients.AsNoTracking().Where(x => patientids.Contains(x.Id) && x.FirstName.Contains(searchText) || x.LastName.Contains(searchText)).Select(x => new { PatientName = x.FirstName + " " + x.LastName, Patientid = x.Id, LiaisonID = x.LiaisonId, AppointmentDate = x.AppointmentDate }).AsQueryable();
            //if (AppointmentStatus == "NoAppointment")
            //{
            //    patients.Where(x => x.AppointmentDate?.Year)
            //}
            //else
            //{
            //    if (AppointmentStatus == "AppointmentPassDo")
            //    {

            //    }
            //    else
            //    {
            //        if (AppointmentStatus == "AppointmentCompleted")
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }
            //}
            var liasionids = _db.Patients.AsNoTracking().Where(x => patientids.Contains(x.Id) && x.LiaisonId != null).Select(x => x.LiaisonId).Distinct().AsQueryable();
            var liaisions = _db.Liaisons.AsNoTracking().Where(x => liasionids.Contains(x.Id)).Select(x => new { LiaisionName = x.FirstName + " " + x.LastName, Liaisionid = x.Id }).AsQueryable();
            var CallHistories = new List<CallHistory>();

            if (startyear == endyear)
            {

                CallHistories = _db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Year == startyear && x.EndTime != null && x.Direction == "Outgoing").ToList();
            }
            else
            {
                for (int i = startyear; i <= endyear; i++)
                {
                    CallHistories.AddRange(_db.CallHistories.AsNoTracking().Where(x => patientids.Contains(x.PatientID)).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Year == i && x.EndTime != null && x.Direction == "Outgoing").ToList());
                }

            }

            var ReviewTimes = new List<ReviewTimeCcm>();
            if (startyear == endyear)
            {
                ReviewTimes = _db.ReviewTimeCcms.AsNoTracking().Where(x => patientids.Contains(x.PatientId.Value)).ToList().Where(x => x.StartTime.Year == startyear && x.EndTime != null).ToList();
            }
            else
            {
                for (int i = startyear; i <= endyear; i++)
                {
                    ReviewTimes.AddRange(_db.ReviewTimeCcms.AsNoTracking().Where(x => patientids.Contains(x.PatientId.Value)).ToList().Where(x => x.StartTime.Year == i && x.EndTime != null).ToList());
                }
            }
            var Careplansubmitted = new List<CCMCycleStatus>();
            if (startyear == endyear)
            {

                Careplansubmitted = _db.CCMCycleStatuses.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CcmClinicalSignOffDate != null).ToList().Where(x => x.CcmClinicalSignOffDate.Value.Year == startyear).ToList();


            }
            else
            {
                for (int i = startyear; i <= endyear; i++)
                {
                    Careplansubmitted.AddRange(_db.CCMCycleStatuses.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CcmClinicalSignOffDate != null).ToList().Where(x => x.CcmClinicalSignOffDate.Value.Year == i).ToList());
                }

            }
            var Appointmentdates = _db.patientAppointments.AsNoTracking().Where(x => patientids.Contains(x.PatientID.Value) && x.PatientID !=null).ToList().Where(x => x.StartTime.Year >= startyear && x.StartTime.Year <= endyear).ToList();
            var BillingCycles = _db.BillingCycles.AsNoTracking().Where(x => patientids.Contains(x.PatientId) && x.CreatedOn.Value.Year >= startyear && x.CreatedOn.Value.Year <= endyear).ToList();
            List<string> lstMonths = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            List<LiasionCallHistory> liasionCallHistories = new List<LiasionCallHistory>();
            LiasionCallHistory liasionCallHistory = new LiasionCallHistory();
            liasionCallHistory.MonthNames = new List<MonthNames>();
            liasionCallHistory.PatientName = "";
            for (int ii = startyear; ii <= endyear; ii++)
            {
                if (ii > startyear && ii == endyear)
                {
                    startmonth = 1;
                    endmonth = DateTo.Month;
                }
                else
                {
                    if (ii > startyear && ii < endyear)
                    {
                        startmonth = 1;
                        endmonth = 12;
                    }
                }
                for (int jj = startmonth; jj <= endmonth; jj++)
                {
                    MonthNames monthNames = new MonthNames();

                    monthNames.MonthName = lstMonths[jj - 1] + "-" + ii.ToString();

                    liasionCallHistory.MonthNames.Add(monthNames);
                }
            }

            liasionCallHistories.Add(liasionCallHistory);
            int totalPage, totalRecord, pageSize;
            if (pageS == 0)
            {
                pageSize = 10;
            }
            else
            {
                pageSize = pageS;
            }

            totalRecord = patientids.Count();
            totalPage = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            ViewBag.totalPage = totalPage;
            var pageids = patientids.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

            foreach (var patientitem in pageids)
            {

                LiasionCallHistory minuteandType = new LiasionCallHistory();
                minuteandType.PatientId = patientitem;
                minuteandType.PatientName = patients.Where(x => x.Patientid == patientitem).FirstOrDefault().PatientName;
                try
                {
                    var liaisonidforthis = patients.Where(x => x.Patientid == patientitem).FirstOrDefault().LiaisonID;
                    minuteandType.PatientName += " (" + liaisions.Where(x => x.Liaisionid == liaisonidforthis).FirstOrDefault().LiaisionName + ")";
                }
                catch (Exception ex)
                {


                }
                minuteandType.MonthNames = new List<MonthNames>();
                startmonth = DateFrom.Month;
                endmonth = DateTo.Month;

                if (endyear > startyear)
                {
                    endmonth = 12;
                }
                for (int ii = startyear; ii <= endyear; ii++)
                {
                    if (ii > startyear && ii == endyear)
                    {
                        startmonth = 1;
                        endmonth = DateTo.Month;
                    }
                    else
                    {
                        if (ii > startyear && ii < endyear)
                        {
                            startmonth = 1;
                            endmonth = 12;
                        }
                    }
                    for (int jj = startmonth; jj <= endmonth; jj++)
                    {



                        MonthNames monthNames = new MonthNames();
                        monthNames.MonthName = jj.ToString();
                        monthNames.YearName = ii.ToString();
                        monthNames.callDurationandTypes = new List<CallDurationandType>();
                        var calldata = CallHistories.Where(x => x.StartTime.Value.Month == jj && x.StartTime.Value.Year == ii && x.PatientID == patientitem).ToList();
                        CallDurationandType callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Completed";
                        callDurationandType.ClassName = "fas fa-phone-volume greenclrcal";
                        var minutes = calldata.Where(x => x.Status != null && x.Status == "completed" && x.Duration != null).Select(x => x.Duration).ToList().Aggregate
                   (TimeSpan.Zero,
                   (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
                        callDurationandType.CallDuration = (minutes.Days * 24 * 60 +
                                                      minutes.Hours * 60 +
                                                      minutes.Minutes).ToString() + "";
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        //
                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Attempts";
                        callDurationandType.ClassName = "fas fa-headset redclrcal";

                        callDurationandType.CallDuration = calldata.Where(x => x.Status != null && x.Status != "completed" && x.Duration != null).ToList().Count().ToString();
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Total Time Spent ";
                        callDurationandType.ClassName = "far fa-clock blueclrcal";
                        minutes = ReviewTimes.Where(x => x.PatientId == patientitem && x.StartTime.Month == jj && x.StartTime.Year == ii).Select(x => x.ReviewTime).ToList().Aggregate
                  (TimeSpan.Zero,
                  (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
                        callDurationandType.CallDuration = (minutes.Days * 24 * 60 +
                                                      minutes.Hours * 60 +
                                                      minutes.Minutes).ToString();
                        monthNames.callDurationandTypes.Add(callDurationandType);

                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Care Plan Submitted";
                        callDurationandType.ClassName = "fas fa-thumbs-down redclrcal";
                        callDurationandType.CallDuration = "X";
                        var totalcounts = Careplansubmitted.Where(x => x.PatientId == patientitem && x.CcmClinicalSignOffDate != null && x.CcmClinicalSignOffDate.Value.Month == jj && x.CcmClinicalSignOffDate.Value.Year == ii).ToList();
                        if (totalcounts.Count() > 0)
                        {
                            var cycleforpatient = totalcounts.FirstOrDefault().Cycle;
                            var cptcode = BillingCycles.Where(x => x.PatientId == patientitem && x.Cycle == cycleforpatient).FirstOrDefault();
                            if (cptcode != null)
                            {
                                if (cptcode.BillingCode1 == "CPT99490" && cptcode.BillingCode2 == "")
                                {
                                    callDurationandType.CallDuration = "20";
                                    callDurationandType.ClassName = "fas fa-thumbs-up twentymincolor";
                                }
                                else
                                {
                                    if (cptcode.BillingCode1 == "CPT99487" && cptcode.BillingCode2 == "")
                                    {
                                        callDurationandType.CallDuration = "60";
                                        callDurationandType.ClassName = "fas fa-thumbs-up sixtymincolor";
                                    }
                                    else
                                    {
                                        if (cptcode.BillingCode2 == "CPT99489")
                                        {
                                            callDurationandType.CallDuration = "60+30";
                                            callDurationandType.ClassName = "fas fa-thumbs-up nintymincolor";
                                        }
                                    }
                                }
                            }


                        }
                        else
                        {
                            callDurationandType.ClassName = "fas fa-thumbs-down redclrcal";

                        }
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        callDurationandType = new CallDurationandType();
                        callDurationandType.CallType = "Appointment";


                        var appointment = Appointmentdates.Where(X => X.PatientID == patientitem && X.StartTime.Month == jj && X.StartTime.Year == ii).FirstOrDefault();
                        if (appointment == null)
                        {
                            callDurationandType.ClassName = "fas fa-calendar-alt redclrcal";
                        }
                        else
                        {
                            if (appointment.AptStatus == "Pending")
                            {
                                callDurationandType.ClassName = "fas fa-calendar-alt blueclrcal";
                                callDurationandType.CallDuration = appointment.StartTime.ToString();
                            }
                            else
                            {
                                callDurationandType.ClassName = "fas fa-calendar-alt greenclrcal";
                                callDurationandType.CallDuration = appointment.StartTime.ToString();
                            }
                        }
                        monthNames.callDurationandTypes.Add(callDurationandType);
                        minuteandType.MonthNames.Add(monthNames);

                    }
                }
                liasionCallHistories.Add(minuteandType);
            }

            return PartialView(liasionCallHistories);
        }
        public ActionResult TestReport()
        {
            return View();
        }
        [HttpGet]
        public ActionResult getData(string from, string to)
        {
            DateTime fromdate = Convert.ToDateTime(from);
            DateTime todate = Convert.ToDateTime(to);
            List<string> months = GetMonthYear(fromdate, todate);
            List<Patient> patients = new List<Patient>();
            List<string> pa = _db.Patients.Take(3).Select(p => p.FirstName).ToList();
            return Json(new { result = true, monthName = months, patientsList = pa }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult data1()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
            var start = Request.Form.GetValues("start")?.FirstOrDefault();
            var length = Request.Form.GetValues("length")?.FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]")?.FirstOrDefault() + "][name]")?.FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            //List<Patient> p = new List<Patient>();
            var p = _db.Patients.OrderBy(pp => pp.Id).Skip(skip).Take(pageSize).ToList();
            var jsonResult = Json(new { result = p }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public List<string> GetMonthYear(DateTime dtStart, DateTime dtEnd)
        {

            List<string> monthList = new List<string>();
            for (DateTime dt = dtStart; dt <= dtEnd; dt = dt.AddMonths(1))
            {
                monthList.Add(dt.ToString("MMM yyyy"));
            }

            return monthList;
        }
        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup")]
        public PartialViewResult TotalCallsandReviewTime(int PatientId, string Month, string Year)
        {


            int month = Convert.ToInt32(Month);
            int year = Convert.ToInt32(Year);
            var reviewtimeccm = _db.ReviewTimeCcms.AsNoTracking().Where(x => x.PatientId == PatientId).ToList().Where(x => x.StartTime.Month == month && x.StartTime.Year == year && x.Activity != null && x.Activity != "").OrderByDescending(x => x.StartTime).ToList();
            var callhistories = _db.CallHistories.AsNoTracking().Where(x => x.PatientID == PatientId).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Year == year && x.StartTime.Value.Month == month && x.EndTime != null && x.Direction == "Outgoing").OrderByDescending(x => x.StartTime).ToList();
            var model = new CallHistoryandReviewTimes();
            model.callHistories = callhistories;
            model.reviewTimeCcms = reviewtimeccm;
            var patients = _db.Patients.AsNoTracking().Where(x => x.Id == PatientId).FirstOrDefault();
            model.PatientID = "PatientID: " + PatientId.ToString();
            model.patientIdandname = "Name: " + patients.FirstName + " " + patients.LastName;
            if (patients.LiaisonId != null)
            {
                model.CounslerName = "Counsler: " + _db.Liaisons.Where(X => X.Id == patients.LiaisonId).FirstOrDefault()?.FirstName + " " + _db.Liaisons.Where(X => X.Id == patients.LiaisonId).FirstOrDefault()?.LastName;
            }

            var calldata = callhistories.Where(x => x.StartTime.Value.Month == month && x.StartTime.Value.Year == year && x.PatientID == PatientId).ToList();

            var minutes = calldata.Where(x => x.Status != null && x.Status == "completed" && x.Duration != null).Select(x => x.Duration).ToList().Aggregate
       (TimeSpan.Zero,
       (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
            model.TotalCallMinutes = (minutes.Days * 24 * 60 +
                                            minutes.Hours * 60 +
                                            minutes.Minutes).ToString() + "";

            //


            model.TotalCallAttempts = calldata.Where(x => x.Status != null && x.Status != "completed" && x.Duration != null).ToList().Count().ToString();

            minutes = reviewtimeccm.Where(x => x.PatientId == PatientId && x.StartTime.Month == month && x.StartTime.Year == year).Select(x => x.ReviewTime).ToList().Aggregate
      (TimeSpan.Zero,
      (sumSoFar, nextMyObject) => sumSoFar + nextMyObject);
            model.TotalActivityTime = (minutes.Days * 24 * 60 +
                                              minutes.Hours * 60 +
                                              minutes.Minutes).ToString();



            model.CarePlanClass = "fas fa-thumbs-down redclrcal";
            model.CarePlanSubmittedMin = "X";
            var totalcounts = _db.CCMCycleStatuses.Where(x => x.PatientId == PatientId && x.CcmClinicalSignOffDate != null && x.CcmClinicalSignOffDate.Value.Month == month && x.CcmClinicalSignOffDate.Value.Year == year).ToList();
            if (totalcounts.Count() > 0)
            {
                var cycleforpatient = totalcounts.FirstOrDefault().Cycle;
                var cptcode = _db.BillingCycles.Where(x => x.PatientId == PatientId && x.Cycle == cycleforpatient).FirstOrDefault();
                if (cptcode != null)
                {
                    if (cptcode.BillingCode1 == "CPT99490" && cptcode.BillingCode2 == "")
                    {
                        model.CarePlanSubmittedMin = "20";
                        model.CarePlanClass = "fas fa-thumbs-up twentymincolor";
                    }
                    else
                    {
                        if (cptcode.BillingCode1 == "CPT99487" && cptcode.BillingCode2 == "")
                        {
                            model.CarePlanSubmittedMin = "60";
                            model.CarePlanClass = "fas fa-thumbs-up sixtymincolor";
                        }
                        else
                        {
                            if (cptcode.BillingCode2 == "CPT99489")
                            {
                                model.CarePlanSubmittedMin = "60+30";
                                model.CarePlanClass = "fas fa-thumbs-up nintymincolor";
                            }
                        }
                    }
                }
            }



            var appointment = _db.patientAppointments.Where(X => X.PatientID == PatientId && X.StartTime.Month == month && X.StartTime.Year == year).FirstOrDefault();
            if (appointment == null)
            {
                model.AppointmentClass = "fas fa-calendar-alt redclrcal";
            }
            else
            {
                if (appointment.AptStatus == "Pending")
                {
                    model.AppointmentClass = "fas fa-calendar-alt blueclrcal";
                    model.AppointmentDate = appointment.StartTime.ToString();
                }
                else
                {
                    model.AppointmentClass = "fas fa-calendar-alt greenclrcal";
                    model.AppointmentDate = appointment.StartTime.ToString();
                }
            }

            return PartialView(model);
        }
        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup")]
        public async Task<PartialViewResult> _DashBoardPartialNew(string userId, DateTime From, DateTime To)
        {
            var user = string.IsNullOrEmpty(userId)
                          ? _db.Users.Find(User.Identity.GetUserId())
                          : _db.Users.Find(userId);
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

            using (var context = new ApplicationdbContect())
            {


                var patients = context.Database
                      .SqlQuery<patients_historyviewmodel>("GetPatientsfromHistory")
                      .ToList();


             
                patients = user.Role == "Liaison"
                              ? patients.Where(p => p.LiaisonId == user.CCMid).ToList()
                              : user.Role == "Physician"
                              ? patients.Where(p => p.PhysicianId == user.CCMid).ToList()
                              : user.Role == "PhysiciansGroup"
                              ? patients.Where(p => physicianids.Contains(p.PhysicianId.Value)).ToList()
                              : user.Role == "LiaisonGroup"
                              ? patients.Where(p => liasionids.Contains(p.LiaisonId)).ToList()
                              : patients.ToList();
                // var patients1 = patients.ToList();
                var liaison = user.Role == "Liaison" ? _db.Liaisons.FirstOrDefault(l => l.UserId == user.Id) : null;
                //var dashboard = await PopulateDashBoardAsync(patients);
                List<DashBoardViewModel> dashBoardViewModels = new List<DashBoardViewModel>();
                var enrollmnetstatuses = _db.EnrollmentStatuss.AsNoTracking().OrderBy(x => x.OrderBy).ToList();
                var substatuses = _db.EnrollmentSubStatuss.AsNoTracking().OrderBy(item => item.OrderBy).ToList();
                var substatusreasons = _db.EnrollmentSubstatusReasons.AsNoTracking().ToList();
                //
                var patients1 = new List<patients_historyviewmodel>(patients);
                dashBoardViewModels.Add(GetPatientDashboardviewmodelbydate(patients, From, enrollmnetstatuses, substatusreasons));
                GetPatientDashboardviewmodelbydate(patients1, To, enrollmnetstatuses, substatusreasons, dashBoardViewModels.FirstOrDefault());
                dashBoardViewModels[0].Datestr = "Historic: " + From.ToString("dddd, dd MMMM yyyy") + "  Current: " + To.ToString("dddd, dd MMMM yyyy");
               
                return PartialView(dashBoardViewModels);
            }
        }

        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup")]
        public async Task<PartialViewResult> _DashBoardLiaison(string userId, DateTime From, DateTime To)
        {
            var user = string.IsNullOrEmpty(userId)
                          ? _db.Users.Find(User.Identity.GetUserId())
                          : _db.Users.Find(userId);
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

            using (var context = new ApplicationdbContect())
            {

                var istranslator = HelperExtensions.isTranslator(userId);
                try
                {
                var liaisonDashBoards = context.Database
                      .SqlQuery<LiaisonDashBoard>("GetDashBoardDataforLiasion @LiaisonID, @Fromdate, @ToDate, @isTranslator, @UserID",
    new SqlParameter("LiaisonID", user.CCMid),
    new SqlParameter("Fromdate", From),
    new SqlParameter("ToDate", To),
    new SqlParameter("isTranslator", istranslator==true?1:0),
    new SqlParameter("UserID", userId))
                      .ToList();

                                liaisonDashBoards = liaisonDashBoards.Where(x => x.ShowHide == 1).ToList();
                var unsortedata = liaisonDashBoards.Where(x => x.DataType == "Sorted").ToList().OrderByDescending(x => Convert.ToDateTime(x.TotalTime).TimeOfDay).ToList();
                var sortedata = liaisonDashBoards.Where(x => x.DataType == "").ToList();
                var rejectedPatients = liaisonDashBoards.Where(x => x.DataType == "rejected").ToList();

                var twiliodata = liaisonDashBoards.Where(x => x.DataType == "Twilio").ToList();
                var totalpatients = sortedata.Where(x => x.Page == "Total Patients").FirstOrDefault();
                var totaltimespent = sortedata.Where(x => x.Page == "Total Time Spent on Patients").FirstOrDefault();
          
                    if (totalpatients != null && totaltimespent != null)
                    {
                        if (totalpatients.TotalTime != null && totaltimespent.TotalTime != null)
                        {
                            LiaisonDashBoard item = new LiaisonDashBoard();
                            item.Page = "Average Time";

                            TimeSpan ts = (Convert.ToDateTime(totaltimespent.TotalTime).TimeOfDay);
                            var totalpatientcount = Convert.ToInt32(totalpatients.TotalTime);
                            TimeSpan result = TimeSpan.FromTicks(ts.Ticks / totalpatientcount);


                            item.TotalTime = result.ToString(@"hh\:mm\:ss");
                            item.TotalHours = item.TotalTime != null ? ((Convert.ToDateTime(item.TotalTime).Second + (Convert.ToDateTime(item.TotalTime).Hour * 60) + (Convert.ToDateTime(item.TotalTime).Minute * 60))).ToString("F2") : "0";
                            sortedata.Insert(2, item);
                        }
                    }
               

                // sortedata.AddRange(unsortedata);
                LiaisonDashBoardViewModel liaisonDashBoardViewModel = new LiaisonDashBoardViewModel();
                liaisonDashBoardViewModel.liaisonDashBoards = sortedata;
                liaisonDashBoardViewModel.liaisonDashBoards1 = unsortedata;
                liaisonDashBoardViewModel.liaisonDashBoardsTwilio = twiliodata;
                liaisonDashBoardViewModel.liaisonDashBoardsGetPatientReject = rejectedPatients;
                liaisonDashBoardViewModel.LiasionID = user.CCMid.Value;
                liaisonDashBoardViewModel.From = From;
                liaisonDashBoardViewModel.To = To;
               

                return PartialView(liaisonDashBoardViewModel);
                }
                catch (Exception ex)
                {

                }
                return PartialView(new LiaisonDashBoardViewModel());

            }
        }
    
        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup")]
        public async Task<PartialViewResult> _DashBoardLiaisonHourly(string userId, DateTime Date)
        {
            try
            {


                var user = string.IsNullOrEmpty(userId)
                              ? _db.Users.Find(User.Identity.GetUserId())
                              : _db.Users.Find(userId);
                var liaisonortranslator = HelperExtensions.isTranslator(user.Id);
                var patientids = _db.Patients.AsNoTracking().Where(x => x.LiaisonId == user.CCMid).Select(x => x.Id).ToList();
                if (liaisonortranslator == true)
                {
                    patientids = _db.Patients.AsNoTracking().Where(x => x.TranslatorId == user.CCMid).Select(x => x.Id).ToList();
                }
                
                var ReviewTimes = _db.ReviewTimeCcms.AsNoTracking().Where(x =>  x.UserId==user.Id).ToList().Where(x => x.StartTime.Date == Date.Date && x.EndTime != null).ToList();
                var CallHistories = _db.CallHistories.AsNoTracking().Where(x =>  x.LiaisonId==user.CCMid).ToList().Where(x => x.StartTime != null && x.StartTime.Value.Date == Date.Date && x.EndTime != null).ToList();
                var LoginTimes = _db.LoginHistories.AsNoTracking().Where(x => x.UserId == userId).ToList().Where(x => x.LoginDateTime.Date == Date.Date).ToList();
                var Careplansubmitted = _db.CCMCycleStatuses.AsNoTracking().Where(x =>  x.CcmClinicalSignOffDate != null && x.SubmittedBy==userId).ToList().Where(x => x.CcmClinicalSignOffDate.Value.Date == Date.Date).ToList();
                var CareplansubmittedTranslators = _db.CCMCycleStatuses.AsNoTracking().Where(x =>  x.CcmReadyforClinicalSignOffDate != null && x.SubmittedByReadyforClinicalSignoff == userId).ToList().Where(x => x.CcmReadyforClinicalSignOffDate.Value.Date == Date.Date).ToList();
                var weekdayname = Date.Date.DayOfWeek.ToString();
                var Bussinesshours = _db.doctorTimings.Where(x => x.LiaisonID == user.CCMid).ToList().Where(x => x.WeekDayName == weekdayname).FirstOrDefault();
                ViewBag.Bussinesshours = Bussinesshours?.StartTime.ToString("hh:mm tt") + " To: " + Bussinesshours?.EndTime.ToString("hh:mm tt");
                List<LiaisonHourlyPerformance> liaisonHourlyPerformances = new List<LiaisonHourlyPerformance>();
                int starthourbussiness = 8;
                int endhourbussiness = 20;
                if (Bussinesshours != null)
                {
                    starthourbussiness = Bussinesshours.StartTime.Hour;
                    endhourbussiness = Bussinesshours.EndTime.Hour;
                }

                for (int i = starthourbussiness; i <= endhourbussiness - 1; i++)
                {
                    LiaisonHourlyPerformance liaisonHourlyPerformance = new LiaisonHourlyPerformance();
                    liaisonHourlyPerformance.Hour = i;
                    if (i == 0)
                    {
                        liaisonHourlyPerformance.HourName = (12).ToString() + ":01 AM-" + (1).ToString() + ":00 AM";
                    }
                    else
                    {
                        if (i > 12)
                        {
                            liaisonHourlyPerformance.HourName = (i - 12).ToString() + ":01 PM-" + (i - 11).ToString() + ":00 PM";
                        }
                        else
                        {
                            if (i == 12)
                            {
                                liaisonHourlyPerformance.HourName = (12).ToString() + ":01 PM-" + (1).ToString() + ":00 PM";
                            }
                            else
                            {
                                liaisonHourlyPerformance.HourName = (i).ToString() + ":01 AM-" + (i + 1).ToString() + ":00 AM";
                            }



                        }
                    }


                    List<MinuteandType> MinuteandTypes = new List<MinuteandType>();
                    for (int j = 0; j <= 59; j++)
                    {
                        MinuteandType minuteandType = new MinuteandType();
                        minuteandType.Minute = j;
                        minuteandType.Type = "Red";
                        MinuteandTypes.Add(minuteandType);
                    }
                    liaisonHourlyPerformance.minuteandTypes = MinuteandTypes;
                    liaisonHourlyPerformances.Add(liaisonHourlyPerformance);
                }
                if (userId != "")
                {


                    foreach (var item in LoginTimes)
                    {
                        try
                        {


                            var starthour = item.LoginDateTime.Hour;
                            var startmin = item.LoginDateTime.Minute;
                            if (starthour < 8)
                            {
                                starthour = 8;
                            }
                            var endhour = item.LogOutDateTime == null ? 21 : item.LogOutDateTime.Value.Hour;
                            var endmin = item.LogOutDateTime == null ? 59 : item.LogOutDateTime.Value.Minute;
                            if (starthour == endhour)
                            {
                                var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == starthour).FirstOrDefault();
                                for (int i = startmin; i <= endmin; i++)
                                {
                                    liasonhourobj.minuteandTypes[i].Type = "Orange";
                                }
                            }
                            if (endhour > starthour)
                            {
                                for (int j = starthour; j <= endhour; j++)
                                {
                                    var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == j).FirstOrDefault();
                                    if (j == starthour)
                                    {
                                        for (int i = startmin; i <= 59; i++)
                                        {
                                            liasonhourobj.minuteandTypes[i].Type = "Orange";
                                        }
                                    }
                                    else if (j > starthour && j < endhour)
                                    {
                                        for (int i = 0; i <= 59; i++)
                                        {
                                            liasonhourobj.minuteandTypes[i].Type = "Orange";
                                        }
                                    }
                                    else if (j == endhour)
                                    {
                                        for (int i = 0; i <= endmin; i++)
                                        {
                                            liasonhourobj.minuteandTypes[i].Type = "Orange";
                                        }
                                    }

                                }

                            }
                        }
                        catch (Exception ex)
                        {


                        }
                    }
                    var lastendhour = -1;
                    var lastendmin = -1;
                    int counter = 0;
                    DateTime? lastendtime = null;
                    try
                    {
                        foreach (var item in ReviewTimes)
                        {
                            try
                            {


                                var starthour = item.StartTime.Hour;
                                var startmin = item.StartTime.Minute;
                                var endhour = item.EndTime.Value.Hour;
                                var endmin = item.EndTime.Value.Minute;
                                if (lastendtime != null)
                                {
                                    var timediff = item.StartTime - lastendtime;
                                    if (timediff.Value.TotalMinutes <= 2)
                                    {
                                        var starthour1 = lastendtime.Value.Hour;
                                        var startmin1 = lastendtime.Value.Minute;
                                        var endhour1 = item.StartTime.Hour;
                                        var endmin1 = item.StartTime.Minute;
                                        if (starthour1 == endhour1)
                                        {
                                            var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == starthour1).FirstOrDefault();
                                            for (int i = startmin1; i <= endmin1; i++)
                                            {
                                                liasonhourobj.minuteandTypes[i].Type = "Yellow";
                                            }
                                        }
                                        else if (endhour1 > starthour1)
                                        {
                                            for (int j = starthour1; j <= endhour1; j++)
                                            {
                                                var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == j).FirstOrDefault();
                                                if (j == starthour)
                                                {

                                                    for (int i = startmin1; i <= 59; i++)
                                                    {
                                                        liasonhourobj.minuteandTypes[i].Type = "Yellow";
                                                    }
                                                }

                                                else if (j == endhour)
                                                {
                                                    for (int i = 0; i < endmin1; i++)
                                                    {
                                                        liasonhourobj.minuteandTypes[i].Type = "Yellow";
                                                    }
                                                }

                                            }

                                        }

                                    }

                                }
                                if (starthour == endhour)
                                {
                                    var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == starthour).FirstOrDefault();
                                    for (int i = startmin; i <= endmin; i++)
                                    {
                                        liasonhourobj.minuteandTypes[i].Type = "Green";
                                    }
                                }

                                else if (endhour > starthour)
                                {
                                    for (int j = starthour; j <= endhour; j++)
                                    {
                                        var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == j).FirstOrDefault();
                                        if (j == starthour)
                                        {

                                            for (int i = startmin; i <= 59; i++)
                                            {
                                                liasonhourobj.minuteandTypes[i].Type = "Green";
                                            }
                                        }
                                        else if (j > starthour && j < endhour)
                                        {

                                            for (int i = 0; i <= 59; i++)
                                            {
                                                liasonhourobj.minuteandTypes[i].Type = "Green";
                                            }
                                        }
                                        else if (j == endhour)
                                        {
                                            for (int i = 0; i <= endmin; i++)
                                            {
                                                liasonhourobj.minuteandTypes[i].Type = "Green";
                                            }
                                        }

                                    }

                                }


                                lastendtime = item.EndTime;
                            }
                            catch (Exception ex)
                            {


                            }
                        }
                        foreach (var item in CallHistories)
                        {
                            try
                            {


                                var timediff = item.StartTime - item.EndTime.Value;
                                var starthour = item.StartTime.Value.Hour;
                                var startmin = item.StartTime.Value.Minute;
                                var endhour = item.EndTime.Value.Hour;
                                var endmin = item.EndTime.Value.Minute;
                                if (starthour == endhour)
                                {
                                    var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == starthour).FirstOrDefault();
                                    for (int i = startmin; i <= endmin; i++)
                                    {
                                        liasonhourobj.minuteandTypes[i].Type = "DarkGreen";
                                    }
                                }
                                if (endhour > starthour)
                                {
                                    for (int j = starthour; j <= endhour; j++)
                                    {
                                        var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == j).FirstOrDefault();
                                        if (j == starthour)
                                        {
                                            for (int i = startmin; i <= 59; i++)
                                            {
                                                liasonhourobj.minuteandTypes[i].Type = "DarkGreen";
                                            }
                                        }
                                        else if (j > starthour && j < endhour)
                                        {
                                            for (int i = 0; i <= 59; i++)
                                            {
                                                liasonhourobj.minuteandTypes[i].Type = "DarkGreen";
                                            }
                                        }
                                        else if (j == endhour)
                                        {
                                            for (int i = 0; i <= endmin; i++)
                                            {
                                                liasonhourobj.minuteandTypes[i].Type = "DarkGreen";
                                            }
                                        }

                                    }

                                }
                            }
                            catch (Exception ex)
                            {


                            }
                        }
                        foreach (var item in Careplansubmitted)
                        {
                            try
                            {

                                var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == item.CcmClinicalSignOffDate.Value.Hour && item.CcmRejectedDate ==null).FirstOrDefault();
                                liasonhourobj.minuteandTypes.Where(x => x.Minute == item.CcmClinicalSignOffDate.Value.Minute).FirstOrDefault().Type = "Blue";

                            }
                            catch (Exception ex)
                            {


                            }
                        }
                        foreach (var item in Careplansubmitted)
                        {
                            try
                            {

                                var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == item.CcmClinicalSignOffDate.Value.Hour && item.CcmRejectedDate != null).FirstOrDefault();
                                liasonhourobj.minuteandTypes.Where(x => x.Minute == item.CcmClinicalSignOffDate.Value.Minute).FirstOrDefault().Type = "DarkBlue";

                            }
                            catch (Exception ex)
                            {


                            }
                        }
                        foreach (var item in CareplansubmittedTranslators)
                        {
                            try
                            {

                                var liasonhourobj = liaisonHourlyPerformances.Where(x => x.Hour == item.CcmReadyforClinicalSignOffDate.Value.Hour).FirstOrDefault();
                                liasonhourobj.minuteandTypes.Where(x => x.Minute == item.CcmReadyforClinicalSignOffDate.Value.Minute).FirstOrDefault().Type = "SkyBlue";

                            }
                            catch (Exception ex)
                            {


                            }
                        }
                    }
                    catch (Exception ex)
                    {


                    }
                }
                List<string> lstTypes = new List<string>() { "Red", "Orange", "Yellow", "Green", "DarkGreen", "Blue", "DarkBlue", "SkyBlue" };
                foreach (var item in liaisonHourlyPerformances)
                {
                    item.minuteandTypesPercent = new List<MinuteandType>();
                    foreach (var type in lstTypes)
                    {
                        MinuteandType minuteandType = new MinuteandType();
                        minuteandType.Type = type;
                        if (type != "Blue" && type != "SkyBlue" && type !="DarkBlue")
                        {
                            var percentage = Convert.ToInt32((Convert.ToDecimal(item.minuteandTypes.Where(x => x.Type == type).ToList().Count) / 60) * 100);
                            minuteandType.MinutesOnly = Convert.ToInt32((Convert.ToDecimal(item.minuteandTypes.Where(x => x.Type == type).ToList().Count)));
                            minuteandType.Minute = percentage;
                        }
                        else
                        {
                            var percentage = Convert.ToInt32((Convert.ToDecimal(item.minuteandTypes.Where(x => x.Type == type).ToList().Count)));
                            minuteandType.MinutesOnly = Convert.ToInt32((Convert.ToDecimal(item.minuteandTypes.Where(x => x.Type == type).ToList().Count)));
                            minuteandType.Minute = percentage;

                        }

                        item.minuteandTypesPercent.Add(minuteandType);
                    }

                    //item.minuteandTypesPercent = item.minuteandTypes.GroupBy(x => x.Type).Select(group => new MinuteandType {
                    //    Type = group.Key,
                    //    Minute =Convert.ToInt32((Convert.ToDecimal(group.Count())/ 60)*100)
                    //}).ToList();
                }

                var res = liaisonHourlyPerformances.SelectMany(a => a.minuteandTypesPercent).ToList().GroupBy(x => x.Type).Select(group => new MinuteandType
                {
                    Type = group.Key,
                    Minute = group.Sum(x => x.Minute),
                    MinutesOnly = group.Sum(x => x.MinutesOnly)
                }).ToList();

                //  var r1 = liaisonHourlyPerformances.SelectMany(a => a.minuteandTypesPercent).Average(b => b.Minute);
                ViewBag.Totals = res;

                ViewBag.DateStr = Date.ToString("dddd, dd MMMM yyyy");
                return PartialView(liaisonHourlyPerformances);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message + ex.InnerException;
                return PartialView(new List<LiaisonHourlyPerformance>());

            }

            //var reviewtime = ReviewTimes.Where(x => (x.StartTime.Hour == i && x.StartTime.Minute == j) || (x.EndTime.Value.Hour == i && x.EndTime.Value.Minute == j)).FirstOrDefault();
            //var callhistory = CallHistories.Where(x => (x.StartTime.Value.Hour == i && x.StartTime.Value.Minute == j) || (x.EndTime.Value.Hour == i && x.EndTime.Value.Minute == j)).FirstOrDefault();
            //var
        }
        [Authorize(Roles = "Liaison, Admin, Physician, PhysiciansGroup, LiaisonGroup")]
        public async Task<PartialViewResult> _DashBoardLiaisonAll(string userId, DateTime From, DateTime To)
        {
            if (userId == "null")
            {
                return PartialView(new List<LiaisonDashBoardViewModel>());
            }
            string[] liasionIDs = userId.Split(',');
            List<LiaisonDashBoardViewModel> liaisonDashBoardViewModels = new List<LiaisonDashBoardViewModel>();
            foreach (var liasionid in liasionIDs)
            {
                var useridint = Convert.ToInt32(liasionid);
                var useridstr = _db.Users.Where(x => x.Role == "Liaison" && x.CCMid ==useridint ).FirstOrDefault().Id;
                using (var context = new ApplicationdbContect())
                {
                    var istranslator = HelperExtensions.isTranslator(liasionid);

                    var liaisonDashBoards = context.Database
                          .SqlQuery<LiaisonDashBoard>("GetDashBoardDataforMultipleLiasions @LiaisonID, @Fromdate, @ToDate, @isTranslator, @UserID",
        new SqlParameter("LiaisonID", Convert.ToInt32(liasionid)),
        new SqlParameter("Fromdate", From),
        new SqlParameter("ToDate", To),
        new SqlParameter("isTranslator", istranslator == true ? 1 : 0),
    new SqlParameter("UserID", useridstr)
        )
                          .ToList();



                    var liaisonName = liaisonDashBoards.Where(x => x.DataType == "LiaisonName").FirstOrDefault();
                    liaisonDashBoards = liaisonDashBoards.Where(x => x.DataType != "LiaisonName").ToList();
                    liaisonDashBoards = liaisonDashBoards.Where(x => x.ShowHide == 1).ToList();
                    var totalpatients = liaisonDashBoards.Where(x => x.Page == "Total Patients").FirstOrDefault();
                    var totaltimespent = liaisonDashBoards.Where(x => x.Page == "Total Time Spent on Patients").FirstOrDefault();
                    try
                    {
                        if (totalpatients != null && totaltimespent != null)
                        {
                            if (totalpatients.TotalTime != null && totaltimespent.TotalTime != null)
                            {
                                LiaisonDashBoard item = new LiaisonDashBoard();
                                item.Page = "Average Time";

                                TimeSpan ts = (Convert.ToDateTime(totaltimespent.TotalTime).TimeOfDay);
                                var totalpatientcount = Convert.ToInt32(totalpatients.TotalTime);
                                TimeSpan result = TimeSpan.FromTicks(ts.Ticks / totalpatientcount);


                                item.TotalTime = result.ToString(@"hh\:mm\:ss");
                                item.TotalHours = item.TotalTime != null ? ((Convert.ToDateTime(item.TotalTime).Second + (Convert.ToDateTime(item.TotalTime).Hour * 60) + (Convert.ToDateTime(item.TotalTime).Minute * 60))).ToString("F2") : "0";
                                liaisonDashBoards.Insert(2, item);
                            }
                            else
                            {
                                LiaisonDashBoard item = new LiaisonDashBoard();
                                item.Page = "Average Time";
                                item.TotalTime = "0";
                                liaisonDashBoards.Insert(2, item);

                            }
                        }
                        else
                        {
                            LiaisonDashBoard item = new LiaisonDashBoard();
                            item.Page = "Average Time";
                            item.TotalTime = "0";
                            liaisonDashBoards.Insert(2, item);

                        }
                    }
                    catch (Exception ex)
                    {


                    }

                    // sortedata.AddRange(unsortedata);
                    LiaisonDashBoardViewModel liaisonDashBoardViewModel = new LiaisonDashBoardViewModel();
                    liaisonDashBoardViewModel.liaisonDashBoards = liaisonDashBoards;


                    liaisonDashBoardViewModel.LiaisonName = liaisonName?.Page;
                    liaisonDashBoardViewModels.Add(liaisonDashBoardViewModel);
                    //var jsonSerialiser = new JavaScriptSerializer();
                    //var json = jsonSerialiser.Serialize(liaisonDashBoards);
                    //liaisonDashBoardViewModel.JsonData = json;



                }

            }
            liaisonDashBoardViewModels[0].Datestr = "From: " + From.ToString("dddd, dd MMMM yyyy") + "  To: " + To.ToString("dddd, dd MMMM yyyy");
            return PartialView(liaisonDashBoardViewModels);
            return PartialView(new List<LiaisonDashBoardViewModel>());
        }
        private DashBoardViewModel GetPatientDashboardviewmodelbydate(List<patients_historyviewmodel> patients, DateTime date, List<EnrollmentStatus> enrollmnetstatuses, List<EnrollmentSubstatusReason> substatusreasons)
        {
            var dashboard = new DashBoardViewModel();
            patients = patients.Where(x => x.UpdatedOn <= date).ToList();
            //New Statuses data
            List<NewDashBoardViewModel> lstStatusCounts = new List<NewDashBoardViewModel>();
            dashboard.Datestr = date.ToString("MM/dd/yyyy");

            dashboard.TotalPatients = patients.Count();
            foreach (var enrollmentstatus in enrollmnetstatuses)
            {
                NewDashBoardViewModel newDashBoardViewModel = new NewDashBoardViewModel();
                newDashBoardViewModel.StatusName = enrollmentstatus.Name;
                newDashBoardViewModel.Totalcount = patients.Count(p => p.EnrollmentStatus == enrollmentstatus.Name);
                newDashBoardViewModel.SubStatuses = new List<NewDashBoardViewModel>();
                var substatuses = _db.EnrollmentSubStatuss.Where(x => x.EnrollmentStatusID == enrollmentstatus.Id).OrderBy(item => item.OrderBy).ToList();
                foreach (var substatus in substatuses)
                {
                    NewDashBoardViewModel newDashBoardViewModelSub = new NewDashBoardViewModel();
                    newDashBoardViewModelSub.StatusName = substatus.Name;
                    newDashBoardViewModelSub.Totalcount = patients.Count(p => p.EnrollmentSubStatus == substatus.Name);

                    if (substatus.Name == "In-Active Enrolled")
                    {

                        newDashBoardViewModelSub.SubStatuses = new List<NewDashBoardViewModel>();
                        foreach (var substatusreason in substatusreasons)
                        {
                            NewDashBoardViewModel objsubstatusreason = new NewDashBoardViewModel();
                            objsubstatusreason.StatusName = substatusreason.Name;
                            objsubstatusreason.Totalcount = patients.Count(p => p.EnrollmentSubStatusReason == substatusreason.Name);
                            newDashBoardViewModelSub.SubStatuses.Add(objsubstatusreason);
                        }
                    }
                    newDashBoardViewModel.SubStatuses.Add(newDashBoardViewModelSub);
                }
                lstStatusCounts.Add(newDashBoardViewModel);
            }
            dashboard.TotalPatients = patients.Count();

            dashboard.newDashBoardViewModels = lstStatusCounts;
            return dashboard;
        }
        private void GetPatientDashboardviewmodelbydate(List<patients_historyviewmodel> patients, DateTime date, List<EnrollmentStatus> enrollmnetstatuses, List<EnrollmentSubstatusReason> substatusreasons, DashBoardViewModel dashboard)
        {

            patients = patients.Where(x => x.UpdatedOn <= date).ToList();
            //New Statuses data
            // List<NewDashBoardViewModel> lstStatusCounts = new List<NewDashBoardViewModel>();
            dashboard.Datestr = date.ToString("MM/dd/yyyy");

            dashboard.TotalPatients1 = patients.Count();
            foreach (var enrollmentstatus in enrollmnetstatuses)
            {
                var newDashBoardViewModel = dashboard.newDashBoardViewModels.Where(item => item.StatusName == enrollmentstatus.Name).FirstOrDefault();

                newDashBoardViewModel.Totalcount1 = patients.Count(p => p.EnrollmentStatus == enrollmentstatus.Name);

                var substatuses = _db.EnrollmentSubStatuss.Where(x => x.EnrollmentStatusID == enrollmentstatus.Id).OrderBy(item => item.OrderBy).ToList();
                foreach (var substatus in substatuses)
                {
                    NewDashBoardViewModel newDashBoardViewModelSub = newDashBoardViewModel.SubStatuses.Where(item => item.StatusName == substatus.Name).FirstOrDefault(); ;


                    newDashBoardViewModelSub.Totalcount1 = patients.Count(p => p.EnrollmentSubStatus == substatus.Name);

                    if (substatus.Name == "In-Active Enrolled")
                    {


                        foreach (var substatusreason in substatusreasons)
                        {
                            NewDashBoardViewModel objsubstatusreason = newDashBoardViewModelSub.SubStatuses.Where(item => item.StatusName == substatusreason.Name).FirstOrDefault(); ;

                            objsubstatusreason.Totalcount1 = patients.Count(p => p.EnrollmentSubStatusReason == substatusreason.Name);

                        }
                    }

                }

            }
            dashboard.TotalPatients1 = patients.Count();

            // dashboard.newDashBoardViewModels = lstStatusCounts;

        }
        private static async Task<DashBoardViewModel> PopulateDashBoardAsync(IQueryable<Patient> patients)
        {
            var today = DateTime.Today.Date;
            var tomorrow = today.AddDays(1);

            return new DashBoardViewModel
            {
                NotEligible = await patients.CountAsync(p => p.EnrollmentStatus == "Not Eligible"),
                TotalPatients = await patients.CountAsync(),
                NotAssigned = await patients.CountAsync(p => p.LiaisonId == null),
                Assigned = await patients.CountAsync(p => p.EnrollmentStatus == "Not Enrolled" && p.LiaisonId != null),
                NotEnrolled = await patients.CountAsync(p => p.EnrollmentStatus == "Not Enrolled"),
                InProgress = await patients.CountAsync(p => p.EnrollmentStatus == "In Progress"),
                LeftVoiceMessage = await patients.CountAsync(p => p.EnrollmentStatus == "Left Voice Message 1" ||
                                                                  p.EnrollmentStatus == "Left Voice Message 2" ||
                                                                  p.EnrollmentStatus == "Left Voice Message 3"),
                InvalidPhoneNumber = await patients.CountAsync(p => p.EnrollmentStatus == "Invalid Phone Number"),
                PatientNotSeeingMd = await patients.CountAsync(p => p.EnrollmentStatus == "Patient Not Seeing MD"),
                NotQualified = await patients.CountAsync(p => p.EnrollmentStatus == "Not Qualified"),
                Refused = await patients.CountAsync(p => p.EnrollmentStatus == "Refused"),
                Deceased = await patients.CountAsync(p => p.EnrollmentStatus == "Deceased"),

                InCcm = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Enrolled"),
                InClinicalSignOff = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Clinical Sign-Off"),
                InClaimSubmission = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission"),

                Code99490 = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission" &&
                                                          (p.CcmBillingCode == "CCM 20 Minutes - CPT 99490" ||
                                                           p.CcmBillingCode2 == "CCM 20 Minutes - CPT 99490")),
                Code99487 = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission" &&
                                                          (p.CcmBillingCode == "Complex CCM 30 Minutes - CPT 99487" ||
                                                           p.CcmBillingCode2 == "Complex CCM 30 Minutes - CPT 99487")),
                Code99489 = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission" &&
                                                          (p.CcmBillingCode == "Complex CCM 60 Minutes - CPT 99489" ||
                                                           p.CcmBillingCode2 == "Complex CCM 60 Minutes - CPT 99489")),
                CallsDueToday = await patients.CountAsync(p => DbFunctions.TruncateTime(p.AppointmentDate) == today),
                CallsDueTomorrow = await patients.CountAsync(p => DbFunctions.TruncateTime(p.AppointmentDate) == tomorrow),
                PastCallDues = await patients.CountAsync(p => DbFunctions.TruncateTime(p.AppointmentDate) < today && p.AppointmentDate.HasValue)
            };
        }
        private static async Task<DashBoardViewModel> PopulateDashBoardAsync(IQueryable<Patients_History> patients)
        {
            var today = DateTime.Today.Date;
            var tomorrow = today.AddDays(1);

            return new DashBoardViewModel
            {
                NotEligible = await patients.CountAsync(p => p.EnrollmentStatus == "Not Eligible"),
                TotalPatients = await patients.CountAsync(),
                NotAssigned = await patients.CountAsync(p => p.LiaisonId == null),
                Assigned = await patients.CountAsync(p => p.EnrollmentStatus == "Not Enrolled" && p.LiaisonId != null),
                NotEnrolled = await patients.CountAsync(p => p.EnrollmentStatus == "Not Enrolled"),
                InProgress = await patients.CountAsync(p => p.EnrollmentStatus == "In Progress"),
                LeftVoiceMessage = await patients.CountAsync(p => p.EnrollmentStatus == "Left Voice Message 1" ||
                                                                  p.EnrollmentStatus == "Left Voice Message 2" ||
                                                                  p.EnrollmentStatus == "Left Voice Message 3"),
                InvalidPhoneNumber = await patients.CountAsync(p => p.EnrollmentStatus == "Invalid Phone Number"),
                PatientNotSeeingMd = await patients.CountAsync(p => p.EnrollmentStatus == "Patient Not Seeing MD"),
                NotQualified = await patients.CountAsync(p => p.EnrollmentStatus == "Not Qualified"),
                Refused = await patients.CountAsync(p => p.EnrollmentStatus == "Refused"),
                Deceased = await patients.CountAsync(p => p.EnrollmentStatus == "Deceased"),

                InCcm = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Enrolled"),
                InClinicalSignOff = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Clinical Sign-Off"),
                InClaimSubmission = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission"),

                Code99490 = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission" &&
                                                          (p.CcmBillingCode == "CCM 20 Minutes - CPT 99490" ||
                                                           p.CcmBillingCode2 == "CCM 20 Minutes - CPT 99490")),
                Code99487 = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission" &&
                                                          (p.CcmBillingCode == "Complex CCM 30 Minutes - CPT 99487" ||
                                                           p.CcmBillingCode2 == "Complex CCM 30 Minutes - CPT 99487")),
                Code99489 = await patients.CountAsync(p => p.EnrollmentStatus == "Enrolled" && p.CcmStatus == "Claims Submission" &&
                                                          (p.CcmBillingCode == "Complex CCM 60 Minutes - CPT 99489" ||
                                                           p.CcmBillingCode2 == "Complex CCM 60 Minutes - CPT 99489")),
                CallsDueToday = await patients.CountAsync(p => DbFunctions.TruncateTime(p.AppointmentDate) == today),
                CallsDueTomorrow = await patients.CountAsync(p => DbFunctions.TruncateTime(p.AppointmentDate) == tomorrow),
                PastCallDues = await patients.CountAsync(p => DbFunctions.TruncateTime(p.AppointmentDate) < today && p.AppointmentDate.HasValue)
            };
        }

        public ActionResult CommingSoon(){
            return View();
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