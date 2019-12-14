import { IRoute } from './route';

export interface IRouterState extends IRoute {
  params: Record<string, string>;
}
