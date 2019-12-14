import { IWidgetMetadata, WidgetMode } from '@impartner/widget-runtime';

export interface IWidgetRegistration extends IWidgetMetadata {
  modes: WidgetMode[];
  path: string;
}
