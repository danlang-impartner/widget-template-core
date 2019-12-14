import { APP_BASE_HREF } from '@angular/common';
import {
  ApplicationRef,
  DoBootstrap,
  Injector,
  NgModule
} from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { environment } from '../environments/environment.prod';
import { AppComponent } from './app.component';
import { appRoutes } from './app.routes';
import { AccountsComponent } from './containers/accounts/accounts.component';
import { JourneysComponent } from './containers/journeys/journeys.component';

@NgModule({
  declarations: [AppComponent, AccountsComponent, JourneysComponent],
  entryComponents: [AppComponent],
  imports: [RouterModule.forRoot(appRoutes, { enableTracing: true }), BrowserModule],
  providers: [{ provide: APP_BASE_HREF, useValue: '/en/s/journey-builder' }]
})
export class AppModule implements DoBootstrap {
  constructor(private readonly _injector: Injector) {
    const rootWebComponent = createCustomElement(AppComponent, { injector: _injector });
    customElements.define('f-journey-builder', rootWebComponent);
  }

  public ngDoBootstrap(appRef: ApplicationRef): void {
    if (!environment.production) {
      appRef.bootstrap(AppComponent);
    }
  }
}
