import { IRowFilter } from 'src/app/core';
import { PrmObject } from 'src/app/core/enums';

export interface IGridConfiguration {
  id?: string;
  resultsPerPage: number[];
  columnsToShow: string[];
  businessObjectName: string | PrmObject;
  rowFilters?: IRowFilter[];
}
