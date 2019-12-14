import { Component, Input } from '@angular/core';
import { IWidgetParams } from '@impartner/widget-runtime';

import { WidgetTag } from 'src/app/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.RichTextEditorView}`,
  templateUrl: './rich-text-editor-view.component.html',
  styleUrls: ['./rich-text-editor-view.component.scss']
})
export class RichTextEditorViewComponent implements IWidgetParams {
  private _data = '';

  @Input()
  public readonly id: number;

  constructor() {}

  @Input()
  public set data(value: string) {
    if (!value) {
      value = '';
    }

    this._data = value;
  }

  public get data(): string {
    return this._data;
  }
}
