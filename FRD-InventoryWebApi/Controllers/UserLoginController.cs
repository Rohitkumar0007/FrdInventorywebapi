
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

namespace FRD_InventoryWebApi.Controllers
{
    public class UserLoginController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString);
        string query = ""; SqlCommand dbcommand;
        UserLogin res = new UserLogin();
        List<UserLoginResponse> ListView = new List<UserLoginResponse>();
        List<UserWareHouse> hws = new List<UserWareHouse>();
        List<AccessRight> rights = new List<AccessRight>();
        UserLoginResponse Ulr = new UserLoginResponse();
       
        [Route("api/FRD/Login")]
        [HttpPost]
        public UserLogin Login(UserLoginRequest ul)
        {

           

            if (ul.UserPin.Trim() != "" && Validation.ValidateUserPin(ul.UserPin.Trim()) != true)
            {
               
                res.Message = "UserPin Must be 6 digit";
                res.Status = "Failure";
                res.UserLoginResponse = ListView;
                
                Ulr.UserId = "";
                Ulr.UserPin = "";
                ListView.Add(Ulr);
                return res;



            }

            else
            {
                bool Flag = true;

                try
                {
                    query = "SP_Login";
                    dbcommand = new SqlCommand(query, conn);
                    dbcommand.Connection.Open();
                    dbcommand.CommandType = CommandType.StoredProcedure;
                    dbcommand.Parameters.AddWithValue("@QueryType", "APIUserAccess");
                    dbcommand.Parameters.AddWithValue("@UserPin", ul.UserPin);
                    SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ul.UserPin.Trim() != "" && ul.Password != "")
                    {

                        if (Convert.ToString(ds.Tables[0].Rows[0]["UserPin"]) == "0")
                        {

                            res.Message = "User has been disabled, please contact administrator!";
                            res.Status = "Failure";
                            res.UserLoginResponse = ListView;
                            Ulr.UserId = "";
                            Ulr.UserPin = "";
                            ListView.Add(Ulr);
                            return res;
                           
                        }
                        else if (Convert.ToString(ds.Tables[0].Rows[0]["UserPin"]) == "-1")
                        {

                            res.Message = "Invalid User Pin!";
                            res.Status = "Failure";
                            res.UserLoginResponse = ListView;
                            Ulr.UserId = "";
                            Ulr.UserPin = "";
                            ListView.Add(Ulr);
                            return res;
                           
                        }
                        else
                        {

                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                bool IsLoginfirsttime = Convert.ToBoolean(ds.Tables[0].Rows[0]["LoginFlag"]);
                                string UserType = Convert.ToString(ds.Tables[0].Rows[0]["UserType"]);

                                if (UserType == "1")
                                {
                                    #region For Admin
                                    string strDbPassword = DBsecurity.Decrypt(Convert.ToString(ds.Tables[0].Rows[0]["Password"]), Convert.ToString(ds.Tables[0].Rows[0]["PasswordKey"]));
                                    if (strDbPassword.Trim() != ul.Password.Trim())
                                    {

                                        res.Message = "Wrong Password.";
                                        res.Status = "Failure";
                                        res.UserLoginResponse = ListView;
                                        Ulr.UserPin = "";
                                        Ulr.UserId = "";
                                        ListView.Add(Ulr);
                                        return res;
                                        
                                    }
                                    //
                                    else
                                    {
                                        res.Message = "Valid User.";
                                        res.Status = "success";
                                        res.UserLoginResponse = ListView;
                                        res.WareHOuse =hws;
                                        res.AccessRight = rights;

                                        if (ds.Tables[1].Rows.Count > 0)
                                        {
                                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                            {
                                                UserWareHouse uw = new UserWareHouse();
                                                uw.WareHouse = Convert.ToString(ds.Tables[1].Rows[i]["Locationid"]);
                                                uw.WareHouseId = Convert.ToString(ds.Tables[1].Rows[i]["WareHouseId"]);
                                                hws.Add(uw);
                                            }

                                              }

                                        if (ds.Tables[2].Rows.Count > 0)
                                        {
                                            if (Convert.ToString(ds.Tables[2].Rows[0]["Mrn"]) != "-1")
                                            {
                                                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                                                {
                                                    AccessRight ar = new AccessRight();
                                                    ar.MRN = Convert.ToString(ds.Tables[2].Rows[i]["Mrn"]);
                                                    ar.shipperList = Convert.ToString(ds.Tables[2].Rows[i]["shipperList"]);
                                                    ar.BranchinventoryReceiving = Convert.ToString(ds.Tables[2].Rows[i]["BranchinventoryReceiving"]);
                                                    ar.inventoryCounting = Convert.ToString(ds.Tables[2].Rows[i]["inventoryCounting"]);
                                                    ar.QRDetails = Convert.ToString(ds.Tables[2].Rows[i]["QRDetails"]);
                                                    ar.QuantityUpdateDetails = Convert.ToString(ds.Tables[2].Rows[i]["QuantityUpdateDetails"]);
                                                    rights.Add(ar);
                                                }
                                            }
                                            else
                                            {
                                                AccessRight ar = new AccessRight();
                                                ar.MRN = false.ToString();
                                                ar.shipperList = false.ToString();
                                                ar.BranchinventoryReceiving = false.ToString();
                                                ar.inventoryCounting = false.ToString();
                                                ar.QRDetails = false.ToString();
                                                ar.QuantityUpdateDetails = false.ToString();
                                                rights.Add(ar);
                                            }


                                        }
                                       

                                        Ulr.UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserID"]);
                                        Ulr.UserPin = "";
                                        Ulr.UserRole= Convert.ToString(ds.Tables[0].Rows[0]["UserRole"]);
                                        ListView.Add(Ulr);
                                        
                                        return res;
                                       



                                    }
                                    #endregion
                                }
                                else
                                {
                                    if (IsLoginfirsttime == true)
                                    {
                                        #region For First Time User Login
                                        string strDbPassword = DBsecurity.Decrypt(Convert.ToString(ds.Tables[0].Rows[0]["Password"]), Convert.ToString(ds.Tables[0].Rows[0]["PasswordKey"]));
                                        if (strDbPassword.Trim() != ul.Password.Trim())
                                        {


                                            res.Message = "Wrong Password.";
                                            res.Status = "Failure";
                                            res.UserLoginResponse = ListView;
                                            Ulr.UserId ="";
                                            Ulr.UserPin = "";
                                            ListView.Add(Ulr);
                                            return res;
                                        }
                                        else
                                        {
                                            res.Message = "Valid User.";
                                            res.Status = "success";
                                            res.UserLoginResponse = ListView;
                                            res.WareHOuse = hws;
                                            res.AccessRight = rights;

                                            if (ds.Tables[1].Rows.Count > 0)
                                            {
                                                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                                {
                                                    UserWareHouse uw = new UserWareHouse();
                                                    uw.WareHouse = Convert.ToString(ds.Tables[1].Rows[i]["Locationid"]);
                                                    uw.WareHouseId = Convert.ToString(ds.Tables[1].Rows[i]["WareHouseId"]);
                                                    hws.Add(uw);
                                                }

                                            }

                                            if (ds.Tables[2].Rows.Count > 0)
                                            {
                                                if (Convert.ToString(ds.Tables[2].Rows[0]["Mrn"]) != "-1")
                                                {
                                                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                                                    {
                                                        AccessRight ar = new AccessRight();
                                                        ar.MRN = Convert.ToString(ds.Tables[2].Rows[i]["Mrn"]);
                                                        ar.shipperList = Convert.ToString(ds.Tables[2].Rows[i]["shipperList"]);
                                                        ar.BranchinventoryReceiving = Convert.ToString(ds.Tables[2].Rows[i]["BranchinventoryReceiving"]);
                                                        ar.inventoryCounting = Convert.ToString(ds.Tables[2].Rows[i]["inventoryCounting"]);
                                                        ar.QRDetails = Convert.ToString(ds.Tables[2].Rows[i]["QRDetails"]);
                                                        ar.QuantityUpdateDetails = Convert.ToString(ds.Tables[2].Rows[i]["QuantityUpdateDetails"]);
                                                        rights.Add(ar);
                                                    }
                                                }
                                                else
                                                {
                                                    AccessRight ar = new AccessRight();
                                                    ar.MRN = false.ToString();
                                                    ar.shipperList = false.ToString();
                                                    ar.BranchinventoryReceiving = false.ToString();
                                                    ar.inventoryCounting = false.ToString();
                                                    ar.QRDetails = false.ToString();
                                                    ar.QuantityUpdateDetails = false.ToString();
                                                    rights.Add(ar);
                                                }

                                            }
                                            
                                            Ulr.UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserID"]);
                                            Ulr.UserPin = "";
                                            Ulr.UserRole = Convert.ToString(ds.Tables[0].Rows[0]["UserRole"]);
                                            ListView.Add(Ulr);
                                            return res;
                                           

                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region For Current user
                                        if (Convert.ToString(ConfigurationManager.AppSettings["DefaultPassword"]) == ul.Password.Trim())
                                        {
                                            res.Message = "Valid User.";
                                            res.Status = "success";
                                            res.UserLoginResponse = ListView;
                                            res.WareHOuse = hws;
                                            res.AccessRight = rights;

                                            if (ds.Tables[1].Rows.Count > 0)
                                            {
                                                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                                                {
                                                    UserWareHouse uw = new UserWareHouse();
                                                    uw.WareHouse = Convert.ToString(ds.Tables[1].Rows[i]["Locationid"]);
                                                    uw.WareHouseId = Convert.ToString(ds.Tables[1].Rows[i]["WareHouseId"]);
                                                    hws.Add(uw);
                                                }

                                            }

                                            if (ds.Tables[2].Rows.Count > 0)
                                            {
                                                if (Convert.ToString(ds.Tables[2].Rows[0]["Mrn"]) != "-1")
                                                {
                                                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                                                    {
                                                        AccessRight ar = new AccessRight();
                                                        ar.MRN = Convert.ToString(ds.Tables[2].Rows[i]["Mrn"]);
                                                        ar.shipperList = Convert.ToString(ds.Tables[2].Rows[i]["shipperList"]);
                                                        ar.BranchinventoryReceiving = Convert.ToString(ds.Tables[2].Rows[i]["BranchinventoryReceiving"]);
                                                        ar.inventoryCounting = Convert.ToString(ds.Tables[2].Rows[i]["inventoryCounting"]);
                                                        ar.QRDetails = Convert.ToString(ds.Tables[2].Rows[i]["QRDetails"]);
                                                        ar.QuantityUpdateDetails = Convert.ToString(ds.Tables[2].Rows[i]["QuantityUpdateDetails"]);
                                                        rights.Add(ar);
                                                    }
                                                }
                                                else
                                                {
                                                    AccessRight ar = new AccessRight();
                                                    ar.MRN = false.ToString();
                                                    ar.shipperList = false.ToString();
                                                    ar.BranchinventoryReceiving = false.ToString();
                                                    ar.inventoryCounting = false.ToString();
                                                    ar.QRDetails = false.ToString();
                                                    ar.QuantityUpdateDetails = false.ToString();
                                                    rights.Add(ar);
                                                }

                                            }
                                           
                                         //   Ulr.UserId = "";
                                            Ulr.UserId= Convert.ToString(ds.Tables[0].Rows[0]["UserID"]);
                                            Ulr.UserPin = ul.UserPin;
                                            Ulr.UserRole = Convert.ToString(ds.Tables[0].Rows[0]["UserRole"]);
                                            ListView.Add(Ulr);
                                            return res;

                                           
                                        }
                                        else
                                        {

                                            res.Message = "Wrong Password.";
                                            res.Status = "Failure";
                                            res.UserLoginResponse = ListView;
                                            Ulr.UserId = "";
                                            Ulr.UserPin = "";
                                            ListView.Add(Ulr);
                                            return res;

                                           
                                        }
                                        #endregion
                                    }
                                }


                            }
                            else
                            {
                                res.Message = "Invalid User.";
                                res.Status = "Failure";
                                res.UserLoginResponse = ListView;
                                Ulr.UserId = "";
                                Ulr.UserPin = "";
                                ListView.Add(Ulr);
                                return res;

                               
                            }
                        }
                    }
                    else
                    {


                        res.Message = "Wrong User Pin or Password.";
                        res.Status = "Failure";
                        res.UserLoginResponse = ListView;
                        Ulr.UserId = "";
                        Ulr.UserPin = "";
                        ListView.Add(Ulr);
                        return res;
                        
                    }



                }
                catch (Exception ex)
                {
                    
                }
                finally
                    {
                    dbcommand.Connection.Close();
                }


                return res;



            }
        }
        [Route("api/FRD/ResetPassword")]
        [HttpPost]
        public UserLogin ResetPassword(ResetPassword rp)
        {
            try
            {
                string EmailId = rp.EmailId;
                if (SendMail(EmailId) == 1)
                {

                    int response = UpdatePassword(EmailId);
                    if (response > 0)
                    {
                        res.Message = "Password has been Reset";
                        res.Status = "success";
                        res.UserLoginResponse = ListView;
                        Ulr.UserId = "";
                        Ulr.UserPin = "";
                        ListView.Add(Ulr);
                        return res;
                       


                        

                    }

                }
                else 
                {
                    res.Message = "Invalid EmailId";
                    res.Status = "Failure";
                    res.UserLoginResponse = ListView;
                    Ulr.UserId = "";
                    Ulr.UserPin = "";
                    ListView.Add(Ulr);
                    return res;
                   
                }
               
                return res;

            }
            catch (Exception ex) {
                
                return res;
            }



        }

       

        private int UpdatePassword(string emailId)
        {


            try
            {
              
                query = "SP_Login";
                dbcommand = new SqlCommand(query, conn);
                dbcommand.Connection.Open();
                dbcommand.CommandType = CommandType.StoredProcedure;
                dbcommand.Parameters.AddWithValue("@QueryType", "UpdateDefaultPassword");
                dbcommand.Parameters.AddWithValue("@Emailid", emailId);
                SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                return dt.Rows.Count;
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return 0;
        }


        private int SendMail(string EmailId)
        {
            
            
            query = "SP_Login";
            dbcommand = new SqlCommand(query, conn);
            dbcommand.Connection.Open();
            dbcommand.CommandType = CommandType.StoredProcedure;
            dbcommand.Parameters.AddWithValue("@QueryType", "ForgotPassword");
            dbcommand.Parameters.AddWithValue("@Emailid", EmailId);
            SqlDataAdapter da = new SqlDataAdapter(dbcommand);
            DataSet ds = new DataSet();
            da.Fill(ds);

            string verifyedEmail = Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]);

           
            if (verifyedEmail != "" && verifyedEmail != "0")
            {
                try
                {
                    string AdminEmail = Convert.ToString(ds.Tables[1].Rows[0]["EmailId"]);
                    StringBuilder sb = new StringBuilder();

                    string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                    string UserId = ConfigurationManager.AppSettings["UserId"].ToString();
                    string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                    string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();
                    string SMTPEnableSsl = ConfigurationManager.AppSettings["SMTPEnableSsl"].ToString();

                    sb.Append("Dear  " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br> <br>");
                    sb.Append(" Reset Default Password is: " + ConfigurationManager.AppSettings["DefaultPassword"] + "<br> <br>");

                    sb.Append("<div><p style='font-size:16px; line-height:22px; color:#ed7d31; font-weight:bold; margin-bottom: 2px;'>Thanks & Regards</p> <div style='background-color:#e7e6e6; padding:6px 0px 15px 6px; border-right: 5px solid #dc9004; width:330px; margin-bottom: 15px;'><p style='font-size:18px; line-height:22px; color:#787878; font-weight:normal; margin-bottom: 5px;'>Support team</p><div><div style='display:inline-block; '><img src='../assets/img/globe-icon.png' style='border:none'/><p style='font-size:12px; color:#787878; text-decoration:underline; padding-left:5px; padding-right:7px; border-right: 1px solid #dc9004'>www.amysoftech.in</p> </div> <div style='display:inline-block; padding-left:4px'><img src='../assets/img/email-icon.png' style='border:none'/><p style='font-size:12px; color:#787878; text-decoration:underline; padding-left:5px'>support@amysoftech.in</p></div></div> </div> <p style=' margin-bottom: 0px; font-weight: bold; color: black; font-size: 16px; overflow: hidden; height: 15px;'>   ************************************************************************</p><p style=' font-size:14px; line-height:18px; color:#ed7d31; font-weight:normal; margin-bottom:0px; padding-bottom:5px'><strong>Note:</strong> This is a system generated email, do not reply on this email.</p><p style=' margin-bottom: 0px; font-weight: bold; color: black; font-size: 16px; overflow: hidden; height: 15px;'> ************************************************************************</p></div>");


                    SmtpClient smtpClient = new SmtpClient();

                    MailMessage mailmsg = new MailMessage();
                    MailAddress mailaddress = new MailAddress(UserId);

                    mailmsg.To.Add(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]));
                    // mailmsg.To.Add("satyendaryadav093@gmail.com");


                    mailmsg.Body = Convert.ToString(sb);


                    mailmsg.From = mailaddress;

                    mailmsg.Subject = "Change Password";
                    mailmsg.IsBodyHtml = true;




                    smtpClient.Host = SMTPHost;
                    smtpClient.Port = Convert.ToInt32(SMTPPort);
                    smtpClient.EnableSsl = Convert.ToBoolean(SMTPEnableSsl);
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new System.Net.NetworkCredential(UserId, MailPassword);
                    smtpClient.Send(mailmsg);
                    conn.Close();
                    return 1;
                }
                catch (Exception ex)
                {

                    return -1;

                }
                finally
                {
                    dbcommand.Connection.Close();
                }


            }

            else
            {
                //res.ResDescription = "Invalid EmailId!";
                //res.Status = "Failure";

                return 0;
            }
          
        }


        [Route("api/FRD/ChangePassword")]
        [HttpPost]
        public UserLogin ChangePassword(UserLoginRequest ul)
        {
            try
            {
                if (ul.UserPin.Trim() == "")
                {
                    res.Message = "User cannot be blank";
                    res.Status = "Failure";
                    res.UserLoginResponse = ListView;
                    Ulr.UserId = "";
                    Ulr.UserPin = "";
                    ListView.Add(Ulr);
                    return res;


                }
                else
                {


                    if (ul.Password.Trim() != "")
                    {
                        string pass = string.Empty;
                        string passkey = string.Empty;

                        pass = DBsecurity.Encrypt(ul.Password.Trim(), ref passkey);

                        query = "SP_Login";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.Connection.Open();
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "APIUpdatePassword");
                        dbcommand.Parameters.AddWithValue("@UserPin", ul.UserPin);
                        dbcommand.Parameters.AddWithValue("@Password", pass);
                        dbcommand.Parameters.AddWithValue("@PasswordKey", passkey);
                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        conn.Close();



                        if (dt.Rows.Count > 0)
                        {
                            res.Message = "Password has been changed";
                            res.Status = "success";
                            res.UserLoginResponse = ListView;
                            Ulr.UserId = "";
                            Ulr.UserPin = "";
                            ListView.Add(Ulr);


                        }
                    }
                    else
                    {
                        res.Message = "Password cannot be blank";
                        res.Status = "Failure";
                        res.UserLoginResponse = ListView;
                        Ulr.UserId = "";
                        Ulr.UserPin = "";
                        ListView.Add(Ulr);

                    }
                }
            }
            catch (Exception ex){
            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res;
        }


        [Route("api/FRD/UserProfile")]
        [HttpPost]
        public UserProFileRes UserProfile(UserProFileReq UP)
        {
            UserProFileRes res1 = new UserProFileRes();
            List<UserProFile> ListView = new List<UserProFile>();
            try
            {
                        query = "Sp_User";
                        dbcommand = new SqlCommand(query, conn);
                        dbcommand.Connection.Open();
                        dbcommand.CommandType = CommandType.StoredProcedure;
                        dbcommand.Parameters.AddWithValue("@QueryType", "APIUserProfile");
                        dbcommand.Parameters.AddWithValue("@UserId", UP.UserId);
                        SqlDataAdapter da = new SqlDataAdapter(dbcommand);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        conn.Close();
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        UserProFile upr = new UserProFile();
                        upr.UserName = row["UserName"].ToString();
                        upr.EmailId = row["EmailId"].ToString();
                        upr.UserPin = row["UserPin"].ToString();
                        upr.WareHouse = row["WareHouseName"].ToString();
                        ListView.Add(upr);
                    }
                    res1.UserProfileResponse = ListView;
                    res1.Status = "Success";
                    res1.Message = "Data retrieved successfully";
                }
                else
                {

                    res1.Status = "failure";
                    res1.Message = "Data retrieved not successfully";
                }




            }
            catch (Exception ex)
            {
            }
            finally
            {
                dbcommand.Connection.Close();
            }
            return res1;
        }
    }
    public class UserLogin
    {

        public string Status { get; set; }
        public string Message { get; set; }
        public List<UserLoginResponse> UserLoginResponse
        { get; set; }
        public List<UserWareHouse> WareHOuse
        { get; set; }
        public List<AccessRight> AccessRight
        { get; set; }

    }

    public class UserLoginRequest
    {

        public string UserPin { get; set; }
        public string Password { get; set; }

    }
    public class UserLoginResponse
    {
        public string UserId { get; set; }
        public string UserPin { get; set; }
        public string UserRole { get; set; }
    }
    public class UserWareHouse
    {
        public string WareHouseId { get; set; }
        public string WareHouse { get; set; }
       
    }

    public class AccessRight
    {
        public string MRN { get; set; }
        public string shipperList { get; set; }
        public string BranchinventoryReceiving { get; set; }
        public string inventoryCounting { get; set; }
        public string QRDetails { get; set; }
        public string QuantityUpdateDetails { get; set; }

    }
    public class UserProFileReq
    {
        public string UserId { get; set; }
    }

    public class UserProFile
    {
        public string EmailId { get; set; }
        public string UserPin { get; set; }
        public string WareHouse { get; set; }
        public string UserName { get; set; }
    }

    public class UserProFileRes
    {

        public string Status { get; set; }
        public string Message { get; set; }
        public List<UserProFile> UserProfileResponse
        { get; set; }
    }
}
