using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183_Web_Projekt_2018.Controllers
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public List<string> Comments { get; set; }
    }
}