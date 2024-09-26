using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactAppMVCNhibernate.Models
{
    public class Contact
    {
        public virtual int ContactId { get; set; }
        public virtual string FName { get; set; }
        public virtual string LName { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual User User { get; set; }
        public virtual IList<ContactDetail> ContactDetails { get; set; } = new List<ContactDetail>();
    }
}