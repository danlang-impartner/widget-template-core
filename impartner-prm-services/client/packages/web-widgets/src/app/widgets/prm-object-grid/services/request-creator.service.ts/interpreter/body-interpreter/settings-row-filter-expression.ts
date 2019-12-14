import { Context } from '../context';
import { IExpression } from '../interfaces';

const OPERATORS_MAPPING: { [operator: string]: string } = {
  contains: "like '%%s%'",
  notContains: "not like '%%s%'",
  equals: "= '%s'",
  notEqual: "!= '%s'",
  startsWith: "like '%s%'",
  endsWith: "like '%%s'",
  lessThan: "< '%s'",
  lessThanOrEqual: "<= '%s'",
  greaterThan: "> '%s'",
  greaterThanOrEqual: ">= '%s'"
};

export class SettingsRowFilterExpression implements IExpression<void> {
  public interpret(context: Context): void {
    const rowFilters = context.rowFilters || [];
    let filter = context.queryParams.filter;

    rowFilters.forEach(rowFilter => {
      const mappedOperator = OPERATORS_MAPPING[rowFilter.operator];
      let currentFilter = '';

      if (typeof mappedOperator === 'undefined') {
        throw new Error('Operator does not exist: ' + rowFilter.operator);
      }

      rowFilter.value.forEach(currentValue => {
        const operatorWithValue = mappedOperator.replace('%s', currentValue);

        if (currentFilter !== '') {
          currentFilter = `${currentFilter} OR`;
        }

        currentFilter = `${currentFilter} ${rowFilter.fact} ${operatorWithValue}`;
      });

      if (filter === '') {
        filter = `(${currentFilter})`;
      } else {
        filter = `${filter} ${rowFilter.booleanOperator} (${currentFilter})`;
      }
    });

    context.queryParams.filter = filter;
  }
}
