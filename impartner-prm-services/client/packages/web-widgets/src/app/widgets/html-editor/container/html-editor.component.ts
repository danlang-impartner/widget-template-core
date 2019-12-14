import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { Logger } from '@impartner/config-utils';
import {
  IDataChangedEvent,
  IWidgetParams,
  SystemEvents,
  WidgetEvent
} from '@impartner/widget-runtime';
import * as ace from 'ace-builds';
import 'ace-builds/src-noconflict/ext-beautify';
import 'ace-builds/src-noconflict/mode-html';
import 'ace-builds/src-noconflict/theme-chrome';

import { WidgetTag } from 'src/app/core';
import { environment } from 'src/environments/environment';
import { FORBIDDEN_TAGS } from '../config';

const THEME = 'ace/theme/chrome';
const LANG = 'ace/mode/html';

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.HtmlEditorEdit}`,
  templateUrl: './html-editor.component.html',
  styleUrls: ['./html-editor.component.scss']
})
export class HtmlEditorComponent implements OnInit, OnDestroy, IWidgetParams {
  public prerenderMode = true;

  @Input()
  public readonly id: number;

  @ViewChild('editor')
  public readonly editorElementRef: ElementRef;

  private _codeEditor: ace.Ace.Editor;
  private _editorBeautify: any;
  private _content = '';

  @Output()
  @WidgetEvent<EventEmitter<IDataChangedEvent<string>>>('emit', SystemEvents.DataChanged)
  public contentChanged = new EventEmitter<IDataChangedEvent<string>>();

  @Input()
  public set data(value: string) {
    this._content = value;
    if (this._codeEditor) {
      try {
        this._codeEditor.setValue(value);
      } catch (err) {
        Logger.warn('Error when setting new value in HTML Editor. Maybe the editor was deleted');
      }
    }
  }

  public get data(): string {
    if (this._codeEditor) {
      return this._codeEditor.getValue();
    }

    return this._content;
  }

  public get isContentEmpty(): boolean {
    return this._content === '';
  }

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {}

  public ngOnInit(): void {
    this._initHtmlCodeEditor();
  }

  public ngOnDestroy(): void {
    this._removeForbiddenTags();
    this._onChange();
  }

  public toggleEditMode(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.prerenderMode = !this.prerenderMode;
    this._changeDetectorRef.detectChanges();
    if (!this.prerenderMode) {
      this._initHtmlCodeEditor();
    }
  }

  @HostListener('document:click', ['$event'])
  public globalClick(event: Event): boolean {
    if (typeof this.editorElementRef !== 'undefined') {
      const isClickInsideRichEditorContainer = this.editorElementRef.nativeElement.contains(
        event.target
      );

      if (!isClickInsideRichEditorContainer) {
        this._removeForbiddenTags();
        this._onChange();
        this.prerenderMode = true;
        this._changeDetectorRef.detectChanges();
      }
    }

    return true;
  }

  private _initHtmlCodeEditor(): void {
    if (this.editorElementRef) {
      const element = this.editorElementRef.nativeElement;
      const editorOptions: Partial<ace.Ace.EditorOptions> = {
        highlightActiveLine: true,
        minLines: 10,
        maxLines: Infinity
      };

      ace.config.set('basePath', environment.deployUrl);
      this._editorBeautify = ace.require('ace/ext/beautify');
      this._codeEditor = this._configureEditor(element, editorOptions);
      this._beautifyContent();
      this._codeEditor.focus();
    }
  }

  private _configureEditor(
    element: HTMLDivElement,
    editorOptions: Partial<
      ace.Ace.EditorOptions & { enableBasicAutocompletion?: boolean | undefined }
    >
  ): ace.Ace.Editor {
    const editor = ace.edit(element, editorOptions);
    editor.setTheme(THEME);
    editor.getSession().setMode(LANG);
    editor.getSession().setUseWorker(false);
    editor.setShowFoldWidgets(true);
    editor.setValue(this._content);
    editor.on('change', (delta: ace.Ace.Delta) => this._onChange());

    return editor;
  }

  private _beautifyContent(): void {
    if (this._codeEditor && this._editorBeautify) {
      const session = this._codeEditor.getSession();

      this._editorBeautify.beautify(session);
    }
  }

  private _onChange(): void {
    this.contentChanged.emit({
      data: this.data,
      widgetId: this.id
    });
  }

  private _removeForbiddenTags(): void {
    let regexp = '';

    for (const forbiddenTag of FORBIDDEN_TAGS) {
      regexp += `|<${forbiddenTag}>`;
      regexp += `|<\/${forbiddenTag}>`;
    }

    regexp = `(${regexp.substring(1)})`;

    this.data = this.data.replace(new RegExp(regexp, 'g'), '');
  }
}
