import { IWidgetEvent } from '../widget-event-interface';

// tslint:disable-next-line: no-any
export interface IDataChangedEvent<T = any> extends IWidgetEvent {
  data: T;
}
