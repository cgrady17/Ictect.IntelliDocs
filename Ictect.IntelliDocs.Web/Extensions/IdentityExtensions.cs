using Ictect.IntelliDocs.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Ictect.IntelliDocs.Web.Extensions
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// Asynchronously retrieve the newest Library for the User.
        /// </summary>
        /// <param name="identity">The inferring IIdentity of the User.</param>
        /// <returns>The User's newest Library.</returns>
        public static async Task<Library> GetLibraryAsync(this IIdentity identity)
        {
            Library userLibrary = null;
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                string userId = identity.GetUserId();
                IReadOnlyList<Library> userLibraries = await db.Libraries.Where(x => x.AspNetUser_Id == userId).OrderByDescending(x => x.libCreatedDate).ToListAsync();
                userLibrary = userLibraries.FirstOrDefault();

                if (userLibrary == null)
                {
                    // Create new Library for User
                    userLibrary = await CreateLibrary(userId);
                }
            }

            return userLibrary;
        }

        /// <summary>
        /// Asynchronously creates a new Library entity for the specified User.
        /// </summary>
        /// <param name="userId">The User ID of the User for which to create a new Library.</param>
        /// <returns>The newly created Library entity.</returns>
        private static async Task<Library> CreateLibrary(string userId)
        {
            using (IntelliDocsEntities db = new IntelliDocsEntities())
            {
                Library newLibrary = new Library()
                {
                    AspNetUser_Id = userId,
                    libCreatedDate = DateTime.Now
                };

                db.Libraries.Add(newLibrary);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return newLibrary;
            }
        }
    }
}