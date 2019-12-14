export const DEFAULT_COLUMNS_BY_PRM_OBJECT: { [key: string]: string[] } = {
  Deal: ['name', 'company.name', 'source', 'amount', 'probability', 'stage.name', 'closeDate'],
  Opportunity: [
    'name',
    'endUserCompany',
    'dollarValue',
    'dateOfLikelyClose',
    'approvalStatus',
    'dateApprovedDenied',
    'opportunityStatus'
  ],
  Sale: ['name', 'invoiceDate', 'shipToAccount.name', 'billToAccount.name', 'createdBy.name']
};
