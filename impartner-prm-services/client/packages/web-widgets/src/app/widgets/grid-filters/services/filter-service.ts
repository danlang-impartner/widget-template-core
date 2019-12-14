import { Injectable } from '@angular/core';

import { OPERATORS } from '../constants';
import { IFieldDefinition, IFieldOperator } from '../interfaces';

@Injectable()
export class FilterService {
  public getValidOperators(fieldMetadata: IFieldDefinition): IFieldOperator[] {
    return OPERATORS.filter(operator => {
      if (
        typeof operator.excludeFieldTypes === 'undefined' &&
        typeof operator.includeOnlyFieldTypes === 'undefined'
      ) {
        return true;
      }

      if (fieldMetadata.fieldDataType) {
        if (
          operator.excludeFieldTypes &&
          !operator.excludeFieldTypes.includes(fieldMetadata.fieldDataType)
        ) {
          return true;
        }

        if (
          operator.includeOnlyFieldTypes &&
          operator.includeOnlyFieldTypes.includes(fieldMetadata.fieldDataType)
        ) {
          return true;
        }
      }

      return false;
    });
  }
}
