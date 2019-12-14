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

import { ITabConfig } from '../../interfaces';

@Component({
  selector: 'w-impartner-editable-tab',
  templateUrl: './editable-tab.component.html',
  styleUrls: ['./editable-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditableTabComponent {
  @Input()
  public tab: ITabConfig;

  @Input()
  public selectedTabIndex: number;

  @Output()
  public readonly tabSelected = new EventEmitter<ITabConfig>();

  @Output()
  public readonly tabNameUpdated = new EventEmitter<string>();

  public isEditMode = false;

  @ViewChild('inputTabName')
  private readonly _inputTabName: ElementRef;

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {}

  public markTabAsSelected(): void {
    this.tabSelected.emit(this.tab);
  }

  public enableEditMode(): void {
    this.isEditMode = true;
    this._changeDetectorRef.detectChanges();
    (this._inputTabName.nativeElement as HTMLInputElement).focus();
    (this._inputTabName.nativeElement as HTMLInputElement).select();
  }

  public updateTabName(event: Event): void {
    event.preventDefault();
    const newTabName = (event.target as HTMLInputElement).value;
    this.isEditMode = false;
    this.tabNameUpdated.emit(newTabName);
  }
}
