import { IGetRowsParams } from 'ag-grid-community';

import { IRowFilter } from 'src/app/core';
import { PrmObject } from 'src/app/core/enums';
import { IQueryParams } from './interfaces';

export class Context {
  public requestUrl: string;
  public queryParams: IQueryParams = {
    fields: [],
    filter: '',
    orderBy: []
  };

  public get agGridParams(): IGetRowsParams {
    return this._agGridParams;
  }

  public get fieldsNameToShow(): string[] {
    return this._fieldsNameToShow;
  }

  public get prmObject(): string {
    return this._prmObject;
  }

  public get rowFilters(): IRowFilter[] {
    return this._rowFilters || [];
  }

  constructor(
    private _prmObject: PrmObject,
    private _agGridParams: IGetRowsParams,
    private _fieldsNameToShow: string[],
    private _rowFilters?: IRowFilter[]
  ) {}
}
