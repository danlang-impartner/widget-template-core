import { Component, Input, NgZone, OnInit, ViewEncapsulation } from '@angular/core';
import { Logger } from '@impartner/config-utils';
import {
  IDataChangedEvent,
  IWidgetEvent,
  IWidgetParams,
  widgetRuntime
} from '@impartner/widget-runtime';

import { WidgetTag } from 'src/app/core';

@Component({
  selector: `w-impartner-${WidgetTag.HelloCustomer}`,
  templateUrl: './hello-customer.component.html',
  styleUrls: ['./hello-customer.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom
})
export class HelloCustomerComponent implements OnInit, IWidgetParams {
  @Input()
  public readonly id: number;

  @Input()
  public readonly data: string;

  @Input()
  public message: string;

  @Input()
  public debug = false;

  private _customersList: string[];

  @Input()
  public set customers(value: string) {
    try {
      this._customersList = JSON.parse(value);
    } catch (error) {
      throw new Error('Error when customers list parsed');
    }
  }

  public get customers(): string {
    let customersStringJson = '';
    try {
      customersStringJson = JSON.stringify(this._customersList);
    } catch (error) {
      throw new Error('Error when customers list parsed');
    }

    return customersStringJson;
  }

  public get customersList(): string[] {
    return this._customersList;
  }

  public dataFromEditor: string;

  constructor(private readonly _zone: NgZone) {}

  public ngOnInit(): void {
    const eventIdToListen = 'w-impartner.hello-world.saidHelloEvent';
    const richTextEditorEventId = 'w-impartner.rich-text-editor.contentChanged';
    type MessageReceivedEventType = { message: string } & IWidgetEvent;
    this.dataFromEditor = '';

    widgetRuntime.eventBus.addEventListener<MessageReceivedEventType>(
      eventIdToListen,
      (event: MessageReceivedEventType) => {
        Logger.log('from hello customer, i received: %s', event);
        this._zone.run(() => (this.message = event.message));
      }
    );

    widgetRuntime.eventBus.addEventListener<IDataChangedEvent<string>>(
      richTextEditorEventId,
      event => {
        Logger.log('from Rich Text Editor, i received: %s', event);
        this._zone.run(() => (this.dataFromEditor = event.data));

        return true;
      }
    );
  }
}
