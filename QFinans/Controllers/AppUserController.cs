using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using QFinans.Areas.Api.Models;
using QFinans.CustomFilters;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static QFinans.ApplicationSignInManager;

namespace QFinans.Controllers
{
    [Authorize]
    [Log]
    public class AppUserController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AppUserController()
        {
        }

        public AppUserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        [CustomAuth(Roles = "IndexAppUser")]
        // GET: AppUser
        public ActionResult Index(string currentFilter, string searchString, int? page, int? customPageSize)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            IQueryable<ApplicationUser> users = db.Users.Where(x => x.UserName != "admin@admin.com").Include(x => x.Roles).OrderByDescending(x => x.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(x => x.Email.Contains(searchString)
                                        || x.UserName.Contains(searchString)
                                        || x.Name.Contains(searchString)
                                        || x.SurName.Contains(searchString));

                if (users.Any() == false)
                {
                    TempData["warning"] = '"' + searchString + '"' + " ile ilgili sonuç bulunamadı.";
                }
            }

            if (customPageSize != null)
            {
                ViewBag.CustomPageSize = customPageSize;
            }
            else
            {
                ViewBag.CustomPageSize = 10;
            }
            int pageSize = (customPageSize ?? 10);
            int pageNumber = (page ?? 1);
            return View(users.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        [CustomAuth(Roles = "EditAppUser")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [CustomAuth(Roles = "EditAppUser")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, string name, string surName, bool isAdmin, bool paparaDashboard, bool havaleEftDashboard, bool isShowCashFlow, string[] roleName)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            try
            {
                if (roleName.Length > 0)
                {

                    if (user.Roles.Any())
                    {
                        var roleIds = user.Roles.Select(x => x.RoleId).ToArray();
                        var userRoles = db.Roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToArray();
                        UserManager.RemoveFromRoles(id, userRoles);
                    }
                    UserManager.AddToRoles(id, roleName);

                    user.Name = name;
                    user.SurName = surName;
                    user.IsAdmin = isAdmin;
                    user.PaparaDashboard = paparaDashboard;
                    user.HavaleEFtDashboard = havaleEftDashboard;
                    user.IsShowCashFlow = isShowCashFlow;
                    db.SaveChanges();
                    TempData["success"] = "Kullanıcı düzenlendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(user);
            }

            return View(user);
        }

        [CustomAuth(Roles = "DeleteAppUser")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [CustomAuth(Roles = "DeleteAppUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            DeletedUser deletedUser = new DeletedUser
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.Roles.Select(x => x.RoleId).FirstOrDefault(),
            };
            db.DeletedUser.Add(deletedUser);
            db.Users.Remove(user);
            db.SaveChanges();
            TempData["success"] = "Kullanıcı silindi.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}