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
                "~/Content/font-awesome.css",
                "~/Content/jquery.auto-complete.css",
                "~/Content/IntelliDocs.css"
                );
            bundles.Add(cssBundle);

            ScriptBundle jsBundle = new ScriptBundle("~/Scripts/IntelliDocsJS");
            jsBundle.Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.form.js",
                "~/Scripts/jquery.auto-complete.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/IntelliDocs/IntelliDocs.js"
                );
            bundles.Add(jsBundle);

            ScriptBundle jqueryValBundle = new ScriptBundle("~/Scripts/jQueryVal");
            jqueryValBundle.Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js"
                );
            bundles.Add(jqueryValBundle);
        }
    }
}