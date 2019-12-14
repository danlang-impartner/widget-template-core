import { IFilterCriteria } from './filter-criteria';
import { IOrderByExpression } from './order-by-expression';

export interface IQueryParams {
  orderBy: IOrderByExpression[];
  skip?: number;
  take?: number;
  fields: string[];
  filter?: string;
  criteria?: {
    filters: IFilterCriteria[];
  };
}
