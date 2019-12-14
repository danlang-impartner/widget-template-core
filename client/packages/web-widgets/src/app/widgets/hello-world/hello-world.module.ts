import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { HelloWorldComponent } from './containers/hello-world.component';

@NgModule({
  declarations: [HelloWorldComponent],
  imports: [CommonModule],
  entryComponents: [HelloWorldComponent]
})
export class HelloWorldModule {}
