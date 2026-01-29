import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AccessibleTableComponent, TableColumn } from './accessible-table.component';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';

describe('AccessibleTableComponent', () => {
  let component: AccessibleTableComponent;
  let fixture: ComponentFixture<AccessibleTableComponent>;

  const mockColumns: TableColumn[] = [
    { key: 'name', header: 'Nome', sortable: true },
    { key: 'email', header: 'E-mail', sortable: true },
    { key: 'role', header: 'Função', sortable: false }
  ];

  const mockData = [
    { name: 'João Silva', email: 'joao@example.com', role: 'Admin' },
    { name: 'Maria Santos', email: 'maria@example.com', role: 'User' },
    { name: 'Pedro Costa', email: 'pedro@example.com', role: 'Editor' }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AccessibleTableComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(AccessibleTableComponent);
    component = fixture.componentInstance;
    component.columns = mockColumns;
    component.data = mockData;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Accessibility Attributes', () => {
    it('should have proper ARIA labels', () => {
      const table = fixture.debugElement.query(By.css('table'));
      expect(table.nativeElement.getAttribute('aria-label')).toBe('Data table');
    });

    it('should have region wrapper with tabindex', () => {
      const wrapper = fixture.debugElement.query(By.css('.accessible-table-wrapper'));
      expect(wrapper.nativeElement.getAttribute('role')).toBe('region');
      expect(wrapper.nativeElement.getAttribute('tabindex')).toBe('0');
    });

    it('should have proper scope attributes on headers', () => {
      const headers = fixture.debugElement.queryAll(By.css('th'));
      headers.forEach(header => {
        expect(header.nativeElement.getAttribute('scope')).toBe('col');
      });
    });

    it('should have aria-sort on sortable columns', () => {
      const sortableHeaders = fixture.debugElement.queryAll(By.css('th[aria-sort]'));
      expect(sortableHeaders.length).toBe(2); // name and email
    });
  });

  describe('Sorting Functionality', () => {
    it('should sort data in ascending order on first click', () => {
      const sortButton = fixture.debugElement.query(By.css('.sort-button'));
      sortButton.nativeElement.click();
      fixture.detectChanges();

      expect(component.sortColumn).toBe('name');
      expect(component.sortDirection).toBe('asc');
      expect(component.data[0].name).toBe('João Silva');
    });

    it('should toggle sort direction on second click', () => {
      const sortButton = fixture.debugElement.query(By.css('.sort-button'));
      
      // First click - ascending
      sortButton.nativeElement.click();
      fixture.detectChanges();
      expect(component.sortDirection).toBe('asc');
      
      // Second click - descending
      sortButton.nativeElement.click();
      fixture.detectChanges();
      expect(component.sortDirection).toBe('desc');
    });

    it('should update aria-sort attribute when sorting', () => {
      const sortButton = fixture.debugElement.query(By.css('.sort-button'));
      const header = sortButton.parent!;
      
      sortButton.nativeElement.click();
      fixture.detectChanges();
      
      expect(header.nativeElement.getAttribute('aria-sort')).toBe('ascending');
    });

    it('should have descriptive aria-label on sort button', () => {
      const sortButton = fixture.debugElement.query(By.css('.sort-button'));
      const ariaLabel = sortButton.nativeElement.getAttribute('aria-label');
      
      expect(ariaLabel).toContain('Ordenar por');
    });
  });

  describe('Keyboard Navigation', () => {
    it('should sort on Enter key', () => {
      const sortButton = fixture.debugElement.query(By.css('.sort-button'));
      const event = new KeyboardEvent('keydown', { key: 'Enter' });
      
      sortButton.nativeElement.dispatchEvent(event);
      fixture.detectChanges();
      
      expect(component.sortColumn).toBe('name');
    });

    it('should sort on Space key', () => {
      const sortButton = fixture.debugElement.query(By.css('.sort-button'));
      const event = new KeyboardEvent('keydown', { key: ' ' });
      
      spyOn(event, 'preventDefault');
      sortButton.nativeElement.dispatchEvent(event);
      fixture.detectChanges();
      
      expect(component.sortColumn).toBe('name');
    });
  });

  describe('Empty State', () => {
    it('should show empty message when no data', () => {
      component.data = [];
      fixture.detectChanges();
      
      const emptyState = fixture.debugElement.query(By.css('.empty-state'));
      expect(emptyState).toBeTruthy();
      expect(emptyState.nativeElement.textContent.trim()).toBe('Nenhum dado disponível');
    });

    it('should have proper colspan for empty state', () => {
      component.data = [];
      fixture.detectChanges();
      
      const emptyState = fixture.debugElement.query(By.css('.empty-state'));
      expect(emptyState.nativeElement.getAttribute('colspan')).toBe('3');
    });
  });

  describe('Caption and Summary', () => {
    it('should show caption when showCaption is true', () => {
      component.showCaption = true;
      component.caption = 'User List';
      fixture.detectChanges();
      
      const caption = fixture.debugElement.query(By.css('caption'));
      expect(caption).toBeTruthy();
      expect(caption.nativeElement.textContent).toBe('User List');
    });

    it('should hide caption when showCaption is false', () => {
      component.showCaption = false;
      fixture.detectChanges();
      
      const caption = fixture.debugElement.query(By.css('caption'));
      expect(caption).toBeFalsy();
    });

    it('should render summary for screen readers', () => {
      component.summary = 'Tabela com 3 usuários';
      fixture.detectChanges();
      
      const summary = fixture.debugElement.query(By.css('.sr-only'));
      expect(summary).toBeTruthy();
      expect(summary.nativeElement.textContent).toBe('Tabela com 3 usuários');
    });
  });
});
