using System.Collections.Generic;
using System.Configuration;

namespace Ictect.IntelliDocs.Web.Models
{
    public partial class Document
    {
        private IntelliDocsEntities db = new IntelliDocsEntities();

        public string GetPath()
        {
            string basePath = ConfigurationManager.AppSettings["LibraryBasePath"];

            string id = docId.ToString();

            Directory parentDir = db.Directories.Find(dirId);

            Directory tempDir = parentDir;

            List<string> pathParts = new List<string>();
            pathParts.Add(id);
            pathParts.Add(parentDir.dirId.ToString());

            while (tempDir.dirParentId > 0 && tempDir.ParentDirectory != null)
            {
                pathParts.Add(tempDir.ParentDirectory.dirId.ToString());
                tempDir = parentDir.ParentDirectory;
            }

            string path = basePath;

            pathParts.Reverse();

            path += string.Join("/", pathParts);

            return path; // Test
        }
    }
}