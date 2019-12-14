export interface IAppConfig {
  resourceUrl: string;
  pageUrl?: string;
  apiUrl?: string;
  rootUrl?: string;
  authorizeUrl?: string;
  loginUrl?: string;
  spaPath?: string;
  buildVersion?: string;
  tenantId?: number;
  xss?: boolean;
  language?: string;
  debug?: boolean;
  fallbackLanguage?: string;
}
