import { ChangeDetectorRef, Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import {
  IConfigChangedEvent,
  IEventBus,
  IListener,
  IWidgetParams,
  SystemEvents
} from '@impartner/widget-runtime';

import { WidgetTag } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { environment } from 'src/environments/environment';
import { DEFAULT_SPACER_CONFIGURATION } from '../config';
import { SpacerEvent } from '../enums';
import { ISpacerConfiguration } from '../interfaces';

@Component({
  selector: `${environment.widgetPrefix}-${WidgetTag.SpacerEdit}`,
  templateUrl: './spacer-edit.component.html',
  styleUrls: ['./spacer-edit.component.scss']
})
export class SpacerEditComponent implements OnInit, OnDestroy, IWidgetParams<ISpacerConfiguration> {
  @Input()
  public readonly id: number;

  private _spacerConfiguration: ISpacerConfiguration = DEFAULT_SPACER_CONFIGURATION;
  private readonly _eventSubscriptions: IListener<IConfigChangedEvent<ISpacerConfiguration>>[] = [];

  constructor(
    private readonly _changeDetectorRef: ChangeDetectorRef,
    @Inject(CORE_TOKENS.IEventBus) private readonly _eventBus: IEventBus
  ) {}

  public ngOnInit(): void {
    this._eventSubscriptions.push(
      this._eventBus.addEventListener<IConfigChangedEvent<ISpacerConfiguration>>(
        SystemEvents.ConfigChanged,
        event => this._configChangedListener(event)
      ),
      this._eventBus.addEventListener<IConfigChangedEvent<ISpacerConfiguration>>(
        SpacerEvent.SpacerSizeChanged,
        event => this._configChangedListener(event)
      )
    );
  }

  public ngOnDestroy(): void {
    this._eventSubscriptions.forEach(subscription =>
      this._eventBus.removeEventListener(subscription)
    );
  }

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

      this._changeDetectorRef.detectChanges();
    } catch (error) {
      throw new Error(`Cannot parse the spacer configuration: ${error}`);
    }
  }

  public get spacerConfiguration(): ISpacerConfiguration {
    return this._spacerConfiguration;
  }

  private _configChangedListener(event: IConfigChangedEvent<ISpacerConfiguration>): void {
    if (event.widgetId === this.id) {
      this.widgetConfig = event.configuration;
    }
  }
}
