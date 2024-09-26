using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactAppMVCNhibernate.Data;
using ContactAppMVCNhibernate.Models;

namespace ContactAppMVCNhibernate.Controllers
{
    [Authorize]
    public class ContactDetailController : Controller
    {
        // GET: ContactDetail
        public ActionResult Index(int contactId)
        {
            Session["contactid"] = contactId;

            return View();
        }

        //public ActionResult GetData(int page, int rows, string sidx, string sord, string searchString)
        //{
        //    var contactId = Session["contactid"]; // Ensure contactId is in session
        //    using (var session = NHibernateHelper.CreateSession())
        //    {
        //        var contactDetails = session.Query<ContactDetail>()
        //            .Where(c => c.Contact.ContactId == (int)contactId) // Filter by ContactId
        //            .ToList();

        //        int totalCount = contactDetails.Count();
        //        int totalPages = (int)Math.Ceiling((double)totalCount / rows);

        //        var jsonData = new
        //        {
        //            total = totalPages,  // Total number of pages
        //            page = page,         // Current page
        //            records = totalCount, // Total number of records
        //            rows = (from detail in contactDetails
        //                    orderby sidx + " " + sord
        //                    select new
        //                    {
        //                        id = detail.ContactDetailId, 
        //                        cell = new string[] {
        //                            detail.ContactDetailId.ToString(),
        //                            detail.Type,  
        //                            detail.Email
        //                        }
        //                    }).Skip((page - 1) * rows).Take(rows).ToArray()
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult GetData(int page, int rows, string sidx, string sord, bool _search, string searchField, string searchString, string searchOper)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var contactId = Session["contactId"]; // Ensure contactId is in session

                var detailList = session.Query<ContactDetail>()
                    .Where(c => c.Contact.ContactId == (int)contactId) // Filter by contactId
                    .ToList();

                if (_search && searchField == "Email" && searchOper == "eq")
                {
                    detailList = detailList.Where(p => p.Email == searchString).ToList();
                }

                int totalCount = detailList.Count();
                int totalPages = (int)Math.Ceiling((double)totalCount / rows);

                switch (sidx)
                {
                    case "Email":
                        detailList = sord == "asc" ? detailList.OrderBy(p => p.Email).ToList()
                            : detailList.OrderByDescending(p => p.Email).ToList();
                        break;

                    case "Type":
                        detailList = sord == "asc" ? detailList.OrderBy(p => p.Type).ToList()
                            : detailList.OrderByDescending(p => p.Type).ToList();
                        break;
                }

                var jsonData = new
                {
                    total = totalPages,
                    page = page,
                    records = totalCount,
                    rows = detailList.Select(detail => new
                    {
                        cell = new string[]
                        {
                    detail.ContactDetailId.ToString(),
                    detail.Email,
                    detail.Type
                        }
                    }).Skip((page - 1) * rows).Take(rows).ToArray()
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Staff")]

        public ActionResult Add(ContactDetail contactDetail)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var contactId = Session["contactid"];
                using (var txn = session.BeginTransaction())
                {
                    var contact = session.Query<Contact>().FirstOrDefault(e => e.ContactId == (int)contactId);

                    contactDetail.Contact = contact;
                    session.Save(contactDetail);
                    txn.Commit();

                    //var id = products.Max(i => i.Id);
                    //product.Id = id + 1;
                    //products.Add(product);
                    return Json(new { success = true, message = "Added New Detail" });
                }
            }

        }
        //[Authorize(Roles = "Staff")]

        //public ActionResult Delete(int detailId)
        //{
        //    using (var session = NHibernateHelper.CreateSession())
        //    {
        //        var contactId = Session["contactid"];
        //        using (var txn = session.BeginTransaction())
        //        {

        //            var targetDetail = session.Get<ContactDetail>(detailId);
        //            session.Delete(targetDetail);
        //            txn.Commit();
        //            return Json(new { success = true, message = "Product deeleted successfuffy" });

        //        }
        //    }

        //}
        [Authorize(Roles = "Staff")]
        public ActionResult Delete(int id)
        {
            using (var session = NHibernateHelper.CreateSession())

            {
                using (var transaction = session.BeginTransaction())
                {
                    var contactDetail = session.Query<ContactDetail>().FirstOrDefault(cd => cd.ContactDetailId == id);

                    session.Delete(contactDetail);

                    transaction.Commit();

                    return Json(new { success = true, message = "Contact Detail Deleted Successfully" });

                }

            }

        }

        [Authorize(Roles = "Staff")]
        public ActionResult Edit(ContactDetail detail)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    var existingContactDetail = session.Get<ContactDetail>(detail.ContactDetailId);
                    if (existingContactDetail != null)
                    {
                        existingContactDetail.Type = detail.Type;
                        existingContactDetail.Email = detail.Email;

                        session.Update(existingContactDetail);
                        txn.Commit();
                        return Json(new { success = true, message = "Detail edited successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Detail not found" });
                    }
                }
            }
        }
    }

}

    
        
        
    