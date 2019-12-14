import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Inject,
  Input,
  Output,
  ViewChild
} from '@angular/core';
import { CKEditor5 } from '@ckeditor/ckeditor5-angular/ckeditor';
import { ChangeEvent } from '@ckeditor/ckeditor5-angular/ckeditor.component';
import {
  IDataChangedEvent,
  IWidgetParams,
  SystemEvents,
  WidgetEvent
} from '@impartner/widget-runtime';
import { TranslateService } from '@ngx-translate/core';

import { WidgetTag } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { environment } from 'src/environments/environment';

const WIDGET_EVENTS_PREFIX = `${environment.widgetPrefix}.${WidgetTag.RichTextEditorEdit}`;

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.RichTextEditorEdit}`,
  templateUrl: './rich-text-editor.component.html',
  styleUrls: ['./rich-text-editor.component.scss']
})
export class RichTextEditorComponent implements IWidgetParams {
  public editor: any;
  public editorInstance: CKEditor5.Editor;
  public content = '';
  public prerenderMode = true;

  @ViewChild('ckeditorTag')
  public ckEditorView: ElementRef;

  @Input()
  public debug = false;

  @Input()
  public readonly id: number;

  @Input()
  public set localeCode(value: string) {
    const defaultLang = value.split('-').shift();
    // fallback for portuguese
    this._editorConfig.language = defaultLang === 'pt' ? 'pt-br' : defaultLang;
    this._translateService.setDefaultLang(defaultLang || value);
  }

  @Output()
  @WidgetEvent<EventEmitter<IDataChangedEvent<string>>>('emit', SystemEvents.DataChanged)
  public contentChanged = new EventEmitter<IDataChangedEvent<string>>();

  @Output()
  @WidgetEvent<EventEmitter<void>>('emit', `${WIDGET_EVENTS_PREFIX}.editorVisible`)
  public editorVisible = new EventEmitter();

  private _editorConfig: CKEditor5.Config;

  constructor(
    private readonly _changeDetectorRef: ChangeDetectorRef,
    private readonly _translateService: TranslateService,
    @Inject(CORE_TOKENS.ClassicEditor) private readonly _ckEditor: any
  ) {
    this.editor = this._ckEditor;
    this._editorConfig = this._ckEditor.defaultConfig;
  }

  @Input()
  public set data(value: string) {
    this.content = value;
  }

  @Input()
  public set widgetConfig(value: string | CKEditor5.Config) {
    try {
      this._editorConfig = JSON.parse(value as string);
    } catch (error) {
      console.error('Error when try to parse the config options : %o', error);
    }
  }

  public get widgetConfig(): string | CKEditor5.Config {
    return this._editorConfig;
  }

  public get isContentEmpty(): boolean {
    return this.content === '';
  }

  public change({ editor }: ChangeEvent): void {
    let data = editor.getData();

    if (data === '<p>&nbsp;</p>') {
      data = '';
    }

    this.content = data;
    this.contentChanged.emit({
      data,
      widgetId: this.id
    });
  }

  public ready(editor: CKEditor5.Editor): void {
    this.editorInstance = editor;
    editor.setData(this.content);
    editor.editing.view.focus();
    this.editorVisible.emit();
  }

  public toggleEditMode(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.prerenderMode = !this.prerenderMode;
    this._changeDetectorRef.detectChanges();
  }

  @HostListener('document:click', ['$event'])
  public globalClick(event: Event): boolean {
    if (typeof this.ckEditorView !== 'undefined') {
      let isClickInsideRichEditorContainer = this.ckEditorView.nativeElement.contains(event.target);
      const ckeditorBalloon = window.document.querySelector('.ck-balloon-panel');

      if (ckeditorBalloon) {
        isClickInsideRichEditorContainer =
          isClickInsideRichEditorContainer || ckeditorBalloon.contains(event.target as Node);
      }

      if (!isClickInsideRichEditorContainer) {
        this.prerenderMode = true;
        this._changeDetectorRef.detectChanges();
      }
    }

    return true;
  }
}
