using CCM.Models;
using CCM.Models.CCMBILLINGS;
using CCM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;

namespace DataManipulationApp
{
    class Program
    {
        private static ApplicationdbContect _DBb = new ApplicationdbContect(Properties.Settings.Default.ConnectionString);

        static void Main(string[] args)
        {
            //IncomingPhoneNumberResourceInfo();
            //CountTotalPhoneCreatedontwilio();
            //maketransection();
            //UpdateData();
            _DBb.Database.Connection.Open();
            UpdatePatients();
            Console.WriteLine("Completed");
            Console.ReadLine();
        }

        private static void UpdatePatients()
        {

            var patients = _DBb.Patients.Where(x => x.EnrollmentSubStatus == "Active Enrolled").AsQueryable().ToList();
            foreach (var x in patients)
            {

                //CheckDatabaseStateOpenIfClosed();
                var transection = _DBb.Database.BeginTransaction();

                try
                {
                    Console.WriteLine("Patient Id"+x.Id+" Cycle is " + x.Cycle);
                    //CheckDatabaseStateOpenIfClosed();
                    var exiting = _DBb.Patients_BillingCategories.Where(c => c.PatientId == x.Id && c.BillingCategoryId == 2 && c.Status == true && c.IsTranslator == false).FirstOrDefault();
                    if (exiting == null)
                    {
                        var PatientBilling = new Patients_BillingCategories();
                        PatientBilling.PatientId = x.Id;
                        PatientBilling.BillingCategoryId = 2;
                        PatientBilling.CreatedOn = DateTime.Now;
                        PatientBilling.CreatedBy = "bcbfb545-1d9e-40a5-b823-3616c5fc4050";//standadmin
                        PatientBilling.EnrolledOn = DateTime.Now;
                        PatientBilling.Status = true;
                        PatientBilling.LiaisonId = 1125;//bruk auston
                        PatientBilling.IsTranslator = false;
                        _DBb.Patients_BillingCategories.Add(PatientBilling);
                        Console.WriteLine("Billing Category Added");
                    }
                    else {
                        Console.WriteLine("Billing Category Already Exit");
                    }
                    //var exiting1 = _DBb.Patients_BillingCategories.Where(c => c.PatientId == x.Id && c.BillingCategoryId == 2 && c.Status == true && c.IsTranslator == true).FirstOrDefault();
                    //if (exiting1 == null)
                    //{
                    //    var PatientBilling1 = new Patients_BillingCategories();
                    //    PatientBilling1.PatientId = x.Id;
                    //    PatientBilling1.BillingCategoryId = 2;
                    //    PatientBilling1.CreatedOn = DateTime.Now;
                    //    PatientBilling1.CreatedBy = "bcbfb545-1d9e-40a5-b823-3616c5fc4050";//standadmin
                    //    PatientBilling1.EnrolledOn = DateTime.Now;
                    //    PatientBilling1.Status = true;
                    //    PatientBilling1.LiaisonId = 1106;//ahmad
                    //    PatientBilling1.IsTranslator = true;
                    //    _DBb.Patients_BillingCategories.Add(PatientBilling1);
                    //}

                    //_DBb.SaveChanges();

                    var existing = _DBb.CategoriesStatuses.Where(c => c.PatientId == x.Id && c.Cycle == x.Cycle && c.BillingCategoryId == 2 && c.Status == "Enrolled").FirstOrDefault();
                    if (existing == null)
                    {
                        var CategoriesStatus = new CategoriesStatuses();
                        CategoriesStatus.PatientId = x.Id;
                        CategoriesStatus.Status = "Enrolled";
                        CategoriesStatus.Cycle = x.Cycle;
                        CategoriesStatus.CreatedOn = DateTime.Now;
                        CategoriesStatus.CreatedBy = "bcbfb545-1d9e-40a5-b823-3616c5fc4050";
                        CategoriesStatus.BillingCategoryId = 2;
                        _DBb.CategoriesStatuses.Add(CategoriesStatus);
                        Console.WriteLine("Category Status Already Exit");
                    }
                    else {
                        Console.WriteLine("Category Status Already Exit");
                    }

                    _DBb.SaveChanges();
                    transection.Commit();
                    Console.WriteLine(x.Id + " Successfully");
                }
                catch (Exception e)
                {
                    transection.Rollback();
                    Console.WriteLine("=========================Error" + x.Id +e+ " =============================");
                }


                transection.Dispose();
            }
        }

        private static void CheckDatabaseStateOpenIfClosed()
        {

            if (_DBb.Database.Connection.State != ConnectionState.Open)
            {
                _DBb.Database.Connection.Open();
            }
        }

        private static void CountTotalPhoneCreatedontwilio()
        {
            TwilioClient.Init(Properties.Settings.Default.TwilioAccountSid, Properties.Settings.Default.TwilioAuthToken);
            var type = new List<string> { "carrier" };

            var incomingPhoneNumbers = IncomingPhoneNumberResource.Read(limit: 300).ToList();
            Console.WriteLine(incomingPhoneNumbers.Count());
        }

