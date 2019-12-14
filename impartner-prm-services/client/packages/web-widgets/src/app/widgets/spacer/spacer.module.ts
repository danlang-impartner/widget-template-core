import { CommonModule } from '@angular/common';
import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { widgetRuntime } from '@impartner/widget-runtime';

import { createLazyWebComponent, IWidgetDependencies } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { SpacerSettingsPopoverComponent, SpacerViewComponent } from './components';
import { SpacerEditComponent } from './containers/spacer-edit.component';

@NgModule({
  imports: [CommonModule],
  declarations: [SpacerEditComponent, SpacerViewComponent, SpacerSettingsPopoverComponent],
  entryComponents: [SpacerEditComponent, SpacerViewComponent, SpacerSettingsPopoverComponent],
  providers: [{ provide: CORE_TOKENS.IEventBus, useValue: widgetRuntime.eventBus }]
})
export class SpacerModule implements DoBootstrap, IWidgetDependencies {
  public readonly widgetDependencies = [];

  constructor(private readonly _injector: Injector) {}

  public ngDoBootstrap(appRef: ApplicationRef): void {
    createLazyWebComponent(SpacerEditComponent, this._injector);
    createLazyWebComponent(SpacerViewComponent, this._injector);
    createLazyWebComponent(SpacerSettingsPopoverComponent, this._injector);
  }
}
