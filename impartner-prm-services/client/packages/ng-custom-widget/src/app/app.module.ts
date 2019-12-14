import {BrowserModule} from '@angular/platform-browser';
import {ApplicationRef, CUSTOM_ELEMENTS_SCHEMA, DoBootstrap, Injector, NgModule} from '@angular/core';

import {AppComponent} from './app.component';
import {environment} from '../environments/environment.prod';
import {HelloWorldComponent} from './widgets/hello-world/containers/hello-world.component';
import {createCustomElement} from '@angular/elements';
import {WidgetTag} from './widget-tag';

@NgModule({
  declarations: [
    AppComponent,
    HelloWorldComponent
  ],
  entryComponents: [AppComponent, HelloWorldComponent],
  imports: [
    BrowserModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule implements DoBootstrap {
  constructor(private readonly _injector: Injector) {
    const helloWorldElement = createCustomElement(HelloWorldComponent, { injector: _injector });
    customElements.define(WidgetTag.HelloWorldView, helloWorldElement);
  }

  public ngDoBootstrap(appRef: ApplicationRef): void {
    if (!environment.production) {
      appRef.bootstrap(AppComponent);
    }
  }
}
