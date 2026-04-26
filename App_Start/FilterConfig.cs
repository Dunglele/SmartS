using System.Web;
using System.Web.Mvc;

namespace Smarts_DoAn_Backup_27_11_2025
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
