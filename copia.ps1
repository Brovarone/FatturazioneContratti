$src = "D:\Progetti\ALLMigrazione"
$dst = "D:\Progetti\FatturazioneContratti\EFMago\Data"

Copy-Item "$src\EFMago\Models\Context\FattureContext.cs" "$dst\FattureContext.cs"

$models = @(
"Allattivita","Allcespiti","AllcustSuppFox","Alldescrizioni","AllordCliAcc","AllordCliAttivita",
"AllordCliContratto","AllordCliContrattoDescFatt","AllordCliContrattoDistCesp","AllordCliContrattoDistinta",
"AllordCliContrattoDistintaServAgg","AllordCliDescrizioni","AllordCliFattEle","AllordCliTipologiaServizi",
"AllordFiglio","AllordPadre","AlltipoRigaServizio","ImCustSuppPolicies","ImSuppliersDiscounts","IntegraInterventi",
"MaAccountingDefaults","MaChartOfAccounts","MaChartOfAccountsBalances","MaChartOfAccountsLang","MaCreditCustomer",
"MaCreditCustomerDocument","MaCrossReferences","MaCrossReferencesNotes","MaCustSupp","MaCustSuppBalances",
"MaCustSuppBlackList","MaCustSuppBranches","MaCustSuppBudget","MaCustSuppCircularLetters","MaCustSuppCustomerOptions",
"MaCustSuppForm","MaCustSuppFormCategory","MaCustSuppNaturalPerson","MaCustSuppNotes","MaCustSuppOutstandings",
"MaCustSuppParameters","MaCustSuppPeople","MaCustSuppSupplierOptions","MaCustomerClassification","MaCustomerCtg",
"MaCustomerSpecification","MaDeclarationOfIntent","MaDeclarationOfIntentNumbers","MaEiItcustSuppAddData",
"MaEiItcustSuppAddDataDet","MaEiItdocAdditionalData","MaEiItfatelWebParameters","MaEiItitemCustomers",
"MaEiItparameters","MaEiparameters","MaEipaymentType","MaIdnumbers","MaItemNotes","MaItems","MaItemsComparableUoM",
"MaItemsFifo","MaItemsFifodomCurr","MaItemsFiscalData","MaItemsFiscalDataDomCurr","MaItemsGoodsData",
"MaItemsIntrastat","MaItemsKit","MaItemsLanguageDescri","MaItemsLifo","MaItemsLifodomCurr","MaItemsManufacturingData",
"MaItemsMonthlyBalances","MaItemsPriceLists","MaItemsPurchaseBarCode","MaItemsStorageQty",
"MaItemsStorageQtyMonthly","MaItemsSubstitute","MaItemsWmszones","MaNonFiscalNumbers","MaPaymentTerms",
"MaPaymentTermsDefaults","MaPaymentTermsLang","MaPaymentTermsPercInstall","MaPlafond","MaSaleDoc",
"MaSaleDocComponents","MaSaleDocDetail","MaSaleDocManufReasons","MaSaleDocNotes","MaSaleDocPymtSched",
"MaSaleDocReferences","MaSaleDocShipping","MaSaleDocSummary","MaSaleDocTaxSummary","MaSaleOrd",
"MaSaleOrdComponents","MaSaleOrdDetails","MaSaleOrdNotes","MaSaleOrdPymtSched","MaSaleOrdReferences",
"MaSaleOrdShipping","MaSaleOrdSummary","MaSaleOrdTaxSummary","MaSaleParameters","MaSalesDefaults",
"MaSalesOrdParameters","MaSalesOrdsDefaults","MaSddmandate","MaStandardCostHistorical","MaTaxCodes",
"MaTaxCodesDefaults","MaTaxCodesLang","MaTaxCodesLists","MaUserDefaultSaleOrders","MaUserDefaultSales",
"MaVcrossReferences"
)
New-Item -ItemType Directory -Force "$dst\Models" | Out-Null
foreach ($m in $models) {
    Copy-Item "$src\EFMago\Models\$m.cs" "$dst\Models\$m.cs"
}