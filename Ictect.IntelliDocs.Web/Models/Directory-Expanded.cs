using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Ictect.IntelliDocs.Web.Models
{
    public partial class Directory
    {
        public string Path
        {
            get
            {
                string basePath = ConfigurationManager.AppSettings["LibraryBasePath"];
                List<string> pathParts = new List<string>();
                pathParts.Add(dirId.ToString());
                if (ParentDirectory != null)
                {
                    pathParts.Add(ParentDirectory.dirId.ToString());
                    Directory tempDir = ParentDirectory;

                    while (tempDir.dirParentId != null)
                    {
                        pathParts.Add(tempDir.ParentDirectory.dirId.ToString());
                        tempDir = tempDir.ParentDirectory;
                    }
                }

                string path = basePath;

                pathParts.Reverse();

                path += string.Join("/", pathParts);

                return path;
            }
        }

        public string FriendlyPath
        {
            get
            {
                string basePath = ConfigurationManager.AppSettings["LibraryBasePath"];

                List<string> pathParts = new List<string>();
                pathParts.Add(dirName);
                if (ParentDirectory != null)
                {
                    pathParts.Add(ParentDirectory.dirName);
                    Directory tempDir = ParentDirectory;

                    while (tempDir.dirParentId != null)
                    {
                        pathParts.Add(tempDir.ParentDirectory.dirName);
                        tempDir = tempDir.ParentDirectory;
                    }
                }

                string path = basePath;

                pathParts.Reverse();

                path += string.Join("/", pathParts);

                return path;
            }
        }

        public MvcHtmlString PathLinks
        {
            get
            {
                List<string> pathParts = new List<string>();
                pathParts.Add("<a href='#' data-action='loadDirectory' data-dirid='" + dirId + "'>" + dirName + "</a>");
                if (ParentDirectory != null)
                {
                    pathParts.Add("<a href='#' data-action='loadDirectory' data-dirid='" + ParentDirectory.dirId + "'>" + ParentDirectory.dirName + "</a>");
                    Directory tempDir = ParentDirectory;

                    while (tempDir.dirParentId != null)
                    {
                        pathParts.Add("<a href='#' data-action='loadDirectory' data-dirid='" + tempDir.dirId + "'>" + tempDir.dirName + "</a>");
                        tempDir = tempDir.ParentDirectory;
                    }
                }

                pathParts.Reverse();

                string newRootName = pathParts.Count > 1 ? "Root" : "/";
                pathParts[0] = pathParts[0].Replace("Root", newRootName);

                string path = string.Join(" / ", pathParts);

                return MvcHtmlString.Create(path);
            }
        }

        /// <summary>
        /// Collection of all Directories within this Directory. Not recursive.
        /// </summary>
        public ICollection<Directory> ChildDirectories
        {
            get
            {
                using (IntelliDocsEntities db = new IntelliDocsEntities())
                {
                    if (dirId == 0) return new List<Directory>();
                    return db.Directories.Where(x => x.dirParentId != null && x.dirParentId == dirId).ToList();
                }
            }
        }

        /// <summary>
        /// This Directory's Parent Directory, if any.
        /// </summary>
        public Directory ParentDirectory
        {
            get
            {
                if (dirParentId.HasValue)
                {
                    using (IntelliDocsEntities db = new IntelliDocsEntities())
                    {
                        return db.Directories.Find(dirParentId.Value);
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}