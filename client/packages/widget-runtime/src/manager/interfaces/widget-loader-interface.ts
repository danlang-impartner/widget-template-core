import { WidgetMode } from '../enums';
import { IWidgetMetadata } from './widget-metadata-interface';

export interface IWidgetLoader extends IWidgetMetadata {
  readonly modes: WidgetMode[];
  readonly sourceUrl?: string;

  loadComponent(widget: IWidgetLoader): Promise<void>;
}
