using CCM.Helpers;
using CCM.Models;
using CCM.Models.BackGroundJob;
using ClientQuickstart;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CCM
{
    public class MvcApplication : HttpApplication
    {
        string connString = ConfigurationManager.ConnectionStrings["myCCMhealthDB"].ConnectionString;
        BackgroundWorker backgroundWorker;
       
        public bool isWorking = false;
        protected void Application_Start()
        {

            // Log4Net for handling Loging
            log4net.Config.XmlConfigurator.Configure();


            //RouteTable.Routes.MapHubs();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //RouteTable.Routes.MapHubs();      // Here is HUB initialization 
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 3600000;//1000 * 60; //10000;Runtime testing
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            //Start SqlDependency with application initialization
            SqlDependency.Start(connString);
           //var i= BillingCodeHelper.cmmid;
        }

        private  void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
          PatientReadingBackGroundJob.AutoSyncPatientReading();
        }
        protected void Application_End()
        {
            //Stop SQL dependency
            SqlDependency.Stop(connString);
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (isWorking == false)
                {
                    backgroundWorker.RunWorkerAsync();
                }
                
            }
            catch (Exception ex)
            {
                HelperExtensions.WriteErrorLog(ex);

            }
        }
        
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                isWorking = true;
                
                CatagoryCyclesStatusUpdate.UpdateCategoryCycle( );
                isWorking = false;
                System.Threading.Thread.Sleep(300000);
            }
            catch (Exception ex)
            {
                isWorking = false;
                HelperExtensions.WriteErrorLog(ex);
            }
        }

       
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // filters.Add(new HandleErrorAttribute());
            filters.Add(new ErrorLoggerAttribute());
        }
        //public static void RegisterRoutes(RouteCollection routes)
        //{
        //    // ...
        //    routes.IgnoreRoute("elmah.axd");
        //    // ...
        //}


  
    }
}