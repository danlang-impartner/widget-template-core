import { ChangeDetectorRef, EventEmitter, Input, Output } from '@angular/core';

import { ITabConfig } from '../interfaces';

export abstract class TabsInteraction {
  private _tabs: ITabConfig[] = [];

  @Output()
  public readonly tabSelected = new EventEmitter<ITabConfig>();

  @Input()
  public selectedTabIndex = 1;

  @Input()
  set tabs(value: ITabConfig[]) {
    this._tabs = value;

    if (this._changeDetectorRef) {
      this._changeDetectorRef.detectChanges();
    }

    if (value.length > 0) {
      this.selectedTabIndex = value[0].id;
    }
  }

  get tabs(): ITabConfig[] {
    return this._tabs;
  }

  constructor(protected readonly _changeDetectorRef: ChangeDetectorRef) {}

  public onTabSelected(tab: ITabConfig): void {
    this.tabSelected.emit(tab);
    this.selectedTabIndex = tab.id;
    this._changeDetectorRef.detectChanges();
  }
}
