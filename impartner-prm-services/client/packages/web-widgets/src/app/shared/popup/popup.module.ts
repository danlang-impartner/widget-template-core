import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NgxSmartModalModule } from 'ngx-smart-modal';

import { PopupComponent } from './container/popup.component';

@NgModule({
  imports: [CommonModule, NgxSmartModalModule.forRoot()],
  declarations: [PopupComponent],
  exports: [PopupComponent]
})
export class PopupModule {}
