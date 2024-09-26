using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactAppMVCNhibernate.Data;
using ContactAppMVCNhibernate.Models;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace ContactAppMVCNhibernate.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index(int userId)
        {
            Session["userid"] = userId;
            using (var session = NHibernateHelper.CreateSession())
            {

                var contacts = session.Query<Contact>().Where(c => c.User.UserId == userId).ToList();
                return View(contacts);
            }
        }

        public ActionResult ActiveContacts()
        {
            var userId = Session["userid"];


            using (var session = NHibernateHelper.CreateSession())
            {
                var contacts = session.Query<Contact>().Where(c => c.User.UserId == (int)userId).Where(c=>c.IsActive==true).ToList();
                return View(contacts);
            }

        }

        [Authorize(Roles = "Staff")]
        public ActionResult Create()
        {
            return View();
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

                    contact.User = user;
                    session.Save(contact);
                    txn.Commit();
                    return RedirectToAction("Index", "Contact", new { userId = Session["userid"] });

                }
            }
        }

        [Authorize(Roles = "Staff")]

        public ActionResult Edit(int contactId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var userId = Session["userid"];
                var targetContact = session.Get<Contact>(contactId);
                return View(targetContact);
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public ActionResult Edit(Contact contact)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var userId = Session["userid"];

                using (var txn = session.BeginTransaction())
                {
                    var existingContact = session.Get<Contact>(contact.ContactId);
                    existingContact.FName = contact.FName;
                    existingContact.LName = contact.LName;

                    session.Update(existingContact);
                    txn.Commit();
                    return RedirectToAction("Index", "Contact", new { userId = Session["userid"] });
                }

            }

        }
        [Authorize(Roles = "Staff")]
        public ActionResult Delete(int contactId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var userId = Session["userid"];
                var user = session.Query<User>().FirstOrDefault(e => e.UserId == (int)userId);
                var targetConatact = user.Contacts.FirstOrDefault(o => o.ContactId == contactId);
                return View(targetConatact);
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteBook(int contactId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var userId = Session["userid"];

                using (var txn = session.BeginTransaction())
                {
                    var targetContact = session.Get<Contact>(contactId);
                    targetContact.IsActive = false;
                    session.Update(targetContact);
                    txn.Commit();
                    return RedirectToAction("Index", "Contact", new { userId = Session["userid"] });
                }
            }
        }



    }
}