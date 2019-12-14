import { IWidgetEvent } from './widget-event-interface';

// tslint:disable-next-line: no-any
export interface IListener<T extends IWidgetEvent = any> {
  eventName: string;
  callback: (event: T) => void;
}
