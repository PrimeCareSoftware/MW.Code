import { TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('MatSnackBar', ['open']);

    TestBed.configureTestingModule({
      providers: [
        NotificationService,
        { provide: MatSnackBar, useValue: spy }
      ]
    });

    service = TestBed.inject(NotificationService);
    snackBarSpy = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should show success notification', () => {
    const message = 'Success message';
    service.success(message);

    expect(snackBarSpy.open).toHaveBeenCalledWith(
      message,
      'OK',
      jasmine.objectContaining({
        duration: 5000,
        panelClass: ['snackbar-success']
      })
    );
  });

  it('should show error notification with longer duration', () => {
    const message = 'Error message';
    service.error(message);

    expect(snackBarSpy.open).toHaveBeenCalledWith(
      message,
      'OK',
      jasmine.objectContaining({
        duration: 7000,
        panelClass: ['snackbar-error']
      })
    );
  });

  it('should show warning notification', () => {
    const message = 'Warning message';
    service.warning(message);

    expect(snackBarSpy.open).toHaveBeenCalledWith(
      message,
      'OK',
      jasmine.objectContaining({
        duration: 5000,
        panelClass: ['snackbar-warning']
      })
    );
  });

  it('should show info notification', () => {
    const message = 'Info message';
    service.info(message);

    expect(snackBarSpy.open).toHaveBeenCalledWith(
      message,
      'OK',
      jasmine.objectContaining({
        duration: 5000,
        panelClass: ['snackbar-info']
      })
    );
  });

  it('should use custom action text', () => {
    const message = 'Message';
    const action = 'Close';
    service.success(message, action);

    expect(snackBarSpy.open).toHaveBeenCalledWith(
      message,
      action,
      jasmine.any(Object)
    );
  });
});
