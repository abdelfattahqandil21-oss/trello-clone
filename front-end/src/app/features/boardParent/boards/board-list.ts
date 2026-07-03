import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import type { Board } from '../../../core/models/board';

const MOCK_BOARDS: Board[] = [
  { id: '1', name: 'Design Sprint', description: 'Q3 product design tasks', backgroundColor: '#2563eb', backgroundImgUrl: null, visibility: 'private', isArchived: false, archivedAt: null, createdAt: '', updatedAt: '' },
  { id: '2', name: 'Development', description: 'Frontend & backend tasks', backgroundColor: '#7c3aed', backgroundImgUrl: null, visibility: 'team', isArchived: false, archivedAt: null, createdAt: '', updatedAt: '' },
  { id: '3', name: 'Marketing', description: null, backgroundColor: '#059669', backgroundImgUrl: null, visibility: 'public', isArchived: false, archivedAt: null, createdAt: '', updatedAt: '' },
  { id: '4', name: 'Research', description: 'User research & analytics', backgroundColor: '#d97706', backgroundImgUrl: null, visibility: 'private', isArchived: false, archivedAt: null, createdAt: '', updatedAt: '' },
  { id: '5', name: 'Ideas', description: 'Feature brainstorming', backgroundColor: '#dc2626', backgroundImgUrl: null, visibility: 'team', isArchived: false, archivedAt: null, createdAt: '', updatedAt: '' },
  { id: '6', name: 'Personal', description: 'My personal tasks', backgroundColor: '#0891b2', backgroundImgUrl: null, visibility: 'private', isArchived: false, archivedAt: null, createdAt: '', updatedAt: '' },
];

@Component({
  selector: 'app-board-list',
  imports: [RouterLink],
  template: `
    <div class="p-6">
      <div class="flex items-center justify-between mb-6">
        <h1 class="text-2xl font-bold text-text-primary">
          <i class="pi pi-th-large mr-2 text-brand-primary"></i>
          Boards
        </h1>
        <button class="bg-brand-primary text-white px-4 py-2 rounded-lg hover:bg-brand-accent transition-colors flex items-center gap-2 cursor-pointer">
          <i class="pi pi-plus"></i>
          New Board
        </button>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        @for (board of boards(); track board.id) {
          <a
            [routerLink]="['/board', board.id]"
            class="block p-4 rounded-lg shadow-md hover:shadow-lg transition-shadow"
            [style.background-color]="board.backgroundColor ?? 'var(--color-brand-primary)'"
          >
            <h3 class="text-white font-semibold text-lg">{{ board.name }}</h3>
            @if (board.description) {
              <p class="text-white/80 text-sm mt-1 line-clamp-2">{{ board.description }}</p>
            }
          </a>
        }
      </div>
    </div>
  `
})
export class BoardList {
  protected readonly boards = signal<Board[]>(MOCK_BOARDS);
}
