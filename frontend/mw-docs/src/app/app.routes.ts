import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { DocViewerComponent } from './components/doc-viewer/doc-viewer.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'doc/:id', component: DocViewerComponent },
  { path: '**', redirectTo: '' }
];
