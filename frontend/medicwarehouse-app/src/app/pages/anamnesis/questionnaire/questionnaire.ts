import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { Navbar } from '../../../shared/navbar/navbar';
import { AnamnesisService } from '../../../services/anamnesis.service';
import { 
  AnamnesisTemplate, 
  AnamnesisResponse, 
  QuestionAnswer,
  QuestionType,
  Question
} from '../../../models/anamnesis.model';

interface FormAnswer {
  questionText: string;
  type: QuestionType;
  value: any;
}

@Component({
  selector: 'app-questionnaire',
  imports: [CommonModule, RouterLink, FormsModule, Navbar],
  templateUrl: './questionnaire.html',
  styleUrl: './questionnaire.scss'
})
export class QuestionnaireComponent implements OnInit {
  template = signal<AnamnesisTemplate | null>(null);
  response = signal<AnamnesisResponse | null>(null);
  answers = signal<Map<string, FormAnswer>>(new Map());
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  appointmentId: string = '';
  templateId: string = '';
  
  QuestionType = QuestionType;
  
  completionPercentage = computed(() => {
    const tmpl = this.template();
    if (!tmpl) return 0;
    
    const totalQuestions = tmpl.sections.reduce(
      (sum, section) => sum + section.questions.length,
      0
    );
    
    if (totalQuestions === 0) return 0;
    
    const answeredQuestions = Array.from(this.answers().values()).filter(
      answer => this.isAnswerProvided(answer.value)
    ).length;
    
    return Math.round((answeredQuestions / totalQuestions) * 100);
  });

