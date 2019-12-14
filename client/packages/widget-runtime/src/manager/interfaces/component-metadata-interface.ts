import { WidgetMode } from '../enums';

export interface IComponentMetadata {
  widgetMode: WidgetMode;
  tagId: string;
  sourceUrl?: string;
}
