import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditableTabTitleComponent } from './editable-tab-title.component';

describe('EditableTabTitleComponent', () => {
  let component: EditableTabTitleComponent;
  let fixture: ComponentFixture<EditableTabTitleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [EditableTabTitleComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditableTabTitleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
