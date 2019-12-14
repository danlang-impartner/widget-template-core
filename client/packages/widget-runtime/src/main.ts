import 'reflect-metadata';
import { IWidgetRuntime, WidgetRuntime } from './runtime';
import { IWindow } from './shared';

export * from './event-bus';
export * from './manager';
export * from './runtime';
export * from './service-manager';

export const widgetRuntime: IWidgetRuntime = (() => {
  const COM_IMPARTNER_WIDGET_RUNTIME = 'com.impartner.widget.runtime';

  if (!(window as IWindow)[COM_IMPARTNER_WIDGET_RUNTIME]) {
    (window as IWindow)[COM_IMPARTNER_WIDGET_RUNTIME] = new WidgetRuntime();
  }

  return (window as IWindow)[COM_IMPARTNER_WIDGET_RUNTIME];
})();
