import { ChangeDetectorRef, Component, ElementRef, Input, ViewChild } from '@angular/core';
import { IWidgetParams } from '@impartner/widget-runtime';
import { ColDef } from 'ag-grid-community';

import { WidgetTag } from 'src/app/core';
import { environment } from 'src/environments/environment';
import { DEFAULT_COLUMNS_BY_PRM_OBJECT, DEFAULT_CONFIG } from '../../config';
import { IGridConfiguration } from '../../interfaces';
import { PrmObjectService } from '../../services';

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.PrmObjectGridView}`,
  templateUrl: './grid-view.component.html',
  styleUrls: ['./grid-view.component.scss']
})
export class GridViewComponent implements IWidgetParams {
  @Input()
  public readonly id: number;

  @ViewChild('gridRef')
  private _gridWidget: ElementRef;

  private _gridConfiguration: IGridConfiguration;
  private _columnDefinition: ColDef[] = [];

  constructor(
    private readonly _prmObjectService: PrmObjectService,
    private readonly _changeDetectorRef: ChangeDetectorRef
  ) {}

  @Input()
  public set widgetConfig(value: string) {
    try {
      const currentValue = JSON.parse(value as string) as IGridConfiguration;

      this._gridConfiguration = currentValue;

      if (!this._gridConfiguration || !this._gridConfiguration.businessObjectName) {
        this._gridConfiguration = DEFAULT_CONFIG;
      }

      if (!this._gridConfiguration.resultsPerPage) {
        this._gridConfiguration.resultsPerPage = DEFAULT_CONFIG.resultsPerPage;
      }

      if (
        typeof this._gridConfiguration.columnsToShow === 'undefined' ||
        this._gridConfiguration.columnsToShow.length === 0
      ) {
        this._gridConfiguration.columnsToShow =
          DEFAULT_COLUMNS_BY_PRM_OBJECT[this._gridConfiguration.businessObjectName];
      }

      if (typeof this._gridConfiguration.rowFilters === 'undefined') {
        this._gridConfiguration.rowFilters = [];
      }

      this._updateGridWidget();
      this._changeDetectorRef.detectChanges();
    } catch (error) {
      throw new Error(`Cannot parse the grid configuration: ${error}`);
    }
  }

  public get widgetConfig(): string {
    return JSON.stringify(this._gridConfiguration);
  }

  public get columnDefinition(): ColDef[] {
    return this._columnDefinition;
  }

  private async _updateGridWidget(): Promise<void> {
    this._columnDefinition = await this._prmObjectService.getColumnDefinition(
      this._gridConfiguration
    );

    if (typeof this._gridWidget !== 'undefined') {
      const gridWidgetElement = this._gridWidget.nativeElement;
      gridWidgetElement.editing = false;
      gridWidgetElement.datasource = this._prmObjectService.getDatasource(this._gridConfiguration);
      gridWidgetElement.paginationSize = this._gridConfiguration.resultsPerPage[0];
    }

    this._changeDetectorRef.detectChanges();
  }
}
