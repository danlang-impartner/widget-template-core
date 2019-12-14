import { DebugElement } from '@angular/core';
import { async, ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, Column, IDatasource } from 'ag-grid-community';

import { GridComponent } from './grid.component';

describe('grid.component.ts', () => {
  let component: GridComponent;
  let fixture: ComponentFixture<GridComponent>;
  let debugElement: DebugElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [AgGridModule.withComponents([])],
      declarations: [GridComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GridComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;

    fixture.detectChanges();
  });

  describe('set metadata()', () => {
    it('should create a header', () => {
      let headerNames: string[] = [];

      component.agGridReference.gridColumnsChanged.subscribe((agGridReference: any) => {
        const columnDef: ColDef[] = agGridReference.columnApi
          .getAllColumns()
          .map((column: Column) => column.getColDef());
        headerNames = columnDef.map<string>(headerDom => {
          return headerDom.headerName as string;
        });

        expect(JSON.stringify(headerNames)).toBe('["Name","Company","Source"]');
      });

      component.metadata = `[
        { "headerName": "Name", "field": "name", "filter": "agTextColumnFilter" },
        { "headerName": "Company", "field": "company.name", "filter": "agTextColumnFilter" },
        { "headerName": "Source", "field": "source", "filter": "agTextColumnFilter" }
      ]`;

      fixture.detectChanges();
    });

    it('should throw an error when the JSON passed as argument isn\'t valid', () => {
      expect(() => {
        component.metadata = '[{field: \'fake\'}]';
      }).toThrowError();
    });
  });

  describe('set data()', () => {
    let agGridDebugElement: DebugElement;
    let testData: object[];

    beforeEach(() => {
      agGridDebugElement = fixture.debugElement.query(By.css('ag-grid-angular'));
      testData = [
        { name: 'Deal #1', company: 'Test Corp.', Source: 'Online' },
        { name: 'Deal #2', company: 'Test Corp.', Source: 'Online' },
        { name: 'Deal #3', company: 'Test Corp.', Source: 'Retargeting' }
      ];
    });

    it('should set data from data attribute', () => {
      spyOn(component.agGridReference.api, 'setRowData');
      component.editing = false;
      component.data = JSON.stringify(testData);
      fixture.detectChanges();

      expect(component.agGridReference.api.setRowData).toHaveBeenCalledWith(testData);
    });

    it('should prevent to update data in agGrid when is in edit mode', () => {
      spyOn(component.agGridReference.api, 'setRowData');
      component.editing = true;
      component.data = JSON.stringify(testData);

      fixture.detectChanges();

      expect(component.agGridReference.api.setRowData).not.toHaveBeenCalled();
    });

    it('should throw error when there is an invalid JSON string', () => {
      const invalidJson = '{name: invalid}';

      expect(() => {
        component.editing = false;
        component.data = invalidJson;
        fixture.detectChanges();
      }).toThrowError();
    });
  });

  describe('set datasource()', () => {
    let datasource: jasmine.SpyObj<IDatasource>;

    beforeEach(() => {
      datasource = jasmine.createSpyObj('IDatasource', ['getRows']);
    });

    it('should set the new datasource in the agGrid component', fakeAsync(() => {
      component.editing = false;
      component.datasource = datasource;

      fixture.detectChanges();
      tick();

      expect(datasource.getRows).toHaveBeenCalled();
    }));

    it('shouldn\'t set the datasource to agGrid when is in editing mode', fakeAsync(() => {
      component.editing = true;
      component.datasource = datasource;

      fixture.detectChanges();
      tick();

      expect(datasource.getRows).not.toHaveBeenCalled();
    }));
  });

  describe('set editing()', () => {
    let datasource: jasmine.SpyObj<IDatasource>;

    beforeEach(() => {
      datasource = jasmine.createSpyObj('IDatasource', ['getRows']);
    });

    it('should change between edit and build mode', () => {
      const agGridDebugElement = fixture.debugElement.query(By.css('ag-grid-angular'));

      component.agGridReference.rowDataChanged.subscribe((data: any) => {
        const cellDataElements: HTMLElement[] = agGridDebugElement
          .queryAll(By.css('[row-index] > .ag-cell'))
          .map(el => el.nativeElement);

        expect(cellDataElements.length).toBe(0);
      });

      component.editing = false;

      fixture.detectChanges();
    });

    it('should enable the datasource to be showed in agGrid', fakeAsync(() => {
      component.editing = true;
      component.datasource = datasource;
      component.editing = false;

      fixture.detectChanges();
      tick();

      expect(datasource.getRows).toHaveBeenCalled();
    }));
  });
});
