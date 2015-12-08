using Ictect.IntelliDocs.Web.Extensions;
using Ictect.IntelliDocs.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ictect.IntelliDocs.Web.Controllers
{
    public class LibraryController : Controller
    {
        // GET: Library
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Load a Partial View with the contents of the specified Directory.
        /// </summary>
        /// <param name="dirId">ID of the Directory.</param>
        /// <param name="libraryId">ID of the Library of which this Directory is a member.</param>
        /// <returns>Partial View.</returns>
        /// <remarks>NOTE: This action is a Child action, meaning it is loaded as part of another (parent) action. Therefore (unfortunately), it must be synchronous.</remarks>
        public ActionResult LoadDirectory(int? dirId, int? libraryId)
        {
            if (!dirId.HasValue || dirId == 0)
            {
                ViewBag.errorMessage = "A Directory ID is required.";
                return PartialView("Error");
            }

            if (!libraryId.HasValue || libraryId == 0)
            {
                ViewBag.errorMessage = "A Library ID is required.";
                return PartialView("Error");
            }

            // Get the User's Library
            Library userLibrary = User.Identity.GetLibrary(libraryId.Value);

            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Directory thisDirectory = db.Directories.Include(dir => dir.Documents).Where(dir => dir.dirId == dirId && dir.Library_libId == libraryId).FirstOrDefault();
                if (thisDirectory == null) return View("Error", new HandleErrorInfo(new Exception("Directory not found."), "Library", "LoadDirectory"));
                if (thisDirectory.Library_libId != libraryId) return View("Error", new HandleErrorInfo(new Exception("Specified Directory is not a part of this your Library."), "Library", "LoadDirectory"));

                return PartialView(thisDirectory);
            }
        }

        public ActionResult NewFolder()
        {
            return PartialView();
        }

        /// <summary>
        /// Creates a new Directory record for the current User's Library with the specified Parent Directory and Name.
        /// </summary>
        /// <param name="parentDirId">The id of the new Directory's Parent.</param>
        /// <param name="dirName">The name of the new Directory.</param>
        /// <param name="libraryId">The ID of a specific Library to use if desired.</param>
        /// <returns>JSON specifying status and the ID of the new Directory if successful.</returns>
        public async Task<ActionResult> CreateFolder(int? parentDirId, string dirName, int? libraryId = null)
        {
            if (parentDirId == null)
            {
                return Json(new { status = "error", message = "A Directory Parent ID is required." });
            }

            if (parentDirId == 0) parentDirId = null;

            if (string.IsNullOrEmpty(dirName))
            {
                return Json(new { status = "error", message = "A Directory name is required." });
            }

            string userId = User.Identity.GetUserId();
            Library userLibrary = null;

            Directory newDir = new Directory()
            {
                dirCreateDate = DateTime.Now,
                dirName = dirName,
                dirParentId = parentDirId
            };

            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                if (libraryId == null || libraryId == 0)
                {
                    userLibrary = await User.Identity.GetLibraryAsync();
                }
                else
                {
                    userLibrary = await db.Libraries.FindAsync(libraryId);
                    if (userLibrary.AspNetUser_Id != userId)
                    {
                        return Json(new { status = "error", message = "The specified Library is not owned by the current User." });
                    }
                }

                newDir.Library_libId = userLibrary.libId;

                if (parentDirId == null)
                {
                    newDir.dirParentId = userLibrary.RootDirectory.dirId;
                }

                db.Directories.Add(newDir);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // TODO: Exception handling
                    return Json(new { status = "error", message = "Error adding new Directory to DB: " + ex.Message });
                }
            }

            return Json(new { status = "success", message = newDir.dirId.ToString() });
        }
    }
}