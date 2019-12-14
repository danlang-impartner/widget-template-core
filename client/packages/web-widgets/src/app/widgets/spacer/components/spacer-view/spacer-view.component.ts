import { Component, Input } from '@angular/core';

import { WidgetTag } from 'src/app/core';
import { environment } from 'src/environments/environment';
import { DEFAULT_SPACER_CONFIGURATION } from '../../config';
import { ISpacerConfiguration } from '../../interfaces';

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.SpacerView}`,
  templateUrl: './spacer-view.component.html',
  styleUrls: ['./spacer-view.component.scss']
})
export class SpacerViewComponent {
  private _spacerConfiguration: ISpacerConfiguration = DEFAULT_SPACER_CONFIGURATION;

  constructor() {}

  @Input()
  public set widgetConfig(value: string | ISpacerConfiguration) {
    try {
      let currentValue: ISpacerConfiguration;

      if (typeof value !== 'string') {
        currentValue = value as ISpacerConfiguration;
      } else {
        currentValue = JSON.parse(value as string);
      }

      this._spacerConfiguration = currentValue;

      if (!this._spacerConfiguration || !this._spacerConfiguration.spacingSize) {
        this._spacerConfiguration = DEFAULT_SPACER_CONFIGURATION;
      }
    } catch (error) {
      throw new Error(`Cannot parse the spacer configuration: ${error}`);
    }
  }

  public get spacerConfiguration(): ISpacerConfiguration {
    return this._spacerConfiguration;
  }
}
