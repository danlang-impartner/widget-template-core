import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SafePipeModule } from 'safe-pipe';

import { HelloCustomerComponent } from './containers/hello-customer.component';

@NgModule({
  declarations: [HelloCustomerComponent],
  imports: [CommonModule, SafePipeModule],
  entryComponents: [HelloCustomerComponent]
})
export class HelloCustomerModule {}
