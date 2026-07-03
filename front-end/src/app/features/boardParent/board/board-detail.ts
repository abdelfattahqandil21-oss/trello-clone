import { Component, computed, input } from '@angular/core';
import type { Board } from '../../../core/models/board';

function mockBoard(id: string): Board {
  const boards: Record<string, Board> = {
    '1': {
      id: '1',
      name: 'Design Sprint',
      description: 'Q3 product design tasks',
      backgroundColor: '#2563eb',
      backgroundImgUrl: null,
      visibility: 'private',
      isArchived: false,
      archivedAt: null,
      createdAt: '',
      updatedAt: '',
      lists: [
        {
          id: 'l1',
          boardId: '1',
          name: 'To Do',
          position: 1000,
          isArchived: false,
          archivedAt: null,
          createdAt: '',
          updatedAt: '',
          cards: [
            { id: 'c1', listId: 'l1', title: 'Design system audit', position: 1000 },
            { id: 'c2', listId: 'l1', title: 'User flow diagrams', position: 2000 },
          ],
        },
        {
          id: 'l2',
          boardId: '1',
          name: 'In Progress',
          position: 2000,
          isArchived: false,
          archivedAt: null,
          createdAt: '',
          updatedAt: '',
          cards: [{ id: 'c3', listId: 'l2', title: 'Homepage wireframes', position: 1000 }],
        },
        {
          id: 'l3',
          boardId: '1',
          name: 'Done',
          position: 3000,
          isArchived: false,
          archivedAt: null,
          createdAt: '',
          updatedAt: '',
          cards: [
            { id: 'c4', listId: 'l3', title: 'Competitor analysis', position: 1000 },
            { id: 'c5', listId: 'l3', title: 'Style guide draft', position: 2000 },
          ],
        },
      ],
    },
    '2': {
      id: '2',
      name: 'Development',
      description: 'Frontend & backend tasks',
      backgroundColor: '#7c3aed',
      backgroundImgUrl: null,
      visibility: 'team',
      isArchived: false,
      archivedAt: null,
      createdAt: '',
      updatedAt: '',
      lists: [
        {
          id: 'l4',
          boardId: '2',
          name: 'Backlog',
          position: 1000,
          isArchived: false,
          archivedAt: null,
          createdAt: '',
          updatedAt: '',
          cards: [{ id: 'c6', listId: 'l4', title: 'Set up CI/CD', position: 1000 }],
        },
        {
          id: 'l5',
          boardId: '2',
          name: 'Sprint',
          position: 2000,
          isArchived: false,
          archivedAt: null,
          createdAt: '',
          updatedAt: '',
          cards: [
            { id: 'c7', listId: 'l5', title: 'API endpoints', position: 1000 },
            { id: 'c8', listId: 'l5', title: 'Database schema', position: 2000 },
          ],
        },
      ],
    },
  };
  return (
    boards[id] ?? {
      id,
      name: 'Unknown Board',
      description: null,
      backgroundColor: 'var(--color-brand-600)',
      backgroundImgUrl: null,
      visibility: 'private',
      isArchived: false,
      archivedAt: null,
      createdAt: '',
      updatedAt: '',
    }
  );
}

@Component({
  selector: 'app-board-detail',
  template: `
    @if (board(); as board) {
      <div class="h-full flex flex-col bg-surface transition-colors duration-200 ">
        <div class="flex items-center gap-3 px-6 py-4 border-b border-border">
          <div
            class="w-10 h-10 rounded-lg shrink-0"
            [style.background-color]="board.backgroundColor ?? 'var(--color-brand-600)'"
          ></div>
          <h1 class="text-2xl font-bold text-text-primary">{{ board.name }}</h1>
        </div>

        <div class="flex-1 flex gap-4 overflow-x-auto p-6 bg-surface-secondary">
          @for (list of board.lists; track list.id) {
            <div
              class="min-w-72 bg-surface rounded-xl p-3 flex flex-col max-h-full border border-border-light shadow-sm"
            >
              <div class="flex items-center justify-between mb-3">
                <h3 class="font-semibold text-text-primary">{{ list.name }}</h3>
                <span
                  class="text-xs font-medium text-text-secondary bg-surface-secondary px-2 py-0.5 rounded-full border border-border"
                >
                  {{ list.cards?.length ?? 0 }}
                </span>
              </div>

              <div class="flex-1 space-y-2 overflow-y-auto">
                @for (card of list.cards; track card.id) {
                  <div
                    class="bg-surface-secondary rounded-lg p-3 shadow-xs hover:shadow-md hover:bg-surface-hover transition-all cursor-pointer border border-border"
                  >
                    <p class="text-text-primary text-sm">{{ card.title }}</p>
                  </div>
                }
              </div>
            </div>
          }

          <div class="min-w-72">
            <button
              class="w-full bg-surface hover:bg-surface-hover rounded-xl p-3 text-left text-text-muted transition-colors border border-border border-dashed"
            >
              <i class="pi pi-plus mr-2 text-xs"></i>
              Add List
            </button>
          </div>
        </div>
      </div>
    }
  `,
})
export class BoardDetail {
  protected readonly boardId = input.required<string>();
  protected readonly board = computed(() => mockBoard(this.boardId()));
}
