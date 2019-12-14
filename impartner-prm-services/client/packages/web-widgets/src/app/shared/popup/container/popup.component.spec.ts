import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NgxSmartModalComponent, NgxSmartModalModule } from 'ngx-smart-modal';

import { PopupComponent } from './popup.component';

describe('popup-component.spec.ts', () => {
  let fixture: ComponentFixture<PopupComponent>;
  let componentUnderTest: PopupComponent;
  let ngxSmartModalComponent: NgxSmartModalComponent;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PopupComponent],
      imports: [NgxSmartModalModule.forRoot()]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PopupComponent);
    componentUnderTest = fixture.componentInstance;
    ngxSmartModalComponent = fixture.debugElement.query(By.css('ngx-smart-modal'))
      .componentInstance;
  });

  describe('set show()', () => {
    it('should show the popup', () => {
      const openListener = jasmine.createSpy('openListener');

      ngxSmartModalComponent.onOpen.subscribe(openListener);
      componentUnderTest.show = true;

      expect(ngxSmartModalComponent.isVisible()).toBe(true);
      expect(openListener).toHaveBeenCalled();
    });

    it('should hide the popup when setted to false', () => {
      const closeListener = jasmine.createSpy('closeListener');

      ngxSmartModalComponent.onClose.subscribe(() => {
        console.log('%cCerro el popup!', 'color:green;font-weight:bolder');
        closeListener();
      });
      componentUnderTest.show = true;
      componentUnderTest.show = false;
      fixture.detectChanges();

      expect(closeListener).toHaveBeenCalled();
    });
  });

  describe('confirmButtonClicked()', () => {
    it('should emit an event', () => {
      const confirmListener = jasmine.createSpy('confirmListener');

      componentUnderTest.confirmAction.subscribe(confirmListener);
      componentUnderTest.confirm();

      expect(confirmListener).toHaveBeenCalled();
    });

    it('should close the modal', () => {
      const closeListener = jasmine.createSpy('closeListener');

      ngxSmartModalComponent.onClose.subscribe(closeListener);
      componentUnderTest.confirm();

      expect(closeListener).toHaveBeenCalled();
    });
  });
});
