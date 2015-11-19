using Ictect.IntelliDocs.Web.Extensions;
using Ictect.IntelliDocs.Web.Models;
using Microsoft.AspNet.Identity;
using System;
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
        /// Creates a new Directory record for the current User's Library with the specified Parent Directory and Name.
        /// </summary>
        /// <param name="parentDirId">The id of the new Directory's Parent.</param>
        /// <param name="dirName">The name of the new Directory.</param>
        /// <param name="libraryId">The ID of a specific Library to use if desired.</param>
        /// <returns>JSON specifying status and the ID of the new Directory if successful.</returns>
        public async Task<ActionResult> CreateFolder(int? parentDirId, string dirName, int? libraryId = null)
        {
            if (parentDirId == null || parentDirId == 0)
            {
                return Json(new { status = "error", message = "A Directory Parent ID is required." });
            }

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
                dirParentId = parentDirId.Value
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

                newDir.Library = userLibrary;

                db.Directories.Add(newDir);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // TODO: Exception handling
                }
            }

            return Json(new { status = "success", message = newDir.dirId.ToString() });
        }
    }
}