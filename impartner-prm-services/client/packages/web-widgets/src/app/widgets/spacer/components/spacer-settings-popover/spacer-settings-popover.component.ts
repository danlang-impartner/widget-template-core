import { ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import {
  IConfigChangedEvent,
  IWidgetEvent,
  IWidgetParams,
  SystemEvents,
  WidgetEvent
} from '@impartner/widget-runtime';

import { WidgetTag } from 'src/app/core';
import { environment } from 'src/environments/environment';
import { DEFAULT_SPACER_CONFIGURATION, SPACING_SELECTIONS } from '../../config';
import { SpacerEvent, SpacingSize } from '../../enums';
import { ISpacerConfiguration } from '../../interfaces';

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.SpacerSettings}`,
  templateUrl: './spacer-settings-popover.component.html',
  styleUrls: ['./spacer-settings-popover.component.scss']
})
export class SpacerSettingsPopoverComponent implements IWidgetParams {
  public selectionOptions = SPACING_SELECTIONS;

  private _spacerConfiguration: ISpacerConfiguration = DEFAULT_SPACER_CONFIGURATION;

  @Input()
  public readonly id: number;

  @Output()
  @WidgetEvent<EventEmitter<IConfigChangedEvent<ISpacerConfiguration>>>(
    'emit',
    SystemEvents.ConfigChanged
  )
  public configurationChanged = new EventEmitter<IConfigChangedEvent<ISpacerConfiguration>>();

  @Output()
  @WidgetEvent<EventEmitter<IWidgetEvent>>('emit', SystemEvents.SettingsApplied)
  public settingsApplied = new EventEmitter<IWidgetEvent>();

  @Output()
  @WidgetEvent<EventEmitter<IConfigChangedEvent<ISpacerConfiguration>>>(
    'emit',
    SpacerEvent.SpacerSizeChanged
  )
  public spacerSizeChanged = new EventEmitter<IConfigChangedEvent<ISpacerConfiguration>>();

  constructor(private readonly _changeDetectorRef: ChangeDetectorRef) {}

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

      if (
        !this._spacerConfiguration ||
        typeof this._spacerConfiguration.spacingSize === 'undefined'
      ) {
        this._spacerConfiguration = DEFAULT_SPACER_CONFIGURATION;
      }
    } catch (error) {
      throw new Error(`Cannot parse the spacer configuration: ${error}`);
    }
  }

  public get spacerConfiguration(): ISpacerConfiguration {
    return this._spacerConfiguration;
  }

  public activateTopOption(option: SpacingSize): void {
    this.widgetConfig = { ...this._spacerConfiguration, spacingSize: option };
    this.spacerSizeChanged.emit({
      widgetId: this.id,
      configuration: this._spacerConfiguration
    });
    this._changeDetectorRef.detectChanges();
  }

  public apply(): void {
    this.configurationChanged.emit({
      widgetId: this.id,
      configuration: this._spacerConfiguration
    });

    this.settingsApplied.emit({
      widgetId: this.id
    });
  }
}
