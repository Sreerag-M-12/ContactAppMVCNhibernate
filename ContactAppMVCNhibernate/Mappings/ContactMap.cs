using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppMVCNhibernate.Models;
using FluentNHibernate.Mapping;

namespace ContactAppMVCNhibernate.Mappings
{
    public class ContactMap:ClassMap<Contact>
    {
        public ContactMap()
        {
            Table("Contacts");
            Id(c=>c.ContactId).GeneratedBy.Identity();
            Map(c => c.FName);
            Map(c => c.LName);
            Map(c => c.IsActive);
            References(c=>c.User).Columns("userId").Cascade.None();
            HasMany(a => a.ContactDetails).Inverse().Cascade.All();

        }
    }
}