import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Logger } from '@impartner/config-utils';
import { widgetRuntime } from '@impartner/widget-runtime';

import { AppModule } from './app/app.module';
import { lazyLoadModule } from './app/core';
import { definePublicPath } from './app/core/functions';
import { WIDGETS } from './app/core/widgets';
import { environment } from './environments/environment';

__webpack_public_path__ = definePublicPath();

if (environment.production) {
  enableProdMode();
}
platformBrowserDynamic()
  .bootstrapModule(AppModule, {
    ngZone: 'noop'
  })
  .then(ngModuleRef => {
    for (const widgetRegistry of WIDGETS) {
      widgetRuntime.manager.addWidgetLoader({
        ...widgetRegistry,
        loadComponent: async (): Promise<void> => {
          await lazyLoadModule(widgetRegistry.path, ngModuleRef.injector, []);
        }
      });
    }
  })
  .catch(err => Logger.error(err));
