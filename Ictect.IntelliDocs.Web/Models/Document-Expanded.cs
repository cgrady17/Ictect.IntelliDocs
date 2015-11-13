using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int dirParentId = parentDir.dirParentId;

            List<string> dirs = new List<string>();

            for (int i = 0; i < dirParentId; i++)
            {
                
            }

            return ""; // Test
        }
    }
}
