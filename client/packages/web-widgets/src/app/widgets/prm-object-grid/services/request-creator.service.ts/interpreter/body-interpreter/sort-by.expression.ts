import { Context } from '../context';
import { IExpression, IOrderByExpression } from '../interfaces';

export class SortByExpression implements IExpression<void> {
  public interpret(context: Context): void {
    let orderCriteria: IOrderByExpression[] = [];

    if (context.agGridParams.sortModel) {
      orderCriteria = context.agGridParams.sortModel.map((model: any) => {
        return {
          field: model.display,
          direction: model.sort
        };
      });
    }

    context.queryParams.orderBy = orderCriteria;
  }
}
