using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace M183_Web_Projekt_2018.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public ActionResult Login()
        {
            // In order to make this code work -> replace all UPPERCASE-Placeholders with the corresponding data!
            var username = Request["username"];
            var password = Request["password"];
            string mobileNumber = "";
            string currentUsername = "";
            string currentPassword = "";
            var mode = "SMS"; // OR SMS
            int userid = 0;


            SqlCommand cmd = GetSqlConnection();
            SqlDataReader reader;

            cmd.CommandText = "SELECT * FROM [dbo].[User] WHERE [username] = '" + username + "' AND [password] = '" + password + "'";
            cmd.Connection.Open();
            reader = cmd.ExecuteReader();


            // Check if User exists
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    mobileNumber = reader.GetString(5);
                    userid = reader.GetInt32(0);
                    currentUsername = reader.GetString(1);
                    currentPassword = reader.GetString(2);
                }
                if (mode == "SMS")
                {
                    var request = (HttpWebRequest)WebRequest.Create("https://rest.nexmo.com/sms/json");

                    //Generate random Token
                    var chars = "0123456789";
                    var stringChars = new char[6];
                    var random = new Random();

                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }

                    var finalString = new String(stringChars);

                    //// Send SMS via Nexmo API
                    //var postData = "api_key=0a98cb9c";
                    //postData += "&api_secret=9ae22e80dcfb1e5a";
                    //postData += "&to=" + mobileNumber;
                    //postData += "&from=\"\"NEXMO\"\"";
                    //postData += "&text=\"" + finalString + "\"";
                    //var data = Encoding.ASCII.GetBytes(postData);

                    //request.Method = "POST";
                    //request.ContentType = "application/x-www-form-urlencoded";
                    //request.ContentLength = data.Length;

                    //using (var stream = request.GetRequestStream())
                    //{
                    //    stream.Write(data, 0, data.Length);
                    //}

                    //var response = (HttpWebResponse)request.GetResponse();
                    //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    //ViewBag.Message = responseString;

                    reader.Close();
                    cmd.Connection.Close();

                    cmd.CommandText = "Insert into Token (Token,UserId,Expiry) VALUES (@finalString,@userid, @Expiry)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@finalString", finalString);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@Expiry", DateTime.Now.AddMinutes(5)); //5 mins

                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            else
            {
                ViewBag.Message = "Wrong Credentials";
            }
            return View();
        }

        [HttpPost]
        public ActionResult TokenLogin()
        {
            var current_user = Request["username"];
            var current_password = Request["password"];
            var token = Request["token"];
            var userid = 0;
            var userRole = "";
            var current_token = "";


            SqlCommand cmd = GetSqlConnection();
            SqlDataReader reader;
            cmd.CommandText = "SELECT Id, Role FROM [dbo].[user] WHERE [username] = @current_user AND [password] = @current_password";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@current_user", current_user);
            cmd.Parameters.AddWithValue("@current_password", current_password);

            cmd.Connection.Open();

            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                userid = reader.GetInt32(0);
                userRole = reader.GetString(1);
            }
            cmd.Connection.Close();
            cmd.CommandText = "SELECT Token FROM [dbo].[Token] WHERE [UserId] = @userid ORDER BY Id Desc";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Connection.Open();

            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                current_token = reader.GetString(0);
            }
            if (token == current_token)
            {
                // -> "Token is correct";
                Session["username"] = current_user;
                Session["userid"] = userid;

                CreateLogs(userid);

                switch (userRole)
                {
                    case "user":
                        return RedirectToAction("Dashboard", "User");
                    case "admin":
                        return RedirectToAction("Dashboard", "Admin");
                    // Log Error
                    default:
                        cmd = GetSqlConnection();
                        cmd.CommandText = "INSERT INTO UserLog (UserId, Action) VALUES (@userId, @action)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@userid", userid);
                        cmd.Parameters.AddWithValue("@action", "error");
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                        break;
                }

            }
            else
            {
                ViewBag.Message = "Wrong Credentials";
            }
            cmd.Connection.Close();
            return null;
        }

        private void CreateLogs(int userId)
        {
            SqlCommand cmd = GetSqlConnection();
            cmd.Connection.Open();

            // Userlog
            cmd.CommandText = "INSERT INTO UserLog (UserId, Action) VALUES (@userId, @action)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@action", "Loged in");
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();


            // UserLogin
            cmd.Connection.Open();
            cmd.CommandText = @"INSERT INTO UserLogin (UserId, IP, SessionId, CreatedOn, ModifiedOn, DeletedOn)
                               VALUES (@uid, @ipAddress, @sessionId, @createdOn, @modifiedOn, @deletedOn)";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@ipAddress", Request.UserHostAddress);
            cmd.Parameters.AddWithValue("@sessionId", Session.SessionID);
            cmd.Parameters.AddWithValue("@createdOn", DateTime.Now);
            cmd.Parameters.AddWithValue("@modifiedOn", DBNull.Value);
            cmd.Parameters.AddWithValue("@deletedOn", DBNull.Value);
            cmd.ExecuteNonQuery();

            cmd.Connection.Close();
        }

        #region Private section
        private SqlCommand GetSqlConnection()
        {
            // DB Connection
            SqlConnection con = new SqlConnection();
            SqlCommand cmd = new SqlCommand();

            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\TEMP\\M183_Projekt_2018\\Data\\m183_project.mdf;Integrated Security=True;Connect Timeout=30";
            cmd.Connection = con;

            return cmd;
        }
        #endregion
    }
}