import { DebugElement, NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
import { CKEditor5 } from '@ckeditor/ckeditor5-angular/ckeditor';
import { ChangeEvent } from '@ckeditor/ckeditor5-angular/ckeditor.component';
import * as ClassicEditor from '@impartner/ckeditor5-build-classic';
import { IDataChangedEvent } from '@impartner/widget-runtime';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { SafePipeModule } from 'safe-pipe';

import { CORE_TOKENS } from 'src/app/core/constants';
import { TranslatePipeMock, TranslateServiceStub } from 'src/app/core/test-utils';
import { RichTextEditorComponent } from './rich-text-editor.component';

describe('rich-text-editor.component.ts', () => {
  let component: RichTextEditorComponent;
  let fixture: ComponentFixture<RichTextEditorComponent>;
  let debuggerElement: DebugElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [RichTextEditorComponent],
      imports: [SafePipeModule, FormsModule, CKEditorModule],
      providers: [
        { provide: TranslateService, useClass: TranslateServiceStub },
        { provide: TranslatePipe, useClass: TranslatePipeMock },
        { provide: CORE_TOKENS.ClassicEditor, useValue: ClassicEditor }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RichTextEditorComponent);
    component = fixture.componentInstance;
    debuggerElement = fixture.debugElement;
    fixture.detectChanges();
  });

  describe('set data()', () => {
    it('should set the initial content, sent from content property', (done: Function) => {
      component.editorVisible.subscribe(() => {
        expect(component.editorInstance.getData()).toBe('<p>this is a test</p>');
        done();
      });
      component.data = 'this is a test';
      component.prerenderMode = false;
      fixture.detectChanges();
    });

    it('should the modified content in the editor be notified to other observers', (done: Function) => {
      component.contentChanged.subscribe((data: IDataChangedEvent<string>) => {
        expect(data.data).toBe('<p>another test sent to observers</p>');
        done();
      });

      component.editorVisible.subscribe(() => {
        document.execCommand('selectAll', true);
        document.execCommand('insertHTML', true, 'another test sent to observers');
      });

      component.prerenderMode = false;
      fixture.detectChanges();
    });
  });

  describe('set change()', () => {
    it('should set the content as empty when there is a space in the editor', () => {
      const editEvent = {
        editor: {
          getData: () => {}
        }
      };

      spyOn(editEvent.editor, 'getData').and.returnValue('<p>&nbsp;</p>');

      component.change(editEvent as ChangeEvent);

      expect(component.content).toBe('');
    });

    it('should emit an event when change content in the editor', (done: Function) => {
      component.contentChanged.subscribe((eventChange: IDataChangedEvent<string>) => {
        expect(eventChange.data).toBe('<p>test of an event</p>');
        done();
      });

      const editEvent = {
        editor: {
          getData: () => {}
        }
      };

      spyOn(editEvent.editor, 'getData').and.returnValue('<p>test of an event</p>');

      component.change(editEvent as ChangeEvent);
    });
  });

  describe('ready()', () => {
    let editorStub: CKEditor5.Editor;

    beforeEach(() => {
      editorStub = {
        ...jasmine.createSpyObj<CKEditor5.Editor>('CKEditorInstance', ['setData']),
        editing: { view: { focus: () => {} } }
      };

      spyOn(editorStub.editing.view, 'focus');
    });

    it("should set content in editor when it's ready", () => {
      component.content = 'test';
      component.ready(editorStub);

      expect(editorStub.setData).toHaveBeenCalledWith('test');
      expect(editorStub.editing.view.focus).toHaveBeenCalled();
    });

    it('should emit event that the editor is visible', (done: Function) => {
      component.content = 'test';
      component.editorVisible.subscribe(() => {
        expect(editorStub.editing.view.focus).toHaveBeenCalled();
        done();
      });

      component.ready(editorStub);
    });
  });

  describe('toggleEditMode()', () => {
    beforeEach(() => {
      component.prerenderMode = false;
    });

    it("shouldn' show the editor when is prerender mode ", () => {
      component.toggleEditMode(new Event(''));

      expect(debuggerElement.query(By.css('ckeditor'))).not.toBeTruthy();
    });

    it("should show the editor when isn't in prerender mode", () => {
      component.prerenderMode = true;
      component.toggleEditMode(new Event(''));

      expect(debuggerElement.query(By.css('ckeditor')).nativeElement).toBeTruthy();
    });
  });
});
