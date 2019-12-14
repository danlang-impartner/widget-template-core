import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpacerViewComponent } from './spacer-view.component';

describe('spacer-view.component.ts', () => {
  let component: SpacerViewComponent;
  let fixture: ComponentFixture<SpacerViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SpacerViewComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpacerViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
