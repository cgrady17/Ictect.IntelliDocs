using System.Web.Optimization;

namespace Ictect.IntelliDocs.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            StyleBundle cssBundle = new StyleBundle("~/Content/IntelliDocsCSS");
            cssBundle.Include(
                "~/Content/bootstrap.css",
                "~/Content/IntelliDocs.css"
                );
            bundles.Add(cssBundle);

            ScriptBundle jsBundle = new ScriptBundle("~/Scripts/IntelliDocsJS");
            jsBundle.Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/IntelliDocs/IntelliDocs.js"
                );
            bundles.Add(jsBundle);
        }
    }
}