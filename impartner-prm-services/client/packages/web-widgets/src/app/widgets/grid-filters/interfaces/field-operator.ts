import { GridFieldTypes, OperatorSymbol } from '../enums';

export interface IFieldOperator {
  name: string;
  icon: string;
  symbol: OperatorSymbol;
  excludeFieldTypes?: GridFieldTypes[];
  includeOnlyFieldTypes?: GridFieldTypes[];
}
