import { IComponentMetadata, IWidgetIcon } from '.';
import { IWidgetMetadata } from './widget-metadata-interface';

export interface IWidget extends IWidgetMetadata {
  readonly widgetComponents: IComponentMetadata[];
  readonly mainIconName?: string;
  readonly widgetIcons?: IWidgetIcon[];
}
