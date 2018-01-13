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

            // DB Connection
            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Daniele\\Documents\\M183_Project_Database.mdf;Integrated Security=True;Connect Timeout=30";

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
                }
                if (mode == "SMS")
                {
                    var request = (HttpWebRequest)WebRequest.Create("https://rest.nexmo.com/sms/json");

                    var secret = "TEST_SECRET";

                    // Send SMS via Nexmo API
                    var postData = "api_key=282e7d22";
                    postData += "&api_secret=865824a393b32341";
                    postData += "&to=" + mobileNumber;
                    postData += "&from=\"\"NEXMO\"\"";
                    postData += "&text=\"" + secret + "\"";
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

                    ViewBag.Message = responseString;
                }
            }
            else
            {
                ViewBag.Message = "Wrong Credentials";
            }
            con.Close();

            return View();
        }
    }
}