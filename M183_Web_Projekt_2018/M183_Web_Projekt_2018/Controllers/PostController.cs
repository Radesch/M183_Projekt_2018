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
        public ActionResult DetailPost(int id)
        {
            SqlCommand cmd = GetSqlConnection();
            SqlDataReader reader;
            cmd.CommandText = "SELECT Commet FROM [dbo].[Comment] WHERE PostId = " + id;
            cmd.Parameters.Clear();
            cmd.Connection.Open();

            reader = cmd.ExecuteReader();

            var list = new List<string>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
            }
            cmd.Connection.Close();
            cmd.Connection.Open();
            cmd.CommandText = "SELECT [Title], [Description], [content] FROM [dbo].[Post] WHERE Id = " + id;
            reader = cmd.ExecuteReader();

            var post = new Post();
            while (reader.Read())
            {
                post = new Post()
                {
                    Title = reader.GetString(0),
                    Description = reader.GetString(1),
                    Content = reader.GetString(2),
                    Id = id
                };
            }
            cmd.Connection.Close();

            post.Comments = list;
            return View(post);
        }

        [HttpPost]
        public ActionResult AddComment()
        {
            var comment = Request["comment"];
            var id = Request["id"];
            if (!string.IsNullOrEmpty(comment) && comment.Length < 200)
            {
                SqlCommand cmd = GetSqlConnection();
                cmd.CommandText = "INSERT INTO [dbo].[Comment] (commet, PostId) VALUES('" + comment + "', " + id + ")";
                cmd.Parameters.Clear();
                cmd.Connection.Open();
                cmd.ExecuteReader();

                ViewBag.Message = "";

                cmd.Connection.Close();
            }
            else
            {
                ViewBag.Message = "Text ist zu lang oder ist leer!";
            }

            return DetailPost(int.Parse(id));
        }
        #region Private section
        private SqlCommand GetSqlConnection()
        {
            // DB Connection
            SqlConnection con = new SqlConnection();
            SqlCommand cmd = new SqlCommand();

            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\M183_Project\\Data\\m183_project.mdf;Integrated Security=True;Connect Timeout=30";
            cmd.Connection = con;

            return cmd;
        }
        #endregion
    }
}
