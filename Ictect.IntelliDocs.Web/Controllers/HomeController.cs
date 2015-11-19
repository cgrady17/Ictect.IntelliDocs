using Ictect.IntelliDocs.Web.Extensions;
using Ictect.IntelliDocs.Web.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

            // Get the User's Root Documents
            IReadOnlyList<Document> rootDocuments = null;
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                string userId = User.Identity.GetUserId();
                rootDocuments = await db.Documents.Where(x => x.AspNetUser.Id == userId && x.Directory_nodeId == 0).ToListAsync();
            }

            ViewBag.RootDocuments = rootDocuments;

            return View(userLibrary);
        }

        public ActionResult SplashPage()
        {
            return PartialView("_SplashPage");
        }
    }
}