import { ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

import { GridFiltersContainer } from '../shared';

@Component({
  selector: 'w-impartner-grid-filters-view',
  templateUrl: './grid-filters-view.component.html',
  styleUrls: ['./grid-filters-view.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GridFiltersViewComponent extends GridFiltersContainer {
  constructor(_changeDetectorRef: ChangeDetectorRef, _translateService: TranslateService) {
    super(_changeDetectorRef, _translateService);
  }
}
