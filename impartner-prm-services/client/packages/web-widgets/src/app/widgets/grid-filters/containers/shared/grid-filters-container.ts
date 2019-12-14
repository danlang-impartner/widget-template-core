import { ChangeDetectorRef, Input, OnInit, ViewChild } from '@angular/core';
import { Logger } from '@impartner/config-utils';
import { TranslateService } from '@ngx-translate/core';

import { ShowTabsComponent } from '../../components';
import { DEFAULT_TABS_CONFIG } from '../../constants';
import { IGridFiltersConfig, ITabConfig } from '../../interfaces';

export abstract class GridFiltersContainer implements OnInit {
  public gridFiltersConfig: IGridFiltersConfig = DEFAULT_TABS_CONFIG;

  public set selectedTab(value: ITabConfig) {
    this._selectedTab = value;

    if (this._changeDetectorRef) {
      this._changeDetectorRef.detectChanges();
    }
  }

  public get selectedTab(): ITabConfig {
    return this._selectedTab;
  }

  private _selectedTab: ITabConfig;

  @Input()
  public id: number;

  @Input()
  set widgetConfig(config: IGridFiltersConfig | string) {
    try {
      if (typeof config === 'string') {
        this.gridFiltersConfig = JSON.parse(config);
      } else {
        this.gridFiltersConfig = config;
      }

      this.gridFiltersConfig =
        this.gridFiltersConfig && this.gridFiltersConfig.tabs
          ? this.gridFiltersConfig
          : DEFAULT_TABS_CONFIG;

      this.selectedTab = this.gridFiltersConfig.tabs[0];
      this._changeDetectorRef.detectChanges();
    } catch (error) {
      Logger.error("I couldn't load config from GridFiltersComponent: %o", error);
    }
  }

  @Input()
  public set localeCode(value: string) {
    this._translateService.setDefaultLang(value);
  }

  public get localeCode(): string {
    return this._translateService.getDefaultLang();
  }

  constructor(
    protected readonly _changeDetectorRef: ChangeDetectorRef,
    protected readonly _translateService: TranslateService
  ) {}

  public ngOnInit(): void {}

  public onTabSelected(tabConfig: ITabConfig): void {
    this.selectedTab = tabConfig;
  }
}
