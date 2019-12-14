import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';
import { SvgIconRegistryService } from 'angular-svg-icon';

import { IDropdownOption } from '../interfaces';

@Component({
  selector: 'w-impartner-dropdown-icon',
  templateUrl: './dropdown-icon.component.html',
  styleUrls: ['./dropdown-icon.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DropdownIconComponent {
  private _options: IDropdownOption[];

  @Input()
  public set options(options: IDropdownOption[]) {
    this._options = options;

    if (this._iconRegistryService) {
      options.forEach(option => {
        this._iconRegistryService.unloadSvg(option.iconName);
        this._iconRegistryService.addSvg(option.iconName, option.icon);
      });
    }
  }

  public get options(): IDropdownOption[] {
    return this._options;
  }

  @Input()
  public set selectedOption(value: IDropdownOption) {
    this._selectedOption = value;
  }

  public get selectedOption(): IDropdownOption {
    return this._selectedOption;
  }

  @Input()
  public dropdownTitle = '';

  public showOptions = false;

  @Output()
  public readonly valueChangedEvent = new EventEmitter<IDropdownOption>();

  private _selectedOption: IDropdownOption;

  constructor(
    private readonly _changeDetectorRef: ChangeDetectorRef,
    private readonly _iconRegistryService: SvgIconRegistryService
  ) {}

  public toggleOptionsList(event: Event): void {
    event.preventDefault();
    this.showOptions = !this.showOptions;
    this._changeDetectorRef.detectChanges();
  }

  public valueSelected(value: IDropdownOption): void {
    this.selectedOption = value;
    this.showOptions = false;
    this._changeDetectorRef.detectChanges();
    this.valueChangedEvent.emit(value);
  }
}
