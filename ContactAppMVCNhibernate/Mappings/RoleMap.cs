using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppMVCNhibernate.Models;
using FluentNHibernate.Mapping;

namespace ContactAppMVCNhibernate.Mappings
{
    public class RoleMap:ClassMap<Role>
    {
        public RoleMap() {
            Table("Roles");
            Id(r => r.Id).GeneratedBy.Identity();
            Map(r => r.RoleName);
            References(r => r.User).Column("UserId").Cascade.None().Unique();
        }
    }
}