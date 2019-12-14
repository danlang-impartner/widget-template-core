import { IListener } from './listener-interface';
import { IWidgetEvent } from './widget-event-interface';

export interface IEventBus {
  addEventListener<T extends IWidgetEvent>(
    eventName: string,
    callback: (event: T) => void
  ): IListener<T>;

  removeEventListener<T extends IWidgetEvent = any>(listener: IListener<T>): void;

  emit<T extends IWidgetEvent>(eventName: string, event: T): void;
}
