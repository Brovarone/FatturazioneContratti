using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaEiItfatelWebParameters
    {
        public int ParameterId { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CompanyCode { get; set; }
        public string Xmlversion { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpiringDate { get; set; }
        public string ProgrCompanyCode { get; set; }
        public int? MaxNumFile { get; set; }
        public string ManageSignature { get; set; }
        public string PortalUrl { get; set; }
        public string Sosmanagement { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string CadiManualSign { get; set; }
        public string CadiSignature { get; set; }
        public int? Eisignature { get; set; }
        public string Eivalidation { get; set; }
        public string CadiValidation { get; set; }
        public string CadiExternalSign { get; set; }
        public string CadiDisabledSent { get; set; }
        public string CadiMasterDataSent { get; set; }
        public string IdCedente { get; set; }
        public string UrlCadi { get; set; }
        public string UserNameCadi { get; set; }
        public string PasswordCadi { get; set; }
        public string CompanyCodeCadi { get; set; }
        public DateTime? ActivationDateCadi { get; set; }
        public DateTime? ExpiringDateCadi { get; set; }
        public string PortalUrlCadi { get; set; }
        public string ProgrCompanyCodeCadi { get; set; }
        public string AsyncCommunication { get; set; }
        public string NoTramitazione { get; set; }
    }
}
