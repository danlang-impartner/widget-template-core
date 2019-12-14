import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'w-impartner-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SelectComponent {
  public itemsList: any[];
  public defaultValue: any;

  @Input()
  public bindLabel: string;
  @Input()
  public bindValue: string;
  @Input()
  public disabled = false;

  private _defaultStringToSearch: string;

  @Output()
  public change = new EventEmitter<any>();

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {
    this.itemsList = [];
  }

  @Input()
  public set items(value: string) {
    try {
      this.itemsList = JSON.parse(value);
    } catch (error) {
      throw new Error(`cannot parse items of the select: ${error}`);
    }
  }

  @Input()
  public set default(value: string) {
    this._defaultStringToSearch = value;
    this._selectDefaultValueInList();
  }

  public changeListener(event: Event): void {
    const objectToEmit: { [label: string]: any } = {};
    objectToEmit[this.bindValue] = (event.target as HTMLSelectElement).value;
    this.defaultValue = objectToEmit[this.bindValue];
    this._changeDetectorRef.detectChanges();
    this.change.emit(objectToEmit);
  }

  private _selectDefaultValueInList(): void {
    if (this.itemsList.length) {
      const defaultItemToSelect = this.itemsList.filter(
        item => item[this.bindValue] === this._defaultStringToSearch
      );

      if (defaultItemToSelect.length) {
        this.defaultValue = defaultItemToSelect[0][this.bindValue];
        this._changeDetectorRef.detectChanges();
      }
    }
  }
}
