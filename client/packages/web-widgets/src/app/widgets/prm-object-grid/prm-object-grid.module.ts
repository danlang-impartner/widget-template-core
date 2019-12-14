import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import {
  ApplicationRef,
  CUSTOM_ELEMENTS_SCHEMA,
  DoBootstrap,
  Injector,
  NgModule
} from '@angular/core';
import { widgetRuntime } from '@impartner/widget-runtime';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { SortablejsModule } from 'angular-sortablejs';
import { AngularSvgIconModule, SvgIconRegistryService } from 'angular-svg-icon';
import { CookieService } from 'ngx-cookie-service';

import { createLazyWebComponent, createTranslateLoader, IWidgetDependencies } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { MultioptionListModule, PopupModule } from 'src/app/shared';
import { HttpClientAdapter } from './adapters';
import { GridSettingsComponent, GridViewComponent } from './components';
import { PrmObjectGridComponent } from './containers/prm-object-grid.component';
import { httpClientFactory } from './factories';
import { EndpointLocatorService, PrmObjectService, RequestCreatorService } from './services';

@NgModule({
  declarations: [PrmObjectGridComponent, GridSettingsComponent, GridViewComponent],
  imports: [
    CommonModule,
    HttpClientModule,
    PopupModule,
    MultioptionListModule,
    AngularSvgIconModule,
    SortablejsModule.forRoot({ animation: 150 }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient, CORE_TOKENS.CurrentWidgetType]
      }
    })
  ],
  entryComponents: [PrmObjectGridComponent, GridSettingsComponent, GridViewComponent],
  providers: [
    PrmObjectService,
    RequestCreatorService,
    CookieService,
    HttpClientAdapter,
    EndpointLocatorService,
    { provide: CORE_TOKENS.IEventBus, useValue: widgetRuntime.eventBus },
    { provide: CORE_TOKENS.WidgetRuntime, useValue: widgetRuntime },
    { provide: CORE_TOKENS.CurrentWidgetType, useValue: 'prm-object-grid' },
    {
      provide: CORE_TOKENS.HttpClient,
      useFactory: httpClientFactory,
      deps: [HttpClientAdapter, CORE_TOKENS.WidgetRuntime]
    }
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class PrmObjectGridModule implements DoBootstrap, IWidgetDependencies {
  public readonly widgetDependencies = ['GridModule'];

  constructor(
    private readonly _injector: Injector,
    private readonly _iconRegistry: SvgIconRegistryService
  ) {
    this._iconRegistry.addSvg('add-square', require('@impartner/svg-icons/add-square.svg'));
  }

  public ngDoBootstrap(appRef: ApplicationRef): void {
    createLazyWebComponent(GridViewComponent, this._injector);
    createLazyWebComponent(PrmObjectGridComponent, this._injector);
    createLazyWebComponent(GridSettingsComponent, this._injector);
  }
}
