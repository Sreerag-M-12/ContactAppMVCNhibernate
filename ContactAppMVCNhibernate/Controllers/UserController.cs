using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using ContactAppMVCNhibernate.Data;
using ContactAppMVCNhibernate.Models;
using ContactAppMVCNhibernate.ViewModels;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace ContactAppMVCNhibernate.Controllers
{
    [Authorize]

    public class UserController : Controller
    {
        // GET: User
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Index()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var users = session.Query<User>().FetchMany(e => e.Contacts).Where(u=>u.IsAdmin==false).ToList();
                return View(users);
            }
        }

        public ActionResult GetAdmins()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var users = session.Query<User>().FetchMany(e => e.Contacts).Where(u => u.IsAdmin == true).ToList();
                return View(users);
            }
        }

        public ActionResult ActiveUsers()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var users = session.Query<User>().FetchMany(e => e.Contacts).Where(u => u.IsActive).ToList();
                return View(users);
            }
            
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(User user)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    user.IsActive= true;
                    if (user.IsAdmin == true)
                    {
                        user.Role.User = user;

                        user.Role.RoleName = "Admin";

                    }
                    else
                    {
                        user.Role.User = user;

                        user.Role.RoleName = "Staff";

                    }
                    session.Save(user);
                    txn.Commit();
                }
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int userId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var user = session.Query<User>().FirstOrDefault(e => e.UserId == userId);
                return View(user);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(User user)
        {

            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    session.Update(user);
                    txn.Commit();
                    return RedirectToAction("Index");
                }
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult ToggleActiveStatus(int userId, bool isActive)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    // Fetch the user based on userId
                    var user = session.Query<User>().FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        user.IsActive = isActive;

                        session.Update(user);
                        txn.Commit();
                        return Json(new { success = true, message = isActive ? "User reactivated." : "User deactivated." });
                    }
                    return Json(new { success = false, message = "User not found." });
                }

            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int userId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var user = session.Get<User>(userId);
                return View(user);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteProduct(int userId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<User>(userId);
                    session.Delete(user);
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
            }
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        public ActionResult Login(LoginVM loginVM)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    if (string.IsNullOrWhiteSpace(loginVM.UserName) || string.IsNullOrWhiteSpace(loginVM.Password))
                    {
                        ModelState.AddModelError("", "Please enter username and password.");
                        return View(loginVM);
                    }
                    var user = session.Query<User>().SingleOrDefault(u => u.FName == loginVM.UserName);
                    

                    if (user != null)
                    {
                        // Check if the user is inactive
                        if (!user.IsActive)
                        {
                            // Add an error message for inactive users
                            ModelState.AddModelError("", "Your account is inactive. Please contact the administrator.");
                            return View(loginVM);
                        }

                        if (BCrypt.Net.BCrypt.Verify(loginVM.Password, user.Password))
                        {
                            FormsAuthentication.SetAuthCookie(loginVM.UserName, true);

                            Session["UserId"] = user.UserId;

                            if (user.IsAdmin && user.IsActive)
                            {
                                return RedirectToAction("Index");
                            }
                            else if (!user.IsAdmin && user.IsActive)
                            {
                                return RedirectToAction("Index", "ContactAjax", new { userid = Session["UserId"] });
                            }
                        }
                    }

                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(loginVM);
                }
            }
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(User user)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    if (user.IsAdmin == true)
                    {
                        user.Role.User = user;

                        user.Role.RoleName = "Admin";

                    }
                    else
                    {
                        user.Role.User = user;

                        user.Role.RoleName = "Staff";

                    }
                    session.Save(user);
                    txn.Commit();
                    return RedirectToAction("Login");
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        

    }

}
