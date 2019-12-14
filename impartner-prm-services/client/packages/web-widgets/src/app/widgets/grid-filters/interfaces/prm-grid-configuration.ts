import { IRowFilter } from 'src/app/core';

export interface IPrmGridConfiguration {
  businessObjectName: string;
  columnsToShow: string[];
  rowFilters: IRowFilter[];
}
