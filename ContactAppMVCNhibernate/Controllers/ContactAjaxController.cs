using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactAppMVCNhibernate.Data;
using ContactAppMVCNhibernate.Models;

namespace ContactAppMVCNhibernate.Controllers
{
    public class ContactAjaxController : Controller
    {
        // GET: ContactAjax
        public ActionResult Index(int userId)
        {
            Session["userid"] = userId;

            using (var session = NHibernateHelper.CreateSession())
            {
                return View();
            }
        }

        public ActionResult GetContactsForAdmin(int userId)
        {

            using (var session = NHibernateHelper.CreateSession())
            {
                var contacts = session.Query<Contact>().Where(c => c.User.UserId == userId).ToList();
                return View(contacts);
            }
        }

        public ActionResult GetAllContacts()
        {

            int userId = (int)Session["userId"];
            using (var session = NHibernateHelper.CreateSession())
            {
                var contacts = session.Query<Contact>()
                    .Where(c => c.User.UserId == userId)
                    .Select(c => new Contact
                    {
                        ContactId = c.ContactId,
                        FName = c.FName,
                        LName = c.LName,
                        IsActive = c.IsActive,
                    }).
                    ToList();
                return Json(contacts, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ActiveContacts()
        {
            var userId = Session["userId"];
            using (var session = NHibernateHelper.CreateSession())
            {
                var contacts = session.Query<Contact>()
                    .Where(c => c.User.UserId == (int)userId)
                    .Where(c => c.IsActive == true)
                    .Select(c => new Contact
                    {
                        ContactId = c.ContactId,
                        FName = c.FName,
                        LName = c.LName,
                        IsActive = c.IsActive,
                    }).
                    ToList();
                return PartialView("_ActiveContacts", contacts);
            }
        }


        [Authorize(Roles = "Staff")]
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Create(Contact contact)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var userId = Session["userid"];
                using (var txn = session.BeginTransaction())
                {
                    var user = session.Query<User>().FirstOrDefault(e => e.UserId == (int)userId);
                    contact.IsActive = true;
                    contact.User = user;
                    session.Save(contact);
                    txn.Commit();
                    //return PartialView("_ContactList",);
                  //  return RedirectToAction("Index", "ContactAjax", new { userId = Session["userid"] });
                   return Json(new {success=true});
                }
            }
        }

        [Authorize(Roles = "Staff")]
        public ActionResult Edit(int contactId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var contact = session.Get<Contact>(contactId);
                return PartialView("_Edit", contact);  
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Edit(Contact contact)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    var existingContact = session.Get<Contact>(contact.ContactId);

                    if (existingContact != null)
                    {
                        existingContact.FName = contact.FName;
                        existingContact.LName = contact.LName;
                        session.Update(existingContact);
                        txn.Commit();
                    }

                    return RedirectToAction("Index", "ContactAjax", new { userId = Session["userid"] });
                }
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult ToggleActiveStatus(int contactId, bool isActive)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    // Fetch the user based on userId
                    var contact = session.Query<Contact>().FirstOrDefault(u => u.ContactId == contactId);
                    if (contact != null)
                    {
                        contact.IsActive = isActive;

                        session.Update(contact);
                        txn.Commit();
                        return Json(new { success = true, message = isActive ? "User reactivated." : "User deactivated." });
                    }
                    return Json(new { success = false, message = "User not found." });
                }

            }
        }


    }
}