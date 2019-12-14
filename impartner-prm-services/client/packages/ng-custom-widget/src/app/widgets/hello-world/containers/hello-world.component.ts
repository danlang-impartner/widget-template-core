import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { IWidgetParams, WidgetEvent } from '@impartner/widget-runtime';

import { environment } from 'src/environments/environment';
import { WidgetTag } from 'src/app/widget-tag';

const WIDGET_EVENTS_PREFIX = `${environment.vendor}.custom-hello-world-view`; // TODO: Refine

@Component({
  selector: WidgetTag.HelloWorldView,
  templateUrl: './hello-world.component.html',
  styleUrls: ['./hello-world.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class HelloWorldComponent implements IWidgetParams {
  @Input()
  public message = '';

  @Input()
  public readonly id: number;

  @Input()
  public readonly data: string;

  @Output()
  @WidgetEvent<EventEmitter<string>>('emit', `${WIDGET_EVENTS_PREFIX}.saidHelloEvent`)
  public saidHelloEvent = new EventEmitter<string>();

  public sayHello(): void {
    const message = 'Hello from widget!';
    this.saidHelloEvent.emit(message);
  }
}
