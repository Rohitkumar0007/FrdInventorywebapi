using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FRD_InventoryWebApi.Controllers
{
    public class ScheduleActivityController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;
        ServiceReference2.CallContext Cct = new ServiceReference2.CallContext();
        ServiceReference2.AMY_FRDServiceClient obj = new ServiceReference2.AMY_FRDServiceClient();

        [Route("api/FRD/getCountingSchedules")]
        [HttpPost]
        public ScheduleActivityResponse ScheduleActivity(ScheduleActivityReq uId)  //
        {

            ScheduleActivityResponse res = new ScheduleActivityResponse();
            List<ScheduleActivityResponseList> ScheduleList = new List<ScheduleActivityResponseList>();
            try
            {


                query = "Sp_ScheduleActivity";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "GetScheduleActivity");
                dbcommand.Parameters.AddWithValue("@userId", uId.UserId);
                //dbcommand.Parameters.AddWithValue("@ReqNo", uId.ReqNo);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    ScheduleActivityResponseList m = new ScheduleActivityResponseList();
                    m.SID = row["SID"].ToString();
                    m.ScheduleActivityNo = row["ScheduleId"].ToString();
                    m.WareHouseNo = row["WareHouse"].ToString();
                    m.FromTime = row["Fromdate_Time"].ToString();
                    m.ToTime = row["Todate_Time"].ToString();
                    //m.ItemNameArabic = row["itemArabicName"].ToString();
                    //m.CreatedBy = row["UserName"].ToString();
                    //m.Branch = row["WareHouseName"].ToString();
                    //m.ApprovedQty = row["ApprovedQty"].ToString();
                    //m.PickedQty = row["PickedQty"].ToString();
                    //m.TONum = row["TransferOrder"].ToString();




                    ScheduleList.Add(m);
                }
                res.dataList = ScheduleList;
                res.Status = "Success";
                res.Message = "Data retrived successfully";



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

        public class ScheduleActivityReq
        {
            public string UserId { get; set; }
            //public string ReqNo { get; set; }
        }

        public class ScheduleActivityResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<ScheduleActivityResponseList> dataList
            { get; set; }

        }
        public class ScheduleActivityResponseList
        {
            public string SID { get; set; }
            public string ScheduleActivityNo { get; set; }
            public string WareHouseNo { get; set; }
            public string FromTime { get; set; }
            public string ToTime { get; set; }
            //public string SID { get; set; }
            //public string ItemNameArabic { get; set; }
            //public string ItemName { get; set; }
            //public string ApprovedQty { get; set; }
            //public string PickedQty { get; set; }
            //public string TONum { get; set; }


        }
    }
    }
