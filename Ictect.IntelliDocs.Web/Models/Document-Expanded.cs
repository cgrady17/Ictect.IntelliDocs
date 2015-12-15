using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace Ictect.IntelliDocs.Web.Models
{
    public partial class Document
    {
        private IntelliDocsEntities db = new IntelliDocsEntities();

        public string Path
        {
            get
            {
                string basePath = ConfigurationManager.AppSettings["LibraryBasePath"];

                basePath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~"), basePath);

                string id = docId + docExtension;

                Directory parentDir = db.Directories.Find(dirId);

                Directory tempDir = parentDir;

                List<string> pathParts = new List<string> {id, parentDir.dirId.ToString()};

                while (tempDir.dirParentId > 0 && tempDir.ParentDirectory != null)
                {
                    pathParts.Add(tempDir.ParentDirectory.dirId.ToString());
                    tempDir = parentDir.ParentDirectory;
                }

                string path = basePath;

                pathParts.Reverse();

                path += string.Join("/", pathParts);

                return path;
            }
        }

        public string FullFilename => docName + docExtension;
    }
}