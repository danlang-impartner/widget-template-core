import { ComponentFactoryResolver, Injector, NgModuleFactoryLoader } from '@angular/core';
import { Logger } from '@impartner/config-utils';

import { MODULE_WIDGETS } from '../widgets';

export async function lazyLoadModule(
  modulePath: string,
  injector: Injector,
  loadedModules: string[]
): Promise<string[]> {
  const childLoadedModules: string[] = [...loadedModules];
  const factoryLoader: NgModuleFactoryLoader = injector.get(NgModuleFactoryLoader);
  const moduleName = modulePath.split('#').pop() as string;

  if (childLoadedModules.includes(moduleName)) {
    return [];
  }

  try {
    const moduleFactory = await factoryLoader.load(modulePath);
    const module = moduleFactory.create(injector);
    const widgetDependencies: undefined | string[] = module.instance.widgetDependencies;

    if (typeof widgetDependencies !== 'undefined') {
      for (const widgetDependency of widgetDependencies as string[]) {
        if (widgetDependency === moduleName) {
          continue;
        }
        const widgetFound = MODULE_WIDGETS.filter(widget => {
          const widgetModule = widget.path.split('#').pop() as string;

          return widgetModule === widgetDependency;
        });
        if (widgetFound.length > 0) {
          const widgetRegistryOfDependency = widgetFound.pop() as { path: string };
          const subchildLoadedModules = await lazyLoadModule(
            widgetRegistryOfDependency.path,
            injector,
            childLoadedModules
          );
          childLoadedModules.push(...subchildLoadedModules);
        }
      }
    }

    module.instance.ngDoBootstrap(injector.get(ComponentFactoryResolver));
    childLoadedModules.push(moduleName);
  } catch (error) {
    Logger.error('error when loading component %s: %o', moduleName, error);
  }

  return childLoadedModules;
}
