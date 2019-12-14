export interface IWidgetParams<T = {}, U = {}> {
  readonly id: number;
  readonly 'widget-config'?: T;
  readonly data?: U;
  readonly 'locale-code'?: string;
  readonly revision?: number;
}
