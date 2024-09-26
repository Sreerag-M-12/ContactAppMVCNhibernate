using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactAppMVCNhibernate.Models
{
    public class ContactDetail
    {
        public virtual int ContactDetailId { get; set; }
        public virtual string Type { get; set; }
        public virtual string Email { get; set; }
        public virtual Contact Contact { get; set; }
    }
}