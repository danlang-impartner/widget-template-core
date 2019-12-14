import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import {
  IConfigChangedEvent,
  ImpartnerWidgetTypes,
  IShowConfigRequestedEvent,
  IWidgetParams,
  SystemEvents,
  WidgetEvent
} from '@impartner/widget-runtime';
import { TranslateService } from '@ngx-translate/core';
import { ColDef } from 'ag-grid-community';

import { WidgetTag } from 'src/app/core';
import { environment } from 'src/environments/environment';
import { DEFAULT_COLUMNS_BY_PRM_OBJECT, DEFAULT_CONFIG } from '../config';
import { IDataSourceDefinition, IGridColumnDef, IGridConfiguration } from '../interfaces';
import { PrmObjectService } from '../services/prm-object.service';

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.PrmObjectGridEdit}`,
  templateUrl: './prm-object-grid.component.html',
  styleUrls: ['./prm-object-grid.component.scss']
})
export class PrmObjectGridComponent implements IDataSourceDefinition, OnInit, IWidgetParams {
  @Output()
  @WidgetEvent<EventEmitter<IConfigChangedEvent<IGridConfiguration>>>(
    'emit',
    SystemEvents.ConfigChanged
  )
  public configurationChanged = new EventEmitter<IConfigChangedEvent<IGridConfiguration>>();

  @Output()
  @WidgetEvent<EventEmitter<IShowConfigRequestedEvent<IDataSourceDefinition>>>(
    'emit',
    SystemEvents.ShowConfigRequested
  )
  public showConfigPanelEvent = new EventEmitter<
    IShowConfigRequestedEvent<IDataSourceDefinition>
  >();

  @Input()
  public debug: boolean;

  @Input()
  public readonly id: number;

  @Input()
  public cmsPageId: number;

  private _gridConfiguration: IGridConfiguration;
  private _isEditModeChanged = false;

  @ViewChild('gridRef')
  private _gridWidget: ElementRef;

  private _columnDefinition: IGridColumnDef[] = [];

  constructor(
    private readonly _prmObjectService: PrmObjectService,
    private readonly _changeDetectorRef: ChangeDetectorRef,
    private readonly _translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    this._gridWidget.nativeElement.editing = true;
  }

  @Input()
  public set widgetConfig(value: string | IGridConfiguration) {
    try {
      const previousValue = { ...this._gridConfiguration };
      let currentValue: IGridConfiguration;

      if (typeof value !== 'string') {
        currentValue = value as IGridConfiguration;
      } else {
        currentValue = JSON.parse(value as string);
      }

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

      if (
        typeof previousValue !== 'undefined' &&
        JSON.stringify(previousValue) !== JSON.stringify(this._gridConfiguration)
      ) {
        this.configurationChanged.emit({
          widgetId: this.id,
          type: ImpartnerWidgetTypes.ImpartnerPrmObjectGrid,
          configuration: this._gridConfiguration
        });
      }

      this._updateColumnDefinition();
    } catch (error) {
      throw new Error(`Cannot parse the grid configuration: ${error}`);
    }
  }

  public get widgetConfig(): string | IGridConfiguration {
    return this._gridConfiguration;
  }

  @Input()
  public set localeCode(value: string) {
    this._translateService.setDefaultLang(value);
  }

  public set columnDefinition(value: IGridColumnDef[]) {
    this._columnDefinition = value;
  }

  public get columnDefinition(): IGridColumnDef[] {
    return this._columnDefinition;
  }

  @Input()
  public set editMode(enable: boolean) {
    if (enable) {
      this.setAsActiveGridForSettings();
    }
  }

  public get widgetId(): number {
    return this.id;
  }

  public setAsActiveGridForSettings(): void {
    this.showConfigPanelEvent.emit({
      widgetId: this.id,
      type: ImpartnerWidgetTypes.ImpartnerPrmObjectGrid,
      configuration: this
    });
    this._isEditModeChanged = true;
    this._changeDetectorRef.detectChanges();
  }

  private async _updateColumnDefinition(): Promise<void> {
    this._columnDefinition = await this._prmObjectService.getColumnDefinition(
      this._gridConfiguration
    );
    if (this._isEditModeChanged) {
      this.showConfigPanelEvent.emit({
        widgetId: this.id,
        type: ImpartnerWidgetTypes.ImpartnerPrmObjectGrid,
        configuration: this
      });
    }
    this._changeDetectorRef.detectChanges();
  }
}
