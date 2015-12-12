using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictect.IntelliDocs.Web.Models;
using Directory = Ictect.IntelliDocs.Web.Models.Directory;

namespace Ictect.IntelliDocs.Web.Controllers
{
    public class FileController : Controller
    {

        public ActionResult Upload(int dirId)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Directory dir = db.Directories.Find(dirId);

                return dir == null ? PartialView("Error") : PartialView(dir);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Upload(int dirId, string fileName, HttpPostedFileBase file)
        {
            return Json(new {status = "success", message = ""});
        }

        public ActionResult Get(int docId)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Document doc = db.Documents.Find(docId);

                if (doc == null)
                {
                    return HttpNotFound("The requested Document could not be found.");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(doc.GetPath());

                return new FileStreamResult(new MemoryStream(fileBytes), "application/doc-x");
            }
        }
    }
}