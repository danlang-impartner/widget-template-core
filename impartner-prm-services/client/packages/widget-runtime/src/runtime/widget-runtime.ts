import { IEventBus } from '../event-bus';
import { EventBus } from '../event-bus';
import { WidgetManager } from '../manager';
import { ServiceManager } from '../service-manager';
import { IWidgetRuntime } from './interfaces';

export class WidgetRuntime implements IWidgetRuntime {
  public readonly manager: WidgetManager = new WidgetManager();
  public readonly eventBus: IEventBus = new EventBus();
  public readonly services: ServiceManager = new ServiceManager();
}
