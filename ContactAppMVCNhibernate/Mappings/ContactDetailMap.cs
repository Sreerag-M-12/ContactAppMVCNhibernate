using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppMVCNhibernate.Models;
using FluentNHibernate.Mapping;

namespace ContactAppMVCNhibernate.Mappings
{
    public class ContactDetailMap:ClassMap<ContactDetail>
    {
        public ContactDetailMap()
        {
            Table("ContactDetails");
            Id(c => c.ContactDetailId);
            Map(c=>c.Type);
            Map(c => c.Email);
            References(c => c.Contact).Columns("contactId").Cascade.None();

        }
    }
}