import { IWidgetEvent } from '../widget-event-interface';

export interface IShowConfigRequestedEvent<T = any> extends IWidgetEvent {
  type: string;
  configuration: T;
}
