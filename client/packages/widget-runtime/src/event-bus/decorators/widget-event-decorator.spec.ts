import { widgetRuntime } from 'src/main';
import { WidgetEvent } from './widget-event-decorator';

describe('widget-event-decorator.ts', () => {
  let eventDecorator: PropertyDecorator;

  beforeEach(() => {
    eventDecorator = WidgetEvent<{ emit: (msg: string) => void }>('emit', 'eventIdTest');
  });

  describe('WidgetEvent()', () => {
    it('should emit a event when original property of widget that emits event send that signal', () => {
      spyOn(widgetRuntime.eventBus, 'emit');
      const objectTest = {
        propertyEventEmitter: {
          emit: (msg: string): void => {}
        }
      };

      eventDecorator(objectTest, 'propertyEventEmitter');
      objectTest.propertyEventEmitter.emit('test msg');

      expect(widgetRuntime.eventBus.emit).toHaveBeenCalledWith('eventIdTest', 'test msg');
    });
  });
});
