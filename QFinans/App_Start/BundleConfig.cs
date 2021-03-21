using System.Web;
using System.Web.Optimization;

namespace QFinans
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Content/toasty/toasty.min.js",
                      "~/Content/select2/js/select2.min.js",
                      "~/Content/select2/js/i18n/tr.js",
                      "~/Scripts/jscolor.js",
                      "~/Content/chartjs/Chart.min.js",
                      "~/Content/chartjs/chartjs-plugin-colorschemes.min.js",
                      "~/Content/DataTables/datatables.min.js",
                      "~/Content/fontawesome/js/all.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css").Include(
                      "~/Content/fontawesome/css/all.min.css", new CssRewriteUrlTransform()).Include(
                      "~/Content/PagedList.css",
                      "~/Content/toasty/toasty.min.css",
                      //"~/Content/DataTable/datatables.min.css",
                      "~/Content/DataTable/DataTables-1.10.20/css/dataTables.bootstrap4.min.css",
                      "~/Content/select2/css/select2.min.css",
                      "~/Content/select2/css/select2-bootstrap4.min.css",
                      "~/Content/site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
