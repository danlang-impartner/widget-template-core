import { CommonModule } from '@angular/common';
import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { AgGridModule } from 'ag-grid-angular';

import { createLazyWebComponent } from 'src/app/core';
import { GridComponent } from './containers/grid.component';

@NgModule({
  declarations: [GridComponent],
  exports: [GridComponent],
  imports: [CommonModule, AgGridModule.withComponents([])],
  providers: [{ provide: 'LAZY_ENTRY_POINT', useValue: [GridComponent] }],
  entryComponents: [GridComponent]
})
export class GridModule implements DoBootstrap {
  constructor(private readonly _injector: Injector) {}

  public ngDoBootstrap(appRef: ApplicationRef): void {
    createLazyWebComponent(GridComponent, this._injector);
  }
}
