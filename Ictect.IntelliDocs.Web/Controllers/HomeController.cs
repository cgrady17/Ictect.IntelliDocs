using Ictect.IntelliDocs.Web.Extensions;
using Ictect.IntelliDocs.Web.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ictect.IntelliDocs.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        /// <summary>
        /// Asynchronously retrieves and displays the current authenticated User's Library.
        /// </summary>
        /// <returns>Razor View.</returns>
        public async Task<ActionResult> Index()
        {
            // Get the User's Library
            Library userLibrary = await User.Identity.GetLibraryAsync();

            return View(userLibrary);
        }

        public ActionResult SplashPage()
        {
            return PartialView("_SplashPage");
        }
    }
}