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
using System.Collections;

namespace FRD_InventoryWebApi.Controllers
{
    public class ItemReceivingController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;
        ServiceReference2.CallContext Cct = new ServiceReference2.CallContext();
        ServiceReference2.AMY_FRDServiceClient obj = new ServiceReference2.AMY_FRDServiceClient();

        [Route("api/FRD/ItemReceiving")]
        [HttpPost]
        public ReceivingControlResponse ItemReceiving(ReceivingReq uId)
        {

            ReceivingControlResponse res = new ReceivingControlResponse();
            List<ReceivingControlResponseList> ListView = new List<ReceivingControlResponseList>();
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
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    ReceivingControlResponseList m = new ReceivingControlResponseList();
                    m.RequisitionNo = row["RequestNumber"].ToString();
                    m.ReqestDate = row["RequestedDate"].ToString();
                    m.ItemId = row["AXitemId"].ToString();
                    m.ItemName = row["itemname"].ToString();
                    m.ItemNameArabic = row["itemArabicName"].ToString();
                    m.CreatedBy = row["UserName"].ToString();
                    m.Branch = row["WareHouseName"].ToString();
                    m.ApprovedQty = row["ApprovedQty"].ToString();
                    m.PickedQty = row["PickedQty"].ToString();
                    m.TONum= row["TransferOrder"].ToString();




                    ListView.Add(m);
                }
                res.dataList = ListView;
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

