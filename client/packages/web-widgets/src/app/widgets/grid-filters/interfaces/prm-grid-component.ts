import { IPrmGridConfiguration } from './prm-grid-configuration';

export interface IPrmGridComponent<U = any> {
  widgetConfig: IPrmGridConfiguration;
  columnDefinition: U[];
}
