using System.Web;
using System.Web.Mvc;

namespace M183_Web_Projekt_2018
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
