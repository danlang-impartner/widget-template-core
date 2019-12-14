import { widgetRuntime } from 'src/main';

function WidgetEvent<T>(emitMethodName: keyof T, eventName: string): PropertyDecorator {
  return (target: Object, propertyKey: string | symbol): any => {
    proxyEventEmitters(target, propertyKey);
  };

  function proxyEventEmitters(target: Object, propertyKey: string | symbol): void {
    let localValue: any;
    const previousPropertyDescriptor = Object.getOwnPropertyDescriptor(target, propertyKey);
    const newPropertyDescriptor: PropertyDescriptor = {
      enumerable:
        previousPropertyDescriptor === undefined ? true : previousPropertyDescriptor.enumerable,
      configurable:
        previousPropertyDescriptor === undefined ? true : previousPropertyDescriptor.configurable
    };

    type WidgetEventProxy = T & { [field: string]: any };

    const proxyHandler: ProxyHandler<WidgetEventProxy> = {
      get(targetHandler: WidgetEventProxy, propKey: PropertyKey, receiver: any): any {
        const origMethod = targetHandler[propKey as string];

        if (propKey === emitMethodName) {
          return function(this: any, ...args: any[]): void {
            const eventNameToEmit = eventName;

            if (typeof args !== 'undefined' && args.length > 0) {
              widgetRuntime.eventBus.emit(eventNameToEmit, args[0]);
            }

            return origMethod.apply(this, args);
          };
        }

        return origMethod;
      }
    };

    if (previousPropertyDescriptor && previousPropertyDescriptor.value) {
      const newVal = previousPropertyDescriptor.value;
      const proxyOfEmitMethod = new Proxy(newVal, proxyHandler);

      newPropertyDescriptor.value = proxyOfEmitMethod;
    } else {
      newPropertyDescriptor.set = function(this: any, newVal: any): any {
        const proxyOfEmitMethod = new Proxy(newVal, proxyHandler);

        if (previousPropertyDescriptor && previousPropertyDescriptor.set) {
          previousPropertyDescriptor.set(proxyOfEmitMethod);
        }

        localValue = proxyOfEmitMethod;
      };

      newPropertyDescriptor.get = function(this: any): any {
        if (previousPropertyDescriptor && previousPropertyDescriptor.get) {
          previousPropertyDescriptor.get();
        }

        return localValue;
      };
    }

    Object.defineProperty(target, propertyKey, newPropertyDescriptor);
  }
}

export { WidgetEvent };
