using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    public class TicketContrrollercs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
