import { IHttpClientOptions } from './http-client-options-interface';

export interface IHttpClient {
  head<T>(url: string, options?: IHttpClientOptions): Promise<T>;

  options<T>(url: string, options?: IHttpClientOptions): Promise<T>;

  get<T>(url: string, options?: IHttpClientOptions): Promise<T>;

  post<T>(url: string, body: any | null, options?: IHttpClientOptions): Promise<T>;

  put<T>(url: string, body: any | null, options?: IHttpClientOptions): Promise<T>;

  patch<T>(url: string, body: any | null, options?: IHttpClientOptions): Promise<T>;

  delete<T>(url: string, body: any | null, options?: IHttpClientOptions): Promise<T>;
}
