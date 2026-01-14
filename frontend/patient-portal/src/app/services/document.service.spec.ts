import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DocumentService } from './document.service';
import { Document } from '../models/document.model';

describe('DocumentService', () => {
  let service: DocumentService;
  let httpMock: HttpTestingController;

  const mockDocuments: Document[] = [
    {
      id: '1',
      title: 'Medical Prescription',
      documentType: 'Prescription',
      description: 'Amoxicillin 500mg',
      doctorName: 'Dr. Smith',
      issuedDate: new Date('2026-01-15T10:00:00Z'),
      fileUrl: '/documents/1/download',
      fileName: 'prescription_001.pdf',
      fileSizeFormatted: '100 KB',
      isAvailable: true
    },
    {
      id: '2',
      title: 'Blood Test Results',
      documentType: 'LabReport',
      description: 'Complete blood count',
      doctorName: 'Lab Tech',
      issuedDate: new Date('2026-01-10T14:30:00Z'),
      fileUrl: '/documents/2/download',
      fileName: 'lab_result_002.pdf',
      fileSizeFormatted: '200 KB',
      isAvailable: true
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DocumentService]
    });

    service = TestBed.inject(DocumentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getMyDocuments', () => {
    it('should retrieve documents with default pagination', (done) => {
      service.getMyDocuments().subscribe(documents => {
        expect(documents).toEqual(mockDocuments);
        expect(documents.length).toBe(2);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents?skip=0&take=50');
      expect(req.request.method).toBe('GET');
      req.flush(mockDocuments);
    });

    it('should retrieve documents with custom pagination', (done) => {
      service.getMyDocuments(10, 20).subscribe(documents => {
        expect(documents).toEqual(mockDocuments);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents?skip=10&take=20');
      expect(req.request.method).toBe('GET');
      req.flush(mockDocuments);
    });
  });

  describe('getRecentDocuments', () => {
    it('should retrieve recent documents with default limit', (done) => {
      const recentDocuments = [mockDocuments[0]];

      service.getRecentDocuments().subscribe(documents => {
        expect(documents).toEqual(recentDocuments);
        expect(documents.length).toBe(1);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/recent?take=5');
      expect(req.request.method).toBe('GET');
      req.flush(recentDocuments);
    });

    it('should retrieve recent documents with custom limit', (done) => {
      service.getRecentDocuments(10).subscribe(documents => {
        expect(documents).toEqual(mockDocuments);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/recent?take=10');
      expect(req.request.method).toBe('GET');
      req.flush(mockDocuments);
    });
  });

  describe('getDocumentById', () => {
    it('should retrieve a single document by id', (done) => {
      const document = mockDocuments[0];

      service.getDocumentById('1').subscribe(result => {
        expect(result).toEqual(document);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/1');
      expect(req.request.method).toBe('GET');
      req.flush(document);
    });

    it('should handle 404 when document not found', (done) => {
      service.getDocumentById('999').subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(404);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/999');
      req.flush({ message: 'Document not found' }, { status: 404, statusText: 'Not Found' });
    });
  });

  describe('getDocumentsByType', () => {
    it('should retrieve documents by type with default pagination', (done) => {
      const prescriptions = [mockDocuments[0]];

      service.getDocumentsByType('Prescription').subscribe(documents => {
        expect(documents).toEqual(prescriptions);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/type/Prescription?skip=0&take=50');
      expect(req.request.method).toBe('GET');
      req.flush(prescriptions);
    });

    it('should retrieve documents by type with custom pagination', (done) => {
      const labResults = [mockDocuments[1]];

      service.getDocumentsByType('LabReport', 5, 10).subscribe(documents => {
        expect(documents).toEqual(labResults);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/type/LabReport?skip=5&take=10');
      expect(req.request.method).toBe('GET');
      req.flush(labResults);
    });
  });

  describe('getDocumentsCount', () => {
    it('should retrieve documents count', (done) => {
      const countResponse = { count: 25 };

      service.getDocumentsCount().subscribe(result => {
        expect(result.count).toBe(25);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/count');
      expect(req.request.method).toBe('GET');
      req.flush(countResponse);
    });
  });

  describe('downloadDocument', () => {
    it('should download document as blob', (done) => {
      const mockBlob = new Blob(['PDF content'], { type: 'application/pdf' });

      service.downloadDocument('1').subscribe(blob => {
        expect(blob).toEqual(mockBlob);
        expect(blob.type).toBe('application/pdf');
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/1/download');
      expect(req.request.method).toBe('GET');
      expect(req.request.responseType).toBe('blob');
      req.flush(mockBlob);
    });

    it('should handle download errors', (done) => {
      service.downloadDocument('999').subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(404);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents/999/download');
      req.error(new ProgressEvent('error'), { status: 404, statusText: 'Not Found' });
    });
  });

  describe('error handling', () => {
    it('should handle network errors', (done) => {
      service.getMyDocuments().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(0);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents?skip=0&take=50');
      req.error(new ProgressEvent('error'), { status: 0, statusText: 'Network error' });
    });

    it('should handle server errors', (done) => {
      service.getMyDocuments().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(500);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/documents?skip=0&take=50');
      req.flush({ message: 'Internal server error' }, { status: 500, statusText: 'Internal Server Error' });
    });
  });
});
