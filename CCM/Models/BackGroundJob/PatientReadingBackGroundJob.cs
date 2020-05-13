using CCM.Helpers;
using CCM.Models.RPM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace CCM.Models.BackGroundJob
{
    public static class PatientReadingBackGroundJob
    {
         static readonly ApplicationdbContect _dbccm = new ApplicationdbContect();

        internal async static  void AutoSyncPatientReading()
        {
            List<PatientDeviceReadingsRequest> PD_RequestList = new List<PatientDeviceReadingsRequest>();
            List<PatientDeviceReadingsRequest> FilteredPD_RequestList = new List<PatientDeviceReadingsRequest>();
            List<GetMeterReadingdetailsclass> GetMeterReadinglist = new List<GetMeterReadingdetailsclass>();
            List<int> ReadingSyncPrimeryKeyID = new List<int>();
            bool IsToUpdateSyncDone = false;
            PD_RequestList = _dbccm.Database.SqlQuery<PatientDeviceReadingsRequest>("GetPatientReadings").ToList();

            if (PD_RequestList.Count % 2 == 0)
            {
                for (int i = 0; i < PD_RequestList.Count;)
                {
                    if (PD_RequestList[i].PatientId == PD_RequestList[i + 1].PatientId &&
                        PD_RequestList[i].RPMServiceId == PD_RequestList[i + 1].RPMServiceId &&
                        PD_RequestList[i].Type == "Attach" && PD_RequestList[i + 1].Type == "Detached"
                        )
                    {
                        FilteredPD_RequestList.Add(PD_RequestList[i]);
                        FilteredPD_RequestList.Add(PD_RequestList[i + 1]);
                        i++;
                    }
                    i++;
                }
            }
            else
            {
                for (int i = 0; i < PD_RequestList.Count - 1;)
                {
                    if (PD_RequestList[i].PatientId == PD_RequestList[i + 1].PatientId &&
                        PD_RequestList[i].RPMServiceId == PD_RequestList[i + 1].RPMServiceId &&
                        PD_RequestList[i].Type == "Attach" && PD_RequestList[i + 1].Type == "Detached"
                        )
                    {
                        FilteredPD_RequestList.Add(PD_RequestList[i]);
                        FilteredPD_RequestList.Add(PD_RequestList[i + 1]);
                        i++;
                    }
                    i++;
                }
            }

            GetMeterReadingdetailsclass GetMeterReadingdetailsclassObj;
            for (int i = 0; i < FilteredPD_RequestList.Count; i = i + 2)
            {
                IsToUpdateSyncDone = false;
                DateTime date_startObj = new DateTime();
                DateTime date_endObj = new DateTime();
                date_startObj = (DateTime)FilteredPD_RequestList[i].DatePerformed;
                date_endObj = (DateTime)FilteredPD_RequestList[i + 1].DatePerformed;
                string date_start = AttachZeroIfRequired(date_startObj.Year) + "-" + AttachZeroIfRequired(date_startObj.Month) + "-" + AttachZeroIfRequired(date_startObj.Day) + "T" + AttachZeroIfRequired(date_startObj.Hour) + ":" + AttachZeroIfRequired(date_startObj.Minute) + ":" + AttachZeroIfRequired(date_startObj.Second);
                string date_end = AttachZeroIfRequired(date_endObj.Year) + "-" + AttachZeroIfRequired(date_endObj.Month) + "-" + AttachZeroIfRequired(date_endObj.Day) + "T" + AttachZeroIfRequired(date_endObj.Hour) + ":" + AttachZeroIfRequired(date_endObj.Minute) + ":" + AttachZeroIfRequired(date_endObj.Second);
                GetMeterReadingdetailsclassObj = new GetMeterReadingdetailsclass();
                GetMeterReadingdetailsclassObj.api_key = "F1AC8019-3BA0-4DFC-A677-27F155B0A012-1584719321";
                GetMeterReadingdetailsclassObj.date_start = date_start;
                GetMeterReadingdetailsclassObj.date_end = date_end;
                GetMeterReadingdetailsclassObj.ingest_date_start = date_start;
                GetMeterReadingdetailsclassObj.ingest_date_end = date_end;
                GetMeterReadingdetailsclassObj.meter_ids = new string[] { FilteredPD_RequestList[i].SerialNumber.ToString() };
                GetMeterReadingdetailsclassObj.reading_type = new string[] { "blood_glucose", "blood_pressure", "weight" };
                string json = JsonConvert.SerializeObject(GetMeterReadingdetailsclassObj);
                string response = await CCMRequestRestAPI.GetResponsePostRequest("https://api.iglucose.com/readings/", json);
                ReadingResponseListParserClass ReadingListFullBO = new ReadingResponseListParserClass();
                ReadingListFullBO = JsonConvert.DeserializeObject<ReadingResponseListParserClass>(response);
                if (ReadingListFullBO != null)
                {
                    if (ReadingListFullBO.readings.Count > 0)
                    {
                        foreach (Reading ReadingObj in ReadingListFullBO.readings)
                        {
                            try
                            {
                                int result = _dbccm.Database.SqlQuery<int>("InsertRpm_PatientDeviceReading @Battery,@Blood_glucose_mgdl,@Reading_type,@Reading_id,@Time_zone_offset,@Blood_glucose_mmol" +
                                    ",@Device_model,@Date_recorded,@Date_received,@Before_meal,@Device_id_RPMVendor,@RPMServiceId,@PatientId,@DevicetId,@IsActive,@ReasonForDeactivate,@CreatedBy,@ModifiedBy",
                                  new SqlParameter("@Battery", ReadingObj.battery), new SqlParameter("@Blood_glucose_mgdl", ReadingObj.blood_glucose_mgdl),
                                  new SqlParameter("@Reading_type", ReadingObj.reading_type), new SqlParameter("@Reading_id", ReadingObj.reading_id),
                                  new SqlParameter("@Time_zone_offset", ReadingObj.time_zone_offset), new SqlParameter("@Blood_glucose_mmol", ReadingObj.blood_glucose_mmol),
                                  new SqlParameter("@Device_model", ReadingObj.device_model), new SqlParameter("@Date_recorded", ReadingObj.date_recorded),
                                  new SqlParameter("@Date_received", ReadingObj.date_received), new SqlParameter("@Before_meal", ReadingObj.before_meal),
                                  new SqlParameter("@Device_id_RPMVendor", ReadingObj.device_id), new SqlParameter("@RPMServiceId", FilteredPD_RequestList[i].RPMServiceId),
                                  new SqlParameter("@PatientId", FilteredPD_RequestList[i].PatientId), new SqlParameter("@DevicetId", FilteredPD_RequestList[i].DevicetId),
                                  new SqlParameter("@IsActive", 1), new SqlParameter("@ReasonForDeactivate", string.Empty),
                                  new SqlParameter("@CreatedBy", "Admin From Service"), new SqlParameter("@ModifiedBy", "Admin From Service")
                                ).FirstOrDefault();
                                IsToUpdateSyncDone = true;
                            }
                            catch (Exception ex)
                            {
                                IsToUpdateSyncDone = false;
                                break;
                            }
                        }
                    }
                }

                if (IsToUpdateSyncDone == true)
                {
                    // Add List of success bit here 
                    ReadingSyncPrimeryKeyID.Add(FilteredPD_RequestList[i].Id);
                    ReadingSyncPrimeryKeyID.Add(FilteredPD_RequestList[i + 1].Id);
                }
                // ToDo Start **************
                // When Data is inserted Sucessfully Please Update Table Name = "DeviceMappingHistories" IsSyncFromLive = true
                // ToDo End   **************
            }

            // ToDO ********************************************
            for (int x = 0; x < ReadingSyncPrimeryKeyID.Count; x++)
            {
                // Update Code here for All Id IN List nam,ely  ReadingSyncPrimeryKeyID.Add(FilteredPD_RequestList[i+1].Id);
                // in table DeviceMappingHistories
                int UpdateResult = 0;
                UpdateResult = _dbccm.Database.SqlQuery<int>("UpdateDeviceMappingHistoriesSync @Id,@IsSyncFormlive",
                                  new SqlParameter("@Id", ReadingSyncPrimeryKeyID[x]),
                                  new SqlParameter("@IsSyncFormlive", 1)
                                ).FirstOrDefault();
            }
        }

        private static string AttachZeroIfRequired(int value)
        {
            string ValueStr = Convert.ToString(value);
            if (value < 10)
            {
                ValueStr = "0" + value;
            }

            return ValueStr;
        }

        #region "Request Classes"
        public class GetMeterclass
        {
            public string api_key { get; set; }
        }
        public class GetMeterReadingByIDsclass
        {
            public string api_key { get; set; }
            public string[] meter_ids { get; set; }

        }
        public class GetMeterReadingdetailsclass
        {
            public string api_key { get; set; }
            public string date_start { get; set; }
            public string date_end { get; set; }
            public string ingest_date_start { get; set; }
            public string ingest_date_end { get; set; }
            public string[] meter_ids { get; set; }
            public string[] reading_type { get; set; }

        }
        #endregion
        //Test  Running Method************************************************************
        #region "Response Classes"

        // First Request Reading Request
        public class Status
        {
            public int status_code { get; set; }
            public int devices_in_response { get; set; }
            public string status_message { get; set; }
            public int readings_in_response { get; set; }
        }
        public class Reading
        {
            public int battery { get; set; }
            public double blood_glucose_mgdl { get; set; }
            public string reading_type { get; set; }
            public int reading_id { get; set; }
            public double time_zone_offset { get; set; }
            public double blood_glucose_mmol { get; set; }
            public string device_model { get; set; }
            public DateTime date_recorded { get; set; }
            public DateTime date_received { get; set; }
            public bool before_meal { get; set; }
            public string device_id { get; set; }
        }
        public class ReadingResponseListParserClass
        {
            public Status status { get; set; }
            public List<Reading> readings { get; set; }
        }


        // Second Request Current Meters List
        public class StatusMeters
        {
            public string status_message { get; set; }
            public int meters_in_response { get; set; }
            public string units_of_measure { get; set; }
            public int status_code { get; set; }
        }

        public class MetersListClass
        {
            public StatusMeters status { get; set; }
            public IList<int> meters { get; set; }
        }

        #endregion
    }
}