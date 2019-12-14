import { ITabConfig } from './tab-config';

export interface IUpdatedGridFiltersConfig {
  selectedTab?: ITabConfig;
  tabs?: ITabConfig[];
  widgetId: number;
}
