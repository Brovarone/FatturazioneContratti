using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppBranches
    {
        public int CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public string Branch { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string IsocountryCode { get; set; }
        public string TaxIdNumber { get; set; }
        public string FiscalCode { get; set; }
        public string Siacode { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Telex { get; set; }
        public string Fax { get; set; }
        public string ContactPerson { get; set; }
        public string Language { get; set; }
        public string Internet { get; set; }
        public string Email { get; set; }
        public string Disabled { get; set; }
        public string WorkingTime { get; set; }
        public string TaxOffice { get; set; }
        public int? MailSendingType { get; set; }
        public string Notes { get; set; }
        public string Salesperson { get; set; }
        public string AreaManager { get; set; }
        public string SkypeId { get; set; }
        public string Region { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address2 { get; set; }
        public string StreetNo { get; set; }
        public string District { get; set; }
        public string FederalState { get; set; }
        public string Ipacode { get; set; }
        public string AdministrationReference { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string FiscalName { get; set; }
        public string EishipTypeData { get; set; }
        public string EishipTextData { get; set; }
        public double? EishipNumberData { get; set; }

        public virtual MaCustSupp CustSuppNavigation { get; set; }
    }
}
