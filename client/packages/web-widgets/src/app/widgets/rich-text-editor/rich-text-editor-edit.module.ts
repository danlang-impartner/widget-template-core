import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ApplicationRef, DoBootstrap, Injector, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
// tslint:disable-next-line: ordered-imports
import * as ClassicEditor from '@impartner/ckeditor5-build-classic';
import '@impartner/ckeditor5-build-classic/build/translations/de';
import '@impartner/ckeditor5-build-classic/build/translations/es';
import '@impartner/ckeditor5-build-classic/build/translations/fr';
import '@impartner/ckeditor5-build-classic/build/translations/pt-br';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { SafePipeModule } from 'safe-pipe';

import { createLazyWebComponent, createTranslateLoader } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { RichTextEditorComponent } from './containers/rich-text-editor.component';

@NgModule({
  declarations: [RichTextEditorComponent],
  imports: [
    CommonModule,
    CKEditorModule,
    SafePipeModule,
    FormsModule,
    HttpClientModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient, CORE_TOKENS.CurrentWidgetType]
      }
    })
  ],
  entryComponents: [RichTextEditorComponent],
  providers: [
    { provide: CORE_TOKENS.ClassicEditor, useValue: ClassicEditor },
    { provide: CORE_TOKENS.CurrentWidgetType, useValue: 'rich-text-editor' }
  ],
  schemas: [NO_ERRORS_SCHEMA]
})
export class RichTextEditorEditModule implements DoBootstrap {
  constructor(
    private readonly _injector: Injector,
    private readonly _translateService: TranslateService
  ) {}

  public ngDoBootstrap(appRef: ApplicationRef): void {
    this._translateService.setDefaultLang('en');
    createLazyWebComponent(RichTextEditorComponent, this._injector);
  }
}
