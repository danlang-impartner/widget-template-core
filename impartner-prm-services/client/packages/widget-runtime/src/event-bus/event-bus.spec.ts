import { EventBus } from './event-bus';
import { IEventBus, IWidgetEvent } from './interfaces';

describe('event-bus.ts', () => {
  let eventBus: IEventBus;
  let eventTest: { testData: string } & IWidgetEvent;

  beforeEach(() => {
    eventBus = new EventBus();
    eventTest = {
      widgetId: 123,
      testData: 'testData'
    };
  });

  describe('addEventListener()', () => {
    it('should add new listener', () => {
      const listenerSpy = jasmine.createSpy('listenerSpy');

      eventBus.addEventListener('testevent', listenerSpy);
      eventBus.emit('testevent', eventTest);

      expect(listenerSpy).toHaveBeenCalledWith(eventTest);
    });
  });

  describe('removeEventListener()', () => {
    it('should remove an existing listener', () => {
      const listenerSpy = jasmine.createSpy('listenerSpy');
      const listener = eventBus.addEventListener('anotherTestEvent', listenerSpy);

      eventBus.removeEventListener(listener);
      eventBus.emit('anotherTestEvent', eventTest);

      expect(listenerSpy).not.toHaveBeenCalled();
    });
  });

  describe('emit()', () => {
    it('should emit event', () => {
      const listenerSpy = jasmine.createSpy('listenerSpy');
      eventBus.addEventListener('anotherTestEvent', listenerSpy);

      eventBus.emit('anotherTestEvent', eventTest);

      expect(listenerSpy).toHaveBeenCalledWith(eventTest);
    });
  });
});
