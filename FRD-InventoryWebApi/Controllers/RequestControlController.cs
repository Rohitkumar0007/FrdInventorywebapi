using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using FRD_InventoryWebApi.Models;
using Newtonsoft.Json;

namespace FRD_InventoryWebApi.Controllers
{
    public class RequestControlController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;
        ServiceReference2.CallContext Cct = new ServiceReference2.CallContext();
        ServiceReference2.AMY_FRDServiceClient obj = new ServiceReference2.AMY_FRDServiceClient();

        [Route("api/FRD/GetRequestNo")]
        [HttpPost]
        public GetrequestNo GetRequestNo(ReqUserId uId)
        {

            GetrequestNo res = new GetrequestNo();
            List<GetrequestNoRes> ListView = new List<GetrequestNoRes>();
            try
            {


                query = "Sp_requestControl";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "GetReqNo");
                dbcommand.Parameters.AddWithValue("@userId", uId.UserId);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    GetrequestNoRes m = new GetrequestNoRes();
                    m.RequisitionNo = row["RequestNumber"].ToString();
                    m.LocationId = row["Locationid"].ToString();
                    m.WareHouseName = row["WarehouseName"].ToString();



                    ListView.Add(m);
                }
                res.dataList = ListView;
                res.Status = "Success";
                res.Message = "Data retrieved successfully";



            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = "Data did not retrieved successfully";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }


        [Route("api/FRD/GetCompleteRequestNo")]
        [HttpPost]
        public GetrequestNo GetCompleteRequestNo(ReqUserId uId)
        {

            GetrequestNo res = new GetrequestNo();
            List<GetrequestNoRes> ListView = new List<GetrequestNoRes>();
            try
            {


                query = "Sp_requestControl";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "GetCompleteReqNo");
                dbcommand.Parameters.AddWithValue("@userId", uId.UserId);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    GetrequestNoRes m = new GetrequestNoRes();
                    m.RequisitionNo = row["RequestNumber"].ToString();
                    m.LocationId = row["Locationid"].ToString();
                    m.WareHouseName = row["WarehouseName"].ToString();



                    ListView.Add(m);
                }
                res.dataList = ListView;
                res.Status = "Success";
                res.Message = "Data retrieved successfully";



            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = "Data did not retrieved successfully";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }


        [Route("api/FRD/RequestControlPending")]
        [HttpPost]
        public requestControl RequestControl(ReqUserId uId)
        {

            requestControl res = new requestControl();
            List<RequestControlReq> ListView = new List<RequestControlReq>();
            try
            {


                query = "Sp_requestControl";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "Getdata");
                dbcommand.Parameters.AddWithValue("@userId", uId.UserId);
                dbcommand.Parameters.AddWithValue("@ReqNo", uId.ReqNo);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    RequestControlReq m = new RequestControlReq();
                    m.RequisitionNo = row["RequestNumber"].ToString();
                    m.ReqestDate = row["RequestedDate"].ToString();
                    m.ItemId = row["AXitemId"].ToString();
                    m.ItemName = row["itemname"].ToString();
                    m.ItemNameArabic = row["itemArabicName"].ToString();
                    m.CreatedBy = row["UserName"].ToString();
                    m.Branch = row["WareHouseName"].ToString();
                    m.ApprovedQty = row["ApprovedQty"].ToString();
                    m.RequisitionDetailId = row["RequestDetId"].ToString();


                    ListView.Add(m);
                }
                res.dataList = ListView;
                res.Status = "Success";
                res.Message = "Data retrieved successfully";



            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = "Data did not retrieved successfully";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }

        [Route("api/FRD/RequestControlcomplete")]
        [HttpPost]
        public requestControl RequestControlcomplete(ReqUserId uId)
        {

            requestControl res = new requestControl();
            List<RequestControlReq> ListView = new List<RequestControlReq>();
            List<stikrList> SL = new List<stikrList>();
            try
            {


                query = "Sp_requestControl";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "GetCompletedata");
                dbcommand.Parameters.AddWithValue("@userId", uId.UserId);
                dbcommand.Parameters.AddWithValue("@ReqNo", uId.ReqNo);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    RequestControlReq m = new RequestControlReq();
                    m.RequisitionNo = row["RequestNumber"].ToString();
                    m.ReqestDate = row["RequestedDate"].ToString();
                    m.ItemId = row["AXitemId"].ToString();
                    m.ItemName = row["itemname"].ToString();
                    m.ItemNameArabic = row["itemArabicName"].ToString();
                    m.CreatedBy = row["UserName"].ToString();
                    m.Branch = row["WareHouseName"].ToString();
                    m.ApprovedQty = row["ApprovedQty"].ToString();
                    m.TONum = row["TransferOrder"].ToString();
                    m.LocationId = row["Locationid"].ToString();
                    m.ShippedQty = row["PickedQty"].ToString();

                    ListView.Add(m);
                }

                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    stikrList s = new stikrList();
                    s.StickerSeq = row["stickerSeqNo"].ToString();
                     SL.Add(s);
                 }
                res.dataList = ListView;
                res.StickerList = SL;
                res.Status = "Success";
                res.Message = "Data retrieved successfully";



            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = "Data did not retrieved successfully";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }

        [Route("api/FRD/GetdataQRWise")]
        [HttpPost]
        public GetDataSeqWiseResPonce GetdataQRWise(GetDataSeqWise seq)
        {

            GetDataSeqWiseResPonce res = new GetDataSeqWiseResPonce();
            // List<RequestControlReq> ListView = new List<RequestControlReq>();
            try
            {


                query = "Sp_requestControl";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "GetDataByQRSeqNo");
                dbcommand.Parameters.AddWithValue("@SeqNo", seq.SeqNo);
                dbcommand.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows[0]["ItemId"].ToString() == "-1")
                {
                    res.Status = "Failure";
                    res.Message = "Invalid QR Code";
                    res.VendorId = "";
                    res.PONumber = "";
                    res.BatchId = "";
                    res.ItemId = "";
                    res.Expdate = "";
                    res.ItemName = "";
                    res.ItemnameArabic = "";
                    res.Config = "";
                    res.StickerQty = "";
                    res.VendorName = "";
                    res.BatchAutoIncreeId = "";
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        res.VendorId = row["VendorId"].ToString();
                        res.PONumber = row["PurchaseOrderId"].ToString();
                        res.BatchId = row["BatchNo"].ToString();
                        res.ItemId = row["ItemId"].ToString();
                        res.Expdate = row["ExpiryDate"].ToString();
                        res.ItemName = row["itemname"].ToString();
                        res.ItemnameArabic = row["itemArabicName"].ToString();
                        res.Config = row["CONFIGID"].ToString();
                        res.StickerQty = row["StickerQty"].ToString();
                        res.VendorName = row["Name"].ToString();
                        res.BatchAutoIncreeId= row["BatchId"].ToString();
                        res.UnitId= row["UNITID"].ToString();

                    }
                    res.Status = "Success";
                    res.Message = "Data retrieved successfully";

                }



            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = "Data did not retrieved successfully";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }

        [Route("api/FRD/Shiping")]
        [HttpPost]
        public ShipingResponse Shiping(ShipingRequest sr)
        {

            ShipingResponse res = new ShipingResponse();
            List<GetrequestNoRes> ListView = new List<GetrequestNoRes>();
            try
            {


                // string conn = ConfigurationManager.ConnectionStrings["Conn"].ToString();
                // SqlConnection Conn = new SqlConnection(conn);

                SqlCommand cmd1 = new SqlCommand("Sp_AxWebserviceIntegration", conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.CommandTimeout = 0;
                DataSet sdAX = new DataSet();


                SqlDataAdapter sd = new SqlDataAdapter();

                sd.SelectCommand = cmd1;

                sd.Fill(sdAX);
                
                obj.ClientCredentials.Windows.ClientCredential.Domain = Convert.ToString(sdAX.Tables[0].Rows[0]["Domain"]);
                obj.ClientCredentials.Windows.ClientCredential.UserName = Convert.ToString(sdAX.Tables[0].Rows[0]["Username"]);
                obj.ClientCredentials.Windows.ClientCredential.Password = Convert.ToString(sdAX.Tables[0].Rows[0]["Password"]);
                Cct.Company = Convert.ToString(sdAX.Tables[0].Rows[0]["Company"]);
                Cct.Language = Convert.ToString(sdAX.Tables[0].Rows[0]["Language"]);
                cmd1.Connection.Close();

                string TOnum = "";
                try
                {
                  TOnum = obj.InsertTransferHeader(Cct, sr.RequisitionNo, sr.FromLocation, sr.ToLocation, sr.WebUser);

                }
                catch (Exception ex) {
                   
                    dbcommand = new SqlCommand();
                    dbcommand.Connection = conn;
                    dbcommand.Connection.Open();
                    dbcommand.CommandText = "INSERTSHIPLOG";
                    dbcommand.CommandType = CommandType.StoredProcedure;
                    dbcommand.Parameters.Add("@Tonumber", SqlDbType.NVarChar).Value = Convert.ToString(sr.RequisitionNo);
                    dbcommand.Parameters.Add("@RequisitionNo", SqlDbType.NVarChar).Value = Convert.ToString(sr.FromLocation);
                    dbcommand.Parameters.Add("@FromLocation", SqlDbType.NVarChar).Value = Convert.ToString(sr.ToLocation);
                    dbcommand.Parameters.Add("@ToLocation", SqlDbType.NVarChar).Value = Convert.ToString(sr.ToLocation);
                    dbcommand.Parameters.Add("@Postedby", SqlDbType.NVarChar).Value = Convert.ToString(sr.WebUser);
                    dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);
                    
                    dbcommand.ExecuteNonQuery();
                    dbcommand.Connection.Close();
                    dbcommand.Parameters.Clear();
                }
                

                List<ShipingRequestItemList> list = new List<ShipingRequestItemList>();
                List<Sticker> stickers = new List<Sticker>();

                foreach (var stickr in sr.StickerList)
                {
                    Sticker s = new Sticker();
                    s.StickerSeq = stickr.StickerSeq;
                    stickers.Add(s);
                }

                string ErrorTOnum = "";  string ErrorItemId = ""; decimal Errorqty = 0; string ErrorConfig = "";
                 string ErrorBatchNo = ""; bool errormsg = false;
                try
                {
                    foreach (var task in sr.ItemList)
                    {


                        if (task.PickedQty == "")
                        {
                            task.PickedQty = "0";
                        }
                        if (task.RemainingQty == "")
                        {
                            task.RemainingQty = "0";
                        }

                        if (task.PickedQty != "0")
                        {

                            foreach (var batch in task.BatchNoList)
                            {
                                if (batch.BatchQty == "")
                                {
                                    batch.BatchQty = "0";
                                }
                                if (task.PickedQty != "0")
                                {
                                    decimal qty = 0;
                                    // try
                                    // {
                                    qty = Convert.ToDecimal(batch.BatchQty);
                                    ErrorTOnum = TOnum; ErrorItemId = task.ItemId; Errorqty = qty; ErrorConfig = batch.Config;
                                    ErrorBatchNo = batch.BatchNo;
                                    obj.InsertTransferOrderLines(Cct, TOnum, task.ItemId, qty, batch.Config, batch.BatchNo);
                                    errormsg = true;
                                    // }
                                    //catch (Exception ex) {
                                    //    dbcommand = new SqlCommand();
                                    //    dbcommand.Connection = conn;
                                    //    dbcommand.Connection.Open();
                                    //    dbcommand.CommandText = "INSERTSHIPLOGTransferorderlines";
                                    //    dbcommand.CommandType = CommandType.StoredProcedure;
                                    //    dbcommand.Parameters.Add("@TONum", SqlDbType.NVarChar).Value = Convert.ToString(TOnum);
                                    //    dbcommand.Parameters.Add("@ItemId", SqlDbType.NVarChar).Value = Convert.ToString(task.ItemId);
                                    //    dbcommand.Parameters.Add("@qty", SqlDbType.Decimal).Value = Convert.ToDecimal(qty);
                                    //    dbcommand.Parameters.Add("@Config", SqlDbType.NVarChar).Value = Convert.ToString(batch.Config);
                                    //    dbcommand.Parameters.Add("@BatchNo", SqlDbType.NVarChar).Value = Convert.ToString(batch.BatchNo);
                                    //    dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);
                                    //    dbcommand.ExecuteNonQuery();
                                    //    dbcommand.Connection.Close();
                                    //}



                                }
                                ShipingRequestItemList b = new ShipingRequestItemList();
                                b.ItemId = task.ItemId;
                                b.Reason = task.Reason;
                                b.RemainingQty = task.RemainingQty;
                                b.PickedQty = task.PickedQty;
                                b.BatchNo = batch.BatchNo;
                                b.UserId = sr.WebUser;
                                b.BatchQty = batch.BatchQty;
                                b.RequisitionDetailsId = task.RequisitionDetailId;
                                b.Config = batch.Config;
                                b.BatchAutoIncreeId = batch.BatchAutoIncreeId;
                                list.Add(b);



                            }
                        }
                        else
                        {
                            ShipingRequestItemList b = new ShipingRequestItemList();
                            b.ItemId = task.ItemId;
                            b.Reason = task.Reason;
                            b.RemainingQty = task.RemainingQty;
                            b.PickedQty = task.PickedQty;
                            b.BatchNo = "";
                            b.UserId = sr.WebUser;
                            b.BatchQty = "0";
                            b.RequisitionDetailsId = task.RequisitionDetailId;
                            b.Config = "";
                            b.BatchAutoIncreeId = "";
                            list.Add(b);

                        }

                    }
                }
                catch (Exception ex) {
                    dbcommand = new SqlCommand();
                    dbcommand.Connection = conn;
                    dbcommand.Connection.Open();
                    dbcommand.CommandText = "INSERTSHIPLOGTransferorderlines";
                    dbcommand.CommandType = CommandType.StoredProcedure;
                    dbcommand.Parameters.Add("@TONum", SqlDbType.NVarChar).Value = Convert.ToString(ErrorTOnum);
                    dbcommand.Parameters.Add("@ItemId", SqlDbType.NVarChar).Value = Convert.ToString(ErrorItemId);
                    dbcommand.Parameters.Add("@qty", SqlDbType.Decimal).Value = Convert.ToDecimal(Errorqty);
                    dbcommand.Parameters.Add("@Config", SqlDbType.NVarChar).Value = Convert.ToString(ErrorConfig);
                    dbcommand.Parameters.Add("@BatchNo", SqlDbType.NVarChar).Value = Convert.ToString(ErrorBatchNo);
                    dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);
                    dbcommand.ExecuteNonQuery();
                    dbcommand.Connection.Close();
                }

                string a = ""; errormsg = false;
                try
                {
                    a = obj.ShipOrder(Cct, TOnum);
                    errormsg = true;
                }
                catch(Exception ex){
                    dbcommand = new SqlCommand();
                    dbcommand.Connection = conn;
                    dbcommand.Connection.Open();
                    dbcommand.CommandText = "INSERTSHIPLOGTransferorder";
                    dbcommand.CommandType = CommandType.StoredProcedure;
                    dbcommand.Parameters.Add("@ToNum", SqlDbType.NVarChar).Value = Convert.ToString(TOnum);
                    dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);
                    dbcommand.ExecuteNonQuery();
                    dbcommand.Connection.Close();
                }
               
                if (a == "success")
                {
                    if (errormsg == true)
                    {
                        string sJSONResponse = JsonConvert.SerializeObject(list);
                        string stkr = JsonConvert.SerializeObject(stickers);

                        query = "Sp_requestControl";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.Connection.Open();
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "UpdateRemQty");

                        dbcommand.Parameters.AddWithValue("@TONum", TOnum);


                        dbcommand.Parameters.AddWithValue("@ReqNo", sr.RequisitionNo);
                        dbcommand.Parameters.AddWithValue("@Jsondata", sJSONResponse);
                        dbcommand.Parameters.AddWithValue("@Sticker", stkr);

                        dbcommand.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        res.Status = "success";
                        res.Message = "Item posted successfully";
                        res.TONumber = TOnum;
                    }

                }
                else
                {
                    res.Status = "Failure";
                    res.Message = a;
                    res.TONumber = "";

                }

            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = ex.Message;
                res.TONumber = "";


            }
            return res;
        }

    }



    public class GetDataSeqWise
    {
        public string SeqNo { get; set; }
    }
    public class GetDataSeqWiseResPonce
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string VendorId { get; set; }
        public string PONumber { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemnameArabic { get; set; }
        public string Expdate { get; set; }
        public string BatchId { get; set; }
        public string Config { get; set; }
        public string VendorName { get; set; }
        public string StickerQty { get; set; }
       public string BatchAutoIncreeId { get; set; }
        public string UnitId { get; set; }

    }

    public class GetrequestNo
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<GetrequestNoRes> dataList
        { get; set; }

    }
    public class GetrequestNoRes
    {
        public string RequisitionNo { get; set; }
        public string LocationId { get; set; }
        public string WareHouseName { get; set; }


    }
    public class ShipingRequest
    {
        public string RequisitionNo { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string WebUser { get; set; }
        public List<ShipingRequestItem> ItemList
        { get; set; }
        public List<Sticker> StickerList
        { get; set; }
        


    }
    public class Sticker
    {
        public string StickerSeq { get; set; }
    }
   
    public class ShipingRequestItem
    {
        public string RequisitionDetailId { get; set; }
        public string ItemId { get; set; }
        public string PickedQty { get; set; }
        public string Reason { get; set; }
        public string RemainingQty { get; set; }
        public string Config { get; set; }
        public List<Batch> BatchNoList
        { get; set; }
        //  public string BatchId { get; set; }

    }
    public class ShipingRequestItemList
    {

        public string ItemId { get; set; }
        public string Reason { get; set; }
        public string PickedQty { get; set; }
        public string RemainingQty { get; set; }

        public string BatchNo { get; set; }
        public string UserId { get; set; }
        public string BatchQty { get; set; }
        public string RequisitionDetailsId { get; set; }
        public string Config { get; set; }
        public string BatchAutoIncreeId { get; set; }



    }
    public class Batch
    {
        public string BatchNo { get; set; }
        public string BatchQty { get; set; }
        public string Config { get; set; }
        public string BatchAutoIncreeId { get; set; }
    }

    public class ShipingResponse
    {

        public string Status { get; set; }
        public string Message { get; set; }
        public string TONumber { get; set; }

    }


    public class requestControl
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<RequestControlReq> dataList
        { get; set; }
        public List<stikrList> StickerList
        { get; set; }


    }
    public class stikrList
    {
        public string StickerSeq { get; set; }
    }

    public class RequestControlReq
    {
        public string RequisitionNo { get; set; }
        public string ReqestDate { get; set; }
        public string CreatedBy { get; set; }
        public string Branch { get; set; }
        public string ItemId { get; set; }
        public string ItemNameArabic { get; set; }
        public string ItemName { get; set; }
        public string ApprovedQty { get; set; }
        public string TONum { get; set; }
        public string LocationId { get; set; }
        public string ShippedQty { get; set; }
        public string RequisitionDetailId { get; set; }



    }
    public class ReqUserId
    {
        public string UserId { get; set; }
        public string ReqNo { get; set; }
    }

    public class RequestControlResponse
    {

        public string RequisitionNo { get; set; }
        public string Location { get; set; }
        public string ReqestDate { get; set; }
        public string CreatedByWH { get; set; }
        public string Branch { get; set; }
        public string ItemNameArabic { get; set; }
        public string ItemNameEnglish { get; set; }
        public string ApprovedQty { get; set; }
        public string PickedQty { get; set; }
        public string RemainingQty { get; set; }
    }
}
