using System;
using Ictect.IntelliDocs.Web.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = Ictect.IntelliDocs.Web.Models.Document;

namespace Ictect.IntelliDocs.Web.Controllers
{
    public class SharesController : Controller
    {
        // GET: Shares
        public async Task<ActionResult> Index()
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                string userId = User.Identity.GetUserId();
                AspNetUser user = await db.AspNetUsers.FindAsync(userId);

                List<Share> shares = user.Shares.ToList();

                List<Directory> shareDirectories =
                    await
                        db.Directories.Where(
                            x =>
                                shares.Where(shr => shr.Directory_nodeId != 0)
                                    .Select(shr => shr.Directory_nodeId)
                                    .ToList()
                                    .Contains(x.dirId)).ToListAsync();

                List<Document> shareDocuments =
                    await
                        db.Documents.Where(
                            doc =>
                                shares.Where(shr => shr.Document_docId != 0)
                                    .Select(x => x.Document_docId)
                                    .ToList()
                                    .Contains(doc.docId)).ToListAsync();

                ViewBag.Directories = shareDirectories;

                ViewBag.Documents = shareDocuments;

                return PartialView();
            }
        }

        public async Task<ActionResult> ShareDirectory(int id)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Directory dir = await db.Directories.FindAsync(id);

                List<Share> dirShares =
                    await
                        db.Shares.Include(shr => shr.AspNetUsers)
                            .Where(shr => shr.Directory_nodeId == dir.dirId)
                            .ToListAsync();

                ViewBag.DirShares = dirShares;

                return PartialView(dir);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ShareDirectory(int dirId, string userName)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                AspNetUser otherUser = await db.AspNetUsers.Where(usr => usr.UserName == userName).FirstOrDefaultAsync();

                if (otherUser == null) return Json(new { status = "error", message = "<strong>Error!</strong> The specified Username couldn't be found." });

                Share newShare = new Share()
                {
                    shrCreatedDate = DateTime.Now,
                    Directory_nodeId = dirId
                };
                newShare.AspNetUsers.Add(otherUser);

                db.Shares.Add(newShare);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", message = "<strong>Error!</strong> Failed to add the new Share: " + ex.Message });
                }

                return Json(new { status = "success", message = "<strong>Success!</strong> This Directory was successfully shared with " + otherUser.UserName + "." });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UnShareDirectory(int dirId, string userName)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                AspNetUser otherUser = await db.AspNetUsers.Where(usr => usr.UserName == userName).FirstOrDefaultAsync();

                if (otherUser == null) return Json(new { status = "error", message = "<strong>Error!</strong> The specified Username couldn't be found." });

                List<Share> docShares = await db.Shares.Where(share => share.Directory_nodeId == dirId && share.AspNetUsers.Select(usr => usr.UserName).Contains(userName)).ToListAsync();

                foreach (Share shr in docShares)
                {
                    List<AspNetUser> updatedUsers = shr.AspNetUsers.ToList();
                    updatedUsers.RemoveAll(usr => usr.UserName == userName);
                    shr.AspNetUsers = updatedUsers;
                    db.Entry(shr).State = EntityState.Modified;
                }

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", message = "<strong>Error!</strong> Failed to modify the Share: " + ex.Message });
                }

                return Json(new { status = "success", message = "<strong>Success!</strong> This directory was successfully un-shared with " + otherUser.UserName + "." });
            }
        }

        public async Task<ActionResult> ShareDocument(int id)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Document doc = await db.Documents.FindAsync(id);

                List<Share> docShares =
                    await
                        db.Shares.Include(shr => shr.AspNetUsers)
                            .Where(shr => shr.Document_docId == doc.docId)
                            .ToListAsync();

                ViewBag.DocShares = docShares;

                return PartialView(doc);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ShareDocument(int docId, string userName)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                AspNetUser otherUser = await db.AspNetUsers.Where(usr => usr.UserName == userName).FirstOrDefaultAsync();

                if (otherUser == null) return Json(new { status = "error", message = "<strong>Error!</strong> The specified Username couldn't be found."});

                Share newShare = new Share()
                {
                    shrCreatedDate = DateTime.Now,
                    Document_docId = docId
                };
                newShare.AspNetUsers.Add(otherUser);

                db.Shares.Add(newShare);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", message = "<strong>Error!</strong> Failed to add the new Share: " + ex.Message });
                }

                return Json(new { status = "success", message = "<strong>Success!</strong> This document was successfully shared with " + otherUser.UserName + "." });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UnShareDocument(int docId, string userName)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                AspNetUser otherUser = await db.AspNetUsers.Where(usr => usr.UserName == userName).FirstOrDefaultAsync();

                if (otherUser == null) return Json(new { status = "error", message = "<strong>Error!</strong> The specified Username couldn't be found." });

                List<Share> docShares = await db.Shares.Where(share => share.Document_docId == docId && share.AspNetUsers.Select(usr => usr.UserName).Contains(userName)).ToListAsync();

                foreach (Share shr in docShares)
                {
                    List<AspNetUser> updatedUsers = shr.AspNetUsers.ToList();
                    updatedUsers.RemoveAll(usr => usr.UserName == userName);
                    shr.AspNetUsers = updatedUsers;
                    db.Entry(shr).State = EntityState.Modified;
                }

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", message = "<strong>Error!</strong> Failed to modify the  Share: " + ex.Message });
                }

                return Json(new { status = "success", message = "<strong>Success!</strong> This document was un-successfully shared with " + otherUser.UserName + "." });
            }
        }

        public async Task<ActionResult> SearchUsers(string q)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                string[] results =
                    await
                        db.AspNetUsers.Where(usr => usr.UserName.Contains(q)).Select(usr => usr.UserName).ToArrayAsync();

                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }
    }
}