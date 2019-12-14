import { HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { prm } from '@impartner/api';
import { IHttpClient } from '@impartner/widget-runtime';
import { ColDef } from 'ag-grid-community';
import { IGetRowsParams } from 'ag-grid-community/dist/lib/rowModels/iDatasource';
import { CookieService } from 'ngx-cookie-service';

import { IRowFilter } from 'src/app/core';
import { CORE_TOKENS } from 'src/app/core/constants';
import { PrmObject } from 'src/app/core/enums';
import { environment } from 'src/environments/environment';
import { ICookie, IFieldDefinition, IPrmObject } from '../../interfaces';
import { EndpointLocatorService } from '../endpoint-locator.service';
import { Context } from './interpreter';
import { BodyInterpreter } from './interpreter/body-interpreter';

@Injectable()
export class RequestCreatorService {
  private _httpHeaders: HttpHeaders;

  constructor(
    @Inject(CORE_TOKENS.HttpClient) private readonly _http: IHttpClient,
    private readonly _cookieService: CookieService,
    private readonly _endpointLocator: EndpointLocatorService
  ) {
    this._configurateRequestHeaders();
  }

  public get httpHeaders(): HttpHeaders {
    return this._httpHeaders;
  }

  public getGridData(
    prmObject: PrmObject,
    params: IGetRowsParams,
    columnsToShow: ColDef[],
    rowFilters?: IRowFilter[]
  ): Promise<any> {
    const columnFieldList = columnsToShow.map(column => column.field) as string[];
    const contextObject = new Context(prmObject, params, columnFieldList, rowFilters);

    const interpreter = new BodyInterpreter();
    const { requestUrl, queryParams } = interpreter.interpret(contextObject);

    const headers: HttpHeaders = this._httpHeaders || new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    let data = this._http.post<prm.IApiResult<any>>(
      this._endpointLocator.getApiUrl() + requestUrl,
      queryParams,
      {
        headers
      }
    );
    data = data.then(resp => resp.data);

    return data;
  }

  public getPrmObjects(): Promise<IPrmObject[]> {
    const endpointUrl = `${this._endpointLocator.getApiUrl()}/objects/v1/_describe`;
    const data = this._http.get<prm.IApiResult<IPrmObject[]>>(endpointUrl, {
      headers: this._httpHeaders
    });
    const prmObjects = data.then(resp => resp.data as IPrmObject[]);

    return prmObjects;
  }

  public async getPrmObjectMetadata(prmObject: string): Promise<IFieldDefinition[]> {
    const endpointUrl = `${this._endpointLocator.getApiUrl()}/objects/v1/${prmObject}/_describe`;
    const response = await this._http.get<prm.IApiResult<IFieldDefinition[]>>(endpointUrl, {
      headers: this._httpHeaders
    });
    const fieldDefinition = response.data as IFieldDefinition[];
    const prmFieldsDefinition = fieldDefinition.filter(
      dataItem => !dataItem.fieldType.includes('List')
    );

    return prmFieldsDefinition;
  }

  private _configurateRequestHeaders(): void {
    const cookiesObject: ICookie = this._cookieService.getAll();
    let cookieString = '';
    let containsValidCookie = false;

    for (const cookieName in cookiesObject) {
      if (cookiesObject.hasOwnProperty(cookieName)) {
        if (cookieName.startsWith('RV.') && cookieName.endsWith('Auth')) {
          containsValidCookie = true;
        }
        cookieString = `${cookieString}; ${cookieName}=${cookiesObject[cookieName]}`;
      }
    }

    if (environment.production && containsValidCookie) {
      cookieString = cookieString.substring(2);
      this._httpHeaders = new HttpHeaders({
        Cookie: cookieString
      });
    } else if (!environment.production) {
      console.warn(
        "We are in development mode. Please ensure to add 'X-PRM-TenantID' and 'Authorization' " +
          "headers in request, and 'Access-Control-Allow-Origin: *' in server response " +
          'through a Chrome/Firefox extension!'
      );
    } else if (containsValidCookie) {
      console.warn("There isn't a valid cookie that allows the authentication in REST API");
    }
  }
}
