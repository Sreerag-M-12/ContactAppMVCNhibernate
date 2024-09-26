using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppMVCNhibernate.Models;
using FluentNHibernate.Mapping;

namespace ContactAppMVCNhibernate.Mappings
{
    public class UserMap:ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(u=>u.UserId).GeneratedBy.Identity();
            Map(u => u.FName);
            Map(u => u.LName);
            Map(u => u.Password);
            Map(u => u.IsActive);
            Map(u => u.IsAdmin);
            HasOne(u => u.Role).PropertyRef(u => u.User).Cascade.All().Constrained();
            HasMany(a => a.Contacts).Inverse().Cascade.All();
        }
    }
}