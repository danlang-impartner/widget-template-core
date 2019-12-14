import { ChangeDetectorRef, Component, Inject, Input } from '@angular/core';
import {
  IEventBus,
  ImpartnerWidgetTypes,
  IShowConfigRequestedEvent,
  IWidgetParams,
  SystemEvents
} from '@impartner/widget-runtime';
import { TranslateService } from '@ngx-translate/core';
import { ColDef } from 'ag-grid-community';
import { SortablejsOptions } from 'angular-sortablejs';

import { WidgetTag } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { SelectListOption } from 'src/app/shared/select/interfaces';
import { DEFAULT_CONFIG } from '../../config';
import { IDataSourceDefinition, IGridConfiguration } from '../../interfaces';

@Component({
  selector: `w-impartner-${WidgetTag.PrmObjectGridSettings}`,
  templateUrl: './grid-settings.component.html',
  styleUrls: ['./grid-settings.component.scss']
})
export class GridSettingsComponent implements IWidgetParams {
  public gridConfiguration: IGridConfiguration = DEFAULT_CONFIG;
  public columnDefinition: ColDef[] = [];
  public sortableJsOptions: SortablejsOptions = {
    onUpdate: (event: Event): void => this.onOrderChanged(event)
  };

  @Input()
  public readonly id: number;

  @Input()
  public debug = false;

  @Input()
  public set localeCode(value: string) {
    this._translateService.setDefaultLang(value);
  }

  private _datasourceDefinition: IDataSourceDefinition;

  public set pageOptionList(options: SelectListOption<number>[]) {
    this.gridConfiguration.resultsPerPage = options.map(option => option.value);
    this.datasourceDefinition.widgetConfig = this.gridConfiguration;
  }

  public get pageOptionList(): SelectListOption<number>[] {
    if (this.gridConfiguration.resultsPerPage.length) {
      return this.gridConfiguration.resultsPerPage.map(result => {
        return { label: String(result), value: result };
      });
    }

    return DEFAULT_CONFIG.resultsPerPage.map(resultNumber => {
      return { label: String(resultNumber), value: resultNumber };
    });
  }

  public previousEditingValue: boolean;

  public newIncrementValue: number;

  public showAddPageOptionBox = false;

  constructor(
    @Inject(CORE_TOKENS.IEventBus) private readonly _eventBus: IEventBus,
    private readonly _changeDetectorRef: ChangeDetectorRef,
    private readonly _translateService: TranslateService
  ) {
    this._eventBus.addEventListener<IShowConfigRequestedEvent<IDataSourceDefinition>>(
      SystemEvents.ShowConfigRequested,
      event => {
        if (event.type === ImpartnerWidgetTypes.ImpartnerPrmObjectGrid) {
          this.datasourceDefinition = event.configuration;
        }
      }
    );
  }

  @Input()
  public set datasourceDefinition(value: IDataSourceDefinition) {
    this._datasourceDefinition = value;
    this.gridConfiguration = { ...(value.widgetConfig as IGridConfiguration) };
    this.columnDefinition = value.columnDefinition ? [...value.columnDefinition] : [];
    if (this._changeDetectorRef) {
      this._changeDetectorRef.detectChanges();
    }
  }

  public get datasourceDefinition(): IDataSourceDefinition {
    return this._datasourceDefinition;
  }

  public get numberSelectedFields(): number {
    return this.columnDefinition.filter(columnDef => !columnDef.hide).length;
  }

  public hideOrUnhideField(columnDefinition: ColDef): void {
    columnDefinition.hide = !columnDefinition.hide;
    const listColumnsExceptUnchecked = this._sortColumnDefinitions(columnDefinition);

    this.columnDefinition = listColumnsExceptUnchecked;
    this.datasourceDefinition.columnDefinition = listColumnsExceptUnchecked;
    this.gridConfiguration.columnsToShow = listColumnsExceptUnchecked
      .filter(currentColumnDefinition => !currentColumnDefinition.hide)
      .map(currentColumnDefinition => currentColumnDefinition.field) as string[];
    this.datasourceDefinition.widgetConfig = this.gridConfiguration;
  }

  public onOrderChanged(event: Event): void {
    this.gridConfiguration.columnsToShow = this.columnDefinition
      .filter(currentColumnDefinition => !currentColumnDefinition.hide)
      .map(currentColumnDefinition => currentColumnDefinition.field) as string[];
    this.datasourceDefinition.widgetConfig = this.gridConfiguration;
  }

  private _sortColumnDefinitions(columnDefinition: ColDef): ColDef[] {
    const listColumnsExceptUnchecked = this.columnDefinition.filter(
      colDef => colDef.field !== columnDefinition.field
    );

    if (columnDefinition.hide) {
      listColumnsExceptUnchecked.push(columnDefinition);
    } else {
      listColumnsExceptUnchecked.unshift(columnDefinition);
    }

    return listColumnsExceptUnchecked;
  }

  public toggleShowAddPageOptionBox(): void {
    this.showAddPageOptionBox = !this.showAddPageOptionBox;
    this._changeDetectorRef.detectChanges();
  }

  public addNewPageOption(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    const value = Number((event.target as HTMLInputElement).value);

    this.pageOptionList = [
      ...this.pageOptionList,
      {
        label: String(value),
        value
      }
    ];

    this.toggleShowAddPageOptionBox();
    this._changeDetectorRef.detectChanges();
  }

  public paginationOptionsModified(options: SelectListOption<number>[]): void {
    this.pageOptionList = options;
  }
}
