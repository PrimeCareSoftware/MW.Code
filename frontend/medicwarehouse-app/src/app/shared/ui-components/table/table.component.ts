import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="table-wrap app-card">
      <table class="app-table">
        <thead>
          <tr>
            @for (header of headers; track header) {
              <th>{{ header }}</th>
            }
          </tr>
        </thead>
        <tbody>
          @for (row of rows; track $index) {
            <tr>
              @for (col of row; track $index) {
                <td>{{ col }}</td>
              }
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
  styles: [`.table-wrap{overflow:auto}`]
})
export class AppTableComponent {
  @Input() headers: string[] = [];
  @Input() rows: (string | number)[][] = [];
}
