using Ictect.IntelliDocs.Web.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ictect.IntelliDocs.Web.Controllers
{
    public class SharesController : Controller
    {
        // GET: Shares
        public ActionResult Index()
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                string userId = User.Identity.GetUserId();
                AspNetUser user = db.AspNetUsers.Find(userId);

                List<Share> shares = user.Shares.ToList();

                List<Directory> shareDirectories = db.Directories.Where(x => shares.Where(shr => shr.Directory_nodeId != 0).Select(shr => shr.Directory_nodeId).ToList().Contains(x.dirId)).ToList();

                List<Document> shareDocuments = db.Documents.Where(doc => shares.Where(shr => shr.Document_docId != 0).Select(x => x.Document_docId).ToList().Contains(doc.docId)).ToList();

                ViewBag.Directories = shareDirectories;

                ViewBag.Documents = shareDocuments;

                return PartialView();
            }
        }
    }
}