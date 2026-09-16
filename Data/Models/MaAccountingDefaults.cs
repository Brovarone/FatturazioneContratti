using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaAccountingDefaults
    {
        public string Customers { get; set; }
        public string Suppliers { get; set; }
        public string Cash { get; set; }
        public string ExchangeRateProfit { get; set; }
        public string ExchangeRateLoss { get; set; }
        public string CreditDiscount { get; set; }
        public string DebitDiscount { get; set; }
        public string WindfallGain { get; set; }
        public string WindfallLoss { get; set; }
        public int AccountingDefaultsId { get; set; }
        public string CashOrderAccTpl { get; set; }
        public string BillCollectionAccTpl { get; set; }
        public string OutstandingAccTpl { get; set; }
        public string VouchersAccTpl { get; set; }
        public string PaymentOrdersAccTpl { get; set; }
        public string DeferralsAccTpl { get; set; }
        public string AccrualsAccTpl { get; set; }
        public string ProfitLossClosingAccTpl { get; set; }
        public string FinStatClosingAccTpl { get; set; }
        public string FinStatOpeningAccTpl { get; set; }
        public string CollectionAccRsn { get; set; }
        public string PaymentAccRsn { get; set; }
        public string PosAllowancesAccRsn { get; set; }
        public string NegAllowancesAccRsn { get; set; }
        public string PositiveRoundingAccRsn { get; set; }
        public string NegativeRoundingAccRsn { get; set; }
        public string ExchangeRateProfitAccRsn { get; set; }
        public string ExchangeRateLossAccRsn { get; set; }
        public string TaxTransferAccRsn { get; set; }
        public string CashOrderAccRsn { get; set; }
        public string BillCollectionAccRsn { get; set; }
        public string OutstandingAccRsn { get; set; }
        public string DeferredChargesAccRsn { get; set; }
        public string DeferredIncomesAccRsn { get; set; }
        public string AccruedIncomesAccRsn { get; set; }
        public string AccruedChargesAccRsn { get; set; }
        public string ProfitLossClosingAccRsn { get; set; }
        public string FinStatClosingAccRsn { get; set; }
        public string FinStatOpeningAccRsn { get; set; }
        public string ApprovalAccTpl { get; set; }
        public string ApprovalAccRsn { get; set; }
        public string OutstandingReopeningAccRsn { get; set; }
        public string BillOfExchAccTpl { get; set; }
        public string DdaccTpl { get; set; }
        public string PaNaccTpl { get; set; }
        public string BillOfExchAccRsn { get; set; }
        public string DdaccRsn { get; set; }
        public string PaNaccRsn { get; set; }
        public string DeferralsTransferAccTpl { get; set; }
        public string AccrualTransferAccTpl { get; set; }
        public string DeferralsTransferAccRsn { get; set; }
        public string AccrualsTransferAccRsn { get; set; }
        public string TaxTransferAccTpl { get; set; }
        public string DecreaseRevenue { get; set; }
        public string CurrencyCash1 { get; set; }
        public string CashCurrency1 { get; set; }
        public string CurrencyCash2 { get; set; }
        public string CashCurrency2 { get; set; }
        public string WithholdingTaxCredit { get; set; }
        public string WtaxCreditTransferAccRsn { get; set; }
        public string CurrencyRevalAccTpl { get; set; }
        public string SummaryAccTpl { get; set; }
        public string SummaryAccRsn { get; set; }
        public string TransferAccTpl { get; set; }
        public string TransferAccRsn { get; set; }
        public string LossRounding { get; set; }
        public string ProfitRounding { get; set; }
        public string BlackListSalesGoods1 { get; set; }
        public string BlackListSalesGoods2 { get; set; }
        public string BlackListSalesGoods3 { get; set; }
        public string BlackListSalesGoods4 { get; set; }
        public string BlackListSalesGoods5 { get; set; }
        public string BlackListSalesGoods6 { get; set; }
        public string BlackListSalesServices1 { get; set; }
        public string BlackListSalesServices2 { get; set; }
        public string BlackListSalesServices3 { get; set; }
        public string BlackListSalesServices4 { get; set; }
        public string BlackListSalesServices5 { get; set; }
        public string BlackListSalesServices6 { get; set; }
        public string BlackListPurchasesGoods1 { get; set; }
        public string BlackListPurchasesGoods2 { get; set; }
        public string BlackListPurchasesGoods3 { get; set; }
        public string BlackListPurchasesGoods4 { get; set; }
        public string BlackListPurchasesGoods5 { get; set; }
        public string BlackListPurchasesGoods6 { get; set; }
        public string BlackListPurchasesServices1 { get; set; }
        public string BlackListPurchasesServices2 { get; set; }
        public string BlackListPurchasesServices3 { get; set; }
        public string BlackListPurchasesServices4 { get; set; }
        public string BlackListPurchasesServices5 { get; set; }
        public string BlackListPurchasesServices6 { get; set; }
        public string ClearingCustAccTpl { get; set; }
        public string ClearingSuppAccTpl { get; set; }
        public string Eucustomers { get; set; }
        public string ExtraEucustomers { get; set; }
        public string Eusuppliers { get; set; }
        public string ExtraEusuppliers { get; set; }
        public string VouchReopAccTpl { get; set; }
        public string VouchReopAccRsn { get; set; }
        public string RetailCollectionAccTpl { get; set; }
        public string RetailCollection { get; set; }
        public string InvoicesToBeIssuedAccTpl { get; set; }
        public string InvoicesToBeReceivedAccTpl { get; set; }
        public string CreditNotesToBeIssuedAccTpl { get; set; }
        public string CreditNotesToBeReceivAccTpl { get; set; }
        public string RevalExchangeRateProfit { get; set; }
        public string RevalExchangeRateLoss { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
