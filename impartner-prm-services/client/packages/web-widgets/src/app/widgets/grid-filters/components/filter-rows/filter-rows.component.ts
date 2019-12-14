import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';

import { IRowFilter } from 'src/app/core';
import { IDropdownOption } from 'src/app/shared';
import { OPERATORS } from '../../constants';
import { IFieldDefinition, IFieldOperator } from '../../interfaces';
import { FilterService } from '../../services';

export const DEFAULT_OPERATOR: IFieldOperator = OPERATORS[0];

@Component({
  selector: 'w-impartner-filter-rows',
  templateUrl: './filter-rows.component.html',
  styleUrls: ['./filter-rows.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FilterRowsComponent implements OnInit {
  @Input()
  public title = '';

  @Input()
  public set rowFilter(value: IRowFilter) {
    this._rowFilter = value;
    this.ngOnInit();
  }
  public get rowFilter(): IRowFilter {
    return this._rowFilter;
  }

  @Input()
  public fieldDefinitions: IFieldDefinition[] = [];

  @Output()
  public readonly filterChanged = new EventEmitter<IRowFilter>();

  public operators: IFieldOperator[] = [DEFAULT_OPERATOR];
  public selectedField: IFieldDefinition | undefined;
  public selectedOperator = DEFAULT_OPERATOR;

  public get defaultOperatorDropdown(): IDropdownOption {
    return {
      iconName: this.selectedOperator.symbol,
      icon: this.selectedOperator.icon,
      label: this.selectedOperator.name,
      value: this.selectedOperator.symbol
    };
  }

  public get operatorsDropdownOptions(): IDropdownOption[] {
    return this.operators.map(operator => {
      return {
        iconName: operator.symbol,
        icon: operator.icon,
        value: operator.symbol,
        label: operator.name
      };
    });
  }

  private _rowFilter: IRowFilter;

  constructor(
    private readonly _filterService: FilterService,
    private readonly _changeDetectorRef: ChangeDetectorRef
  ) {}

  public ngOnInit(): void {
    if (typeof this.selectedField === 'undefined' && this.fieldDefinitions) {
      this.selectedField = this.fieldDefinitions.find(
        field => field.colId === this._rowFilter.fact
      );

      if (this.selectedField) {
        this.operators = this._filterService.getValidOperators(this.selectedField);
        this.selectedOperator =
          this.operators.find(operator => operator.symbol === this._rowFilter.operator) ||
          DEFAULT_OPERATOR;
      }
    }
  }

  public fieldChanged(event: Event): void {
    const colId = (event.target as HTMLSelectElement).value;

    if (colId) {
      this.selectedField = this.fieldDefinitions.find(field => field.colId === colId);

      if (this.selectedField) {
        this.operators = this._filterService.getValidOperators(this.selectedField);
        this._rowFilter.fact = this.selectedField.colId;
        this.filterChanged.emit(this.rowFilter);
        this._changeDetectorRef.detectChanges();
      }
    }
  }

  public operatorChanged(option: IDropdownOption): void {
    if (option && option.value) {
      this.selectedOperator = this.operators.find(
        operator => operator.symbol === option.value
      ) as IFieldOperator;
      this._rowFilter.operator = this.selectedOperator.symbol;
      this.filterChanged.emit(this.rowFilter);
      this._changeDetectorRef.detectChanges();
    }
  }

  public filterValueChanged(event: Event, valueIndex: number): void {
    const newValue = (event.target as HTMLInputElement).value;

    if (newValue && newValue.trim().length > 0) {
      this._rowFilter.value[valueIndex] = newValue;

      this.filterChanged.emit(this.rowFilter);
    }
  }

  public addNewValue(event: Event): void {
    event.preventDefault();
    this._rowFilter.value.push('');
    this._changeDetectorRef.detectChanges();
  }
}
