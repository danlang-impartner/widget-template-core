import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpClient, IHttpClientOptions } from '@impartner/widget-runtime';

@Injectable()
export class HttpClientAdapter implements IHttpClient {
  constructor(private readonly _http: HttpClient) {}

  public head<T>(url: string, options?: IHttpClientOptions | undefined): Promise<T> {
    return this._http.head<T>(url, options).toPromise();
  }

  public options<T>(url: string, options?: IHttpClientOptions | undefined): Promise<T> {
    return this._http.options<T>(url, options).toPromise();
  }

  public get<T>(url: string, options?: IHttpClientOptions | undefined): Promise<T> {
    return this._http.get<T>(url, options).toPromise();
  }

  public post<T>(url: string, body: any, options?: IHttpClientOptions | undefined): Promise<T> {
    return this._http.post<T>(url, body, options).toPromise();
  }

  public put<T>(url: string, body: any, options?: IHttpClientOptions | undefined): Promise<T> {
    return this._http.put<T>(url, body, options).toPromise();
  }

  public patch<T>(url: string, body: any, options?: IHttpClientOptions | undefined): Promise<T> {
    return this._http.patch<T>(url, body, options).toPromise();
  }

  public delete<T>(url: string, body: any, options?: IHttpClientOptions | undefined): Promise<T> {
    return this._http.delete<T>(url, options).toPromise();
  }
}
