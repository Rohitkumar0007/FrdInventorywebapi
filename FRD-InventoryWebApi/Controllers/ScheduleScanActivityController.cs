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
    public class ScheduleScanActivityController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;
        ServiceReference2.CallContext Cct = new ServiceReference2.CallContext();
        ServiceReference2.AMY_FRDServiceClient obj = new ServiceReference2.AMY_FRDServiceClient();

        [Route("api/FRD/ScheduleScanActivity")]
        [HttpPost]
        public ScheduleScanActivityResponse ScheduleActivity(ScheduleScanActivityReq uId)  //ScheduleActivityReq uId
        {

            ScheduleScanActivityResponse res = new ScheduleScanActivityResponse();
            List<ScheduleScanActivityResponseList> ScheduleScanList = new List<ScheduleScanActivityResponseList>();
            try
            {


                query = "Sp_ScheduleActivity";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "ScheduleScanActivity");
                dbcommand.Parameters.AddWithValue("@SID", uId.SID);
                dbcommand.Parameters.AddWithValue("@ScheduleActivityNo", uId.ScheduleActivityNo);
                dbcommand.Parameters.AddWithValue("@WareHouseNo", uId.WareHouseNo);
                dbcommand.Parameters.AddWithValue("@UserId", uId.UserId);
                dbcommand.Parameters.AddWithValue("@StickerSeq", uId.StickerSeq);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["value"].ToString() == "3")
                        {
                            res.Status = "Failure";
                            res.Message = "Data is already exist";

                        }
                        else
                        {

                            ScheduleScanActivityResponseList m = new ScheduleScanActivityResponseList();
                            m.ItemId = row["ItemId"].ToString();
                            m.ItemName = row["ItemName"].ToString();
                            m.ItemnameArabic = row["ItemnameArabic"].ToString();
                            m.BatchId = row["BatchId"].ToString();
                            m.StickerQty = row["StickerQty"].ToString();
                            m.Config = row["Config"].ToString();
                            m.StickerSeq = row["Stickersequence"].ToString();
                            m.StickerSeqId = row["RandomStickerid"].ToString();



                            ScheduleScanList.Add(m);
                            res.dataList = ScheduleScanList;
                            res.Status = "Success";
                            res.Message = "Data retrived successfully";



                        }
                    }
                }
                else
                {
                    res.Status = "Failure";
                    res.Message = "Sticker does not exist";
                }
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

        public class ScheduleScanActivityReq
        {
            public string SID { get; set; }
            public string ScheduleActivityNo{ get; set; }
            public string WareHouseNo{ get; set; }
            public string UserId{ get; set; }
            public string StickerSeq { get; set; }
        }

        public class ScheduleScanActivityResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<ScheduleScanActivityResponseList> dataList
            { get; set; }

        }
        public class ScheduleScanActivityResponseList
        {
            public string ItemId { get; set; }
            public string ItemName { get; set; }
            public string ItemnameArabic { get; set; }
            public string BatchId { get; set; }
            public string StickerQty { get; set; }
            public string Config { get; set; }
            public string StickerSeq { get; set; }
            public string StickerSeqId { get; set; }
            //public string ItemName { get; set; }
            //public string ApprovedQty { get; set; }
            //public string PickedQty { get; set; }
            //public string TONum { get; set; }


        }

    }
}
