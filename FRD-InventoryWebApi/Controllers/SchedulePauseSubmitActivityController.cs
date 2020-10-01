using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace FRD_InventoryWebApi.Controllers
{
    public class SchedulePauseSubmitActivityController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;
        ServiceReference2.CallContext Cct = new ServiceReference2.CallContext();
        ServiceReference2.AMY_FRDServiceClient obj = new ServiceReference2.AMY_FRDServiceClient();
        [Route("api/FRD/SchedulePauseSubmitActivity")]
        [HttpPost]
        public SchedulePauseSubmitActivityResponse ScheduleActivity(SchedulePauseSubmitActivityReq uId)  //ScheduleActivityReq uId
        {

            SchedulePauseSubmitActivityResponse res = new SchedulePauseSubmitActivityResponse();

           string json = JsonConvert.SerializeObject(uId.StickerSequenceList);

            List<SchedulePauseSubmitActivityResponseList> SchedulePauseSubmit = new List<SchedulePauseSubmitActivityResponseList>();
            try
            {
                query = "Sp_ScheduleActivity";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "SchedulePauseSubmitActivity");
                dbcommand.Parameters.AddWithValue("@IsSubmit", uId.IsSubmit);
                dbcommand.Parameters.AddWithValue("@SID", uId.SID);
                dbcommand.Parameters.AddWithValue("@ScheduleActivityNo", uId.ScheduleActivityNo);
                dbcommand.Parameters.AddWithValue("@WareHouseNo", uId.WareHouseNo);
                dbcommand.Parameters.AddWithValue("@UserId", uId.UserId);
                dbcommand.Parameters.AddWithValue("@StickerSeq", json);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    if (row["value"].ToString() == "1")
                    {
                        res.Status = "Success";
                        res.Message = "Pause Successfully";

                    }
                    else
                    {
                        res.Status = "Success";
                        res.Message = "Submit Successfully";

                    }



                   // SchedulePauseSubmit.Add(m);
                }
                //res.dataList = SchedulePauseSubmit;
                //res.Status = "Success";
                //res.Message = "Data retrived successfully";



            }


            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = "Data did not retrived successfully";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }

        public class SchedulePauseSubmitActivityReq
        {
            public string IsSubmit { get; set; }
            public string SID { get; set; }
            public string ScheduleActivityNo { get; set; }
            public string WareHouseNo { get; set; }
            public string UserId { get; set; }
            public List<StickerSequenceList> StickerSequenceList { get; set; }
        }
        

        public class SchedulePauseSubmitActivityResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<SchedulePauseSubmitActivityResponseList> dataList
            { get; set; }

        }
        public class SchedulePauseSubmitActivityResponseList
        {
            public string Status { get; set; }
            public string Message { get; set; }

        }
        //public class StickerSequence
        //{
        //    public List<StickerSequenceList> StickerSequenceList { get; set; }
        //    public string IsSubmit { get; set; }
        //    public string SID { get; set; }
        //    public string ScheduleActivityNo { get; set; }
        //    public string WareHouseNo { get; set; }
        //    public string UserId { get; set; }

        //}
        public class StickerSequenceList
        {
            public string StickerSeq { get; set; }

        }

        //public static DataTable ToDataTable<T>(List<T> items)
        //{
        //    DataTable dataTable = new DataTable(typeof(T).Name);
        //    //Get all the properties by using reflection   
        //    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (PropertyInfo prop in Props)
        //    {
        //        //Setting column names as Property names  
        //        dataTable.Columns.Add(prop.Name);
        //    }
        //    foreach (T item in items)
        //    {
        //        var values = new object[Props.Length];
        //        for (int i = 0; i < Props.Length; i++)
        //        {

        //            values[i] = Props[i].GetValue(item, null);
        //        }
        //        dataTable.Rows.Add(values);
        //    }

        //    return dataTable;
        //}

    }
}
