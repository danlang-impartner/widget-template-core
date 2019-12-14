import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild
} from '@angular/core';

import { IItemListInterface } from '../interfaces';

@Component({
  selector: 'w-impartner-multioption-list',
  templateUrl: './multioption-list.component.html',
  styleUrls: ['./multioption-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MultioptionListComponent {
  @Input()
  public options: IItemListInterface[] = [];

  @Output()
  public readonly optionsModified = new EventEmitter<IItemListInterface[]>();

  private readonly _isItemEditMode: { [itemValue: string]: boolean } = {};

  @ViewChild('inputItem')
  private _inputItem: ElementRef;

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {}

  public removeItem(item: IItemListInterface): void {
    this.options = this.options.filter(option => option.value !== item.value);
    this._changeDetectorRef.detectChanges();
    this.optionsModified.emit(this.options);
  }

  public updateValue(event: Event, item: IItemListInterface): void {
    event.preventDefault();
    event.stopPropagation();
    const newValue = (event.target as HTMLInputElement).value;

    delete this._isItemEditMode[item.value];

    this.options = this.options.map(option => {
      if (option.value === item.value) {
        option.value = typeof option.value === 'number' ? Number(newValue) : newValue;
        option.label = newValue;
      }

      return option;
    });

    this.optionsModified.emit(this.options);
    this._isItemEditMode[item.value] = false;
    this._changeDetectorRef.detectChanges();
  }

  public enableEditMode(item: IItemListInterface): void {
    this._isItemEditMode[item.value] = true;
    this._changeDetectorRef.detectChanges();
    this._inputItem.nativeElement.focus();
    this._inputItem.nativeElement.select();
  }

  public isItemInEditMode(item: IItemListInterface): boolean {
    if (this._isItemEditMode.hasOwnProperty(item.value)) {
      return this._isItemEditMode[item.value];
    }

    return false;
  }

  public disableEditMode(): void {
    Object.keys(this._isItemEditMode).forEach(key => (this._isItemEditMode[key] = false));
    this._changeDetectorRef.detectChanges();
  }
}
