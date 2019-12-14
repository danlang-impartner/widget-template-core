import { IWidgetEvent } from '../widget-event-interface';

// tslint:disable-next-line: no-any
export interface IConfigChangedEvent<T = any> extends IWidgetEvent {
  configuration: T;
}
