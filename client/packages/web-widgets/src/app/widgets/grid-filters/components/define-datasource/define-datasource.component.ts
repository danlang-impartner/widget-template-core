import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { PrmObject } from 'src/app/core/enums';
import { SelectListOption } from 'src/app/shared/select/interfaces';
import { FilterTab } from '../../enums';

@Component({
  selector: 'w-impartner-define-datasource',
  templateUrl: './define-datasource.component.html',
  styleUrls: ['./define-datasource.component.scss']
})
export class DefineDatasourceComponent {
  @Input()
  public businessObjectName: PrmObject;

  @Output()
  public readonly businessObjectChangeEvent = new EventEmitter<string>();

  @Output()
  public readonly tabSelectedEvent = new EventEmitter<FilterTab>();

  public FilterTab = FilterTab;

  public currentSelectedTab = FilterTab.Datasource;

  public readonly prmObjectList: SelectListOption<string>[] = (Object.values(
    PrmObject
  ) as string[]).map<SelectListOption<string>>(prmObjectName => {
    return { label: prmObjectName, value: prmObjectName };
  });

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {}

  public businessObjectChanged(event: SelectListOption<string>): void {
    if (event.value) {
      this.businessObjectName = event.value as PrmObject;
      this.businessObjectChangeEvent.emit(event.value);
    }
  }

  public markTabAsSelected(tab: FilterTab): void {
    this.currentSelectedTab = tab;
    this._changeDetectorRef.detectChanges();
    this.tabSelectedEvent.emit(tab);
  }
}
