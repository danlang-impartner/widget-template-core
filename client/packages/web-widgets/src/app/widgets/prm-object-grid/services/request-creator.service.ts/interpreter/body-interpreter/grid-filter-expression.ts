import { Context } from '../context';
import { IExpression } from '../interfaces';

export class GridFilterExpression implements IExpression<void> {
  private readonly _equivalenceOperators: { [prop: string]: string } = {
    contains: "like '%%s%'",
    notContains: "not like '%%s%'",
    equals: "= '%s'",
    notEqual: "!= '%s'",
    startsWith: "like '%s%'",
    endsWith: "like '%%s'",
    lessThan: "< '%s'",
    lessThanOrEqual: "<= '%s'",
    greaterThan: "> '%s'",
    greaterThanOrEqual: ">= '%s'",
    inRange: ">= '%s' and %f <= '%s2'"
  };

  public interpret(context: Context): void {
    if (context.agGridParams && context.agGridParams.filterModel) {
      let filter = '';
      for (const fieldName in context.agGridParams.filterModel) {
        if (context.agGridParams.filterModel.hasOwnProperty(fieldName)) {
          const parametersOfFilter = context.agGridParams.filterModel[fieldName];
          const filterType = parametersOfFilter.filterType;
          let valueToSearchFor = parametersOfFilter.filter;
          let secondValueForBetweenOperations = null;
          let operator: string = parametersOfFilter.type;

          if (!(operator in this._equivalenceOperators)) {
            throw new Error(`Filter '${operator}' isn't recognized as valid`);
          }

          if (filterType === 'date') {
            valueToSearchFor = this._formatDate(parametersOfFilter.dateFrom);
            if (parametersOfFilter.dateTo != null) {
              secondValueForBetweenOperations = this._formatDate(parametersOfFilter.dateTo);
            }
          } else if (filterType === 'number') {
            secondValueForBetweenOperations = parametersOfFilter.filterTo;
          }

          if (operator in this._equivalenceOperators) {
            operator = this._equivalenceOperators[operator];
            let valueWithOperator = operator.replace('%s', valueToSearchFor);
            if (secondValueForBetweenOperations != null) {
              valueWithOperator = valueWithOperator.replace('%f', fieldName);
              valueWithOperator = valueWithOperator.replace('%s2', secondValueForBetweenOperations);
            }
            filter =
              filter === ''
                ? `${fieldName} ${valueWithOperator}`
                : `${filter} AND ${fieldName} ${valueWithOperator}`;
          }
        }
      }

      if (filter) {
        let currentFilter = context.queryParams.filter;
        if (currentFilter !== '') {
          currentFilter += ' AND ';
        }
        currentFilter = `(${filter})`;
        context.queryParams.filter = currentFilter;
      }
    }
  }

  private _formatDate(dateInString: string): string {
    const regexDate = /([0-9]{4})-([0-9]{2})-([0-9]{2})/;
    let valueToSearchFor = dateInString;
    const partsOfDate = dateInString.match(regexDate);
    if (partsOfDate) {
      valueToSearchFor = `${partsOfDate[1]}/${partsOfDate[2]}/${partsOfDate[3]}`;
    }

    return valueToSearchFor;
  }
}
