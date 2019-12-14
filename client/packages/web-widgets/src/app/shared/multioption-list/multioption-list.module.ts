import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AngularSvgIconModule, SvgIconRegistryService } from 'angular-svg-icon';

import { MultioptionListComponent } from './container';

@NgModule({
  imports: [CommonModule, AngularSvgIconModule],
  declarations: [MultioptionListComponent],
  exports: [MultioptionListComponent]
})
export class MultioptionListModule {
  constructor(private readonly _iconRegistry: SvgIconRegistryService) {
    this._iconRegistry.addSvg('close-circle', require('@impartner/svg-icons/close-circle.svg'));
  }
}
