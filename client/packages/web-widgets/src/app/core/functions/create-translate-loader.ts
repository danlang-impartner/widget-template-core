import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { definePublicPath } from './define-public-path.function';

export function createTranslateLoader(http: HttpClient, widgetType: string): TranslateHttpLoader {
  const publicUrl = definePublicPath() || '.';

  return new TranslateHttpLoader(http, `${publicUrl}assets/i18n/${widgetType}/`, '.json');
}
