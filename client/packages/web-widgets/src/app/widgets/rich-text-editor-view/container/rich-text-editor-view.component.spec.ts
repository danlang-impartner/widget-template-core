import { DebugElement } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { SafePipeModule } from 'safe-pipe';

import { RichTextEditorViewComponent } from './rich-text-editor-view.component';

describe('rich-text-editor-view.component.ts', () => {
  let component: RichTextEditorViewComponent;
  let fixture: ComponentFixture<RichTextEditorViewComponent>;
  let debuggerElement: DebugElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [RichTextEditorViewComponent],
      imports: [SafePipeModule]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RichTextEditorViewComponent);
    component = fixture.componentInstance;
    debuggerElement = fixture.debugElement;
    fixture.detectChanges();
  });

  describe('set data()', () => {
    it('should set the data in the view of the component', () => {
      component.data = 'this is a test';
      fixture.detectChanges();

      const createdElement = debuggerElement.query(By.css('div')).nativeElement.innerHTML;
      expect(createdElement).toContain('this is a test');
    });
  });
});
