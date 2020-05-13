using CCM.Helpers;
using CCM.Models.CCMBILLINGS;
using CCM.Models.CCMBILLINGS.ViewModels;
using CCM.Models.DataModels;
using CCM.Models.ViewModels;
using CCM.signalr.hubs;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Transactions;
namespace CCM.Models.BulkChanges
{
    public class TempBulkChangeViewModel
    {
        public string Title { get; set; }
        public List<BulkChangesLogViewModel> BulkChangesLogList { get; set; }
        public TempBulkChangeViewModel()
        {
            this.BulkChangesLogList = new List<BulkChangesLogViewModel>();
        }
    }

    public class BulkChangesModel
    {
        private bool CheckConnectionState(ApplicationdbContect db)
        {
            if (db.Database.Connection.State != ConnectionState.Open)
            {
                db.Database.Connection.Open();
                db.Database.Connection.State.ConsoleMassage("Connection State");
                return true;
            }
            else
            {

                return false;
            }
        }
        public IPrincipal User { get; set; }

        internal TempBulkChangeViewModel EnrollPatients(BulkChangeViewModel bulkChange)
        {
            TempBulkChangeViewModel result = new TempBulkChangeViewModel();
            result.Title = "Change Status=<b style=\"color:red\">Yes</b> and Selected Patients are <b style=\"color:red\">Not Active Enrolled</b> ";
            var _db1 = new ApplicationdbContect();
            //_db1.Database.Connection.Open();
            int ind = 1;

            foreach (var patients in bulkChange.PatientsList)
            {

                //there will be only those patients which are not currently enrolled 
                //we have to enroll them

                var transaction = _db1.Database.BeginTransaction();
                try
                {
                    if (_db1.Database.Connection.State != ConnectionState.Open)
                    {
                        _db1.Database.Connection.Open();
                        //_db1.Database.Connection.State.ConsoleMassage("Connection State");

                    }
                    #region Saving Patient Data
                    var patient = _db1.Patients.Where(p => p.Id == patients).FirstOrDefault();


                    var oldCycle = patient.Cycle;
                    if (!string.IsNullOrEmpty(bulkChange.EnrollmentStatus))
                    {
                        patient.EnrollmentStatus = bulkChange.EnrollmentStatus;
                    }
                    if (!string.IsNullOrEmpty(bulkChange.EnollmentSubStatus))
                    {
                        patient.EnrollmentSubStatus = bulkChange.EnollmentSubStatus;
                    }
                    if (!string.IsNullOrEmpty(bulkChange.EnrollemntStatusReson))
                    {
                        patient.EnrollmentSubStatusReason = bulkChange.EnrollemntStatusReson;

                    }
                    patient.CcmStatus = "Enrolled";
                    _db1.Entry(patient).State = EntityState.Modified;
                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    {
                        PatientId = patient.Id,
                        ResultMassage = "021 Patient id=<b style=\"color:forestgreen\">" + patient.Id
                        + "</b> EnrollmentStatus=<b style=\"color:forestgreen\">" + patient.EnrollmentStatus + "</b> " +
                        ", EnrollmentSubStatus =<b style=\"color:forestgreen\">" + patient.EnrollmentSubStatus + "</b> " +
                        ", EnrollmentSubStatusReason= <b style=\"color:forestgreen\">" + patient.EnrollmentSubStatusReason + "</b> are Updated",
                        Status = (int)BulkChangesStatus.success
                    });

                    if (string.IsNullOrEmpty(patient.CCMEnrolledOn.ToString()))
                    {
                        // if enrolled on is null

                        patient.CCMEnrolledOn = DateTime.Now;
                        patient.CCMEnrolledBy = User.Identity.GetUserId();
                        if (patient.Cycle == 0)
                        {
                            patient.Cycle = (((DateTime.Now.Year - patient.CCMEnrolledOn.Value.Year) * 12) + DateTime.Now.Month - patient.CCMEnrolledOn.Value.Month) + 1;

                        }
                        _db1.Entry(patient).State = EntityState.Modified;
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "Patient id=<b style=\"color:forestgreen\">"
                            + patient.Id + "</b> Cycle Updated from Cycle=<b style=\"color:forestgreen\">"
                            + oldCycle + "</b> to New Cycle= <b style=\"color:forestgreen\">" + patient.Cycle + "</b> .",
                            Status = (int)BulkChangesStatus.success
                        });
                    }
                    #endregion
                    #region Disabling Pre-Counselor Pre-Translator
                    //disabling PreLiasion 
                    if (patient.Patients_PreLiaisonsId > 0)
                    {
                        // _db1.Database.Connection.State.ToString().ConsoleMassage("Connection");
                        var preliaison = _db1.Patients_PreLiaisons.Where(p => p.Id == patient.Patients_PreLiaisonsId).FirstOrDefault();
                        if (preliaison != null)
                        {
                            preliaison.Status = false;
                            preliaison.UpdatedBy = User.Identity.GetUserId();
                            preliaison.UpdatedOn = DateTime.Now;
                            _db1.Entry(preliaison).State = EntityState.Modified;
                            //_db1.SaveChanges();
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Patient Pre-Counselor "+ preliaison.LiaisonId!=null? preliaison.LiaisonId.GetLiaisonNameFromID():"Null"+ " and Pre-Translator "+ preliaison.TranslatorId!=null? preliaison.TranslatorId.GetLiaisonNameFromID():"Null"+ " are Disabled now Pre-Counselor Id=" + preliaison.Id,
                                Status = (int)BulkChangesStatus.success
                            });
                        }
                    }
                    #endregion

                    #region Update Final Care Plane
                    var finalcareplans = _db1.FinalCarePlanNotes.Where(x => x.PatientId == patient.Id).ToList().Where(x => x.CarePlanCreatedOn.Value.Month == DateTime.Now.Month && x.CarePlanCreatedOn.Value.Year == DateTime.Now.Year && x.CarePlanCreatedOn != null).ToList();
                    foreach (var item in finalcareplans)
                    {
                        if (item.Cycle != patient.Cycle)
                        {
                            item.Cycle = patient.Cycle;
                            _db1.Entry(item).State = EntityState.Modified;
                            //Db.SaveChanges();
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Patient id=<b style=\"color:forestgreen\">"
                                                    + patient.Id + "</b> Final care Plane cycle are Updated to the cycle " + patient.Cycle + " .",
                                Status = (int)BulkChangesStatus.success
                            });
                        }
                    }
                    #endregion

                    foreach (var categories in bulkChange.EnrollementList)
                    {
                        int? BillingCategoryId = categories.BillingcategoryId.GetNullableInteger();

                        #region Update Review Time Activity
                        var reviewtimeccms = _db1.ReviewTimeCcms.Where(x => x.PatientId == patient.Id && x.Cycle == oldCycle && x.BillingcategoryId == BillingCategoryId).ToList().Where(x => x.StartTime.Date.Month == DateTime.Now.Month).ToList();

                        if (reviewtimeccms.Count() > 0)
                        {
                            reviewtimeccms.ForEach(x =>
                            {
                                x.Cycle = patient.Cycle;
                                _db1.Entry(x).State = EntityState.Modified;
                            });
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "0121 Patient id=<b style=\"color:forestgreen\">"
                           + patient.Id + "</b> ReviewTime Activities cycle are Updated from Cycle=<b style=\"color:forestgreen\">" + oldCycle + "</b> to  cycle= <b style=\"color:forestgreen\">" + patient.Cycle + "</b> for Billing Category "+BillingCategoryId.GetInteger().GetBillingCatagoryNameById()+".",
                                Status = (int)BulkChangesStatus.success
                            });
                        }
                        #endregion

                        
                        int? SelectedLiaisonId = categories.LiaisonId.GetNullableInteger();
                        int? SelectedTranslatorId = categories.TranslatorId.GetNullableInteger();

                        #region Updating CCM Counselor and Translator In Patient Table
                        if (BillingCategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                        {
                            //if its ccm
                            
                            patient.LiaisonId = SelectedLiaisonId;
                            patient.TranslatorId = SelectedTranslatorId;
                            patient.UpdatedBy = User.Identity.GetUserId();
                            patient.UpdatedOn = DateTime.Now;
                            _db1.Entry(patient).State = EntityState.Modified;
                            //_db1.SaveChanges();
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Updating Patient Counselor "+ SelectedLiaisonId!=null? SelectedLiaisonId.GetLiaisonNameFromID():"Null" 
                                + " and Translator "+SelectedTranslatorId!=null? SelectedTranslatorId.GetLiaisonNameFromID():"Null"+" for Billing Category "+BillingCategoryId.GetInteger().GetBillingCatagoryNameById(),
                                Status = (int)BulkChangesStatus.success
                            });
                        }
                        #endregion

                        int? oldLiasionOfCurrentBillingCate = null;
                        int? oldTranslatorOfCurrentBillingCate = null;

                        #region  disabling old Post Counselor if any one there 
                        //old code
                        //var patLiaison = _db1.Patients_BillingCategories.Where(p => p.PatientId == patients && p.BillingCategoryId == BillingCategoryId && p.Status == true && p.IsTranslator == false).FirstOrDefault();

                        var patLiaison = _db1.Patients_BillingCategories.Where(p => p.PatientId == patients && p.BillingCategoryId == BillingCategoryId && p.Status == true).FirstOrDefault();

                        if (patLiaison != null)
                        {
                            oldLiasionOfCurrentBillingCate = patLiaison.LiaisonId;
                            patLiaison.Status = false;
                            patLiaison.UpdatedBy = User.Identity.GetUserId();
                            patLiaison.UpdatedOn = DateTime.Now;

                            _db1.Entry(patLiaison).State = EntityState.Modified;
                        }
                        #endregion
                        #region  adding new  Post counselor to patient
                        Patients_BillingCategories patCat = new Patients_BillingCategories();
                        patCat.BillingCategoryId = BillingCategoryId;
                        patCat.LiaisonId = SelectedLiaisonId;
                        patCat.CreatedOn = DateTime.Now;
                        patCat.CreatedBy = User.Identity.GetUserId();
                        patCat.IsTranslator = false;
                        patCat.Status = true;
                        patCat.PatientId = patients;
                        _db1.Patients_BillingCategories.Add(patCat);
                        //_db1.SaveChanges();
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "Updating Post Counselor from "+ patLiaison.LiaisonId!=null? patLiaison.LiaisonId.GetLiaisonNameFromID():"Null" + " to new Post Counselor " + SelectedLiaisonId!=null? SelectedLiaisonId.GetLiaisonNameFromID():"Null" + " for Category " + BillingCategoryId.GetInteger().GetBillingCatagoryNameById(),
                            Status = (int)BulkChangesStatus.success
                        });
                        #endregion

                        #region Updating Post Translator 
                        if (!string.IsNullOrEmpty(categories.TranslatorId))
                        {
                            #region disabling old translator if any one there
                            var patTranslator = _db1.Patients_BillingCategories.Where(p => p.PatientId == patients && p.BillingCategoryId == BillingCategoryId && p.Status == true && p.IsTranslator == true).FirstOrDefault();
                            oldTranslatorOfCurrentBillingCate = patTranslator.TranslatorId;
                            patTranslator.IsTranslator = true;
                            patTranslator.LiaisonId = SelectedTranslatorId;
                            patTranslator.UpdatedBy = User.Identity.GetUserId();
                            patTranslator.UpdatedOn = DateTime.Now;
                            _db1.Entry(patTranslator).State = EntityState.Modified;
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Updating Post Translator from " + patTranslator != null ? oldTranslatorOfCurrentBillingCate.GetLiaisonNameFromID() : "Null" + " to New Post Translator " + SelectedTranslatorId != null ? SelectedTranslatorId.GetLiaisonNameFromID() : "Null" + " for Category " + BillingCategoryId.GetInteger().GetBillingCatagoryNameById(),
                                Status = (int)BulkChangesStatus.success
                            });
                            #endregion

                            #region old code disabling old translator if any one there

                            //var patTranslator = _db1.Patients_BillingCategories.Where(p => p.PatientId == patients && p.BillingCategoryId == BillingCategoryId && p.Status == true && p.IsTranslator == true).FirstOrDefault();

                            //if (patTranslator != null)
                            //{
                            //    oldTranslatorOfCurrentBillingCate = patTranslator.LiaisonId;

                            //    patTranslator.Status = false;
                            //    patTranslator.UpdatedBy = User.Identity.GetUserId();
                            //    patTranslator.UpdatedOn = DateTime.Now;
                            //    _db1.Entry(patTranslator).State = EntityState.Modified;
                            //}
                            #endregion

                            //no need to add new row
                            //#region Adding new Post Translator to patients
                            //Patients_BillingCategories patCat2 = new Patients_BillingCategories();
                            //patCat2.BillingCategoryId = BillingCategoryId;
                            //patCat2.IsTranslator = true;
                            //patCat2.LiaisonId = SelectedTranslatorId;
                            //patCat2.CreatedOn = DateTime.Now;
                            //patCat2.CreatedBy = User.Identity.GetUserId();
                            //patCat2.Status = true;
                            //patCat2.PatientId = patients;
                            //_db1.Patients_BillingCategories.Add(patCat2);
                            //// _db1.SaveChanges();
                            //result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //{
                            //    PatientId = patient.Id,
                            //    ResultMassage = "Updating Post Translator from "+ patTranslator!=null? patTranslator.LiaisonId.GetLiaisonNameFromID():"Null" + " to New Post Translator " + SelectedTranslatorId!=null? SelectedTranslatorId.GetLiaisonNameFromID():"Null" + " for Category " + BillingCategoryId.GetInteger().GetBillingCatagoryNameById(),
                            //    Status = (int)BulkChangesStatus.success
                            //});
                            //#endregion

                        }
                        #endregion



                        #region MigrateAppointment

                        if (bulkChange.MigrateAppointment == "Yes")
                        {
                            bulkChange.MigrateAppointmentIn_Id.ForEach(c =>
                            {
                                if (BillingCategoryId == c)
                                {
                                    #region Migrate Counselor Appointment
                                    //bool SelectedliasionSCreated = false;
                                    //bool SelectedTranslatorSCreated = false;
                                    if (SelectedLiaisonId > 0)
                                    {
                                        var Selectedliasionschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == SelectedLiaisonId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                        if (Selectedliasionschedule != null)
                                        {
                                            //SelectedliasionSCreated = true;
                                            if (oldLiasionOfCurrentBillingCate != null)
                                            {
                                                var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldLiasionOfCurrentBillingCate && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                                if (pa.Count > 0)
                                                {
                                                    pa.ForEach(x =>
                                                    {
                                                        x.LiaisonID = SelectedLiaisonId;
                                                        x.BillingCategoryId = c;
                                                        x.UpdateOn = DateTime.Now;
                                                        x.UpdatedBy = 0;
                                                        _db1.Entry(x).State = EntityState.Modified;
                                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                        {
                                                            PatientId = patient.Id,
                                                            ResultMassage = "Counselor Appointment Id=" + x.ID + " migrate Successfully to new Counselor " + SelectedLiaisonId.GetLiaisonNameFromID() + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                            Status = (int)BulkChangesStatus.success
                                                        });
                                                    });

                                                    //result = "Liaison has been changed along with Appointments successfully.!";
                                                }
                                                else
                                                {
                                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                    {
                                                        PatientId = patient.Id,
                                                        ResultMassage = "No Upcoming appointment found for this Patient with old Counselor" + oldLiasionOfCurrentBillingCate.GetLiaisonNameFromID() + "For Billing Category " + c.GetBillingCatagoryNameById(),
                                                        Status = (int)BulkChangesStatus.Warning
                                                    });
                                                }

                                            }
                                            else
                                            {
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "Appointment Could Not be Migrate as no old Counselor Found For this Patient for Billing Category " + c.GetBillingCatagoryNameById(),
                                                    Status = (int)BulkChangesStatus.Warning
                                                });
                                            }
                                        }
                                        else
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "Can not migrate Appointment for Billing Category " + c.GetBillingCatagoryNameById() + " as Counselor " + SelectedLiaisonId.GetLiaisonNameFromID() + " Schedules not found, please create Counselor schedule first.!",
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                        }
                                    }
                                    else if (SelectedLiaisonId == null || SelectedLiaisonId == 0)
                                    {
                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                        {
                                            PatientId = patient.Id,
                                            ResultMassage = "Can not migrate Appointment for Billing Category" + c.GetBillingCatagoryNameById() + " as you have not select any Counselor",
                                            Status = (int)BulkChangesStatus.Warning
                                        });
                                    }
                                   
                                    #endregion

                                    #region Migrate Translator Appointment
                                    if (SelectedTranslatorId > 0)
                                    {
                                        var Selectedtranslatorschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == SelectedTranslatorId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                        if (Selectedtranslatorschedule != null)
                                        {
                                            //SelectedliasionSCreated = true;
                                            if (oldTranslatorOfCurrentBillingCate != null)
                                            {
                                                var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldTranslatorOfCurrentBillingCate && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                                if (pa.Count > 0)
                                                {
                                                    pa.ForEach(x =>
                                                    {
                                                        x.LiaisonID = SelectedTranslatorId;
                                                        x.BillingCategoryId = c;
                                                        x.UpdateOn = DateTime.Now;
                                                        x.UpdatedBy = 0;
                                                        _db1.Entry(x).State = EntityState.Modified;
                                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                        {
                                                            PatientId = patient.Id,
                                                            ResultMassage = "Translator Appointment Id=" + x.ID + " migrate Successfully to new Translator" + SelectedTranslatorId.GetLiaisonNameFromID() + "for Billing Category" + c.GetBillingCatagoryNameById(),
                                                            Status = (int)BulkChangesStatus.success
                                                        });
                                                    });

                                                }
                                                else
                                                {
                                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                    {
                                                        PatientId = patient.Id,
                                                        ResultMassage = "No Upcoming appointment found for this Patient with old Translator " + oldTranslatorOfCurrentBillingCate.GetLiaisonNameFromID() + " for Billing category " + c.GetBillingCatagoryNameById(),
                                                        Status = (int)BulkChangesStatus.Warning
                                                    });
                                                }

                                            }
                                            else
                                            {
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "Appointment Could Not be Migrate as no old Translator Found For this Patient for Billing Category " + c.GetBillingCatagoryNameById(),
                                                    Status = (int)BulkChangesStatus.Warning
                                                });
                                            }
                                        }
                                        else
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "Can not migrate Appointment for Billing Category " + c.GetBillingCatagoryNameById() + " as Translator " + SelectedTranslatorId.GetLiaisonNameFromID() + " Schedules not found, please create Translator schedule first.!",
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                            //result = "Counselor has been changed successfully And No Activity performed, Counselor schedule is not created, please create schedule first.!";
                                        }
                                    }
                                    else if (SelectedTranslatorId == null || SelectedTranslatorId == 0)
                                    {
                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                        {
                                            PatientId = patient.Id,
                                            ResultMassage = "Can not migrate Appointment for Billing Category " + c.GetBillingCatagoryNameById() + " as you have not select any translator",
                                            Status = (int)BulkChangesStatus.Warning
                                        });
                                    }
                                    
                                    #endregion
                                }

                            });


                        }
                        //else
                        //{
                        //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        //    {
                        //        PatientId = patient.Id,
                        //        ResultMassage = "No Appointment Migrate As you have selected to not Migrate Appointment",
                        //        Status = (int)BulkChangesStatus.Warning
                        //    });
                        //}

                        #endregion
                    }
                    //HelperExtensions.UpdateCurrentMonthActivityfromCycleZeroToOne(patient.Id);

                    patient.UpdatedOn = DateTime.Now;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    _db1.Entry(patient).State = EntityState.Modified;
                    _db1.SaveChanges();
                    transaction.Commit();
                    // patients.ConsoleMassage(ind + " Successful Patient id ");

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var rolebackResult = result.BulkChangesLogList.Where(x => x.PatientId == patients && x.Status == (int)BulkChangesStatus.success).ToList();
                    rolebackResult.ForEach(x =>
                    {
                        result.BulkChangesLogList.Remove(x);
                    });
                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    {
                        PatientId = patients,
                        ResultMassage = "Patient Id= " + patients + "  Saving Failed" + ex.ToString(),
                        Status = (int)BulkChangesStatus.Failed
                    });
                }


                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), ind);
                ind++;
                transaction.Dispose();
            }
            _db1.Database.Connection.Close();
            // _db1.Database.Connection.State.ToString().ConsoleMassage("Connection");
            //result.ConsoleMassage("Result");
            if (result.BulkChangesLogList.Count() == 0)
            {
                result.BulkChangesLogList.Add(new BulkChangesLogViewModel
                {
                    PatientId = 0,
                    ResultMassage = "Some Thing Went Wrong",
                    Status = (int)BulkChangesStatus.Failed
                });
            }
            return result;
        }

        internal int SaveBulkChangeLog(TempBulkChangeViewModel bulkChangesLogs)
        {
            int id = 0;
            var _db = new ApplicationdbContect();
            _db.Database.Connection.State.ConsoleMassage("Connection State");
            if (bulkChangesLogs != null)
            {
                var transection = _db.Database.BeginTransaction();

                try
                {
                    _db.Database.Connection.State.ConsoleMassage("Connection State");
                    //_db.Database.Connection.Open();
                    CheckConnectionState(_db);
                    BulkChange bulkChange = new BulkChange();
                    bulkChange.Title = bulkChangesLogs.Title;
                    bulkChange.CreatedOn = DateTime.Now;
                    bulkChange.CreatedBy = User.Identity.GetUserId();
                    _db.BulkChangeses.Add(bulkChange);

                    bulkChangesLogs.BulkChangesLogList.ForEach(x =>
                    {
                        BulkChangesLog bulkChangesLog = new BulkChangesLog();
                        bulkChangesLog.PatientId = x.PatientId;
                        bulkChangesLog.ResultMessage = x.ResultMassage;
                        bulkChangesLog.Status = x.Status;
                        bulkChangesLog.Createdby = bulkChange.CreatedBy;
                        bulkChangesLog.CreatedOn = bulkChange.CreatedOn;
                        bulkChangesLog.BulkChangeId = bulkChange.id;
                        _db.BulkChangesLogs.Add(bulkChangesLog);
                    });
                    _db.SaveChanges();
                    transection.Commit();
                    id = bulkChange.id;
                    //int totalcount = bulkChange.bulkChangesLogs.GroupBy(x => x.PatientId).ToList().Count();
                    //CalculateBulkChangesProgress(totalcount, totalcount + 1);
                }
                catch (Exception e)
                {
                    transection.Rollback();
                    throw e;
                }
                //currentPatientCount++;
                transection.Dispose();
            }
            _db.Database.Connection.Close();
            return id;
        }

        internal void CalculateBulkChangesProgress(float totalPatients, float CurrentPatient)
        {
            var Num = (CurrentPatient / totalPatients);
            var Percentage = Num * 100;
            string UserId = User.Identity.GetUserId();
            BulkChangingesHub.BulkChangesProgress(UserId, Percentage);
        }

        internal TempBulkChangeViewModel DeEnrollPatients(BulkChangeViewModel bulkChange)
        {
            TempBulkChangeViewModel result = new TempBulkChangeViewModel();
            int ind = 1;


            int? PreLiasionId = bulkChange.PreLiaisonId.ToString().GetNullableInteger();
            int? PreTranslatorId = bulkChange.PreTranslaterId.ToString().GetNullableInteger();



            int currentPatientCount = 1;
            var _db1 = new ApplicationdbContect();
            _db1.Database.Connection.Open();
            //means selected patients are enrolled and now user want to de-enroll them
            result.Title = "Change Status=Yes and Selected Patients are Active Enrolled";
            #region DeEnrolled Active Enrolled Patients
            foreach (var item in bulkChange.PatientsList)
            {
                //CheckConnectionState(_db1);
                var transaction = _db1.Database.BeginTransaction();
                if (_db1.Database.Connection.State != ConnectionState.Open)
                {
                    _db1.Database.Connection.Open();
                    _db1.Database.Connection.State.ConsoleMassage("Connection State");

                }
                try
                {

                    //item.ConsoleMassage(ind + " Patient id ");
                    //if (PreLiasionId == null && (PreTranslatorId > 0))
                    //{
                    //    transaction.Dispose();
                    //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    //    {
                    //        PatientId = 0,
                    //        ResultMassage = "No Data Saved. Pre-Counselor Name is mandatory to Select any status of Pre-Translator excluding Remove Translator",
                    //        Status = (int)BulkChangesStatus.Failed
                    //    });
                    //    return result;
                    //}
                    var patient = _db1.Patients.Where(p => p.Id == item).FirstOrDefault();
                    patient.Cycle.ConsoleMassage("Patient Cycle");
                    Patients_PreLiaisons patientPreliaison = _db1.Patients_PreLiaisons.Where(p => p.Id == patient.Patients_PreLiaisonsId).FirstOrDefault();
                    if (patientPreliaison != null)
                    {
                        if (PreLiasionId != 0)
                        {
                            if ((patientPreliaison.LiaisonId == null && PreLiasionId == null && patientPreliaison.TranslatorId > 0)
                                || (patientPreliaison.LiaisonId == null && PreLiasionId == null && PreTranslatorId > 0))
                            {
                                //error
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "01-Continue Skip Below: Please Select Pre-Counselor First For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }
                            //if (patientPreliaison.LiaisonId == null && PreLiasionId == null && PreTranslatorId>0)
                            //{
                            //    //error
                            //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //    {
                            //        PatientId = patient.Id,
                            //        ResultMassage = "02-Continue Skip Below: Please Select Pre-Counselor First For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                            //        Status = (int)BulkChangesStatus.Failed
                            //    });
                            //    CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                            //    currentPatientCount++;

                            //    continue;
                            //}
                        }
                        if (PreLiasionId == 0)
                        {
                            if (patientPreliaison.LiaisonId == null && patientPreliaison.TranslatorId > 0)
                            {
                                //error
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "03-Continue Skip Below: Please Select Pre-Counselor First For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (PreLiasionId != 0)
                        {
                            if ((PreLiasionId == null && patientPreliaison.TranslatorId > 0)
                                || (PreLiasionId == null && PreTranslatorId > 0))
                            {
                                //error
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "04-Continue Skip Below: Please Select Pre-Counselor First For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }
                            //if (PreLiasionId == null && PreTranslatorId>0)
                            //{
                            //    //error
                            //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //    {
                            //        PatientId = patient.Id,
                            //        ResultMassage = "05-Continue Skip Below: Please Select Pre-Counselor First For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                            //        Status = (int)BulkChangesStatus.Failed
                            //    });
                            //    CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                            //    currentPatientCount++;

                            //    continue;
                            //}
                        }
                        if (PreLiasionId == 0)
                        {
                            if (patientPreliaison.LiaisonId == null && patientPreliaison.TranslatorId > 0)
                            {
                                //error
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "06-Continue Skip Below: Please Select Pre-Counselor First For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }
                        }
                    }

                    if (!User.IsInRole("Admin") && !User.IsInRole("LiaisonGroup"))
                    {
                        if ((patient.EnrollmentStatus == "Enrolled" && patient.EnrollmentSubStatus == "Active Enrolled") && (bulkChange.EnrollmentStatus != "Enrolled" && bulkChange.EnollmentSubStatus != "Active Enrolled"))
                        {
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = " 07-Continue Skip Below- As Patient is Active Enrolled and you have select to change its status so based on Your rights you are not allow to change hia/her Status",
                                Status = (int)BulkChangesStatus.Failed
                            });
                            CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                            currentPatientCount++;
                            transaction.Dispose();
                            continue;
                        }
                        else
                        {
                            patient.EnrollmentStatus = bulkChange.EnrollmentStatus;
                            patient.EnrollmentSubStatus = bulkChange.EnollmentSubStatus;
                            patient.EnrollmentSubStatusReason = bulkChange.EnrollemntStatusReson;
                            patient.LiaisonId = null;
                            patient.TranslatorId = null;
                            _db1.Entry(patient).State = EntityState.Modified;
                            _db1.SaveChanges();
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Patient EnrollmentStatus, EnrollmentSubStatus, EnrollmentSubStatusReason, Post-Counselor, Post-Translator, are Updated " + patient.EnrollmentStatus + ", " + patient.EnrollmentSubStatus + ", " + patient.EnrollmentSubStatusReason + ", " + patient.LiaisonId + ", " + patient.TranslatorId,
                                Status = (int)BulkChangesStatus.success
                            });
                        }
                    }
                    else
                    {
                        patient.EnrollmentStatus = bulkChange.EnrollmentStatus;
                        patient.EnrollmentSubStatus = bulkChange.EnollmentSubStatus;
                        patient.EnrollmentSubStatusReason = bulkChange.EnrollemntStatusReson;
                        patient.LiaisonId = null;
                        patient.TranslatorId = null;
                        _db1.Entry(patient).State = EntityState.Modified;
                        _db1.SaveChanges();
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "Patient EnrollmentStatus, EnrollmentSubStatus, EnrollmentSubStatusReason, Post-Counselor, Post-Translator, are Updated " + patient.EnrollmentStatus + ", " + patient.EnrollmentSubStatus + ", " + patient.EnrollmentSubStatusReason + ", " + patient.LiaisonId + ", " + patient.TranslatorId,
                            Status = (int)BulkChangesStatus.success
                        });
                    }


                    //DeEnrolling from Exiting Billing Category
                    foreach (var deenrollreason in bulkChange.DeEnrollmentReason)
                    {

                        var BillingcatId = Convert.ToInt32(deenrollreason.BillingcategoryId);

                        var deEnroll = _db1.Patients_BillingCategories.Where(p => p.PatientId == item && p.BillingCategoryId == BillingcatId && p.Status == true).FirstOrDefault();
                        if (deEnroll != null)
                        {
                            deEnroll.DeEnrolledOn = DateTime.Now;
                            deEnroll.DeEnrollmentReason = deenrollreason.DeEnrollmentReason;
                            deEnroll.Status = false;
                            deEnroll.UpdatedOn = DateTime.Now;
                            deEnroll.UpdatedBy = User.Identity.GetUserId();
                            _db1.Entry(deEnroll).State = EntityState.Modified;
                            _db1.SaveChanges();
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "DeEnrolling Patient From Billing Category " + BillingcatId.GetBillingCatagoryNameById(),
                                Status = (int)BulkChangesStatus.success
                            });
                        }

                    }
                    if (patientPreliaison != null)
                    {
                        var oldpreL = patientPreliaison.LiaisonId;
                        var oldpreT = patientPreliaison.TranslatorId;
                        patientPreliaison.LiaisonId = PreLiasionId;
                        patientPreliaison.TranslatorId = PreTranslatorId;
                        patientPreliaison.Status = true;
                        patientPreliaison.UpdatedOn = DateTime.Now;
                        patientPreliaison.UpdatedBy = User.Identity.GetUserId();
                        _db1.Entry(patientPreliaison).State = EntityState.Modified;
                        _db1.SaveChanges();
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "Updating Pre-Counselor/Translator from Counselor " + oldpreL.GetLiaisonNameFromID() + ", Translator " + oldpreT.GetLiaisonNameFromID() + "to with new Counselor " + patientPreliaison.LiaisonId.GetLiaisonNameFromID() + ", Translator " + patientPreliaison.TranslatorId.GetLiaisonNameFromID(),
                            Status = (int)BulkChangesStatus.success
                        });
                    }
                    else/* if (patient.Patients_PreLiaisonsId == null)*/
                    {
                        Patients_PreLiaisons preLiaisons = new Patients_PreLiaisons();

                        preLiaisons.LiaisonId = PreLiasionId;
                        preLiaisons.TranslatorId = PreTranslatorId;
                        preLiaisons.CreatedOn = DateTime.Now;
                        preLiaisons.CreatedBy = User.Identity.GetUserId();
                        preLiaisons.Status = true;
                        _db1.Patients_PreLiaisons.Add(preLiaisons);
                        // _db.SaveChanges();
                        patient.Patients_PreLiaisons = preLiaisons;
                        patient.Patients_PreLiaisonsId = preLiaisons.Id;
                        _db1.Entry(patient).State = EntityState.Modified;
                        _db1.SaveChanges();
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "Adding New Pre-Counselor " + preLiaisons.LiaisonId.GetLiaisonNameFromID() + " Pre-Translator " + preLiaisons.TranslatorId.GetLiaisonNameFromID(),
                            Status = (int)BulkChangesStatus.success
                        });
                    }
                    //_db1.Entry(patient).State = EntityState.Modified;
                    #region For G0506
                    int g0506Id = BillingCodeHelper.G0506BillingCatagoryid;
                    var G_Cycles = _db1.CategoriesStatuses.Where(x => x.PatientId == patient.Id && x.Cycle == patient.Cycle && x.BillingCategoryId == g0506Id).FirstOrDefault();
                    if (G_Cycles != null)
                    {
                        var oldccmCycleStatus = G_Cycles.Status;
                        if (!string.IsNullOrEmpty(bulkChange.EnollmentSubStatus))
                        {

                            if (G_Cycles.Status != "Claims Submission" && G_Cycles.Status != "Clinical Sign-Off" && G_Cycles.Status != "Ready for Clinical Sign-Off" /*&& G_Cycles.Status != "In Progress"*/)
                            {
                                G_Cycles.Status = "Expired";
                                G_Cycles.UpdatedOn = DateTime.Now;
                                G_Cycles.UpdatedBy = User.Identity.GetUserId();
                                _db1.Entry(G_Cycles).State = EntityState.Modified;
                                _db1.SaveChanges();
                                //return "Expired";
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "G0506 Cycle status updated from " + oldccmCycleStatus + " to new G0506 cycle status " + G_Cycles.Status,
                                    Status = (int)BulkChangesStatus.success
                                });
                            }
                            //if (bulkChange.EnollmentSubStatus == "Active Enrolled")
                            //{
                            //    if (G_Cycles.Status != "Claims Submission" && G_Cycles.Status != "Clinical Sign-Off" && G_Cycles.Status != "Ready for Clinical Sign-Off" && G_Cycles.Status != "In Progress")
                            //    {

                            //        G_Cycles.Status = "Enrolled";
                            //        G_Cycles.UpdatedOn = DateTime.Now;
                            //        G_Cycles.UpdatedBy = User.Identity.GetUserId();
                            //        _db1.Entry(G_Cycles).State = EntityState.Modified;
                            //        _db1.SaveChanges();
                            //        //return "Enrolled";
                            //        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //        {
                            //            PatientId = patient.Id,
                            //            ResultMassage = "G0506 Cycle status updated from " + oldccmCycleStatus + " to new G0506 cycle status " + G_Cycles.Status,
                            //            Status = (int)BulkChangesStatus.success
                            //        });
                            //    }

                            //}
                            //else
                            //{

                            //    if (G_Cycles.Status != "Claims Submission" && G_Cycles.Status != "Clinical Sign-Off" && G_Cycles.Status != "Ready for Clinical Sign-Off" && G_Cycles.Status != "In Progress")
                            //    {
                            //        G_Cycles.Status = "Expired";
                            //        G_Cycles.UpdatedOn = DateTime.Now;
                            //        G_Cycles.UpdatedBy = User.Identity.GetUserId();
                            //        _db1.Entry(G_Cycles).State = EntityState.Modified;
                            //        _db1.SaveChanges();
                            //        //return "Expired";
                            //        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //        {
                            //            PatientId = patient.Id,
                            //            ResultMassage = "G0506 Cycle status updated from " + oldccmCycleStatus + " to new G0506 cycle status " + G_Cycles.Status,
                            //            Status = (int)BulkChangesStatus.success
                            //        });
                            //    }
                            //}
                            // _db1.Database.Connection.State.ToString().ConsoleMassage("Connection");
                            //var previouscycles = _db1.CategoriesStatuses.Where(x => x.PatientId == patient.Id && x.Cycle == patient.Cycle && x.Status != "Claims Submission" && x.Status != "Clinical Sign-Off" && x.Status != "Ready for Clinical Sign-Off" && x.Status != "In Progress").ToList();
                            //foreach (var Gitem in previouscycles)
                            //{
                            //    string oldpreccmcyclstatus = Gitem.Status;
                            //    if (Gitem.Status == "Enrolled")
                            //    {
                            //        Gitem.Status = "Expired";
                            //        Gitem.UpdatedOn = DateTime.Now;
                            //        Gitem.UpdatedBy = User.Identity.GetUserId();
                            //        _db1.Entry(Gitem).State = EntityState.Modified;
                            //        _db1.SaveChanges();
                            //        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //        {
                            //            PatientId = patient.Id,
                            //            ResultMassage = "Old G0506 Cycle status are updated from " + oldpreccmcyclstatus + " to new G0506 cycle status " + Gitem.Status,
                            //            Status = (int)BulkChangesStatus.success
                            //        });
                            //    }
                            //}
                        }
                    }
                    //else
                    //{
                    //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    //    {
                    //        PatientId = patient.Id,
                    //        ResultMassage = "No G0506 Cycle status Found",
                    //        Status = (int)BulkChangesStatus.Warning
                    //    });
                    //}

                    #endregion
                    #region UpdateCCMCyclesStatus
                    var CCMCycleStatuses = _db1.CCMCycleStatuses.Where(x => x.PatientId == patient.Id && x.Cycle == patient.Cycle && x.CCMStatus != "Claims Submission" && x.CCMStatus != "Clinical Sign-Off" && x.CCMStatus != "Ready for Clinical Sign-Off" /*&& x.CCMStatus != "In Progress"*/ && x.CCMStatus == "Enrolled").FirstOrDefault();
                    //var CCMCycleStatuses = _db1.CCMCycleStatuses.Where(x => x.PatientId == patient.Id && x.Cycle == patient.Cycle && x.CCMStatus == "Enrolled" ).FirstOrDefault();
                    if (CCMCycleStatuses != null)
                    {
                        CCMCycleStatuses.CCMStatus = "Expired";
                        CCMCycleStatuses.UpdatedOn = DateTime.Now;
                        CCMCycleStatuses.UpdatedBy = User.Identity.GetUserId();
                        _db1.Entry(CCMCycleStatuses).State = EntityState.Modified;
                        _db1.SaveChanges();
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "Expiring Old CCM Cycles Id=" + CCMCycleStatuses.Id + " From old CCM Status Enrolled to" + CCMCycleStatuses.CCMStatus,
                            Status = (int)BulkChangesStatus.success
                        });
                    }
                    else
                    {
                        var ccmcycles = _db1.CCMCycleStatuses.Where(x => x.PatientId == patient.Id && x.Cycle == patient.Cycle).FirstOrDefault();
                        if (ccmcycles != null)
                        {
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Patient <b>" + patient.LastName + " " + patient.FirstName + "</b> Current CCM Status is <b style=\"color:red\" >" + ccmcycles.CCMStatus + "</b> for Current Cycle " + patient.Cycle,
                                Status = (int)BulkChangesStatus.Warning
                            });
                        }
                        else
                        {
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "No CCM Cycles Status Found For this Patient " + patient.LastName + " " + patient.FirstName + " for Cycle " + patient.Cycle,
                                Status = (int)BulkChangesStatus.Warning
                            });
                        }

                    }

                    #endregion
                    #region MigrateAppointment
                    bulkChange.MigrateAppointmentIn_Id.ForEach(c =>
                    {
                        if (bulkChange.MigrateAppointment == "Yes")
                        {
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Appointment can not be Migrate As you are De-Enrolling Patients",//, Active Enrolled Patients Appointment with Post Counselor/Translator can not be migrate to Pre Counselor/Translators.",
                                Status = (int)BulkChangesStatus.Warning
                            });
                        }

                    });
                    _db1.Entry(patient).State = EntityState.Modified;
                    _db1.SaveChanges();
                    transaction.Commit();
                    #endregion
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var rolebackResult = result.BulkChangesLogList.Where(x => x.PatientId == item && x.Status == (int)BulkChangesStatus.success).ToList();
                    rolebackResult.ForEach(x =>
                    {
                        result.BulkChangesLogList.Remove(x);
                    });
                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    {
                        PatientId = item,
                        ResultMassage = "Patient Id=" + item + " Saving Failed " + ex.ToString(),
                        Status = (int)BulkChangesStatus.Failed
                    });
                }

                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                currentPatientCount++;
                transaction.Dispose();
            }

            #endregion
            _db1.Database.Connection.Close();
            if (result.BulkChangesLogList.Count() == 0)
            {
                result.BulkChangesLogList.Add(new BulkChangesLogViewModel
                {
                    PatientId = 0,
                    ResultMassage = "Some Thing Went Wrong",
                    Status = (int)BulkChangesStatus.Failed
                });
            }
            return result;
        }

        internal TempBulkChangeViewModel ChangeStatusOnly(BulkChangeViewModel bulkChange)
        {
            TempBulkChangeViewModel result = new TempBulkChangeViewModel();
            int ind = 1;


            int? PreLiasionId = bulkChange.PreLiaisonId.ToString().GetNullableInteger();
            int? PreTranslatorId = bulkChange.PreTranslaterId.ToString().GetNullableInteger();
            var _db1 = new ApplicationdbContect();
            _db1.Database.Connection.Open();


            //means selected patients are deEnrolled and now use just want to change theirs status other than Active Enrolled
            result.Title = "Changing Status=<b style=\"color:red\"> Yes</b> and Selected Patients are <b style=\"color:red\">Not Active Enrolled</b>";
            #region Patients are already DeEnrolled and now user just want to change its status
            int currentPatientCount = 1;

            foreach (var item in bulkChange.PatientsList)
            {
                //CheckConnectionState(_db1);
                var transaction = _db1.Database.BeginTransaction();
                if (_db1.Database.Connection.State != ConnectionState.Open)
                {
                    _db1.Database.Connection.Open();
                    // _db1.Database.Connection.State.ConsoleMassage("Connection State");

                }
                try
                {

                    var patient = _db1.Patients.Where(p => p.Id == item).FirstOrDefault();

                    Patients_PreLiaisons patientPreliaison = _db1.Patients_PreLiaisons.Where(p => p.Id == patient.Patients_PreLiaisonsId).FirstOrDefault();
                    int? oldPreliasions = null;
                    int? oldPreTranslator = null;

                    if (patientPreliaison != null)
                    {
                        if (patientPreliaison.TranslatorId > 0 || PreTranslatorId > 0)
                        {
                            if (patientPreliaison.LiaisonId == null && (PreLiasionId == null || PreLiasionId == 0))
                            {
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "07-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }
                            if (patientPreliaison.LiaisonId != null && (PreLiasionId == null || PreLiasionId == 0) && PreTranslatorId != null)
                            {
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "012-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }
                        }
                        oldPreliasions = patientPreliaison.LiaisonId;
                        oldPreTranslator = patientPreliaison.TranslatorId;
                        if (PreLiasionId != 0)
                        {
                            patientPreliaison.LiaisonId = PreLiasionId;
                        }
                        if (PreTranslatorId != 0)
                        {
                            patientPreliaison.TranslatorId = PreTranslatorId;
                        }

                        patientPreliaison.Status = true;
                        patientPreliaison.UpdatedOn = DateTime.Now;
                        patientPreliaison.UpdatedBy = User.Identity.GetUserId();
                        _db1.Entry(patientPreliaison).State = EntityState.Modified;
                        _db1.SaveChanges();
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "031 Patient id=<b style=\"color:forestgreen\">" + patient.Id
                            + "</b> are updated from Counselor <b style=\"color:forestgreen\">"
                            + oldPreliasions != null ? oldPreliasions.GetLiaisonNameFromID() : "Null"
                            + "</b> , Translator <b style=\"color:forestgreen\">"
                            + oldPreTranslator != null ? oldPreTranslator.GetLiaisonNameFromID() : "Null"
                            + "</b> to new Counselor  <b style=\"color:forestgreen\">"
                            + patientPreliaison.LiaisonId != null ? patientPreliaison.LiaisonId.GetLiaisonNameFromID() : "Null"
                            + "</b>,  Translator <b style=\"color:forestgreen\">"
                            + patientPreliaison.TranslatorId != null ? patientPreliaison.TranslatorId.GetLiaisonNameFromID() : "Null" + "</b>",
                            Status = (int)BulkChangesStatus.success
                        });

                    }
                    else if (PreLiasionId != 0)
                    {
                        if (PreTranslatorId > 0 && PreLiasionId == null)
                        {
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "12-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                Status = (int)BulkChangesStatus.Failed
                            });
                            CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                            currentPatientCount++;
                            transaction.Dispose();
                            continue;
                        }
                        Patients_PreLiaisons preLiaisons = new Patients_PreLiaisons();
                        if (PreLiasionId != 0)
                        {
                            patientPreliaison.LiaisonId = PreLiasionId;
                        }
                        if (PreTranslatorId != 0)
                        {
                            patientPreliaison.TranslatorId = PreTranslatorId;
                        }
                        //preLiaisons.LiaisonId = PreLiasionId;
                        //preLiaisons.TranslatorId = PreTranslatorId;
                        preLiaisons.CreatedOn = DateTime.Now;
                        preLiaisons.CreatedBy = User.Identity.GetUserId();
                        preLiaisons.Status = true;
                        _db1.Patients_PreLiaisons.Add(preLiaisons);
                        // _db.SaveChanges();
                        patient.Patients_PreLiaisons = preLiaisons;
                        patient.Patients_PreLiaisonsId = preLiaisons.Id;
                        _db1.Entry(patient).State = EntityState.Modified;
                        _db1.SaveChanges();
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patient.Id,
                            ResultMassage = "New Pre Counselor/Translator are Added Counselor <b style=\"color:forestgreen\">"
                            + preLiaisons.LiaisonId != null ? preLiaisons.LiaisonId.GetLiaisonNameFromID() : "Null"
                            + "</b>, Translator <b style=\"color:forestgreen\">"
                            + preLiaisons.TranslatorId != null ? preLiaisons.TranslatorId.GetLiaisonNameFromID() : "Null" + "</b>",
                            Status = (int)BulkChangesStatus.success
                        });




                    }

                    patient.EnrollmentStatus = bulkChange.EnrollmentStatus;
                    patient.EnrollmentSubStatus = bulkChange.EnollmentSubStatus;
                    patient.EnrollmentSubStatusReason = bulkChange.EnrollemntStatusReson;
                    _db1.Entry(patient).State = EntityState.Modified;
                    //_db1.SaveChanges();
                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    {
                        PatientId = patient.Id,
                        ResultMassage = "Patient id=<b style=\"color:forestgreen\">" + patient.Id
                        + "</b> EnrollmentStatus=<b style=\"color:forestgreen\">" + patient.EnrollmentStatus + "</b> " +
                        ", EnrollmentSubStatus =<b style=\"color:forestgreen\">" + patient.EnrollmentSubStatus + "</b> " +
                        ", EnrollmentSubStatusReason= <b style=\"color:forestgreen\">" + patient.EnrollmentSubStatusReason + "</b> are Updated",
                        Status = (int)BulkChangesStatus.success
                    });


                    #region MigrateAppointment

                    if (bulkChange.MigrateAppointment == "Yes")
                    {
                        #region Migrate Counselor Appointment
                        bulkChange.MigrateAppointmentIn_Id.ForEach(c =>
                        {
                            if (PreLiasionId > 0)
                            {
                                var Selectedliasionschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == PreLiasionId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                if (Selectedliasionschedule != null)
                                {
                                    //SelectedliasionSCreated = true;
                                    if (patientPreliaison.LiaisonId != null)
                                    {
                                        var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldPreliasions && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                        if (pa.Count > 0)
                                        {
                                            pa.ForEach(x =>
                                            {
                                                x.LiaisonID = PreLiasionId;
                                                x.BillingCategoryId = c;
                                                x.UpdateOn = DateTime.Now;
                                                x.UpdatedBy = 0;
                                                _db1.Entry(x).State = EntityState.Modified;
                                                _db1.SaveChanges();
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "Counselor Appointment Id=" + x.ID + " migrate Successfully to new Counselor " + PreLiasionId != null ? PreLiasionId.GetLiaisonNameFromID() : "Null" + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                    Status = (int)BulkChangesStatus.success
                                                });
                                            });

                                        }
                                        else
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "No Upcoming appointment found for this Patient with old Counselor " + oldPreliasions != null ? oldPreliasions.GetLiaisonNameFromID() : "Null" + " for Billing Category " + c.GetBillingCatagoryNameById(),
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                        }

                                    }
                                    else
                                    {
                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                        {
                                            PatientId = patient.Id,
                                            ResultMassage = "Appointment for Billing Category " + c.GetBillingCatagoryNameById() + " Could Not be Migrate as no old Counselor Found For this Patient",
                                            Status = (int)BulkChangesStatus.Warning
                                        });
                                    }
                                }
                                else
                                {
                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "Can not migrate Appointment  for Billing Category " + c.GetBillingCatagoryNameById() + " as Counselor " + patientPreliaison.LiaisonId.GetLiaisonNameFromID() + " Schedules not found, please create Counselor schedule first.!",
                                        Status = (int)BulkChangesStatus.Warning
                                    });
                                }
                            }
                            else if (PreLiasionId == null || PreLiasionId == 0)
                            {
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "Can not migrate Appointment  for Billing Category " + c.GetBillingCatagoryNameById() + " as you have not selected any Counselor",
                                    Status = (int)BulkChangesStatus.Warning
                                });
                            }

                            #endregion

                            #region Migrate Translator Appointment
                            if (PreTranslatorId > 0)
                            {
                                var SelectedTranslatorschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == PreTranslatorId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                if (SelectedTranslatorschedule != null)
                                {
                                    //SelectedliasionSCreated = true;
                                    if (patientPreliaison.LiaisonId != null)
                                    {
                                        var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldPreTranslator && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                        if (pa.Count > 0)
                                        {
                                            pa.ForEach(x =>
                                            {
                                                x.LiaisonID = PreTranslatorId;
                                                x.BillingCategoryId = c;
                                                x.UpdateOn = DateTime.Now;
                                                x.UpdatedBy = 0;
                                                _db1.Entry(x).State = EntityState.Modified;
                                                _db1.SaveChanges();
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "Translator Appointment Id=" + x.ID + " migrate Successfully to new Translator " + PreTranslatorId != null ? PreTranslatorId.GetLiaisonNameFromID() : "Null" + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                    Status = (int)BulkChangesStatus.success
                                                });
                                            });

                                        }
                                        else
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "No Upcoming appointment found for this Patient with old Translator " + oldPreTranslator != null ? oldPreTranslator.GetLiaisonNameFromID() : "Null" + "for Billing Category" + c.GetBillingCatagoryNameById(),
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                        }

                                    }
                                    else
                                    {
                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                        {
                                            PatientId = patient.Id,
                                            ResultMassage = "Appointment for Billing Category " + c.GetBillingCatagoryNameById() + " Could Not be Migrate as no old Translator Found For this Patient",
                                            Status = (int)BulkChangesStatus.Warning
                                        });
                                    }
                                }
                                else
                                {
                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "Can not migrate Appointment  for Billing Category " + c.GetBillingCatagoryNameById() + " as Translator " + patientPreliaison.TranslatorId != null ? patientPreliaison.TranslatorId.GetLiaisonNameFromID() : "Null" + " Schedules not found, please create Counselor schedule first.!",
                                        Status = (int)BulkChangesStatus.Warning
                                    });
                                }
                            }
                            else if (PreTranslatorId == 0 || PreTranslatorId == null)
                            {

                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "Can not migrate Appointment  for Billing Category " + c.GetBillingCatagoryNameById() + " as you have not selected any Pre-Translator",
                                    Status = (int)BulkChangesStatus.Warning
                                });
                            }
                            #region commnet
                            //else if (PreTranslatorId == 0)
                            //{
                            //    if (oldPreliasions != null)
                            //    {
                            //        //below code will work when we will manage appointment by Billing category
                            //        var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldPreTranslator && x.StartTime > DateTime.Now && x.BillingCategoryId != bulkChange.MigrateAppointmentIn_Id).ToList();

                            //        if (pa.Count > 0)
                            //        {
                            //            pa.ForEach(x =>
                            //            {
                            //                //x.LiaisonID = SelectedLiaisonId;
                            //                x.UpdateOn = DateTime.Now;
                            //                x.BillingCategoryId = bulkChange.MigrateAppointmentIn_Id;
                            //                x.UpdatedBy = 0;
                            //                _db1.Entry(x).State = EntityState.Modified;
                            //                result.Add(new BulkChangesLogViewModel()
                            //                {
                            //                    PatientId = patient.Id,
                            //                    ResultMassage = "Translator Appointment Id=" + x.ID + " migrate Successfully to new Category " + bulkChange.MigrateAppointmentIn_Id,
                            //                    Status = true
                            //                });
                            //            });

                            //            //result = "Liaison has been changed along with Appointments successfully.!";
                            //        }
                            //        else
                            //        {
                            //            result.Add(new BulkChangesLogViewModel()
                            //            {
                            //                PatientId = patient.Id,
                            //                ResultMassage = "No Upcoming appointment found for this Patient with old Translator",
                            //                Status = true
                            //            });
                            //        }

                            //    }
                            //    else
                            //    {
                            //        result.Add(new BulkChangesLogViewModel()
                            //        {
                            //            PatientId = patient.Id,
                            //            ResultMassage = "No old Translator Found For this Patient",
                            //            Status = false
                            //        });
                            //        // result = "Counselor has been changed successfully.No Counselor was found so No Appointments Migrates.!";
                            //    }
                            //}
                            #endregion
                            #endregion
                        });

                    }
                    //else
                    //{
                    //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    //    {
                    //        PatientId = patient.Id,
                    //        ResultMassage = "No Appointment Migrate As you have selected to not Migrate Appointment",
                    //        Status = (int)BulkChangesStatus.Warning
                    //    });
                    //}


                    #endregion


                    patient.UpdatedOn = DateTime.Now;
                    patient.UpdatedBy = User.Identity.GetUserId();
                    _db1.Entry(patient).State = EntityState.Modified;
                    _db1.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var rolebackResult = result.BulkChangesLogList.Where(x => x.PatientId == item && x.Status == (int)BulkChangesStatus.success).ToList();
                    rolebackResult.ForEach(x =>
                    {
                        result.BulkChangesLogList.Remove(x);
                    });
                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                    {
                        PatientId = item,
                        ResultMassage = "Patient Id=" + item + " Saving Failed" + ex.ToString(),
                        Status = (int)BulkChangesStatus.Failed
                    });
                }

                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                currentPatientCount++;
                transaction.Dispose();
            }

            #endregion




            // result.ConsoleMassage("Result");
            _db1.Database.Connection.Close();
            //_db1.Database.Connection.State.ConsoleMassage("Connection");
            if (result.BulkChangesLogList.Count() == 0)
            {
                result.BulkChangesLogList.Add(new BulkChangesLogViewModel
                {
                    PatientId = 0,
                    ResultMassage = "Some Thing Went Wrong",
                    Status = (int)BulkChangesStatus.Failed
                });
            }
            return result;
        }

        internal TempBulkChangeViewModel ChangePostCounselorForEnrolledPatients(BulkChangeViewModel bulkChange)
        {
            TempBulkChangeViewModel result = new TempBulkChangeViewModel();
            var _db1 = new ApplicationdbContect();

            int ind = 1;
            //if selected patients are enrolled
            int currentPatientCount = 1;

            result.Title = "Change Status=<b style=\"color:red\">No</b> and Selected Patients are <b style=\"color:red\">Enrolled</b>-Changing Post Counselor/Translator";
            foreach (var patients in bulkChange.PatientsList)
            {
                //patients.ConsoleMassage(ind + "- Patient id ");

                foreach (var categories in bulkChange.PostCounselorTranslatorList)
                {

                    int? billingcategoryId = categories.BillingcategoryId.GetNullableInteger();
                    using (var transaction = _db1.Database.BeginTransaction())
                    {

                        if (_db1.Database.Connection.State != ConnectionState.Open)
                        {
                            _db1.Database.Connection.Open();
                            _db1.Database.Connection.State.ConsoleMassage("Connection State");

                        }
                        var patient = _db1.Patients.Where(p => p.Id == patients).FirstOrDefault();

                        var oldLiaisonIDs = _db1.Patients_BillingCategories.Where(x => x.PatientId == patient.Id &&  x.Status == true && x.BillingCategoryId == billingcategoryId).Select(x => x.LiaisonId).FirstOrDefault();
                        oldLiaisonIDs = oldLiaisonIDs.ToString().GetNullableInteger();

                        //old code
                        //var oldTranslatorIDs = _db1.Patients_BillingCategories.Where(x => x.PatientId == patient.Id && x.IsTranslator == true && x.Status == true && x.BillingCategoryId == billingcategoryId).Select(x => x.LiaisonId).FirstOrDefault();
                        //oldTranslatorIDs = oldTranslatorIDs.ToString().GetNullableInteger();
                        //new code
                        var oldTranslatorIDs = _db1.Patients_BillingCategories.Where(x => x.PatientId == patient.Id && x.Status == true && x.BillingCategoryId == billingcategoryId).Select(x => x.TranslatorId).FirstOrDefault();
                        oldTranslatorIDs = oldTranslatorIDs.ToString().GetNullableInteger();

                        int? SelectedLiaisonId = categories.LiaisonId.GetNullableInteger();
                        int? SelectedTranslatorId = categories.TranslatorId.GetNullableInteger();

                        if (SelectedTranslatorId > 0 || oldTranslatorIDs > 0)
                        {
                            if (SelectedLiaisonId == null && oldLiaisonIDs == null)
                            {
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "30-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName + " for Category <b style=\"color:red\">" + categories.BillingcategoryId.GetBillingCatagoryIdByName() + "</b>",
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }

                        }


                        try
                        {



                            #region For ccm billing category make changes in patients table

                            if (billingcategoryId == BillingCodeHelper.cmmBillingCatagoryid)
                            {

                                if (SelectedLiaisonId > 0 /*|| SelectedLiaisonId == null*/)
                                {
                                    var ccmoldL = patient.LiaisonId;
                                    patient.LiaisonId = SelectedLiaisonId;

                                    //result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    //{
                                    //    PatientId = patient.Id,
                                    //    ResultMassage = "Counselor Changed From <b style=\"color:green\">" + ccmoldL.GetLiaisonNameFromID() + "</b>  to <b style=\"color:green\">" + patient.LiaisonId.GetLiaisonNameFromID() + "</b> for Billing Category <b style=\"color:green\">" + categories.BillingcategoryId.GetBillingCatagoryIdByName() + "</b> Successful",
                                    //    Status = (int)BulkChangesStatus.success
                                    //});
                                }
                                if (SelectedTranslatorId > 0)
                                {
                                    var ccmoldT = patient.TranslatorId;
                                    patient.TranslatorId = SelectedTranslatorId;

                                    //result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    //{
                                    //    PatientId = patient.Id,
                                    //    ResultMassage = "Translator Changed From " + ccmoldT.GetLiaisonNameFromID() + " to " + patient.TranslatorId.GetLiaisonNameFromID() + " for Billing Category " + categories.BillingcategoryId.GetBillingCatagoryIdByName() + "Successful",
                                    //    Status = (int)BulkChangesStatus.success
                                    //});
                                }

                                patient.UpdatedBy = User.Identity.GetUserId();
                                patient.UpdatedOn = DateTime.Now;
                                _db1.Entry(patient).State = EntityState.Modified;


                            }
                            #endregion

                            if (SelectedLiaisonId > 0)
                            {
                                var patLiaison = _db1.Patients_BillingCategories.Where(p => p.PatientId == patients && p.BillingCategoryId == billingcategoryId && p.Status == true).FirstOrDefault();
                                if (patLiaison != null)
                                {

                                    //patLiaison.BillingCategoryId = billingcategoryId;
                                    //patLiaison.LiaisonId = LiaisonId;
                                    patLiaison.Status = false;
                                    patLiaison.UpdatedBy = User.Identity.GetUserId();
                                    patLiaison.UpdatedOn = DateTime.Now;
                                    _db1.Entry(patLiaison).State = EntityState.Modified;
                                    //_db.SaveChanges();

                                }

                                Patients_BillingCategories patCat = new Patients_BillingCategories();
                                patCat.BillingCategoryId = billingcategoryId;
                                patCat.LiaisonId = SelectedLiaisonId;
                                patCat.CreatedOn = DateTime.Now;
                                patCat.CreatedBy = User.Identity.GetUserId();
                                patCat.IsTranslator = false;
                                patCat.Status = true;
                                patCat.PatientId = patients;
                                _db1.Patients_BillingCategories.Add(patCat);
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "Counselor " + patCat.LiaisonId.GetLiaisonNameFromID() + " for Billing Category " + billingcategoryId.GetInteger().GetBillingCatagoryNameById() + "Successful",
                                    Status = (int)BulkChangesStatus.success
                                });
                                //_db.SaveChanges();
                            }
                            if (SelectedTranslatorId > 0)
                            {
                                var patTranslator = _db1.Patients_BillingCategories.Where(p => p.PatientId == patients && p.BillingCategoryId == billingcategoryId && p.Status == true).FirstOrDefault();
                                if (patTranslator != null)
                                {
                                    //patTranslator.BillingCategoryId = billingcategoryId;
                                    //patTranslator.LiaisonId = TranslatorId;
                                    patTranslator.TranslatorId = SelectedTranslatorId;
                                    patTranslator.UpdatedBy = User.Identity.GetUserId();
                                    patTranslator.UpdatedOn = DateTime.Now;
                                    _db1.Entry(patTranslator).State = EntityState.Modified;
                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "Translator " + patTranslator.TranslatorId.GetLiaisonNameFromID() + " for Billing Category " + categories.BillingcategoryId.GetBillingCatagoryIdByName() + "Successful",
                                        Status = (int)BulkChangesStatus.success
                                    });
                                    //_db.SaveChanges();
                                }

                                //Patients_BillingCategories patCat2 = new Patients_BillingCategories();
                                //patCat2.BillingCategoryId = billingcategoryId;
                                //patCat2.LiaisonId = SelectedTranslatorId;
                                //patCat2.CreatedOn = DateTime.Now;
                                //patCat2.CreatedBy = User.Identity.GetUserId();
                                //patCat2.IsTranslator = true;
                                //patCat2.Status = true;
                                //patCat2.PatientId = patients;
                                //_db1.Patients_BillingCategories.Add(patCat2);
                                // _db.SaveChanges();
                         

                            }
                            else if (SelectedTranslatorId == null)
                            {
                                var patTranslator = _db1.Patients_BillingCategories.Where(p => p.PatientId == patients && p.BillingCategoryId == billingcategoryId && p.Status == true).FirstOrDefault();
                                if (patTranslator != null)
                                {
                                    //patTranslator.BillingCategoryId = billingcategoryId;
                                    //patTranslator.LiaisonId = TranslatorId;
                                    patTranslator.TranslatorId = null;
                                    patTranslator.IsTranslator = false;
                                    patTranslator.UpdatedBy = User.Identity.GetUserId();
                                    patTranslator.UpdatedOn = DateTime.Now;
                                    _db1.Entry(patTranslator).State = EntityState.Modified;
                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "Translator " + patTranslator.LiaisonId.GetLiaisonNameFromID() + " Removed Successful",
                                        Status = (int)BulkChangesStatus.success
                                    });
                                    // _db.SaveChanges();
                                }
                            }




                            #region MigrateAppointment

                            if (bulkChange.MigrateAppointment == "Yes")
                            {
                                bulkChange.MigrateAppointmentIn_Id.ForEach(c =>
                                {
                                    if (billingcategoryId == c)
                                    {
                                        #region Migrate Counselor Appointment

                                        if (SelectedLiaisonId > 0)
                                        {
                                            var Selectedliasionschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == SelectedLiaisonId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                            if (Selectedliasionschedule != null)
                                            {
                                                //SelectedliasionSCreated = true;
                                                if (oldLiaisonIDs != null)
                                                {
                                                    var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldLiaisonIDs && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                                    if (pa.Count > 0)
                                                    {
                                                        pa.ForEach(x =>
                                                        {
                                                            x.LiaisonID = SelectedLiaisonId;
                                                            x.BillingCategoryId = c;
                                                            x.UpdateOn = DateTime.Now;
                                                            x.UpdatedBy = 0;
                                                            _db1.Entry(x).State = EntityState.Modified;
                                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                            {
                                                                PatientId = patient.Id,
                                                                ResultMassage = "Counselor Appointment Id=" + x.ID + " migrate Successfully to new Counselor" + SelectedLiaisonId.GetLiaisonNameFromID() + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                                Status = (int)BulkChangesStatus.success
                                                            });
                                                        });

                                                    }
                                                    else
                                                    {
                                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                        {
                                                            PatientId = patient.Id,
                                                            ResultMassage = "No Upcoming appointment found for this Patient with old Counselor" + oldLiaisonIDs.GetLiaisonNameFromID() + " for Billing Category " + c.GetBillingCatagoryNameById(),
                                                            Status = (int)BulkChangesStatus.Warning
                                                        });
                                                    }

                                                }
                                                else
                                                {
                                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                    {
                                                        PatientId = patient.Id,
                                                        ResultMassage = "Appointment Could Not be Migrate as no old Counselor Found For this Patient",
                                                        Status = (int)BulkChangesStatus.Warning
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "Can not migrate Appointment as Counselor " + SelectedLiaisonId.GetLiaisonNameFromID() + " Schedules not found, please create Counselor schedule first.!",
                                                    Status = (int)BulkChangesStatus.Warning
                                                });
                                            }
                                        }
                                        else if (SelectedLiaisonId == null || SelectedLiaisonId == 0)
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "Can not migrate Appointment as you have not select any Counselor",
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                        }
                                        #region Else Part Comment
                                        //else if (SelectedLiaisonId == 0)
                                        //{
                                        //    if (oldLiaisonIDs != null)
                                        //    {
                                        //        //below code will work when we will manage appointment by Billing category
                                        //        var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldLiaisonIDs && x.StartTime > DateTime.Now && x.BillingCategoryId ==c).ToList();

                                        //        if (pa.Count > 0)
                                        //        {
                                        //            pa.ForEach(x =>
                                        //            {
                                        //                //x.LiaisonID = SelectedLiaisonId;
                                        //                x.UpdateOn = DateTime.Now;
                                        //                x.BillingCategoryId = c;
                                        //                x.UpdatedBy = 0;
                                        //                _db1.Entry(x).State = EntityState.Modified;
                                        //                result.Add(new BulkChangesLogViewModel()
                                        //                {
                                        //                    PatientId = patient.Id,
                                        //                    ResultMassage = "Counselor Appointment Id=" + x.ID + " migrate Successfully to new Category " + bulkChange.MigrateAppointmentIn_Id,
                                        //                    Status = true
                                        //                });
                                        //            });

                                        //            //result = "Liaison has been changed along with Appointments successfully.!";
                                        //        }
                                        //        else
                                        //        {
                                        //            result.Add(new BulkChangesLogViewModel()
                                        //            {
                                        //                PatientId = patient.Id,
                                        //                ResultMassage = "No Upcoming appointment found for this Patient with old Counselor",
                                        //                Status = true
                                        //            });
                                        //        }

                                        //    }
                                        //    else
                                        //    {
                                        //        result.Add(new BulkChangesLogViewModel()
                                        //        {
                                        //            PatientId = patient.Id,
                                        //            ResultMassage = "No old Counselor Found For this Patient",
                                        //            Status = false
                                        //        });
                                        //        // result = "Counselor has been changed successfully.No Counselor was found so No Appointments Migrates.!";
                                        //    }
                                        //}
                                        #endregion
                                        #endregion

                                        #region Migrate Translator Appointment
                                        if (SelectedTranslatorId > 0)
                                        {
                                            var Selectedtranslatorschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == SelectedTranslatorId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                            if (Selectedtranslatorschedule != null)
                                            {
                                                if (oldTranslatorIDs != null)
                                                {
                                                    var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldTranslatorIDs && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                                    if (pa.Count > 0)
                                                    {
                                                        pa.ForEach(x =>
                                                        {
                                                            x.LiaisonID = SelectedTranslatorId;
                                                            x.BillingCategoryId = c;
                                                            x.UpdateOn = DateTime.Now;
                                                            x.UpdatedBy = 0;
                                                            _db1.Entry(x).State = EntityState.Modified;
                                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                            {
                                                                PatientId = patient.Id,
                                                                ResultMassage = "Translator Appointment Id=" + x.ID + " migrate Successfully to new Translator" + SelectedTranslatorId.GetLiaisonNameFromID() + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                                Status = (int)BulkChangesStatus.success
                                                            });
                                                        });

                                                    }
                                                    else
                                                    {
                                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                        {
                                                            PatientId = patient.Id,
                                                            ResultMassage = "No Upcoming appointment found for this Patient with old Translator" + oldTranslatorIDs.GetLiaisonNameFromID() + " for Billing Category " + c.GetBillingCatagoryNameById(),
                                                            Status = (int)BulkChangesStatus.Warning
                                                        });
                                                    }

                                                }
                                                else
                                                {
                                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                    {
                                                        PatientId = patient.Id,
                                                        ResultMassage = "Appointment Could Not be Migrate as no old Translator Found For this Patient",
                                                        Status = (int)BulkChangesStatus.Warning
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "Can not migrate Appointment as Translator " + SelectedTranslatorId.GetLiaisonNameFromID() + ") Schedules not found, please create Translator schedule first.!",
                                                    Status = (int)BulkChangesStatus.Warning
                                                });
                                            }
                                        }
                                        else if (SelectedTranslatorId == null || SelectedTranslatorId == 0)
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "Can not migrate Appointment as you have not select any translator",
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                        }
                                        #region else part commnets
                                        //else if (SelectedTranslatorId == 0)
                                        //{
                                        //    if (oldTranslatorIDs != null)
                                        //    {
                                        //        //below code will work when we will manage appointment by Billing category
                                        //        var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldTranslatorIDs && x.StartTime > DateTime.Now && x.BillingCategoryId != bulkChange.MigrateAppointmentIn_Id).ToList();

                                        //        if (pa.Count > 0)
                                        //        {
                                        //            pa.ForEach(x =>
                                        //            {
                                        //                //x.LiaisonID = SelectedLiaisonId;
                                        //                x.UpdateOn = DateTime.Now;
                                        //                x.BillingCategoryId = bulkChange.MigrateAppointmentIn_Id;
                                        //                x.UpdatedBy = 0;
                                        //                _db1.Entry(x).State = EntityState.Modified;
                                        //                result.Add(new BulkChangesLogViewModel()
                                        //                {
                                        //                    PatientId = patient.Id,
                                        //                    ResultMassage = "Translator Appointment Id=" + x.ID + " migrate Successfully to new Category " + bulkChange.MigrateAppointmentIn_Id,
                                        //                    Status = true
                                        //                });
                                        //            });

                                        //            //result = "Liaison has been changed along with Appointments successfully.!";
                                        //        }
                                        //        else
                                        //        {
                                        //            result.Add(new BulkChangesLogViewModel()
                                        //            {
                                        //                PatientId = patient.Id,
                                        //                ResultMassage = "No Upcoming appointment found for this Patient with old Translator",
                                        //                Status = true
                                        //            });
                                        //        }

                                        //    }
                                        //    else
                                        //    {
                                        //        result.Add(new BulkChangesLogViewModel()
                                        //        {
                                        //            PatientId = patient.Id,
                                        //            ResultMassage = "No old Translator Found For this Patient",
                                        //            Status = false
                                        //        });
                                        //        // result = "Counselor has been changed successfully.No Counselor was found so No Appointments Migrates.!";
                                        //    }
                                        //}
                                        #endregion
                                        #endregion
                                    }

                                });


                            }
                            //else
                            //{
                            //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //    {
                            //        PatientId = patient.Id,
                            //        ResultMassage = "No Appointment Migrate As you have selected to not Migrate Appointment",
                            //        Status = (int)BulkChangesStatus.Warning
                            //    });
                            //}

                            #endregion
                            patient.UpdatedBy = User.Identity.GetUserId();
                            patient.UpdatedOn = DateTime.Now;
                            _db1.Entry(patient).State = EntityState.Modified;
                            _db1.SaveChanges();
                            transaction.Commit();
                            // patient.Id.ConsoleMassage(ind + " Successful- Patient id ");

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            var rolebackResult = result.BulkChangesLogList.Where(x => x.PatientId == patients && x.Status == (int)BulkChangesStatus.success).ToList();
                            rolebackResult.ForEach(x =>
                            {
                                result.BulkChangesLogList.Remove(x);
                            });
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patients,
                                ResultMassage = "Patient Id=" + patients + " Saving Failed" + ex.ToString(),
                                Status = (int)BulkChangesStatus.Failed
                            });
                            //throw ex;
                        }



                        //categories.BillingcategoryId.ConsoleMassage("Category Id");
                        transaction.Dispose();
                    }


                }

                ind++;

                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                currentPatientCount++;

            }


            _db1.Database.Connection.Close();
            if (result.BulkChangesLogList.Count() == 0)
            {
                result.BulkChangesLogList.Add(new BulkChangesLogViewModel
                {
                    PatientId = 0,
                    ResultMassage = "Some Thing Went Wrong",
                    Status = (int)BulkChangesStatus.Failed
                });
            }
            return result;
        }

        internal TempBulkChangeViewModel ChnagePreCounselorOfNotEnrolledPatients(BulkChangeViewModel bulkChange)
        {
            TempBulkChangeViewModel result = new TempBulkChangeViewModel();
            var _db1 = new ApplicationdbContect();
            _db1.Database.Connection.Open();
            //_db1.Database.Connection.State.ToString().ConsoleMassage("Connection");
            int? PreliasionId = bulkChange.PreLiaisonId.ToString().GetNullableInteger();
            int? PreTranslatorId = bulkChange.PreTranslaterId.ToString().GetNullableInteger();
            int ind = 1;


            //if selected Patients are not enrolled
            result.Title = "Change Status=<b style=\"color:red\">No</b> and Selected Patients are <b style=\"color:red\">Not Active Enrolled</b>-Change Pre Counselor/Translator ";
            int currentPatientCount = 1;

            foreach (var patients in bulkChange.PatientsList)
            {
                // patients.ConsoleMassage(ind + "- Patient id ");
                using (var transaction = _db1.Database.BeginTransaction())
                {
                    if (_db1.Database.Connection.State != ConnectionState.Open)
                    {
                        _db1.Database.Connection.Open();
                        _db1.Database.Connection.State.ConsoleMassage("Connection State");

                    }
                    var patient = _db1.Patients.Where(p => p.Id == patients).FirstOrDefault();
                    //as there will be only those patients which status sub-status are not equal Active Enrolled
                    //var oldLiaison = patient.LiaisonId;
                    //var oldTranslator = patient.TranslatorId;
                    try
                    {
                        #region Change Pre Counselor and Pre Translator
                        int? oldPreliasions = (int?)null;
                        int? oldPreTranslator = (int?)null;
                        var patientPreliaison = _db1.Patients_PreLiaisons.Where(p => p.Id == patient.Patients_PreLiaisonsId && p.Status == true).FirstOrDefault();

                        if (patientPreliaison != null)
                        {
                            if (patientPreliaison.TranslatorId > 0 || PreTranslatorId > 0)
                            {
                                if (patientPreliaison.LiaisonId == null && (PreliasionId == null || PreliasionId == 0))
                                {
                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "07-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                        Status = (int)BulkChangesStatus.Failed
                                    });
                                    CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                    currentPatientCount++;
                                    transaction.Dispose();
                                    continue;
                                }
                                if (patientPreliaison.LiaisonId != null && (PreliasionId == null || PreliasionId == 0) && PreTranslatorId != null)
                                {

                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "012-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                        Status = (int)BulkChangesStatus.Failed
                                    });
                                    CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                    currentPatientCount++;
                                    transaction.Dispose();
                                    continue;
                                }
                            }
                            oldPreliasions = patientPreliaison.LiaisonId;
                            oldPreTranslator = patientPreliaison.TranslatorId;
                            if (PreliasionId != 0)
                            {
                                patientPreliaison.LiaisonId = PreliasionId;
                            }
                            if (PreTranslatorId != 0)
                            {
                                patientPreliaison.TranslatorId = PreTranslatorId;
                            }

                            patientPreliaison.Status = true;
                            patientPreliaison.UpdatedOn = DateTime.Now;
                            patientPreliaison.UpdatedBy = User.Identity.GetUserId();
                            _db1.Entry(patientPreliaison).State = EntityState.Modified;
                            _db1.SaveChanges();
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "Patient id=<b style=\"color:forestgreen\">" + patient.Id + "</b> are updated from Counselor <b style=\"color:forestgreen\">" + oldPreliasions.GetLiaisonNameFromID() + "</b> , Translator <b style=\"color:forestgreen\">" + oldPreTranslator.GetLiaisonNameFromID() + "</b> to new Counselor  <b style=\"color:forestgreen\">" + patientPreliaison.LiaisonId.GetLiaisonNameFromID() + "</b>,  Translator <b style=\"color:forestgreen\">" + patientPreliaison.TranslatorId.GetLiaisonNameFromID() + "</b>",
                                Status = (int)BulkChangesStatus.success
                            });



                        }
                        else
                        {
                            if (PreTranslatorId > 0 && PreliasionId == null)
                            {
                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                {
                                    PatientId = patient.Id,
                                    ResultMassage = "12-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                                    Status = (int)BulkChangesStatus.Failed
                                });
                                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                                currentPatientCount++;
                                transaction.Dispose();
                                continue;
                            }
                            //if (PreTranslatorId > 0 && PreliasionId == null)
                            //{
                            //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            //    {
                            //        PatientId = patient.Id,
                            //        ResultMassage = "12-Continue Skip Below: <b style=\"color:red\">Pre-Counselor is must</b> to Select Pre-Translator For Patient Id " + patient.Id + " " + patient.FirstName + " " + patient.LastName,
                            //        Status = (int)BulkChangesStatus.Failed
                            //    });
                            //    CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                            //    currentPatientCount++;
                            //    transaction.Dispose();
                            //    continue;
                            //}
                            Patients_PreLiaisons preLiaisons = new Patients_PreLiaisons();
                            if (PreliasionId != 0)
                            {
                                preLiaisons.LiaisonId = PreliasionId;
                            }
                            if (PreTranslatorId != 0)
                            {
                                preLiaisons.TranslatorId = PreTranslatorId;
                            }

                            preLiaisons.CreatedOn = DateTime.Now;
                            preLiaisons.CreatedBy = User.Identity.GetUserId();
                            preLiaisons.Status = true;
                            _db1.Patients_PreLiaisons.Add(preLiaisons);
                            // _db.SaveChanges();
                            patient.Patients_PreLiaisons = preLiaisons;
                            patient.Patients_PreLiaisonsId = preLiaisons.Id;
                            _db1.Entry(patient).State = EntityState.Modified;
                            _db1.SaveChanges();
                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                            {
                                PatientId = patient.Id,
                                ResultMassage = "New Pre Counselor/Translator are Added Counselor <b style=\"color:forestgreen\">" + preLiaisons.LiaisonId.GetLiaisonNameFromID() + "</b>, Translator <b style=\"color:forestgreen\">" + preLiaisons.TranslatorId.GetLiaisonNameFromID() + "</b>",
                                Status = (int)BulkChangesStatus.success
                            });



                        }

                        #endregion

                        #region MigrateAppointment

                        if (bulkChange.MigrateAppointment == "Yes")
                        {
                            bulkChange.MigrateAppointmentIn_Id.ForEach(c =>
                            {
                                #region Migrate Counselor Appointment

                                if (PreliasionId > 0)
                                {
                                    var Selectedliasionschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == PreliasionId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                    if (Selectedliasionschedule != null)
                                    {
                                        //SelectedliasionSCreated = true;
                                        if (patientPreliaison.LiaisonId != null)
                                        {
                                            var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldPreliasions && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                            if (pa.Count > 0)
                                            {
                                                pa.ForEach(x =>
                                                {
                                                    x.LiaisonID = PreliasionId;
                                                    x.BillingCategoryId = c;
                                                    x.UpdateOn = DateTime.Now;
                                                    x.UpdatedBy = 0;
                                                    _db1.Entry(x).State = EntityState.Modified;
                                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                    {
                                                        PatientId = patient.Id,
                                                        ResultMassage = "Counselor Appointment Id=" + x.ID + " migrate Successfully to new Counselor " + PreliasionId.GetLiaisonNameFromID() + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                        Status = (int)BulkChangesStatus.success
                                                    });
                                                });

                                                //result = "Liaison has been changed along with Appointments successfully.!";
                                            }
                                            else
                                            {
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "No Upcoming appointment found for this Patient with old Counselor " + oldPreliasions.GetLiaisonNameFromID() + " for billing category " + c.GetBillingCatagoryNameById(),
                                                    Status = (int)BulkChangesStatus.Warning
                                                });
                                            }

                                        }
                                        else
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "Appointment Could Not be Migrate as no old Counselor Found For this Patient",
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                        }
                                    }
                                    else
                                    {
                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                        {
                                            PatientId = patient.Id,
                                            ResultMassage = "Can not migrate Appointment as Counselor " + patientPreliaison.LiaisonId.GetLiaisonNameFromID() + " Schedules not found, please create Counselor schedule first.!",
                                            Status = (int)BulkChangesStatus.Warning
                                        });
                                    }
                                }
                                else if (PreliasionId == null || PreliasionId == 0)
                                {
                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "Can not migrate Appointment as you have not selected any Counselor",
                                        Status = (int)BulkChangesStatus.Warning
                                    });
                                }
                                #region commentCode
                                //else if (PreliasionId == 0)
                                //{
                                //    if (oldPreliasions != null)
                                //    {
                                //        //below code will work when we will manage appointment by Billing category
                                //        var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldPreliasions && x.StartTime > DateTime.Now && x.BillingCategoryId != bulkChange.MigrateAppointmentIn_Id).ToList();

                                //        if (pa.Count > 0)
                                //        {
                                //            pa.ForEach(x =>
                                //            {
                                //                //x.LiaisonID = SelectedLiaisonId;
                                //                x.UpdateOn = DateTime.Now;
                                //                x.BillingCategoryId = bulkChange.MigrateAppointmentIn_Id;
                                //                x.UpdatedBy = 0;
                                //                _db1.Entry(x).State = EntityState.Modified;
                                //                result.Add(new BulkChangesLogViewModel()
                                //                {
                                //                    PatientId = patient.Id,
                                //                    ResultMassage = "Counselor Appointment Id=" + x.ID + " migrate Successfully to new Category " + bulkChange.MigrateAppointmentIn_Id,
                                //                    Status = true
                                //                });
                                //            });

                                //            //result = "Liaison has been changed along with Appointments successfully.!";
                                //        }
                                //        else
                                //        {
                                //            result.Add(new BulkChangesLogViewModel()
                                //            {
                                //                PatientId = patient.Id,
                                //                ResultMassage = "No Upcoming appointment found for this Patient with old Counselor",
                                //                Status = true
                                //            });
                                //        }

                                //    }
                                //    else
                                //    {
                                //        result.Add(new BulkChangesLogViewModel()
                                //        {
                                //            PatientId = patient.Id,
                                //            ResultMassage = "No old Counselor Found For this Patient",
                                //            Status = false
                                //        });
                                //        // result = "Counselor has been changed successfully.No Counselor was found so No Appointments Migrates.!";
                                //    }
                                //}

                                #endregion
                                #endregion

                                #region Migrate Translator Appointment
                                if (PreTranslatorId > 0)
                                {
                                    var SelectedTranslatorschedule = _db1.doctorSchedules.Where(x => x.LiaisonID == PreTranslatorId).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                    if (SelectedTranslatorschedule != null)
                                    {
                                        //SelectedliasionSCreated = true;
                                        if (patientPreliaison.LiaisonId != null)
                                        {
                                            var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldPreTranslator && x.StartTime > DateTime.Now && x.BillingCategoryId == c).ToList();

                                            if (pa.Count > 0)
                                            {
                                                pa.ForEach(x =>
                                                {
                                                    x.LiaisonID = PreTranslatorId;
                                                    x.BillingCategoryId = c;
                                                    x.UpdateOn = DateTime.Now;
                                                    x.UpdatedBy = 0;
                                                    _db1.Entry(x).State = EntityState.Modified;
                                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                    {
                                                        PatientId = patient.Id,
                                                        ResultMassage = "Translator Appointment Id=" + x.ID + " migrate Successfully to new Translator " + PreTranslatorId.GetLiaisonNameFromID() + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                        Status = (int)BulkChangesStatus.success
                                                    });
                                                });

                                            }
                                            else
                                            {
                                                result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                                {
                                                    PatientId = patient.Id,
                                                    ResultMassage = "No Upcoming appointment found for this Patient with old Translator " + oldPreTranslator.GetLiaisonNameFromID() + " for Billing Category" + c.GetBillingCatagoryNameById(),
                                                    Status = (int)BulkChangesStatus.Warning
                                                });
                                            }

                                        }
                                        else
                                        {
                                            result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                            {
                                                PatientId = patient.Id,
                                                ResultMassage = "Appointment Could Not be Migrate as no old Translator Found For this Patient",
                                                Status = (int)BulkChangesStatus.Warning
                                            });
                                        }
                                    }
                                    else
                                    {
                                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                        {
                                            PatientId = patient.Id,
                                            ResultMassage = "Can not migrate Appointment as Translator " + patientPreliaison.TranslatorId.GetLiaisonNameFromID() + " Schedules not found, please create Counselor schedule first.!",
                                            Status = (int)BulkChangesStatus.Warning
                                        });
                                    }
                                }
                                else if (PreTranslatorId == 0 || PreTranslatorId == null)
                                {

                                    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                                    {
                                        PatientId = patient.Id,
                                        ResultMassage = "Can not migrate Appointment as you have not selected any Pre-Translator",
                                        Status = (int)BulkChangesStatus.Warning
                                    });
                                }
                                #region commnet
                                //else if (PreTranslatorId == 0)
                                //{
                                //    if (oldPreliasions != null)
                                //    {
                                //        //below code will work when we will manage appointment by Billing category
                                //        var pa = _db1.patientAppointments.Where(x => x.PatientID == patient.Id && x.LiaisonID == oldPreTranslator && x.StartTime > DateTime.Now && x.BillingCategoryId != bulkChange.MigrateAppointmentIn_Id).ToList();

                                //        if (pa.Count > 0)
                                //        {
                                //            pa.ForEach(x =>
                                //            {
                                //                //x.LiaisonID = SelectedLiaisonId;
                                //                x.UpdateOn = DateTime.Now;
                                //                x.BillingCategoryId = bulkChange.MigrateAppointmentIn_Id;
                                //                x.UpdatedBy = 0;
                                //                _db1.Entry(x).State = EntityState.Modified;
                                //                result.Add(new BulkChangesLogViewModel()
                                //                {
                                //                    PatientId = patient.Id,
                                //                    ResultMassage = "Translator Appointment Id=" + x.ID + " migrate Successfully to new Category " + bulkChange.MigrateAppointmentIn_Id,
                                //                    Status = true
                                //                });
                                //            });

                                //            //result = "Liaison has been changed along with Appointments successfully.!";
                                //        }
                                //        else
                                //        {
                                //            result.Add(new BulkChangesLogViewModel()
                                //            {
                                //                PatientId = patient.Id,
                                //                ResultMassage = "No Upcoming appointment found for this Patient with old Translator",
                                //                Status = true
                                //            });
                                //        }

                                //    }
                                //    else
                                //    {
                                //        result.Add(new BulkChangesLogViewModel()
                                //        {
                                //            PatientId = patient.Id,
                                //            ResultMassage = "No old Translator Found For this Patient",
                                //            Status = false
                                //        });
                                //        // result = "Counselor has been changed successfully.No Counselor was found so No Appointments Migrates.!";
                                //    }
                                //}
                                #endregion
                                #endregion
                            });

                        }
                        //else
                        //{
                        //    result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        //    {
                        //        PatientId = patient.Id,
                        //        ResultMassage = "No Appointment Migrate As you have selected to not Migrate Appointment",
                        //        Status = (int)BulkChangesStatus.Warning
                        //    });
                        //}


                        #endregion



                        _db1.Entry(patient).State = EntityState.Modified;
                        _db1.SaveChanges();
                        transaction.Commit();
                        //patient.Id.ConsoleMassage(ind + " Successful- Patient id ");
                    }
                    catch (TransactionException ex)
                    {
                        transaction.Rollback();
                        var rolebackResult = result.BulkChangesLogList.Where(x => x.PatientId == patients && x.Status == (int)BulkChangesStatus.success).ToList();
                        rolebackResult.ForEach(x =>
                        {
                            result.BulkChangesLogList.Remove(x);
                        });
                        result.BulkChangesLogList.Add(new BulkChangesLogViewModel()
                        {
                            PatientId = patients,
                            ResultMassage = "Patient Id=" + patient + " Saving Failed " + ex.ToString(),
                            Status = (int)BulkChangesStatus.Failed
                        });
                    }
                    transaction.Dispose();
                }


                //CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), ind);
                ind++;

                CalculateBulkChangesProgress(bulkChange.PatientsList.Count(), currentPatientCount);
                currentPatientCount++;

            }



            // _db1.Database.Connection.State.ToString().ConsoleMassage("Connection");
            // result.ConsoleMassage("Result");
            _db1.Database.Connection.Close();
            if (result.BulkChangesLogList.Count() == 0)
            {
                result.BulkChangesLogList.Add(new BulkChangesLogViewModel
                {
                    PatientId = 0,
                    ResultMassage = "Some Thing Went Wrong",
                    Status = (int)BulkChangesStatus.Failed
                });
            }
            return result;
        }
    }
}