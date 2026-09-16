using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppNaturalPerson
    {
        public int CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string CityOfBirth { get; set; }
        public string CountyOfBirth { get; set; }
        public string Professional { get; set; }
        public string FeeTpl { get; set; }
        public string Inpsaccount { get; set; }
        public string Form770Letter { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string ForfeitRegime { get; set; }

        public virtual MaCustSupp CustSuppNavigation { get; set; }
    }
}
