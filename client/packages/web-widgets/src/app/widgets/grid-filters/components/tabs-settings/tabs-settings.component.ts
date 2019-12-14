import { ChangeDetectionStrategy, Component, EventEmitter, OnInit, Output } from '@angular/core';

import { DEFAULT_CONFIG_NEW_TAB } from '../../constants';
import { ITabConfig } from '../../interfaces';
import { TabsInteraction } from '../../shared';

@Component({
  selector: 'w-impartner-tabs-settings',
  templateUrl: './tabs-settings.component.html',
  styleUrls: ['./tabs-settings.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TabsSettingsComponent extends TabsInteraction {
  @Output()
  public readonly tabsConfigChanged = new EventEmitter<ITabConfig[]>();

  public addTab(event: Event): void {
    event.preventDefault();
    const tabId = this.tabs[this.tabs.length - 1].id + 1;
    const newTabConfig = JSON.parse(JSON.stringify(DEFAULT_CONFIG_NEW_TAB));

    this.tabs.push({
      ...newTabConfig,
      id: tabId
    });

    this._changeDetectorRef.detectChanges();
  }

  public updateTabName(newName: string, tab: ITabConfig): void {
    const tabsConfigCopy: ITabConfig[] = JSON.parse(JSON.stringify(this.tabs));
    const tabUpdated = tabsConfigCopy.find(currentTab => currentTab.id === tab.id);

    if (tabUpdated) {
      tabUpdated.name = newName;
      this._changeDetectorRef.detectChanges();
      this.tabsConfigChanged.emit(tabsConfigCopy);
    }
  }
}
