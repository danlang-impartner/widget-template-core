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
import { TranslateService } from '@ngx-translate/core';

import { CORE_TOKENS } from 'src/app/core/constants';
import { GridFiltersEvents } from '../../enums';
import { IGridFiltersConfig, ITabConfig, IUpdatedGridFiltersConfig } from '../../interfaces';
import { GridFiltersContainer } from '../shared';

@Component({
  selector: 'w-impartner-grid-filters-edit',
  templateUrl: './grid-filters-edit.component.html',
  styleUrls: ['./grid-filters-edit.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GridFiltersEditComponent extends GridFiltersContainer implements OnDestroy {
  @Output()
  @WidgetEvent<EventEmitter<IShowConfigRequestedEvent>>('emit', SystemEvents.ShowConfigRequested)
  public readonly showConfigEvent = new EventEmitter<
    IShowConfigRequestedEvent<IUpdatedGridFiltersConfig>
  >();

  @Output()
  @WidgetEvent<EventEmitter<IConfigChangedEvent>>('emit', SystemEvents.ConfigChanged)
  public readonly configurationChanged = new EventEmitter<
    IConfigChangedEvent<IGridFiltersConfig>
  >();

  public setEditModePrmGrid = false;

  private readonly _busListeners: IListener[] = [];

  constructor(
    @Inject(CORE_TOKENS.IEventBus) private readonly _eventBus: IEventBus,
    _changeDetectorRef: ChangeDetectorRef,
    _translateService: TranslateService
  ) {
    super(_changeDetectorRef, _translateService);

    this._busListeners.push(
      this._eventBus.addEventListener<IConfigChangedEvent>(SystemEvents.ConfigChanged, event =>
        this._updateGridConfiguration(event)
      ),
      this._eventBus.addEventListener<IConfigChangedEvent>(
        GridFiltersEvents.TabsConfigUpdated,
        event => this._updateTabsConfig(event)
      )
    );
  }

  public ngOnDestroy(): void {
    this._busListeners.forEach(listener => this._eventBus.removeEventListener(listener));
  }

  public onTabSelected(tabConfig: ITabConfig): void {
    this.selectedTab = tabConfig;
    this.setAsActiveGridForSettings();
  }

  public setAsActiveGridForSettings(): void {
    this.showConfigEvent.emit({
      widgetId: this.id,
      type: ImpartnerWidgetTypes.ImpartnerGridFilters,
      configuration: {
        selectedTab: this.selectedTab,
        tabs: this.gridFiltersConfig.tabs,
        widgetId: this.id
      }
    });
    this.setEditModePrmGrid = true;
    this._changeDetectorRef.detectChanges();
  }

  private _updateGridConfiguration(event: IConfigChangedEvent): void {
    if (
      event.type === ImpartnerWidgetTypes.ImpartnerPrmObjectGrid &&
      event.widgetId === this.selectedTab.id
    ) {
      if (JSON.stringify(this.selectedTab.gridConfig) !== JSON.stringify(event.configuration)) {
        this.selectedTab.gridConfig = event.configuration;
        this._changeDetectorRef.detectChanges();

        this.configurationChanged.emit({
          widgetId: this.id,
          type: ImpartnerWidgetTypes.ImpartnerGridFilters,
          configuration: this.gridFiltersConfig
        });
      }
    }
  }

  private _updateTabsConfig(event: IConfigChangedEvent<IUpdatedGridFiltersConfig>): void {
    if (event.type === ImpartnerWidgetTypes.ImpartnerGridFilters && event.widgetId === this.id) {
      if (event.configuration.selectedTab) {
        this.selectedTab = event.configuration.selectedTab;

        this.showConfigEvent.emit({
          widgetId: this.id,
          type: ImpartnerWidgetTypes.ImpartnerGridFilters,
          configuration: {
            selectedTab: this.selectedTab,
            tabs: this.gridFiltersConfig.tabs,
            widgetId: this.id
          }
        });
      }

      if (event.configuration.tabs) {
        this.gridFiltersConfig.tabs = event.configuration.tabs;
        this.configurationChanged.emit({
          widgetId: this.id,
          type: ImpartnerWidgetTypes.ImpartnerGridFilters,
          configuration: this.gridFiltersConfig
        });
      }

      this._changeDetectorRef.detectChanges();
    }
  }
}
