import { CommonModule } from '@angular/common';
import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { SafePipeModule } from 'safe-pipe';

import { createLazyWebComponent, IWidgetDependencies } from 'src/app/core';
import { HtmlEditorComponent } from './container/html-editor.component';

@NgModule({
  imports: [CommonModule, SafePipeModule],
  declarations: [HtmlEditorComponent],
  entryComponents: [HtmlEditorComponent]
})
export class HtmlEditorModule implements DoBootstrap, IWidgetDependencies {
  public readonly widgetDependencies = [];

  constructor(private readonly _injector: Injector) {}

  public ngDoBootstrap(appRef: ApplicationRef): void {
    createLazyWebComponent(HtmlEditorComponent, this._injector);
  }
}
