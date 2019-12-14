import { Injectable } from '@angular/core';
import { ColDef, IDatasource, IGetRowsParams } from 'ag-grid-community';

import { PrmObject } from 'src/app/core/enums';
import {
  IColumnDefinitionCache,
  IFieldDefinition,
  IGridColumnDef,
  IGridConfiguration
} from '../interfaces';
import { RequestCreatorService } from './request-creator.service.ts';

@Injectable()
export class PrmObjectService {
  public _paginationPageSize: number;
  private _columnDefinitionCache: IColumnDefinitionCache[] = [];

  constructor(private readonly _requestService: RequestCreatorService) {
    this._paginationPageSize = 15;
  }

  public async getColumnDefinition(
    gridConfiguration: IGridConfiguration
  ): Promise<IGridColumnDef[]> {
    const columnDefinition = this._columnDefinitionCache.find(
      columnDefinitionCache =>
        columnDefinitionCache.prmObject === gridConfiguration.businessObjectName
    );

    if (columnDefinition) {
      let cacheColumnDef = [...columnDefinition.columnDefinition];

      cacheColumnDef = cacheColumnDef.map(currentColumnDef => {
        const transforemdColumnDefinition: ColDef = { ...currentColumnDef, hide: true };

        if (currentColumnDef.colId) {
          transforemdColumnDefinition.hide = !gridConfiguration.columnsToShow.includes(
            currentColumnDef.colId
          );
        }

        return transforemdColumnDefinition;
      });

      return this._sortByFieldsShowedFirst(cacheColumnDef, gridConfiguration);
    }

    const fieldDefinitionList = await this._requestService.getPrmObjectMetadata(
      gridConfiguration.businessObjectName
    );

    let columnDef = fieldDefinitionList.map(fieldDefinition => {
      return {
        colId: this._convertToFieldName(fieldDefinition),
        headerName: fieldDefinition.display,
        field: this._convertToFieldName(fieldDefinition),
        filter: this._convertToAgGridFieldType(fieldDefinition.fieldType),
        hide: this._detectFieldShouldBeHide(fieldDefinition, gridConfiguration.columnsToShow),
        fieldDataType: fieldDefinition.filterCriteriaType
      } as IGridColumnDef;
    });
    columnDef = this._sortByFieldsShowedFirst(columnDef, gridConfiguration);

    this._columnDefinitionCache.push({
      prmObject: gridConfiguration.businessObjectName,
      columnDefinition: columnDef.map(colDef => {
        const colDefReset = { ...colDef };
        colDefReset.hide = true;

        return colDefReset;
      })
    });

    return columnDef;
  }

  private _sortByFieldsShowedFirst(
    columnDefinitions: ColDef[],
    gridConfiguration: IGridConfiguration
  ): ColDef[] {
    const columnDefinitionsToShow = gridConfiguration.columnsToShow.map(colId => {
      const columnDefinitionFound = columnDefinitions.filter(
        columnDefinition => columnDefinition.field === colId
      );

      return columnDefinitionFound[0];
    });

    const columnDefinitionsToHide = columnDefinitions.filter(
      columnDefinition => columnDefinition.hide
    );

    return [...columnDefinitionsToShow, ...columnDefinitionsToHide];
  }

  private _detectFieldShouldBeHide(
    fieldDefinition: IFieldDefinition,
    columnsToShow: string[]
  ): boolean {
    return !columnsToShow.includes(this._convertToFieldName(fieldDefinition));
  }

  private _convertToAgGridFieldType(fieldType: string): string {
    let filterType = 'agTextColumnFilter';
    switch (fieldType) {
      case 'DateTime': {
        filterType = 'agDateColumnFilter';
        break;
      }
      case 'Integer': {
        filterType = 'agNumberColumnFilter';
        break;
      }
      default: {
        filterType = 'agTextColumnFilter';
        break;
      }
    }

    return filterType;
  }

  private _convertToFieldName(fieldDefinition: IFieldDefinition): string {
    let fieldName = fieldDefinition.name.charAt(0).toLowerCase() + fieldDefinition.name.slice(1);

    if (fieldDefinition.fieldType === 'Fk') {
      fieldName = `${fieldName}.name`;
    }

    return fieldName;
  }

  public getDatasource(gridConfiguration: IGridConfiguration): IDatasource {
    const datasourceDefinition: IDatasource = {
      getRows: (params: IGetRowsParams): void => {
        this.getColumnDefinition(gridConfiguration).then(columnDefinitions => {
          const columnDefinitionsToShow = columnDefinitions.filter(
            columnDefinition => !columnDefinition.hide
          );

          const dataObs$ = this._requestService.getGridData(
            gridConfiguration.businessObjectName as PrmObject,
            params,
            columnDefinitionsToShow,
            gridConfiguration.rowFilters
          );

          dataObs$.then(
            (data: any) => {
              params.successCallback(data.results, data.count as number);
              datasourceDefinition.rowCount = data.count as number;
            },
            e => params.failCallback()
          );
        });
      }
    };

    return datasourceDefinition;
  }
}
