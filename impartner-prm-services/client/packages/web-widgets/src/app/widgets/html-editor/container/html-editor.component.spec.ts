import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { SafePipeModule } from 'safe-pipe';

import { HtmlEditorComponent } from './html-editor.component';

describe('html-editor.component.ts', () => {
  let componentUnderTest: HtmlEditorComponent;
  let fixture: ComponentFixture<HtmlEditorComponent>;
  let debuggerElement: DebugElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HtmlEditorComponent],
      imports: [SafePipeModule]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HtmlEditorComponent);
    componentUnderTest = fixture.componentInstance;
    debuggerElement = fixture.debugElement;
    fixture.detectChanges();
  });

  describe('toggleEditMode()', () => {
    it('should create a new instance of the HTML Editor Library when changes to edit mode', () => {
      componentUnderTest.toggleEditMode(
        jasmine.createSpyObj<Event>('event', ['preventDefault', 'stopPropagation'])
      );
      const aceEditorElement = debuggerElement.query(By.css('div.ace_editor'));

      expect(aceEditorElement).toBeTruthy();
    });

    it('should show the static editor when prerender mode is active', () => {
      const aceEditorElement = debuggerElement.query(By.css('.w-static-editor'));

      expect(aceEditorElement).toBeTruthy();
    });
  });
});