  constructor(
    private anamnesisService: AnamnesisService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.appointmentId = params['appointmentId'];
    });
    
    this.route.queryParams.subscribe(params => {
      this.templateId = params['templateId'];
    });

    if (!this.appointmentId || !this.templateId) {
      this.errorMessage.set('Parâmetros inválidos');
      return;
    }

    this.loadTemplateAndResponse();
  }

  loadTemplateAndResponse(): void {
    this.isLoading.set(true);
    
    // Load template
    this.anamnesisService.getTemplateById(this.templateId).subscribe({
      next: (template) => {
        this.template.set(template);
        this.initializeAnswers(template);
        
        // Try to load existing response
        this.loadExistingResponse();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar template');
        this.isLoading.set(false);
        console.error('Error loading template:', error);
      }
    });
  }

  loadExistingResponse(): void {
    this.anamnesisService.getResponseByAppointment(this.appointmentId).subscribe({
      next: (response) => {
        this.response.set(response);
        this.populateAnswers(response.answers);
        this.isLoading.set(false);
      },
      error: (error) => {
        // No existing response, that's ok
        this.isLoading.set(false);
      }
    });
  }

  initializeAnswers(template: AnamnesisTemplate): void {
    const newAnswers = new Map<string, FormAnswer>();
    
    template.sections.forEach(section => {
      section.questions.forEach(question => {
        const key = this.getQuestionKey(question);
        newAnswers.set(key, {
          questionText: question.questionText,
          type: question.type,
          value: null
        });
      });
    });
    
    this.answers.set(newAnswers);
  }

  populateAnswers(questionAnswers: QuestionAnswer[]): void {
    const currentAnswers = new Map(this.answers());
    
    questionAnswers.forEach(qa => {
      const key = this.getQuestionKey({ questionText: qa.questionText } as Question);
      const existing = currentAnswers.get(key);
      
      if (existing) {
        existing.value = this.convertAnswerValue(qa);
        currentAnswers.set(key, existing);
      }
    });
    
    this.answers.set(currentAnswers);
  }

  getQuestionKey(question: Question | { questionText: string }): string {
    // Generate a more unique key by including position hash to avoid collisions
    const baseKey = question.questionText
      .replace(/[^a-zA-Z0-9]/g, '_')
      .toLowerCase();
    return baseKey;
  }

  private isAnswerProvided(value: any): boolean {
    // Check if an answer has been provided
    // For arrays (MultipleChoice), check if it has items
    // For numbers and scale, 0 is valid
    // For strings, empty string is not valid
    // For booleans, any value is valid
    if (value === null || value === undefined) return false;
    if (Array.isArray(value)) return value.length > 0;
    if (typeof value === 'string') return value.trim() !== '';
    if (typeof value === 'number') return true; // 0 is valid
    if (typeof value === 'boolean') return true;
    return false;
  }

  getAnswer(question: Question): any {
    const key = this.getQuestionKey(question);
    return this.answers().get(key)?.value;
  }

  setAnswer(question: Question, value: any): void {
    const key = this.getQuestionKey(question);
    const currentAnswers = new Map(this.answers());
    const answer = currentAnswers.get(key);
    
    if (answer) {
      answer.value = value;
      currentAnswers.set(key, answer);
      this.answers.set(currentAnswers);
    }
  }

  toggleMultipleChoice(question: Question, option: string): void {
    const key = this.getQuestionKey(question);
    const currentAnswers = new Map(this.answers());
    const answer = currentAnswers.get(key);
    
    if (answer) {
      const currentValue = Array.isArray(answer.value) ? answer.value : [];
      const index = currentValue.indexOf(option);
      
      if (index > -1) {
        currentValue.splice(index, 1);
      } else {
        currentValue.push(option);
      }
      
      answer.value = [...currentValue];
      currentAnswers.set(key, answer);
      this.answers.set(currentAnswers);
    }
  }

  isOptionSelected(question: Question, option: string): boolean {
    const value = this.getAnswer(question);
    return Array.isArray(value) && value.includes(option);
  }

  buildQuestionAnswers(): QuestionAnswer[] {
    const questionAnswers: QuestionAnswer[] = [];
    
    this.answers().forEach((answer) => {
      if (this.isAnswerProvided(answer.value)) {
        const qa: QuestionAnswer = {
          questionText: answer.questionText,
          type: answer.type,
          answer: String(answer.value)
        };
        
        // Add type-specific fields
        switch (answer.type) {
          case QuestionType.YesNo:
            qa.booleanValue = answer.value === 'yes' ? true : (answer.value === 'no' ? false : undefined);
            break;
          case QuestionType.Number:
          case QuestionType.Scale:
            qa.numericValue = this.parseNumericValue(answer.value);
            break;
          case QuestionType.MultipleChoice:
            qa.selectedOptions = Array.isArray(answer.value) ? answer.value : [];
            break;
          case QuestionType.Date:
            qa.dateValue = answer.value;
            break;
        }
        
        questionAnswers.push(qa);
      }
    });
    
    return questionAnswers;
  }

  async save(isComplete: boolean): Promise<void> {
    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    
    const questionAnswers = this.buildQuestionAnswers();
    
    try {
      // Create response if it doesn't exist
      if (!this.response()) {
        const createResponse = await firstValueFrom(
          this.anamnesisService.createResponse({
            appointmentId: this.appointmentId,
            templateId: this.templateId
          })
        );
        
        if (createResponse) {
          this.response.set(createResponse);
        }
      }
      
      const responseId = this.response()?.id;
      if (!responseId) {
        throw new Error('Falha ao criar resposta');
      }
      
      // Save answers
      const updatedResponse = await firstValueFrom(
        this.anamnesisService.saveAnswers(responseId, {
          answers: questionAnswers,
          isComplete: isComplete
        })
      );
      
      if (updatedResponse) {
        this.response.set(updatedResponse);
        this.successMessage.set(isComplete ? 'Anamnese finalizada com sucesso!' : 'Rascunho salvo com sucesso!');
        
        if (isComplete) {
          setTimeout(() => {
            this.router.navigate(['/appointments', this.appointmentId, 'attendance']);
          }, 1500);
        }
      }
    } catch (error: any) {
      this.errorMessage.set('Erro ao salvar: ' + (error.message || 'Erro desconhecido'));
      console.error('Error saving:', error);
    } finally {
      this.isSaving.set(false);
    }
  }

  async onSaveDraft(): Promise<void> {
    await this.save(false);
  }

  async onFinalize(): Promise<void> {
    if (!this.validateRequiredFields()) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios');
      return;
    }
    
    await this.save(true);
  }

  validateRequiredFields(): boolean {
    const tmpl = this.template();
    if (!tmpl) return false;
    
    for (const section of tmpl.sections) {
      for (const question of section.questions) {
        if (question.isRequired && !this.isAnswerProvided(question)) {
          return false;
        }
      }
    }
    
    return true;
  }

  onCancel(): void {
    if (confirm('Deseja sair sem salvar as alterações?')) {
      this.router.navigate(['/appointments', this.appointmentId, 'attendance']);
    }
  }

  private convertAnswerValue(qa: QuestionAnswer): any {
    switch (qa.type) {
      case QuestionType.YesNo:
        return qa.booleanValue === true ? 'yes' : (qa.booleanValue === false ? 'no' : null);
      case QuestionType.Number:
      case QuestionType.Scale:
        return qa.numericValue;
      case QuestionType.MultipleChoice:
        return qa.selectedOptions || [];
      case QuestionType.Date:
        return qa.dateValue;
      default:
        return qa.answer;
    }
  }

  private parseNumericValue(value: any): number | undefined {
    const numValue = Number(value);
    return isNaN(numValue) ? undefined : numValue;
  }
}
