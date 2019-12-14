import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SelectComponent } from './container/select.component';

@NgModule({
  imports: [CommonModule, FormsModule],
  declarations: [SelectComponent],
  exports: [SelectComponent]
})
export class SelectModule {}
