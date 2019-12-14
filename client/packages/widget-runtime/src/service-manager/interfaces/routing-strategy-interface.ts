import { IRouterState } from './router-state-interface';

export interface IRoutingStrategy {
  navigate(path: string): Promise<void>;
  getState(): IRouterState;
}
