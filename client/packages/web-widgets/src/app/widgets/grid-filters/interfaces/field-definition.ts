import { GridFieldTypes } from '../enums';

export interface IFieldDefinition {
  colId: string;
  headerName: string;
  hide: boolean;
  fieldDataType?: GridFieldTypes;
}
