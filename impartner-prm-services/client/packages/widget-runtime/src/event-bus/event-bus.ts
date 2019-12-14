import { IEventBus, IListener, IWidgetEvent } from './interfaces';

export class EventBus implements IEventBus {
  private _eventListeners: { [eventName: string]: Array<IListener<any>> } = {};

  public addEventListener<T extends IWidgetEvent>(
    eventName: string,
    callback: (event: T) => void
  ): IListener<T> {
    const listener: IListener<T> = {
      eventName,
      callback
    };

    if (!(eventName in this._eventListeners)) {
      this._eventListeners[eventName] = [];
    }

    this._eventListeners[eventName].push(listener);

    return listener;
  }

  public removeEventListener<T extends IWidgetEvent = any>(listener: IListener<T>): void {
    if (listener.eventName in this._eventListeners) {
      this._eventListeners[listener.eventName] = this._eventListeners[listener.eventName].filter(
        currentListener => currentListener.callback !== listener.callback
      );
    }
  }

  public emit<T extends IWidgetEvent>(eventName: string, event: T): void {
    if (eventName in this._eventListeners) {
      this._eventListeners[eventName].forEach(eventListener => eventListener.callback(event));
    }
  }
}
