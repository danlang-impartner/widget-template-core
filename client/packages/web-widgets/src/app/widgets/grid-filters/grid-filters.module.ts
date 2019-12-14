import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import {
  ApplicationRef,
  CUSTOM_ELEMENTS_SCHEMA,
  DoBootstrap,
  Injector,
  NgModule
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { widgetRuntime } from '@impartner/widget-runtime';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { AngularSvgIconModule, SvgIconRegistryService } from 'angular-svg-icon';
import { PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarModule } from 'ngx-perfect-scrollbar';

import { createLazyWebComponent, createTranslateLoader } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { DropdownIconModule, SelectModule } from 'src/app/shared';
import {
  DefineDatasourceComponent,
  EditableTabComponent,
  FilterRowsComponent,
  ShowTabsComponent,
  TabsSettingsComponent
} from './components';
import { DEFAULT_PERFECT_SCROLLBAR_CONFIG } from './constants';
import {
  GridFiltersEditComponent,
  GridFiltersSettingsComponent,
  GridFiltersViewComponent
} from './containers';
import { FilterService } from './services';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SelectModule,
    DropdownIconModule,
    HttpClientModule,
    AngularSvgIconModule,
    PerfectScrollbarModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient, CORE_TOKENS.CurrentWidgetType]
      }
    })
  ],
  declarations: [
    GridFiltersEditComponent,
    GridFiltersViewComponent,
    ShowTabsComponent,
    GridFiltersSettingsComponent,
    DefineDatasourceComponent,
    FilterRowsComponent,
    TabsSettingsComponent,
    EditableTabComponent
  ],
  providers: [
    { provide: CORE_TOKENS.IEventBus, useValue: widgetRuntime.eventBus },
    { provide: CORE_TOKENS.CurrentWidgetType, useValue: 'grid-filters' },
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    },
    FilterService
  ],
  entryComponents: [
    GridFiltersEditComponent,
    GridFiltersViewComponent,
    GridFiltersSettingsComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class GridFiltersModule implements DoBootstrap {
  public readonly widgetDependencies = ['PrmObjectGridModule'];

  constructor(
    private readonly _injector: Injector,
    private readonly _iconRegistry: SvgIconRegistryService
  ) {
    this._iconRegistry.addSvg('gear', require('@impartner/svg-icons/gear.svg'));
    this._iconRegistry.addSvg('add-square', require('@impartner/svg-icons/add-square.svg'));
  }

  public ngDoBootstrap(appRef: ApplicationRef): void {
    createLazyWebComponent(GridFiltersEditComponent, this._injector);
    createLazyWebComponent(GridFiltersViewComponent, this._injector);
    createLazyWebComponent(GridFiltersSettingsComponent, this._injector);
  }
}
