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
    public class InventoryCountingController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;
        ServiceReference2.CallContext Cct = new ServiceReference2.CallContext();
        ServiceReference2.AMY_FRDServiceClient obj = new ServiceReference2.AMY_FRDServiceClient();

        [Route("api/FRD/InventoryCountingHeader")]
        [HttpPost]
        public InventoryCountingRes InventoryCountingHeader(InventoryCountingReq uId)
        {

            InventoryCountingRes res = new InventoryCountingRes();
           
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


            
              string JournalNo=  obj.InventCountingHeader(Cct, uId.UserId);

                query = "InventoryCounting";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "InsertJournalNo");

                dbcommand.Parameters.AddWithValue("@JournalId", JournalNo);
                dbcommand.Parameters.AddWithValue("@userId", uId.UserId);
                

                dbcommand.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                res.Status = "Success";
                res.Message = "InventJounralId Genrated Successfully";
                res.InventJounralId = JournalNo;


            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = ex.Message;
                res.InventJounralId = "";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }


        [Route("api/FRD/DeleteJounralFromDevice")]
        [HttpPost]
        public InventoryCountingRes DeleteJounralFromDevice(DeleteJounralFromDeviceReq Jno)
        {

            InventoryCountingRes res = new InventoryCountingRes();

            try
            {


                List<DeleteJounralFromDevice> list = new List<DeleteJounralFromDevice>();
                foreach (var task in Jno.ItemList)
                {
                    DeleteJounralFromDevice b = new DeleteJounralFromDevice();

                    b.Journal = task.Journal;
                     list.Add(b);
                 }

                string sJSONResponse = JsonConvert.SerializeObject(list);

                query = "InventoryCounting";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "DeleteJounralFromDevice");

                dbcommand.Parameters.AddWithValue("@Jsondata", sJSONResponse);



                dbcommand.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                res.Status = "Success";
                res.Message = "data updated Successfully";
               


            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = ex.Message;
                res.InventJounralId = "";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }


        [Route("api/FRD/InventoryCountingLine")]
        [HttpPost]
        public InventoryCountingRes InventoryCountingLine(CountingRequest CR)
        {

            InventoryCountingRes res = new InventoryCountingRes();

            try
            {
                List<CountingRequestItem> list = new List<CountingRequestItem> ();
                foreach (var task in CR.ItemList)
                {
                    CountingRequestItem b = new CountingRequestItem();

                    b.ItemId = task.ItemId;
                    b.Config = task.Config;
                    b.StickerQty = task.StickerQty;
                    b.Location = CR.Location;
                    b.BatchId = task.BatchId;
                    b.JounralNo = CR.JounralNo;
                    b.UserId = CR.UserId;



                    list.Add(b);

                   

                }
                string sJSONResponse = JsonConvert.SerializeObject(list);

                query = "InventoryCounting";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "InsertCountingData");

                dbcommand.Parameters.AddWithValue("@Jsondata", sJSONResponse);
                dbcommand.Parameters.AddWithValue("@JournalId", CR.JounralNo);



                dbcommand.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (Convert.ToString(dt.Rows[0]["Response"]) == "-1")
                {
                    res.Status = "Failure";
                    res.Message = "Data has already been sent";
                    res.InventJounralId = "";
                }
                else
                {

                    res.Status = "Success";
                    res.Message = "Data Saved Successfully";
                    res.InventJounralId = "";
                }

            }
            catch (Exception ex)
            {
                res.Status = "Failure";
                res.Message = ex.Message;
                res.InventJounralId = "";

            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }

    }

    public class InventoryCountingReq
    {
        public string UserId { get; set; }
        public string Description { get; set; }

    }
    public class DeleteJounralFromDeviceReq
    {
        public List<DeleteJounralFromDevice> ItemList
        { get; set; }


    }
    public class DeleteJounralFromDevice
    {
        public string Journal { get; set; }


    }

    public class InventoryCountingLine
    {
        public string InventJounralId { get; set; }
        public string ItemId { get; set; }
        public string ScanQty { get; set; }
        public string Configration { get; set; }
        public string Location { get; set; }
        public string BatchNo { get; set; }
    }

    public class InventoryCountingRes
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string InventJounralId { get; set; }


    }

    public class CountingRequest
    {
        public string JounralNo { get; set; }
        public string UserId { get; set; }
        public string Location { get; set; }
        public List<CountingRequestItem> ItemList
        { get; set; }


    }
    public class CountingRequestItem
    {
       
        public string ItemId { get; set; }
        public string StickerQty { get; set; }

        public string Location { get; set; }
        public string Config { get; set; }
        public string BatchId { get; set; }
        public string JounralNo { get; set; }
        public string UserId { get; set; }

    }
    public class CountingRequestItemList
    {

        public string ItemId { get; set; }
        public string PickedQty { get; set; }

        public string Location { get; set; }
        public string Config { get; set; }
        public string BatchId { get; set; }

    }

}
