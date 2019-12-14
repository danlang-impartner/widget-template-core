import { IWidgetEvent } from '@impartner/widget-runtime';
import { ColDef } from 'ag-grid-community';

import { IGridConfiguration } from './grid-configuration';

export interface IDataSourceDefinition extends IWidgetEvent {
  widgetConfig: string | IGridConfiguration;
  columnDefinition: ColDef[];
}
