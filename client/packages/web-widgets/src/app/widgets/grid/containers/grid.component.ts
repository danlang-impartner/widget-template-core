import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild
} from '@angular/core';
import { AgGridNg2 } from 'ag-grid-angular';
import { ColDef, GridOptions, IDatasource, IGetRowsParams } from 'ag-grid-community';
import { ColumnState } from 'ag-grid-community/dist/lib/columnController/columnController';

import { WidgetTag } from 'src/app/core';

@Component({
  selector: `w-impartner-${WidgetTag.Grid}`,
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.scss']
})
export class GridComponent implements AfterViewInit {
  public columnDefs: ColDef[];
  public rowData: object[];
  public gridOptions: GridOptions;

  @ViewChild(AgGridNg2)
  public agGridReference: AgGridNg2;

  @Output()
  public dataChanged = new EventEmitter<object[]>();

  private _datasource: IDatasource;
  private _editing: boolean;
  private _previousPageSize = 10;

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {
    this.columnDefs = [];
    this.rowData = [];
    this.gridOptions = {
      rowModelType: 'infinite',
      pagination: true,
      cacheBlockSize: 10,
      defaultColDef: {
        sortable: true,
        resizable: true,
        suppressMovable: true
      },
      overlayNoRowsTemplate: '<span>This is a custom \'no rows\' overlay</span>'
    };
  }

  public ngAfterViewInit(): void {
    this.editing = true;
  }

  @Input()
  public set metadata(value: string) {
    try {
      this.columnDefs = JSON.parse(value);
      this.agGridReference.api.setColumnDefs(this.columnDefs);
      this._updateColumnDefinitionsOrder();
      this._changeDetectorRef.detectChanges();
    } catch (error) {
      throw new Error(`There is an error in the parse of the JSON in metadata: ${error}`);
    }
  }

  public get metadata(): string {
    return JSON.stringify(this.columnDefs);
  }

  @Input()
  public set data(value: string) {
    if (!this._editing) {
      try {
        const dataChanged: object[] = JSON.parse(value);

        this.agGridReference.api.setRowData(dataChanged);
        this.dataChanged.emit(dataChanged);
        this._changeDetectorRef.detectChanges();
      } catch (error) {
        this.rowData = [];
        throw new Error(`Impossible to parse data: ${error}`);
      }
    }
  }

  @Input()
  public set datasource(datasource: IDatasource) {
    this._datasource = datasource;

    if (!this._editing) {
      this.agGridReference.api.purgeInfiniteCache();
      this.agGridReference.api.setDatasource(this._datasource);
      this.agGridReference.api.paginationSetPageSize(this._previousPageSize);
      this.agGridReference.api.setDomLayout('normal');
      this._changeDetectorRef.detectChanges();
    }
  }

  @Input()
  public set editing(value: boolean) {
    this._editing = value;

    if (this._editing) {
      this.agGridReference.api.setDatasource({
        rowCount: 0,
        getRows: (params: IGetRowsParams): void => {
          params.successCallback([]);
        }
      });

      this.agGridReference.api.setDomLayout('autoHeight');
      this.agGridReference.api.paginationSetPageSize(5);
    } else if (!this._editing && this._datasource) {
      this.datasource = this._datasource;
    }
  }

  public get editing(): boolean {
    return this._editing;
  }

  @Input()
  public set paginationSize(value: number) {
    this._previousPageSize = value;
    this.agGridReference.api.paginationSetPageSize(value);
    this.agGridReference.gridOptions.cacheBlockSize = value;
    this._changeDetectorRef.detectChanges();
  }

  public get paginationSize(): number {
    return this._previousPageSize;
  }

  private _updateColumnDefinitionsOrder(): void {
    const columnState = this.columnDefs.map<ColumnState>((columnDefinition: ColDef) => {
      return {
        colId: columnDefinition.colId,
        hide: columnDefinition.hide,
        width: columnDefinition.width
      } as ColumnState;
    });
    this.agGridReference.columnApi.setColumnState(columnState);
  }
}
