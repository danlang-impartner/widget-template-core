import { Context } from '../context';
import { IExpression } from '../interfaces';

export class PaginationExpression implements IExpression<void> {
  public interpret(context: Context): void {
    if (
      typeof context.agGridParams.startRow !== 'undefined' &&
      typeof context.agGridParams.endRow !== 'undefined'
    ) {
      context.queryParams.skip = context.agGridParams.startRow;
      context.queryParams.take = context.agGridParams.endRow - context.agGridParams.startRow;
    }
  }
}
