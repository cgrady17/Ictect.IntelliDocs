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

            string id = this.docId.ToString();

            Directory parentDir = db.Directories.Find(this.Directory_nodeId);

            Directory tempDir = parentDir;

            List<string> pathParts = new List<string>();
            pathParts.Add(id);
            pathParts.Add(parentDir.dirId.ToString());

            while (tempDir.dirParentId > 0 && tempDir.Directory1 != null)
            {
                pathParts.Add(tempDir.Directory1.dirId.ToString());
                tempDir = parentDir.Directory1;
            }

            string path = basePath;

            for (int i = pathParts.Count; i > 0; i--)
            {
                path += pathParts[i];
            }

            return path; // Test
        }
    }
}