        private static void maketransection()
        {
            using (_DBb)
            {
                using (var transaction = _DBb.Database.BeginTransaction())
                {
                    try
                    {

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        private static void IncomingPhoneNumberResourceInfo()
        {
            TwilioClient.Init(Properties.Settings.Default.TwilioAccountSid, Properties.Settings.Default.TwilioAuthToken);
            var type = new List<string> { "carrier" };

            var incomingPhoneNumbers = IncomingPhoneNumberResource.Read(limit: 300).ToList();
            var TwilioNumber = new List<TwilioNumbersTable>();
            TwilioNumber = _DBb.TwilioNumbersTable.ToList();

            //match twilio table number with api number not to delete tham
            int inde = 1;
            Console.WriteLine("===============Matched Number with API number with Our Account===============");
            TwilioNumber.ForEach(x =>
            {

                var number = incomingPhoneNumbers.Where(n => n.PhoneNumber.ToString() == x.MobilePhoneNumber).FirstOrDefault();
                Console.WriteLine(inde + ": API Number Count=" + incomingPhoneNumbers.Count());
                if (number != null)
                {
                    Console.WriteLine("API-Number= " + number.PhoneNumber.ToString() + " vs " + "Local table Number= " + x.MobilePhoneNumber);
                    incomingPhoneNumbers.Remove(number);
                }
                else
                {
                    Console.WriteLine(x.MobilePhoneNumber + " Does not Exit");
                    try
                    {
                        var liasion = _DBb.Liaisons.Where(l => l.TwilioNumbersTableId == x.Id).FirstOrDefault();
                        if (liasion != null)
                        {
                            Console.WriteLine("Liasion ID= " + liasion.Id + "  Name= " + liasion.LastName + liasion.FirstName + "IsTranslator=" + liasion.IsTranslator);
                            liasion.TwilioNumbersTableId = null;
                            _DBb.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("No Liasion Found");
                        }
                        _DBb.TwilioNumbersTable.Remove(x);
                        _DBb.SaveChanges();
                        Console.WriteLine(x.MobilePhoneNumber + "=> Number Deleted Successfully from Local Table");
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                }

                Console.WriteLine("==============================");
                inde++;
            });

            //delete unmatch number from twilio account
            Console.WriteLine("===============Un Matched Number only available on API with Our Account===============");
            Console.WriteLine(incomingPhoneNumbers.Count().ToString());
            incomingPhoneNumbers.ForEach(x =>
            {

                Console.WriteLine(x.FriendlyName);
                Console.WriteLine(x.PhoneNumber);
                Console.WriteLine(x.Sid);
                Console.WriteLine(x.DateCreated);
                Console.WriteLine(x.DateUpdated);
                Console.WriteLine(x.Uri);
                try
                {
                    IncomingPhoneNumberResource.Delete(pathSid: x.Sid);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

        }



        private static void UpdateData()
        {

            List<Liaison> liaisons = new List<Liaison>();

            using (var _db = new ApplicationdbContect(Properties.Settings.Default.ConnectionString))
            {

                liaisons = _db.Liaisons.Where(x => x.TwilioNumbersTableId != null).ToList();
                foreach (var i in liaisons)
                {

                    TwilioClient.Init(Properties.Settings.Default.TwilioAccountSid, Properties.Settings.Default.TwilioAuthToken);

                    //var availablePhoneNumberCountry = AvailablePhoneNumberCountryResource.Fetch(
                    // pathCountryCode: "US"
                    // );

                    //Console.WriteLine();

                    #region Check Phone Number info friendly name etc
                    var type = new List<string> { "carrier" };

                    var phoneNumber = PhoneNumberResource.Fetch(
                        type: type,
                        pathPhoneNumber: new Twilio.Types.PhoneNumber(i.TwilioCallerId)
                    );

                    Console.WriteLine(phoneNumber.Carrier);
                    Console.WriteLine(phoneNumber.NationalFormat);
                    Console.WriteLine(phoneNumber.PhoneNumber);
                    #endregion


                    #region Map existing number in twilio table
                    if (phoneNumber != null)
                    {
                        TwilioNumbersTable tn = new TwilioNumbersTable();
                        tn.FriendlyPhoneNumer = phoneNumber.NationalFormat;
                        tn.MobilePhoneNumber = phoneNumber.PhoneNumber.ToString();
                        tn.Status = true;
                        tn.CreatedOn = DateTime.Now;
                        tn.CreatedBy = "bcbfb545-1d9e-40a5-b823-3616c5fc4050";
                        _db.TwilioNumbersTable.Add(tn);
                        _db.SaveChanges();
                        i.TwilioNumbersTableId = tn.Id;
                        i.UpdatedBy = "bcbfb545-1d9e-40a5-b823-3616c5fc4050";
                        i.UpdatedOn = DateTime.Now;
                        _db.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    #endregion
                }

            }
        }


    }
}