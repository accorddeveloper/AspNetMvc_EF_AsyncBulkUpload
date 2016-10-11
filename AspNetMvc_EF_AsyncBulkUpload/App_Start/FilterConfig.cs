using System.Web;
using System.Web.Mvc;

namespace AspNetMvc_EF_AsyncBulkUpload
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
