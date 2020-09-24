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
using FRD_InventoryWebApi.ServiceReference2;
using FRD_InventoryWebApi.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FRD_InventoryWebApi.Controllers
{
    public class MRNDetailsController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;

        [Route("api/FRD/GetMrn")]
        [HttpPost]
        public GetMRN GetMrn()
        {

            GetMRN res = new GetMRN();
            List<GetMRNs> ListView = new List<GetMRNs>();
            try
            {


                query = "SP_Login";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "GetMrn");

                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    GetMRNs m = new GetMRNs();
                    m.MRNNumber = row["MRNNumber"].ToString();
                    m.MRNDate = row["MRNDate"].ToString();
                    m.ActivityNumber = row["ActivityNumber"].ToString();

                    ListView.Add(m);
                }
                res.MRNList = ListView;
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

        [Route("api/FRD/GetMrnDetails")]
        [HttpPost]
        public GetMrnDeta GetMrnDetails(MRNRequest MR)
        {
            GetMrnDeta res = new GetMrnDeta();
            List<MRNDetails> ListView = new List<MRNDetails>();
            
            try
            {

                query = "InsertMRN";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "APIGetMRNDetails");
                dbcommand.Parameters.AddWithValue("@MRNNumber", MR.MNRNumber);
                dbcommand.Parameters.AddWithValue("@userId", MR.UserId);
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows[0]["BatchNo"].ToString() == "-1")
                {
                    res.Status = "Failure";
                    res.Message = "This MRN opened by another user";
                }
                else { 
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    MRNDetails bnk = new MRNDetails();
                    List<StickerSeq> listStickerSeq = new List<StickerSeq>();
                    bnk.ItemId = row["ItemId"].ToString();
                    bnk.BatchNo = row["BatchNo"].ToString();
                    bnk.ReceivedQuantity = row["BatchQuantity"].ToString();
                    bnk.Config = row["CONFIGID"].ToString();
                    bnk.itemArabicName = row["itemArabicName"].ToString();
                    bnk.itemname = row["itemname"].ToString();
                    bnk.ExpiryDate = row["ExpiryDate"].ToString();
                    bnk.LineNo = row["ItemLineNumber"].ToString();
                    bnk.ConfigId= row["Config"].ToString();


                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {

                        
                        if (bnk.BatchNo == ds.Tables[1].Rows[i]["BatchNo"].ToString() && bnk.ConfigId == ds.Tables[1].Rows[i]["Config"].ToString())
                        {
                            StickerSeq sq = new StickerSeq();
                            string qrsequence = ds.Tables[1].Rows[i]["Stickersequence"].ToString();
                            string qty = ds.Tables[1].Rows[i]["StickerQty"].ToString(); 
                            string Stickersequenceid = ds.Tables[1].Rows[i]["StickerSequenceId"].ToString();
                            sq.StickerSequence = qrsequence;
                            sq.StickerQty = qty;
                                sq.StickerSequenceId = Stickersequenceid;
                                listStickerSeq.Add(sq);
                            bnk.StickerSeq = listStickerSeq;

                        }
                    
                       
                    }

                    bnk.VendorCode = row["VendorId"].ToString();
                    bnk.VendorName = row["Name"].ToString();
                    bnk.PurchaseOrderNo = row["PurchaseOrderId"].ToString();
                    ListView.Add(bnk);
                }
               
               
                res.MRNDetailsList = ListView;
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

        [Route("api/FRD/GetCountData")]
        [HttpPost]
        public DatacountResponse GetCountData(DatacountRequest dr)
        {
            DatacountResponse res = new DatacountResponse();

            List<JounralIdList> Jounrallist = new List<JounralIdList>();
            List<ItemReason> ReasonList = new List<ItemReason>();

            try
            {

                query = "InsertMRN";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "APICountRecord");
                dbcommand.Parameters.AddWithValue("@userId", dr.userId);
              
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
               
              
               
                res.Status = "Success";
                res.Message = "Data retrived successfully";
                res.TotalMrn = Convert.ToString(ds.Tables[0].Rows[0]["TotalMRN"]);
                res.TotalPending = Convert.ToString(ds.Tables[1].Rows[0]["TotalPending"]);
                res.Totalcomplete = Convert.ToString(ds.Tables[2].Rows[0]["TotalComplete"]);
                if (Convert.ToString(ds.Tables[3].Rows[0]["JOURNALID"]) != "0")
                {
                    for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
                    {
                        JounralIdList j = new JounralIdList();
                        j.JounralID = Convert.ToString(ds.Tables[3].Rows[i]["JOURNALID"]);
                        Jounrallist.Add(j);
                    }
                    res.JounralNO = Jounrallist;
                }
                else
                {
                    res.JounralNO = Jounrallist;


                }

                if (Convert.ToString(ds.Tables[4].Rows[0]["Reason"]) != "0")
                {
                    for (int i = 0; i < ds.Tables[4].Rows.Count; i++)
                    {
                        ItemReason r = new ItemReason();
                        r.Reason = Convert.ToString(ds.Tables[4].Rows[i]["Reason"]);
                        ReasonList.Add(r);
                    }
                    res.Reason = ReasonList;
                }
                else
                {
                    res.Reason = ReasonList;


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

        [Route("api/FRD/PackingSlip")]
        [HttpPost]
        public PackingSlipResponse MRNRegisterPackingSlip(PackingSlipRequest MR)
        {
            PackingSlipResponse res = new PackingSlipResponse();
            List<MRNDetails> ListView = new List<MRNDetails>();
            ServiceReference2.CallContext Cct = new ServiceReference2.CallContext();
            ServiceReference2.AMY_FRDServiceClient obj = new ServiceReference2.AMY_FRDServiceClient();
            try
            {
                string conn = ConfigurationManager.ConnectionStrings["Conn"].ToString();
                SqlConnection Conn = new SqlConnection(conn);

                SqlCommand cmd1 = new SqlCommand("Sp_AxWebserviceIntegration", Conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                DataSet sdAX = new DataSet();
                SqlDataAdapter sd = new SqlDataAdapter();

                sd.SelectCommand = cmd1;

                sd.Fill(sdAX);

               

                obj.ClientCredentials.Windows.ClientCredential.Domain = Convert.ToString(sdAX.Tables[0].Rows[0]["Domain"]);
                obj.ClientCredentials.Windows.ClientCredential.UserName = Convert.ToString(sdAX.Tables[0].Rows[0]["Username"]);
                obj.ClientCredentials.Windows.ClientCredential.Password = Convert.ToString(sdAX.Tables[0].Rows[0]["Password"]);

                

                Cct.Company = Convert.ToString(sdAX.Tables[0].Rows[0]["Company"]);
                Cct.Language = Convert.ToString(sdAX.Tables[0].Rows[0]["Language"]);



              

                SqlCommand cmd = new SqlCommand("InsertMRN", Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "APIGetLineNo");
                cmd.Parameters.AddWithValue("@MRNNumber", MR.MRNNumber);
                cmd.Parameters.AddWithValue("@activityNo", MR.ActivityNo);
                DataTable dt = new DataTable();
                SqlDataAdapter sd1 = new SqlDataAdapter();

                sd1.SelectCommand = cmd;

                sd1.Fill(dt);
                //   string jsondata = DataTableToJSON(dt);

                decimal lineNo = 0; decimal Qty = 0; string PurchId = ""; string ItemId = ""; string MrnNo = "";
               
                string BatchId = ""; bool ermsg = false; string servicetype = "0";
                try
                {
                   
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        
                        lineNo = Convert.ToDecimal(dt.Rows[i]["ItemLineNumber"]);
                        Qty = Convert.ToDecimal(dt.Rows[i]["BatchQuantity"]);
                        PurchId = Convert.ToString(dt.Rows[i]["PurchaseOrderId"]);
                        ItemId = Convert.ToString(dt.Rows[i]["ItemId"]);
                        MrnNo = Convert.ToString(dt.Rows[i]["MRNNumber"]);
                        BatchId = Convert.ToString(dt.Rows[i]["BatchNo"]);



                        string a = "";

                        servicetype = "1";
                        a = obj.RegisterPackingSlip(Cct, PurchId, ItemId, lineNo, BatchId, Qty);
                        ermsg = true;
                       
                        if (a == "Registration success")
                        {

                            string b = "";
                            //  try
                            //  {
                            servicetype = "2"; ermsg = false;
                                b = obj.InsertPackingSlipData(Cct, PurchId, Qty, MrnNo, lineNo);
                            ermsg = true;
                          //  }
                            //catch (Exception ex)
                            //{
                            //    dbcommand = new SqlCommand();
                            //    dbcommand.Connection = Conn;
                            //    dbcommand.Connection.Open();
                            //    dbcommand.CommandText = "InsertpackingSliplog";
                            //    dbcommand.CommandType = CommandType.StoredProcedure;
                            //    dbcommand.Parameters.Add("@PurchId", SqlDbType.NVarChar).Value = Convert.ToString(PurchId);
                            //    dbcommand.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(Qty);
                            //    dbcommand.Parameters.Add("@MrnNo", SqlDbType.NVarChar).Value = Convert.ToString(MrnNo);
                            //    dbcommand.Parameters.Add("@lineNo", SqlDbType.BigInt).Value = Convert.ToInt64(lineNo);
                            //    dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);

                            //    dbcommand.ExecuteNonQuery();
                            //    dbcommand.Connection.Close();
                            //    dbcommand.Parameters.Clear();
                            //}
                        }


                    }
                }
                catch (Exception ex) {
                    if (ermsg == false && servicetype == "1")
                    {
                        dbcommand = new SqlCommand();
                        dbcommand.Connection = Conn;
                        dbcommand.Connection.Open();
                        dbcommand.CommandText = "InsertRegisterpackinglog";
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.Add("@PurchId", SqlDbType.NVarChar).Value = Convert.ToString(PurchId);
                        dbcommand.Parameters.Add("@ItemId", SqlDbType.NVarChar).Value = Convert.ToString(ItemId);
                        dbcommand.Parameters.Add("@lineNo", SqlDbType.BigInt).Value = Convert.ToInt64(lineNo);
                        dbcommand.Parameters.Add("@BatchId", SqlDbType.BigInt).Value = Convert.ToInt64(BatchId);
                        dbcommand.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(Qty);
                        dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);

                        dbcommand.ExecuteNonQuery();
                        dbcommand.Connection.Close();
                        dbcommand.Parameters.Clear();
                    }

                    if (ermsg == false && servicetype == "2")
                    {
                        dbcommand = new SqlCommand();
                        dbcommand.Connection = Conn;
                        dbcommand.Connection.Open();
                        dbcommand.CommandText = "InsertpackingSliplog";
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.Add("@PurchId", SqlDbType.NVarChar).Value = Convert.ToString(PurchId);
                        dbcommand.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(Qty);
                        dbcommand.Parameters.Add("@MrnNo", SqlDbType.NVarChar).Value = Convert.ToString(MrnNo);
                        dbcommand.Parameters.Add("@lineNo", SqlDbType.BigInt).Value = Convert.ToInt64(lineNo);
                        dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);

                        dbcommand.ExecuteNonQuery();
                        dbcommand.Connection.Close();
                        dbcommand.Parameters.Clear();
                    }


                }

                string PurchId1 = Convert.ToString(dt.Rows[0]["PurchaseOrderId"]);

                string PackingSlipId = ""; 
                try
                {
                    servicetype = "3"; ermsg = false;
                    PackingSlipId = obj.PackingSlip(Cct, PurchId1);
                    ermsg = true;
                }
                catch (Exception ex) {
                    if (servicetype == "3" && ermsg == false)
                    {
                        dbcommand = new SqlCommand();
                        dbcommand.Connection = Conn;
                        dbcommand.Connection.Open();
                        dbcommand.CommandText = "InsertpackingSliplog1";
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.Add("@PurchId", SqlDbType.NVarChar).Value = Convert.ToString(PurchId1);
                        dbcommand.Parameters.Add("@ErrorMsg", SqlDbType.NVarChar).Value = Convert.ToString(ex.Message);

                        dbcommand.ExecuteNonQuery();
                        dbcommand.Connection.Close();
                        dbcommand.Parameters.Clear();
                    }
                }


                obj.DeleteData(Cct);
                if (PackingSlipId == "")
                {
                    res.Status = "Failure";
                    res.Message = "Packingslip not generated";
                }
                else
                {
                    SqlCommand cm = new SqlCommand("InsertMRN", Conn);
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@QueryType", "UpdateMRNFlag");
                    cm.Parameters.AddWithValue("@MRNNumber", MR.MRNNumber);
                    // cm.Parameters.AddWithValue("@dtQty",jsondata);
                    DataTable tab = new DataTable();
                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = cm;
                    sda.Fill(tab);


                    res.Status = "Success";
                    res.Message = "Data posting successfully";
                }


            }
            catch (Exception ex)
            {
                obj.DeleteData(Cct);
                res.Status = "Failure";
                  res.Message = ex.Message;
                    
               
            }

            return res;
        }
        public string DataTableToJSON(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

        public DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }

        [Route("api/FRD/UpdateStickerQty")]
        [HttpPost]
        public PackingSlipResponse UpdateStickerQty(UpdateConsumeQtyReq Uc)
        {

            PackingSlipResponse res = new PackingSlipResponse();
            
            try
            {
                List<UpdateQtyReqList> list = new List<UpdateQtyReqList>();

                foreach (var task in Uc.DataList)
                {
                    UpdateQtyReqList b = new UpdateQtyReqList();
                    b.StickerSeqNo = task.StickerSeqNo;
                    b.ConsumeQty = task.ConsumeQty;
                    b.UserId = Uc.UserId;
                    list.Add(b);
                }
              
                string sJSONResponse = JsonConvert.SerializeObject(list);

                query = "Sp_requestControl";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "UpdateConsumeQty");
                dbcommand.Parameters.AddWithValue("@Jsondata", sJSONResponse);
                dbcommand.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (Convert.ToString(dt.Rows[0]["Response"]) == "1")
                {
                    res.Status = "success";
                    res.Message = "Quantity updated  successfully";
                }
                else {
                    res.Status = "Failure";
                    res.Message = "Quantity not updated ";
                }
                

            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = ex.Message;
               


            }
            return res;
        }


    }
    public class DatacountRequest
    {
        public string userId { get; set; }
      

    }
    public class DatacountResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string TotalMrn { get; set; }
        public string TotalPending { get; set; }
        public string Totalcomplete { get; set; }
        public List<JounralIdList> JounralNO
        { get; set; }
        public List<ItemReason> Reason
        { get; set; }
    }

    public class PackingSlipResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
    public class PackingSlipReq
    {
        public List<PackingSlipRequest> MRNDetailsList
        { get; set; }
    }
    public class PackingSlipRequest
        {
        public string PurchId { get; set; }
        public string ItemId { get; set; }
        
        public string LineNo { get; set; }
        public string BatchId { get; set; }
        public string ActivityNo { get; set; }
        public string MRNNumber { get; set; }

    }
    public class GetMrnDeta
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<MRNDetails> MRNDetailsList
        { get; set; }
        
    }
    public class MRNDetails
    {
        
        public string ItemId { get; set; }
        public string BatchNo { get; set; }
        public string Config { get; set; }
        public string ExpiryDate { get; set; }
        public string ReceivedQuantity { get; set; }
        public string itemArabicName { get; set; }
        public string itemname { get; set; }
        public string LineNo { get; set; }
        public List<StickerSeq> StickerSeq
        { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string ConfigId { get; set; }

    }
    public class StickerSeq
    {
        public string StickerSequence { get; set; }
        public string StickerQty { get; set; }
        public string StickerSequenceId { get; set; }

    }

    public class GetMRN
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<GetMRNs> MRNList
        { get; set; }
    }
    public class GetMRNs
    {

        public string MRNNumber { get; set; }
        public string MRNDate { get; set; }

        public string ActivityNumber { get; set; }
       

           

    }
    public class MRNRequest
    {
        public string MNRNumber { get; set; }
        public string UserId { get; set; }
    }

   
    public class JounralIdList
    {
        public string JounralID { get; set; }
    }
    public class ItemReason
    {
        public string Reason { get; set; }
    }

    public class UpdateConsumeQtyReq
    {
        public string UserId { get; set; }
        public List<UpdateQtyReqList> DataList
        { get; set; }

    }
    public class UpdateQtyReqList
    {
        public string StickerSeqNo { get; set; }
        public string ConsumeQty { get; set; }
        public string UserId { get; set; }
    }

}
