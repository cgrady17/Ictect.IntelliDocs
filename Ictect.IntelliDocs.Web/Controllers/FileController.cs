using Ictect.IntelliDocs.Web.Helpers;
using Ictect.IntelliDocs.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Directory = Ictect.IntelliDocs.Web.Models.Directory;

namespace Ictect.IntelliDocs.Web.Controllers
{
    public class FileController : Controller
    {
        public async Task<ActionResult> FileDetails(int id)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Document doc = await db.Documents.FirstOrDefaultAsync(x => x.docId == id);

                ViewBag.Shares = await db.Shares.Where(shr => shr.Document_docId == doc.docId).ToListAsync();

                return doc == null ? PartialView("Error") : PartialView(doc);
            }
        }

        public ActionResult Upload(int id)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Directory dir = db.Directories.Find(id);

                return dir == null ? PartialView("Error") : PartialView(dir);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Upload(int dirId, string fileName, HttpPostedFileBase file)
        {
            if (string.IsNullOrEmpty(fileName)) return Json(new { status = "error", message = "A file name is required to upload." });
            if (file == null || file.ContentLength == 0) return Json(new { status = "error", message = "A file is required." });
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Directory thisDirectory = db.Directories.Find(dirId);

                string extension = Path.GetExtension(file.FileName);

                Document newDoc = new Document()
                {
                    dirId = dirId,
                    docCreatedDate = DateTime.Now,
                    docName = fileName,
                    docExtension = extension,
                    docContentType = file.ContentType,
                    User_userId = User.Identity.GetUserId()
                };

                db.Documents.Add(newDoc);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", message = "Error uploading new Document: " + ex.Message });
                }

                string basePath = Path.Combine(Server.MapPath("~"), thisDirectory.Path);
                string fullFileName = newDoc.docId + newDoc.docExtension;
                if (!System.IO.Directory.Exists(basePath))
                {
                    System.IO.Directory.CreateDirectory(basePath);
                }
                file.SaveAs(Path.Combine(basePath, fullFileName));
            }

            return Json(new { status = "success", message = "The new Document was uploaded successfully." });
        }

        public ActionResult ValidateDocument(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return Json(new { status = "error", message = "A file is required." });
            string extension = Path.GetExtension(file.FileName);
            if (extension != ".doc" && extension != ".docx") return Json(new { status = "error", message = "Only Microsoft Office Word documents are supported." });

            using (MemoryStream ms = new MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                string results = DocumentProcessor.ValidateWordDocument(ms);
                results = results.Replace(Environment.NewLine, "<br />");
                string stats = DocumentProcessor.BasicStats(ms);
                stats = stats.Replace("\n", "<br />");
                return Json(new { status = "success", message = "<strong>Validated!</strong> " + results + "<br />" + stats });
            }
        }

        public ActionResult Get(int id)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Document doc = db.Documents.Find(id);

                if (doc == null)
                {
                    return HttpNotFound("The requested Document could not be found.");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(doc.Path);

                return File(new MemoryStream(fileBytes), doc.docContentType, doc.FullFilename);
            }
        }

        public ActionResult Delete(int id)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Document doc = db.Documents.Find(id);

                string path = doc.Path;

                db.Entry(doc).State = EntityState.Deleted;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", message = "Failed to delete file: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }

                System.IO.File.Delete(path);
            }

            return Json(new { status = "success", message = "File successfully deleted!" }, JsonRequestBehavior.AllowGet);
        }
    }
}