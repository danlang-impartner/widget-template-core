import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Inject,
  OnDestroy,
  Output
} from '@angular/core';
import {
  IConfigChangedEvent,
  IEventBus,
  IListener,
  ImpartnerWidgetTypes,
  IShowConfigRequestedEvent,
  SystemEvents,
  WidgetEvent
} from '@impartner/widget-runtime';

import { IRowFilter } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { FilterTab, GridFiltersEvents } from '../../enums';
import { IPrmGridComponent, ITabConfig, IUpdatedGridFiltersConfig } from '../../interfaces';

const DEFAULT_FILTER_CONFIGURATION: IRowFilter = {
  id: 0,
  booleanOperator: 'and',
  fact: '',
  operator: 'equals',
  value: ['']
};

@Component({
  selector: 'w-impartner-grid-filters-settings',
  templateUrl: './grid-filters-settings.component.html',
  styleUrls: ['./grid-filters-settings.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GridFiltersSettingsComponent implements OnDestroy {
  public tabConfig: ITabConfig;
  public tabs: ITabConfig[];
  public widgetId: number;
  public currentTabSelected = FilterTab.Datasource;
  public readonly FilterTab = FilterTab;
  public prmObjectGridInstance: IPrmGridComponent;

  @Output()
  @WidgetEvent('emit', GridFiltersEvents.TabsConfigUpdated)
  public readonly tabsConfigUpdated = new EventEmitter<
    IConfigChangedEvent<IUpdatedGridFiltersConfig>
  >();

  private readonly _eventListeners: IListener[] = [];
  private _rowFilters: IRowFilter[] = [];

  public get rowFilters(): IRowFilter[] {
    if (this._rowFilters.length === 0) {
      this._rowFilters.push({
        ...DEFAULT_FILTER_CONFIGURATION,
        value: [''],
        fact: this.prmObjectGridInstance.columnDefinition[0].colId
      });
    }

    return this._rowFilters;
  }

  constructor(
    @Inject(CORE_TOKENS.IEventBus) private readonly _eventBus: IEventBus,
    private readonly _changeDetectorRef: ChangeDetectorRef
  ) {
    this._eventListeners.push(
      this._eventBus.addEventListener<IShowConfigRequestedEvent<IUpdatedGridFiltersConfig>>(
        SystemEvents.ShowConfigRequested,
        event => this._updateTabConfig(event)
      ),
      this._eventBus.addEventListener<IShowConfigRequestedEvent<IPrmGridComponent>>(
        SystemEvents.ShowConfigRequested,
        event => this._updateGridInstance(event)
      )
    );
  }

  public ngOnDestroy(): void {
    this._eventListeners.forEach(listener => this._eventBus.removeEventListener(listener));
  }

  public onBusinessObjectChanged(newBusinessObject: string): void {
    const modifiedWidgetConfig = {
      ...this.prmObjectGridInstance.widgetConfig,
      businessObjectName: newBusinessObject,
      columnsToShow: []
    };

    this.prmObjectGridInstance.widgetConfig = modifiedWidgetConfig;
    this._changeDetectorRef.detectChanges();
  }

  public onTabChanged(tabSelected: FilterTab): void {
    this.currentTabSelected = tabSelected;
    this._changeDetectorRef.detectChanges();
  }

  public updateFilters(filter: IRowFilter): void {
    const modifiedWidgetConfig = { ...this.prmObjectGridInstance.widgetConfig };
    const rowFilters: IRowFilter[] = modifiedWidgetConfig.rowFilters
      ? [...modifiedWidgetConfig.rowFilters]
      : [];

    const indexFilter = rowFilters.findIndex(currentFilter => currentFilter.id === filter.id);

    if (indexFilter !== -1 && rowFilters[indexFilter]) {
      rowFilters.splice(indexFilter, 1, filter);
    } else {
      rowFilters.push(filter);
    }

    modifiedWidgetConfig.rowFilters = rowFilters;
    this._rowFilters = rowFilters;
    ((this.prmObjectGridInstance.widgetConfig as unknown) as string) = JSON.stringify(
      modifiedWidgetConfig
    );

    this.tabConfig.gridConfig = modifiedWidgetConfig;

    this.udpateTabsConfig(this.tabs);
  }

  public addFilter(): void {
    const modifiedWidgetConfig = { ...this.prmObjectGridInstance.widgetConfig };
    const rowFilters: IRowFilter[] = modifiedWidgetConfig.rowFilters
      ? [...modifiedWidgetConfig.rowFilters]
      : [];

    if (rowFilters.length === 0) {
      rowFilters.push({
        ...DEFAULT_FILTER_CONFIGURATION,
        fact: this.prmObjectGridInstance.columnDefinition[0].colId
      });
    }

    const filterNextId = rowFilters[rowFilters.length - 1].id + 1;
    rowFilters.push({
      ...DEFAULT_FILTER_CONFIGURATION,
      id: filterNextId,
      fact: this.prmObjectGridInstance.columnDefinition[0].colId,
      value: ['']
    });

    modifiedWidgetConfig.rowFilters = rowFilters;
    this._rowFilters = rowFilters;
    ((this.prmObjectGridInstance.widgetConfig as unknown) as string) = JSON.stringify(
      modifiedWidgetConfig
    );
    this._changeDetectorRef.detectChanges();
  }

  public udpateTabsConfig(newTabsConfig: ITabConfig[]): void {
    this.tabs = newTabsConfig;
    this._changeDetectorRef.detectChanges();
    this.tabsConfigUpdated.emit({
      type: ImpartnerWidgetTypes.ImpartnerGridFilters,
      widgetId: this.widgetId,
      configuration: {
        widgetId: this.widgetId,
        tabs: newTabsConfig
      }
    });
  }

  public updateSelectedTab(tab: ITabConfig): void {
    this.tabsConfigUpdated.emit({
      type: ImpartnerWidgetTypes.ImpartnerGridFilters,
      widgetId: this.widgetId,
      configuration: {
        widgetId: this.widgetId,
        selectedTab: tab
      }
    });
  }

  private _updateGridInstance(event: IShowConfigRequestedEvent<IPrmGridComponent>): void {
    if (event.type === ImpartnerWidgetTypes.ImpartnerPrmObjectGrid) {
      this.prmObjectGridInstance = event.configuration;
      if (this.prmObjectGridInstance.widgetConfig.rowFilters) {
        this._rowFilters = JSON.parse(
          JSON.stringify(this.prmObjectGridInstance.widgetConfig.rowFilters)
        );
      }
      this._changeDetectorRef.detectChanges();
    }
  }

  private _updateTabConfig(event: IShowConfigRequestedEvent<IUpdatedGridFiltersConfig>): void {
    if (event.type === ImpartnerWidgetTypes.ImpartnerGridFilters) {
      this.tabConfig = event.configuration.selectedTab || this.tabConfig;
      this.tabs = event.configuration.tabs || this.tabs;
      this.widgetId = event.configuration.widgetId;
      this._rowFilters.splice(0, this._rowFilters.length);
      this._changeDetectorRef.detectChanges();
    }
  }
}
