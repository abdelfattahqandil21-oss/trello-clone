import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'boards', pathMatch: 'full' },

  // Public routes (no shell)
  { path: 'login', loadComponent: () => import('./public/login/login').then((m) => m.Login) },
  {
    path: 'register',
    loadComponent: () => import('./public/register/register').then((m) => m.Register),
  },

  // App routes (inside shell)
  {
    path: '',
    loadComponent: () => import('./layout/shell').then((m) => m.Shell),
    children: [
      {
        path: '',
        loadChildren: () => import('./features/boardParent/board.routes'),
      },
    ],
  },
];
