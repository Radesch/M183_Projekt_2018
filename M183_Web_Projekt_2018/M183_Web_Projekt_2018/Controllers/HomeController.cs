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
            var mode = "SMS"; // OR SMS
            int userid = 0;
            // DB Connection
            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\GitHub Project\\Data\\m183_project.mdf;Integrated Security=True;Connect Timeout=30";

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT * FROM [dbo].[User] WHERE [username] = '" + username + "' AND [password] = '" + password + "'";
            cmd.Connection = con;

            con.Open();

            reader = cmd.ExecuteReader();

            // Check if User exists
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    mobileNumber = reader.GetString(5);
                    userid = reader.GetInt32(0);
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


                    // Send SMS via Nexmo API
                    var postData = "api_key=282e7d22";
                    postData += "&api_secret=865824a393b32341";
                    postData += "&to=" + mobileNumber;
                    postData += "&from=\"\"NEXMO\"\"";
                    postData += "&text=\"" + finalString + "\"";
                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();

                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    reader.Close();

                    ViewBag.Message = responseString;
                    cmd.CommandText = "Insert into Token (Token,UserId,Expiry) VALUES (@finalString,@userid, @Expiry)";
                    cmd.Parameters.AddWithValue("@finalString", finalString);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@Expiry", DateTime.Now.AddMinutes(5)); //5 mins
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                ViewBag.Message = "Wrong Credentials";
            }
            con.Close();

            return View();
        }
        [HttpPost]
        public void TokenLogin()
        {
            var token = Request["token"];

            if (token == "TEST_SECRET")
            {
                // -> "Token is correct";
            }
            else
            {
                // -> "Wrong Token";
            }

        }
    }
}