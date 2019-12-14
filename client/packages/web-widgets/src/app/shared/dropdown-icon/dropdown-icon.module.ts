import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AngularSvgIconModule, SvgIconRegistryService } from 'angular-svg-icon';

import { DropdownIconComponent } from './container/dropdown-icon.component';

export * from './interfaces';

@NgModule({
  imports: [CommonModule, AngularSvgIconModule],
  declarations: [DropdownIconComponent],
  exports: [DropdownIconComponent]
})
export class DropdownIconModule {
  constructor(private readonly _iconRegistryService: SvgIconRegistryService) {
    this._iconRegistryService.addSvg('kebab', require('@impartner/svg-icons/kebab.svg'));
    this._iconRegistryService.addSvg('close', require('@impartner/svg-icons/close.svg'));
  }
}
