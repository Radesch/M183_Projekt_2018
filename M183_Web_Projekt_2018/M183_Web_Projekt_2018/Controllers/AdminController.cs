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
    public class AdminController : Controller
    {

        public ActionResult Dashboard()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var current_user = (string)Session["username"];
                var userid = (int)Session["userid"];
                SqlCommand cmd = GetSqlConnection();
                SqlDataReader reader;
                var userRole = string.Empty;
                cmd.CommandText = "SELECT Role FROM [dbo].[user] WHERE [username] = @current_user AND [id] = @userid";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@current_user", current_user);
                cmd.Parameters.AddWithValue("@userid", userid);

                cmd.Connection.Open();

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userRole = reader.GetString(0);
                }

                if (userRole == "admin")
                {

                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            

            return View();
        }
        private void CreateLogs(int userId)
        {
            SqlCommand cmd = GetSqlConnection();
            cmd.Connection.Open();

            // Userlog
            cmd.CommandText = "INSERT INTO UserLog (UserId, Action) VALUES (@userId, @action)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@action", "deleted");
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
            cmd.Parameters.AddWithValue("@deletedOn", DateTime.Now);
            cmd.ExecuteNonQuery();

            cmd.Connection.Close();
        }
        public ActionResult Logout()
        {
            if (Session["userid"] != null)
            {
                var userid = (int)Session["userid"];
                Session.Abandon();
                CreateLogs(userid);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #region Private section
        private SqlCommand GetSqlConnection()
        {
            // DB Connection
            SqlConnection con = new SqlConnection();
            SqlCommand cmd = new SqlCommand();

            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\GitHub Project\\Data\\m183_project.mdf;Integrated Security=True;Connect Timeout=30";
            cmd.Connection = con;

            return cmd;
        }
        #endregion
    }
}
