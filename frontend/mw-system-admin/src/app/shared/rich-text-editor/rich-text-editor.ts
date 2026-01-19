import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  OnDestroy,
  ElementRef,
  ViewChild,
  forwardRef,
  signal,
  computed
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { MedicationService } from '../../services/medication';
import { ExamCatalogService } from '../../services/exam-catalog';
import { MedicationAutocomplete } from '../../models/medication.model';
import { ExamAutocomplete } from '../../models/exam-catalog.model';

export interface AutocompleteItem {
  id: string;
  text: string;
  type: 'medication' | 'exam';
  originalItem: MedicationAutocomplete | ExamAutocomplete;
}

@Component({
  selector: 'app-rich-text-editor',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rich-text-editor.html',
  styleUrl: './rich-text-editor.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RichTextEditor),
      multi: true
    }
  ]
})
export class RichTextEditor implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() placeholder = '';
  @Input() rows = 4;
  @Input() enableMedicationAutocomplete = true;
  @Input() enableExamAutocomplete = true;
  @Input() medicationTrigger = '@@';
  @Input() examTrigger = '##';
  @Input() label = '';
  @Input() id = '';
  @Input() minSearchLength = 2;
  @Input() searchDebounceMs = 300;

  @Output() medicationSelected = new EventEmitter<MedicationAutocomplete>();
  @Output() examSelected = new EventEmitter<ExamAutocomplete>();

  @ViewChild('editor') editorRef!: ElementRef<HTMLTextAreaElement>;

  content = signal<string>('');
  showAutocomplete = signal<boolean>(false);
  autocompleteItems = signal<AutocompleteItem[]>([]);
  autocompleteType = signal<'medication' | 'exam' | null>(null);
  autocompletePosition = signal<{ top: number; left: number }>({ top: 0, left: 0 });
  selectedIndex = signal<number>(0);
  isLoading = signal<boolean>(false);
  searchTerm = signal<string>('');
  triggerStartPosition = signal<number>(0);

  private searchSubject = new Subject<{ term: string; type: 'medication' | 'exam' }>();
  private destroy$ = new Subject<void>();
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};
  private isDisabled = false;

  constructor(
    private medicationService: MedicationService,
    private examCatalogService: ExamCatalogService
  ) {}

  ngOnInit(): void {
    this.searchSubject
      .pipe(debounceTime(this.searchDebounceMs), distinctUntilChanged((a, b) => a.term === b.term && a.type === b.type), takeUntil(this.destroy$))
      .subscribe(({ term, type }) => {
        this.performSearch(term, type);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ControlValueAccessor implementation
  writeValue(value: string): void {
    this.content.set(value || '');
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  // Event handlers
  onInput(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    const value = textarea.value;
    this.content.set(value);
    this.onChange(value);

    this.checkForTrigger(textarea);
  }

  onKeyDown(event: KeyboardEvent): void {
    if (!this.showAutocomplete()) return;

    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.selectedIndex.update((i) => Math.min(i + 1, this.autocompleteItems().length - 1));
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.selectedIndex.update((i) => Math.max(i - 1, 0));
        break;
      case 'Enter':
        event.preventDefault();
        this.selectItem(this.selectedIndex());
        break;
      case 'Escape':
        event.preventDefault();
        this.closeAutocomplete();
        break;
      case 'Tab':
        if (this.autocompleteItems().length > 0) {
          event.preventDefault();
          this.selectItem(this.selectedIndex());
        }
        break;
    }
  }

  onBlur(): void {
    this.onTouched();
    // Delay closing to allow click on autocomplete items
    setTimeout(() => this.closeAutocomplete(), 200);
  }

  // Formatting methods
  applyFormat(format: string): void {
    const textarea = this.editorRef.nativeElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = textarea.value.substring(start, end);
    let formattedText = '';
    let cursorOffset = 0;

    switch (format) {
      case 'bold':
        formattedText = `**${selectedText}**`;
        cursorOffset = selectedText ? 0 : 2;
        break;
      case 'italic':
        formattedText = `_${selectedText}_`;
        cursorOffset = selectedText ? 0 : 1;
        break;
      case 'underline':
        formattedText = `__${selectedText}__`;
        cursorOffset = selectedText ? 0 : 2;
        break;
      case 'list':
        formattedText = `\n- ${selectedText}`;
        cursorOffset = selectedText ? 0 : 3;
        break;
      case 'numbered':
        formattedText = `\n1. ${selectedText}`;
        cursorOffset = selectedText ? 0 : 4;
        break;
      case 'heading':
        formattedText = `\n## ${selectedText}`;
        cursorOffset = selectedText ? 0 : 4;
        break;
      default:
        return;
    }

    const newValue = textarea.value.substring(0, start) + formattedText + textarea.value.substring(end);
    this.content.set(newValue);
    this.onChange(newValue);

    // Restore focus and cursor position
    setTimeout(() => {
      textarea.focus();
      const newPos = start + formattedText.length - cursorOffset;
      textarea.setSelectionRange(newPos, newPos);
    }, 0);
  }

  insertMedicationTrigger(): void {
    this.insertText(this.medicationTrigger);
  }

  insertExamTrigger(): void {
    this.insertText(this.examTrigger);
  }

  private insertText(text: string): void {
    const textarea = this.editorRef.nativeElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;

    const newValue = textarea.value.substring(0, start) + text + textarea.value.substring(end);
    this.content.set(newValue);
    this.onChange(newValue);

    setTimeout(() => {
      textarea.focus();
      const newPos = start + text.length;
      textarea.setSelectionRange(newPos, newPos);
      this.checkForTrigger(textarea);
    }, 0);
  }

  private checkForTrigger(textarea: HTMLTextAreaElement): void {
    const value = textarea.value;
    const cursorPos = textarea.selectionStart;

    // Look backwards from cursor to find trigger
    const textBeforeCursor = value.substring(0, cursorPos);

    if (this.enableMedicationAutocomplete) {
      const medicationTriggerIndex = textBeforeCursor.lastIndexOf(this.medicationTrigger);
      if (medicationTriggerIndex !== -1) {
        const searchText = textBeforeCursor.substring(medicationTriggerIndex + this.medicationTrigger.length);
        // Check if there's no space or newline between trigger and cursor
        if (!searchText.includes(' ') && !searchText.includes('\n')) {
          this.triggerStartPosition.set(medicationTriggerIndex);
          this.searchTerm.set(searchText);
          this.autocompleteType.set('medication');
          this.showAutocomplete.set(true);
          this.updateAutocompletePosition(textarea);
          this.searchSubject.next({ term: searchText, type: 'medication' });
          return;
        }
      }
    }

    if (this.enableExamAutocomplete) {
      const examTriggerIndex = textBeforeCursor.lastIndexOf(this.examTrigger);
      if (examTriggerIndex !== -1) {
        const searchText = textBeforeCursor.substring(examTriggerIndex + this.examTrigger.length);
        // Check if there's no space or newline between trigger and cursor
        if (!searchText.includes(' ') && !searchText.includes('\n')) {
          this.triggerStartPosition.set(examTriggerIndex);
          this.searchTerm.set(searchText);
          this.autocompleteType.set('exam');
          this.showAutocomplete.set(true);
          this.updateAutocompletePosition(textarea);
          this.searchSubject.next({ term: searchText, type: 'exam' });
          return;
        }
      }
    }

    this.closeAutocomplete();
  }

  private updateAutocompletePosition(textarea: HTMLTextAreaElement): void {
    // Calculate position based on textarea
    const rect = textarea.getBoundingClientRect();
    const lineHeight = parseInt(getComputedStyle(textarea).lineHeight) || 20;
    const lines = textarea.value.substring(0, textarea.selectionStart).split('\n');
    const currentLine = lines.length - 1;

    this.autocompletePosition.set({
      top: Math.min(rect.height - 10, currentLine * lineHeight + lineHeight + 40),
      left: 10
    });
  }

  private performSearch(term: string, type: 'medication' | 'exam'): void {
    if (term.length < this.minSearchLength) {
      this.autocompleteItems.set([]);
      return;
    }

    this.isLoading.set(true);

    if (type === 'medication') {
      this.medicationService.search(term).subscribe({
        next: (results) => {
          this.autocompleteItems.set(
            results.map((item) => ({
              id: item.id,
              text: `${item.name} ${item.dosage} - ${item.pharmaceuticalForm}`,
              type: 'medication' as const,
              originalItem: item
            }))
          );
          this.selectedIndex.set(0);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.autocompleteItems.set([]);
        }
      });
    } else {
      this.examCatalogService.search(term).subscribe({
        next: (results) => {
          this.autocompleteItems.set(
            results.map((item) => ({
              id: item.id,
              text: item.name,
              type: 'exam' as const,
              originalItem: item
            }))
          );
          this.selectedIndex.set(0);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.autocompleteItems.set([]);
        }
      });
    }
  }

  selectItem(index: number): void {
    const items = this.autocompleteItems();
    if (index < 0 || index >= items.length) return;

    const selectedItem = items[index];
    const textarea = this.editorRef.nativeElement;
    const triggerStart = this.triggerStartPosition();
    const cursorPos = textarea.selectionStart;
    const triggerLength = this.autocompleteType() === 'medication' ? this.medicationTrigger.length : this.examTrigger.length;

    // Replace trigger + search text with selected item
    const newValue =
      textarea.value.substring(0, triggerStart) + selectedItem.text + textarea.value.substring(cursorPos);

    this.content.set(newValue);
    this.onChange(newValue);

    // Emit event based on type
    if (selectedItem.type === 'medication') {
      this.medicationSelected.emit(selectedItem.originalItem as MedicationAutocomplete);
    } else {
      this.examSelected.emit(selectedItem.originalItem as ExamAutocomplete);
    }

    this.closeAutocomplete();

    // Restore focus and move cursor after inserted text
    setTimeout(() => {
      textarea.focus();
      const newPos = triggerStart + selectedItem.text.length;
      textarea.setSelectionRange(newPos, newPos);
    }, 0);
  }

  private closeAutocomplete(): void {
    this.showAutocomplete.set(false);
    this.autocompleteItems.set([]);
    this.autocompleteType.set(null);
    this.selectedIndex.set(0);
    this.searchTerm.set('');
  }
}
