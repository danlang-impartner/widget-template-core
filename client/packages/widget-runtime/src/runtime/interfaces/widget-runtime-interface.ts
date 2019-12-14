import { IEventBus } from 'src/event-bus';
import { WidgetManager } from 'src/manager';
import { ServiceManager } from 'src/service-manager';

export interface IWidgetRuntime {
  manager: WidgetManager;
  eventBus: IEventBus;
  services: ServiceManager;
}
