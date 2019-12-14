import { ChangeDetectionStrategy, Component } from '@angular/core';

import { TabsInteraction } from '../../shared';

@Component({
  selector: 'w-impartner-show-tabs',
  templateUrl: './show-tabs.component.html',
  styleUrls: ['./show-tabs.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ShowTabsComponent extends TabsInteraction {}
