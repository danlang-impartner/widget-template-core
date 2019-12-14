import {Component, Injector} from '@angular/core';
import {createCustomElement} from '@angular/elements';
import {HelloWorldComponent} from './widgets/hello-world/containers/hello-world.component';
import {WidgetTag} from './widget-tag';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {

}
