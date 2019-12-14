import { CommonModule } from '@angular/common';
import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { SafePipeModule } from 'safe-pipe';

import { createLazyWebComponent, WidgetTag } from 'src/app/core';
import { environment } from 'src/environments/environment';
import { RichTextEditorViewComponent } from './container/rich-text-editor-view.component';

@NgModule({
  imports: [CommonModule, SafePipeModule],
  entryComponents: [RichTextEditorViewComponent],
  declarations: [RichTextEditorViewComponent]
})
export class RichTextEditorViewModule implements DoBootstrap {
  constructor(private readonly _injector: Injector) {}

  public ngDoBootstrap(appRef: ApplicationRef): void {
    createLazyWebComponent(RichTextEditorViewComponent, this._injector);
    createLazyWebComponent(
      RichTextEditorViewComponent,
      this._injector,
      `${environment.widgetPrefix}-${WidgetTag.HtmlEditorView}`
    );
  }
}
