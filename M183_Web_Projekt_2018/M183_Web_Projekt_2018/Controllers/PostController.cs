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
    public class PostController : Controller
    {   
        public ActionResult Dashboard()
        {
            SqlCommand cmd = GetSqlConnection();
            SqlDataReader reader;
            cmd.CommandText = "SELECT [Id], [Title], [Description], [content] FROM [dbo].[Post] WHERE DeletedOn IS Null";
            cmd.Parameters.Clear();
            cmd.Connection.Open();
            reader = cmd.ExecuteReader();

            var post = new Post();
            var list = new List<Post>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    list.Add(new Post()
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.GetString(2),
                        Content = reader.GetString(3),
                    });
                }
            }
                cmd.Connection.Close();

            return View(list);

        }
        [HttpPost]
        public ActionResult AddComment()
        {
            return View();
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
