using CCM.Helpers;
using CCM.Models.CCMBILLINGS;
using CCM.Models.CCMBILLINGS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CCM.Models
{
    public static class DataManipulation
    {
        private static readonly ApplicationdbContect _db = new ApplicationdbContect();



        private static void ManipulateLiasionsData(string userid)
        {
            var liaisonList = _db.Liaisons.ToList();
            List<int> bilingcatagoryid = new List<int>();
            bilingcatagoryid.Add(BillingCodeHelper.cmmBillingCatagoryid);
            //bilingcatagoryid.Add(BillingCodeHelper.G0506BillingCatagoryid);
            try
            {
                foreach (var item in liaisonList)
                {
                    //enrolling liasion in billing cataegory
                    List<Liaisons_BillingCategories> liaisons_BillingCategoriesList = new List<Liaisons_BillingCategories>();
                    foreach (var i in bilingcatagoryid)
                    {
                        Liaisons_BillingCategories laisonCategories = new Liaisons_BillingCategories();
                        laisonCategories.LiaisonId = item.Id;
                        laisonCategories.Status = true;
                        laisonCategories.CreatedOn = DateTime.Now;
                        laisonCategories.CreatedBy = userid;
                        laisonCategories.EnrolledOn = DateTime.Now;
                        laisonCategories.BillingCategoryId = i;

                        laisonCategories.Liaison = item;
                        laisonCategories.BillingCategory = _db.BillingCategories.Where(p => p.BillingCategoryId == i).Select(p => p).FirstOrDefault();
                        liaisons_BillingCategoriesList.Add(laisonCategories);

                    }
                    _db.Liaisons_BillingCategories.AddRange(liaisons_BillingCategoriesList);
                    item.Liaisons_BillingCategories = liaisons_BillingCategoriesList;
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();


                    //saving liasion salary rates
                    List<Liaisons_BillingCategories> billingCategories = item.Liaisons_BillingCategories;

                    foreach (var billingCata in billingCategories)
                    {
                        List<BillingCodes> billingCode = billingCata.BillingCategory.BillingCodes;

                        foreach (var b in billingCode)
                        {
                            try
                            {

                                Liaison_CPTRates laisonrates = new Liaison_CPTRates();

                                int lisasonId = Convert.ToInt32(item.Id);

                                laisonrates.CreatedOn = DateTime.Now;
                                laisonrates.CreatedBy = userid;
                                laisonrates.UpdatedOn = null;
                                laisonrates.UpdatedBy = null;
                                laisonrates.LiaisonId = lisasonId;
                                laisonrates.BillingCode = b.Name;
                                laisonrates.SalaryRate = 1;
                                laisonrates.BillingCodeId = b.Id;

                                _db.Liaison_CPTRates.Add(laisonrates);
                                _db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }

                        }

                    }


                }


            }
            catch (Exception ex)
            {

            }

        }

        internal static void Mapdata(string userid)
        {


            //enrolling all the liaison in the cmm and g0506 billing catagory
            //add new liasion page 
            //SeedDataHelper.SeedData();
            //ManipulateLiasionsData(userid);
            //ManipulatePhysicianData(userid);
            //MapPatientsData(userid);


        }

        private static void ManipulatePhysicianData(string userid)
        {
            List<Physician> physicians = _db.Physicians.ToList();
            foreach (Physician phy in physicians)
            {
                List<BillingCategory> billingCategory = _db.BillingCategories.ToList();
                foreach (BillingCategory billing in billingCategory)
                {
                    List<BillingCodes> billingCodes = billing.BillingCodes;
                    foreach (var code in billingCodes)
                    {
                        try
                        {
                            Physician_CPTRates physicianrate = new Physician_CPTRates();


                            physicianrate.CreatedOn = DateTime.Now;
                            physicianrate.CreatedBy = userid;
                            physicianrate.BillingCodeId = Convert.ToInt32(code.Id);

                            physicianrate.BillingRate = 1;
                            physicianrate.PhysicianId = Convert.ToInt32(phy.Id);


                            physicianrate.InvoiceRate = 1;
                            _db.Physician_CPTRates.Add(physicianrate);
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
        }

        private static void MapPatientsData(string userid)
        {

            int patientsCount = _db.Patients.Count();
            int skip = 0;
            int take = 5;
            int LDeepanID = _db.Liaisons.Where(x => x.FirstName == "Deepan" && x.IsTranslator == false).Select(x => x.Id).FirstOrDefault();
            int TDeepanID = _db.Liaisons.Where(x => x.FirstName == "Deepan" && x.IsTranslator == true).Select(x => x.Id).FirstOrDefault();
            for (int i = skip; i <= patientsCount; i = +skip)
            {
                var patients = _db.Patients.Where(x => x.EnrollmentStatus == "Enrolled" && x.EnrollmentSubStatus == "Active Enrolled").OrderBy(x => x.Id).Skip(skip).Take(take).ToList();
                foreach (var p in patients)
                {

                    //enrolling in ccm
                    //liasion
                    if (p.LiaisonId > 0)
                    {

                        try
                        {
                            var LPatientBilling = new Patients_BillingCategories();
                            LPatientBilling.PatientId = p.Id;
                            LPatientBilling.BillingCategoryId = BillingCodeHelper.cmmBillingCatagoryid;
                            LPatientBilling.CreatedOn = DateTime.Now;
                            LPatientBilling.CreatedBy = userid;
                            LPatientBilling.EnrolledOn = DateTime.Now;
                            LPatientBilling.Status = true;
                            LPatientBilling.LiaisonId = Convert.ToInt32(p.LiaisonId);
                            LPatientBilling.IsTranslator = false;
                            _db.Patients_BillingCategories.Add(LPatientBilling);
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }
                    //Translator
                    if (p.TranslatorId > 0)
                    {
                        try
                        {
                            var TPatientBilling = new Patients_BillingCategories();
                            TPatientBilling.PatientId = p.Id;
                            TPatientBilling.BillingCategoryId = BillingCodeHelper.cmmBillingCatagoryid;
                            TPatientBilling.CreatedOn = DateTime.Now;
                            TPatientBilling.CreatedBy = userid;
                            TPatientBilling.EnrolledOn = DateTime.Now;
                            TPatientBilling.Status = true;
                            TPatientBilling.LiaisonId = Convert.ToInt32(p.TranslatorId);
                            TPatientBilling.IsTranslator = true;
                            _db.Patients_BillingCategories.Add(TPatientBilling);
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }

                    //enrolling in g0506
                    //Liasion
                    if (LDeepanID > 0)
                    {
                        try
                        {
                            var LPatientBilling1 = new Patients_BillingCategories();
                            LPatientBilling1.PatientId = p.Id;
                            LPatientBilling1.BillingCategoryId = BillingCodeHelper.G0506BillingCatagoryid;
                            LPatientBilling1.CreatedOn = DateTime.Now;
                            LPatientBilling1.CreatedBy = userid;
                            LPatientBilling1.EnrolledOn = DateTime.Now;
                            LPatientBilling1.Status = true;
                            LPatientBilling1.LiaisonId = Convert.ToInt32(LDeepanID);
                            LPatientBilling1.IsTranslator = false;
                            _db.Patients_BillingCategories.Add(LPatientBilling1);
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }

                    //Translator
                    if (TDeepanID > 0)
                    {
                        try
                        {
                            var TPatientBilling1 = new Patients_BillingCategories();
                            TPatientBilling1.PatientId = p.Id;
                            TPatientBilling1.BillingCategoryId = BillingCodeHelper.G0506BillingCatagoryid;
                            TPatientBilling1.CreatedOn = DateTime.Now;
                            TPatientBilling1.CreatedBy = userid;
                            TPatientBilling1.EnrolledOn = DateTime.Now;
                            TPatientBilling1.Status = true;
                            TPatientBilling1.LiaisonId = Convert.ToInt32(TDeepanID);
                            TPatientBilling1.IsTranslator = true;
                            _db.Patients_BillingCategories.Add(TPatientBilling1);
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }

                    //changing pre-liaison
                    if (p.LiaisonId > 0 || p.TranslatorId > 0)
                    {
                        try
                        {
                            var Patients_PreLiaison = new Patients_PreLiaisons();
                            Patients_PreLiaison.LiaisonId = p.LiaisonId;
                            Patients_PreLiaison.TranslatorId = p.TranslatorId;
                            Patients_PreLiaison.Status = false;
                            Patients_PreLiaison.CreatedOn = DateTime.Now.Date;
                            Patients_PreLiaison.CreatedBy = userid;
                            _db.Patients_PreLiaisons.Add(Patients_PreLiaison);
                            _db.SaveChanges();
                            p.Patients_PreLiaisonsId = Patients_PreLiaison.Id;
                            p.Patients_PreLiaisons = Patients_PreLiaison;
                            _db.Entry(p).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }

                }


                skip = skip + take;

            }
            skip = 0;
            take = 5;
            for (int j = skip; j < patientsCount; j = +skip)
            {
                var patient2 = _db.Patients.Where(x => x.EnrollmentStatus != "Enrolled" && x.EnrollmentSubStatus != "Active Enrolled").OrderBy(x => x.Id).Skip(skip).Take(take).ToList();

                foreach (var x in patient2)
                {
                    if (x.LiaisonId > 0)
                    {
                        try
                        {
                            var Patients_PreLiaison = new Patients_PreLiaisons();
                            Patients_PreLiaison.LiaisonId = x.LiaisonId;
                            Patients_PreLiaison.TranslatorId = x.TranslatorId;
                            Patients_PreLiaison.Status = true;
                            Patients_PreLiaison.CreatedOn = DateTime.Now.Date;
                            Patients_PreLiaison.CreatedBy = userid;
                            _db.Patients_PreLiaisons.Add(Patients_PreLiaison);
                            _db.SaveChanges();
                            x.Patients_PreLiaisonsId = Patients_PreLiaison.Id;
                            x.LiaisonId = null;
                            x.TranslatorId = null;
                            _db.Entry(x).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        catch (Exception E)
                        {
                            throw E;
                        }
                    }

                }
                skip = skip + take;
            }

        }
    }
}