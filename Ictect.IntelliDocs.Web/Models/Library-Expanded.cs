using System;
using System.Linq;

namespace Ictect.IntelliDocs.Web.Models
{
    public partial class Library
    {
        /// <summary>
        /// The root (highest-level) Directory of the Library.
        /// </summary>
        public Directory RootDirectory
        {
            get
            {
                Directory rootDir = Directories.Where(x => x.dirParentId == null).OrderBy(x => x.dirCreateDate).FirstOrDefault();
                if (rootDir != null)
                {
                    return rootDir;
                }

                // Add root directory
                using (IntelliDocsEntities db = new IntelliDocsEntities())
                {
                    rootDir = new Directory()
                    {
                        dirCreateDate = DateTime.Now,
                        dirName = "Root",
                        dirParentId = null,
                        Library_libId = libId
                    };

                    db.Directories.Add(rootDir);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Exception handling
                        throw ex;
                    }
                }

                return rootDir;
            }
        }
    }
}