        [Route("api/FRD/Receiving")]
        [HttpPost]
        public ReceivingResponse Receiving(ReceivingRequest sr)
        {

            ReceivingResponse res = new ReceivingResponse();
         
            try
            {

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

                

                List<ReceivingRequestItem> list = new List<ReceivingRequestItem>();
                List<ReturnTOClass> ReturnList = new List<ReturnTOClass>();


                ArrayList arr = new ArrayList();
                

                for (int i= 0;i<sr.ItemList.Count;i++)
                {
                    if (i == 0)
                    {
                        arr.Add(sr.ItemList[i].TONum);
                    }

                   bool checkexists=  arr.Contains(sr.ItemList[i].TONum);
                    if (checkexists == false)
                    {
                        arr.Add(sr.ItemList[i].TONum);
                    }

                  //  for (int j = 0; j < arr.Count; j++)
                 //   {
                        
                        //if (sr.ItemList[i].TONum.ToString().Trim() .ToLower() .Contains(arr[j].ToString().Trim() .ToLower()))
                        //{
                           
                        //}
                        //else
                        //{

                        //    arr.Add(sr.ItemList[i].TONum);

                        //}
                  //  }
                    
                }
                for (int i = 0; i < arr.Count; i++)
                {
                    string responseobj = obj.RecieveOrder(Cct, arr[i].ToString());
                    if (responseobj == "success")
                    {
                        foreach (var taskdata in sr.ItemList)
                        {
                            if (taskdata.TONum == arr[i].ToString())
                            {
                                ReceivingRequestItem b = new ReceivingRequestItem();
                                b.ItemId = taskdata.ItemId;
                                b.Reason = taskdata.Reason;
                                b.PickedQty = taskdata.PickedQty;
                                b.TONum = taskdata.TONum;
                                b.UserId = sr.UserId;
                                list.Add(b);
                            }
                        }
                        string sJSONResponse = JsonConvert.SerializeObject(list);
                        // string ReturnTOJason = JsonConvert.SerializeObject(ReturnList);

                        query = "Sp_requestControl";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.Connection.Open();
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "UpdateReceiveQty");


                        dbcommand.Parameters.AddWithValue("@ReqNo", sr.RequisitionNo);
                        dbcommand.Parameters.AddWithValue("@Jsondata", sJSONResponse);
                        // dbcommand.Parameters.AddWithValue("@ReturnTOJason", ReturnTOJason);

                        dbcommand.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dbcommand.Connection.Close();
                        res.Status = "success";
                        res.Message = "Item Receiving Successfully";
                    }
                    else
                    {
                        res.Status = "success";
                        res.Message = responseobj;
                    }


                }


               



                //foreach (var task in sr.ItemList)
                //   {
                //     ReceivingRequestItem b = new ReceivingRequestItem();
                //        b.ItemId = task.ItemId;
                //        b.Reason = task.Reason;
                //        b.PickedQty = task.PickedQty;
                //        b.TONum = task.TONum;
                //        b.UserId = sr.UserId;
                //         list.Add(b);

                //}

                //foreach (var header in sr.ItemList)
                //{
                //    var OriginalTonum = header.BatchList.Where(c => c.ReturnQty != "0").Select(c => new { header.TONum }).Distinct().ToList();
                //     foreach (var Item in OriginalTonum)
                //      {
                    
                //     string TO = obj.InsertReturnTransferHeader(Cct,sr.RequisitionNo, Item.TONum, sr.UserId);

                //        var NewItemList = sr.ItemList.Where(c => c.TONum == Item.TONum).Where(c => c.ItemId == header.ItemId).SelectMany(c => c.BatchList)
                //        .Where(BatchList => BatchList.ReturnQty != "0").Select(c => new { c.ReturnQty,c.BatchNo,c.Config, header.ItemId, header.Reason }).ToList();


                //    foreach (var temp in NewItemList)
                //    {
                //        decimal ReturnQuantity = Convert.ToDecimal(temp.ReturnQty);
                //         obj.InsertReturnTransferLine(Cct,TO,temp.ItemId,temp.Config,temp.BatchNo, ReturnQuantity);
                //        ReturnTOClass ret = new ReturnTOClass();
                //        ret.ItemId = temp.ItemId;
                //        ret.OldTONum = Item.TONum;
                //        ret.NewTONum = TO;
                //        ret.ReturnQty = temp.ReturnQty;
                //        ret.Reason = temp.Reason;
                //        ret.UserId = sr.UserId;
                //        ret.BatchNo = temp.BatchNo;
                //        ReturnList.Add(ret);
                //    }
                //   obj.ShipOrder(Cct, TO);
                //      }
                //}
                   // var OriginalTonum = sr.ItemList.Where(c => c.ReturnQty != "0").Select(c =>new { c.TONum }).Distinct().ToList();
                   //foreach (var Item in OriginalTonum)
                   //   {
                    
                   // string TO = obj.InsertReturnTransferHeader(Cct, Item.TONum, sr.UserId);

                   // //var result = sr.ItemList.Where(c => c.TONum == Item.TONum).Select(c => c.ItemId).SelectMany(c => c.BatchList)
                   // //.Where(BatchList => BatchList.ReturnQty != "0").ToList();
                  

                   // var NewItemList = sr.ItemList.Where(c => c.ReturnQty != "0").Where(c => c.TONum == Item.TONum).Select(c => new { c.ReturnQty, c.ItemId,c.Reason }).ToList();
                   // foreach (var temp in NewItemList)
                   // {
                   //     decimal ReturnQuantity = Convert.ToDecimal(temp.ReturnQty);
                   //      obj.InsertReturnTransferLine(Cct, TO, temp.ItemId, ReturnQuantity);
                   //     ReturnTOClass ret = new ReturnTOClass();
                   //     ret.ItemId = temp.ItemId;
                   //     ret.OldTONum = Item.TONum;
                   //     ret.NewTONum = TO;
                   //     ret.ReturnQty = temp.ReturnQty;
                   //     ret.Reason = temp.Reason;
                   //     ret.UserId = sr.UserId;
                   //     ReturnList.Add(ret);
                   // }
                   // obj.ShipOrder(Cct, TO);
                   //   }

               // string sJSONResponse = JsonConvert.SerializeObject(list);
               //// string ReturnTOJason = JsonConvert.SerializeObject(ReturnList);

               // query = "Sp_requestControl";
               // dbcommand = new SqlCommand(query, conn);
               // dbcommand.Connection.Open();
               // dbcommand.CommandType = CommandType.StoredProcedure;
               // dbcommand.Parameters.AddWithValue("@QueryType", "UpdateReceiveQty");

                
               // dbcommand.Parameters.AddWithValue("@ReqNo", sr.RequisitionNo);
               // dbcommand.Parameters.AddWithValue("@Jsondata", sJSONResponse);
               //// dbcommand.Parameters.AddWithValue("@ReturnTOJason", ReturnTOJason);

               // dbcommand.CommandTimeout = 0;

               // SqlDataAdapter da = new SqlDataAdapter(dbcommand);
               // DataTable dt = new DataTable();
               // da.Fill(dt);

               // res.Status = "success";
               // res.Message = "Item Receiving Successfully";
               


            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = ex.Message;
                
               

            }
            return res;
        }
    }
    public class ReceivingReq
    {
        public string UserId { get; set; }
        public string ReqNo { get; set; }
    }
    public class ReceivingControlResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ReceivingControlResponseList> dataList
        { get; set; }

    }
    public class ReceivingControlResponseList
    {
        public string RequisitionNo { get; set; }
        public string ReqestDate { get; set; }
        public string CreatedBy { get; set; }
        public string Branch { get; set; }
        public string ItemId { get; set; }
        public string ItemNameArabic { get; set; }
        public string ItemName { get; set; }
        public string ApprovedQty { get; set; }
        public string PickedQty { get; set; }
        public string TONum { get; set; }


    }
    public class ReceivingRequest
    {
        public string RequisitionNo { get; set; }
        public string UserId { get; set; }


        public List<ReceivingRequestItem> ItemList
        { get; set; }


    }
    public class ReceivingRequestItem
    {

        public string ItemId { get; set; }
        public string Reason { get; set; }
        public string PickedQty { get; set; }
        
        public string TONum { get; set; }
        public string UserId { get; set; }
        public string ReturnQty { get; set; }
        public string Location { get; set; }
        public string Config { get; set; }
        public List<BatchReturn> BatchList
        { get; set; }



    }

    public class BatchReturn
    {
        public string BatchNo { get; set; }
        public string Config { get; set; }
        public string ReturnQty { get; set; }
    }
    public class ReturnTOClass
    {

        public string ItemId { get; set; }
        public string Reason { get; set; }
        public string ReturnQty { get; set; }

        public string NewTONum { get; set; }
        public string OldTONum { get; set; }

        public string UserId { get; set; }
        public string BatchNo { get; set; }





    }
    public class ReceivingResponse
    {

        public string Status { get; set; }
        public string Message { get; set; }
        

    }
}
