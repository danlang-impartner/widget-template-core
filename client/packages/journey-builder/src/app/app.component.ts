import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  // tslint:disable-next-line: component-selector
  selector: 'f-journey-builder',
  templateUrl: './app.component.html'
})
export class AppComponent {
  @Input()
  public set path(value: string) {
    this.router.navigateByUrl(value);
    this._path = value;
  }

  constructor(private router: Router) {}
  private _path: string;
}
