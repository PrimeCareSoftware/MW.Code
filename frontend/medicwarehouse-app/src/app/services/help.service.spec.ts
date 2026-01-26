import { TestBed } from '@angular/core/testing';
import { HelpService } from './help.service';

describe('HelpService', () => {
  let service: HelpService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HelpService]
    });
    service = TestBed.inject(HelpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return help content for patients module', () => {
    const content = service.getHelpContent('patients');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo de Pacientes');
    expect(content?.sections.length).toBeGreaterThan(0);
  });

  it('should return help content for appointments module', () => {
    const content = service.getHelpContent('appointments');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo de Agendamentos');
    expect(content?.sections.length).toBeGreaterThan(0);
  });

  it('should return help content for attendance module', () => {
    const content = service.getHelpContent('attendance');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo de Atendimento');
  });

  it('should return help content for prescriptions module', () => {
    const content = service.getHelpContent('prescriptions');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo de Prescrições');
  });

  it('should return help content for financial module', () => {
    const content = service.getHelpContent('financial');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo Financeiro');
  });

  it('should return help content for tiss module', () => {
    const content = service.getHelpContent('tiss');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo TISS/TUSS');
  });

  it('should return help content for telemedicine module', () => {
    const content = service.getHelpContent('telemedicine');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo de Telemedicina');
  });

  it('should return help content for soap-records module', () => {
    const content = service.getHelpContent('soap-records');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo SOAP');
  });

  it('should return help content for anamnesis module', () => {
    const content = service.getHelpContent('anamnesis');
    expect(content).toBeDefined();
    expect(content?.title).toBe('Ajuda - Módulo de Anamnese');
  });

  it('should return undefined for non-existent module', () => {
    const content = service.getHelpContent('non-existent-module');
    expect(content).toBeUndefined();
  });

  it('should have test data in patients module help', () => {
    const content = service.getHelpContent('patients');
    const firstSection = content?.sections[0];
    expect(firstSection?.testData).toBeDefined();
    expect(firstSection?.testData!.length).toBeGreaterThan(0);
    
    const firstTestData = firstSection?.testData![0];
    expect(firstTestData?.field).toBeDefined();
    expect(firstTestData?.validExample).toBeDefined();
    expect(firstTestData?.description).toBeDefined();
  });

  it('should have test data in appointments module help', () => {
    const content = service.getHelpContent('appointments');
    const firstSection = content?.sections[0];
    expect(firstSection?.testData).toBeDefined();
    expect(firstSection?.testData!.length).toBeGreaterThan(0);
  });

  it('should have HTML content in sections', () => {
    const content = service.getHelpContent('patients');
    const firstSection = content?.sections[0];
    expect(firstSection?.content).toContain('<');
    expect(firstSection?.content).toContain('>');
  });
});
