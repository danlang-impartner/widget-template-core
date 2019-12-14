import { Context } from '../context';
import { IExpression } from '../interfaces';

export class PaginationExpression implements IExpression {
  public interpret(context: Context): string {
    let paginationExpression = '';
    if (
      typeof context.agGridParams.startRow !== 'undefined' &&
      typeof context.agGridParams.endRow !== 'undefined'
    ) {
      paginationExpression += `&skip=${context.agGridParams.startRow}`;
      paginationExpression += `&take=${context.agGridParams.endRow -
        context.agGridParams.startRow}`;
    }

    return paginationExpression;
  }
}
