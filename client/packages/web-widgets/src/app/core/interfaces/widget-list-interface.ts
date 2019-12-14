import { IWidgetRegistration } from './widget-registration-interface';

export interface IWidgetList {
  [key: string]: IWidgetRegistration;
}
