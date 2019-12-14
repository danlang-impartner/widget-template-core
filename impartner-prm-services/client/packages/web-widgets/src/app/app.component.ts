import { Component } from '@angular/core';

@Component({
  selector: 'w-impartner-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  public isInEditMode: boolean;

  constructor() {
    this.isInEditMode = false;
  }
}
