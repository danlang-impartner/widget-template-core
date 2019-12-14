import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild
} from '@angular/core';
import { NgxSmartModalComponent } from 'ngx-smart-modal';

@Component({
  selector: 'w-impartner-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.scss']
})
export class PopupComponent {
  @Input()
  public title: string;

  @Input()
  public text = '';

  @Input()
  public buttonText: string;

  @Output()
  public confirmAction = new EventEmitter<void>();

  @ViewChild('customPopup') private _smartModalInstance: NgxSmartModalComponent;

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {}

  @Input()
  public set show(value: boolean) {
    if (value) {
      this._smartModalInstance.open();
    } else {
      this._smartModalInstance.close();
    }
    this._changeDetectorRef.detectChanges();
  }

  public get visible(): boolean {
    return this._smartModalInstance.visible;
  }

  public confirm(): void {
    this.confirmAction.emit();
    this._smartModalInstance.close();
  }
}
