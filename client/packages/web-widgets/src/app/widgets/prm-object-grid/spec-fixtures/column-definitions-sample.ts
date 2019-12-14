import { ColDef } from 'ag-grid-community';

export const columnDefinitionsSample: ColDef[] = [
  { headerName: 'Accounts', field: 'accounts', filter: 'agTextColumnFilter', hide: false },
  { headerName: 'Close Date', field: 'closeDate', filter: 'agDateColumnFilter', hide: false },
  { headerName: 'Company', field: 'company.name', filter: 'agTextColumnFilter', hide: false },
  { headerName: 'Created', field: 'created', filter: 'agDateColumnFilter', hide: true },
  { headerName: 'Created By', field: 'createdBy.name', filter: 'agTextColumnFilter', hide: true }
];

export const columnDefinitionOfDeal: ColDef[] = [
  { headerName: 'Name', field: 'name', filter: 'agTextColumnFilter' },
  { headerName: 'Company', field: 'company.name', filter: 'agTextColumnFilter' },
  { headerName: 'Source', field: 'source', filter: 'agTextColumnFilter' },
  { headerName: 'Amount', field: 'amount', filter: 'agNumberColumnFilter' },
  { headerName: 'Probability', field: 'probability', filter: 'agNumberColumnFilter' },
  { headerName: 'Stage', field: 'stage.name', filter: 'agTextColumnFilter' },
  { headerName: 'Probability', field: 'closeDate', filter: 'agDateColumnFilter' }
];
