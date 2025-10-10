import { Component, OnInit, SecurityContext } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { MarkdownModule } from 'ngx-markdown';
import { DocumentationService } from '../../services/documentation.service';
import { DocItem } from '../../models/doc-item.model';

@Component({
  selector: 'app-doc-viewer',
  standalone: true,
  imports: [CommonModule, MarkdownModule],
  templateUrl: './doc-viewer.component.html',
  styleUrls: ['./doc-viewer.component.scss']
})
export class DocViewerComponent implements OnInit {
  docContent = '';
  currentDoc: DocItem | undefined;
  isLoading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private docService: DocumentationService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const docId = params['id'];
      this.loadDocument(docId);
    });
  }

  loadDocument(docId: string) {
    this.isLoading = true;
    this.error = '';
    
    const allDocs = this.docService.getAllDocs();
    this.currentDoc = allDocs.find(doc => doc.id === docId);

    if (!this.currentDoc) {
      this.error = 'Documento não encontrado';
      this.isLoading = false;
      return;
    }

    this.docService.getDocContent(this.currentDoc.path).subscribe({
      next: (content) => {
        this.docContent = content;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar documento: ' + err.message;
        this.isLoading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/']);
  }
}
