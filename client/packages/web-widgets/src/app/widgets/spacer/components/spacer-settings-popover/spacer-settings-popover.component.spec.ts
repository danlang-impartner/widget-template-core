import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpacerSettingsPopoverComponent } from './spacer-settings-popover.component';

describe('SpacerSettingsPopoverComponent', () => {
  let component: SpacerSettingsPopoverComponent;
  let fixture: ComponentFixture<SpacerSettingsPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SpacerSettingsPopoverComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpacerSettingsPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
