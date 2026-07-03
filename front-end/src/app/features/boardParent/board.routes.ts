import { Routes } from '@angular/router';

export default [
  { path: '', redirectTo: 'boards', pathMatch: 'full' },
  { path: 'boards', loadComponent: () => import('./boards/board-list').then((m) => m.BoardList) },
  {
    path: 'board/:id',
    loadComponent: () => import('./board/board-detail').then((m) => m.BoardDetail),
  },
] satisfies Routes