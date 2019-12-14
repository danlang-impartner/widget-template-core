import { Context } from '../context';
import { IExpression } from '../interfaces';

export class SortByExpression implements IExpression {
  public interpret(context: Context): string {
    let fieldsToSort = '';
    if (context.agGridParams.sortModel) {
      context.agGridParams.sortModel.forEach((model: any) => {
        fieldsToSort = `${fieldsToSort},${model.colId} ${model.sort}`;
      });
      if (fieldsToSort.length > 0) {
        fieldsToSort = fieldsToSort.substr(1);
        fieldsToSort = `&orderBy=${fieldsToSort}`;
      }
    }

    return fieldsToSort;
  }
}
