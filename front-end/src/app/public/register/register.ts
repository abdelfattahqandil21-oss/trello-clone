import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [RouterLink],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-surface-secondary p-4">
      <div class="w-full max-w-sm bg-surface rounded-xl shadow-lg p-8">
        <div class="text-center mb-8">
          <i class="pi pi-trello text-4xl text-brand-600"></i>
          <h1 class="text-2xl font-bold text-text-primary mt-2">TreClone</h1>
          <p class="text-text-muted text-sm">Create your account</p>
        </div>

        <form class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-text-secondary mb-1">Name</label>
            <input type="text" class="w-full px-3 py-2 rounded-lg border border-border bg-surface text-text-primary focus:outline-none focus:ring-2 focus:ring-brand-500" placeholder="Your name" />
          </div>
          <div>
            <label class="block text-sm font-medium text-text-secondary mb-1">Email</label>
            <input type="email" class="w-full px-3 py-2 rounded-lg border border-border bg-surface text-text-primary focus:outline-none focus:ring-2 focus:ring-brand-500" placeholder="you@example.com" />
          </div>
          <div>
            <label class="block text-sm font-medium text-text-secondary mb-1">Password</label>
            <input type="password" class="w-full px-3 py-2 rounded-lg border border-border bg-surface text-text-primary focus:outline-none focus:ring-2 focus:ring-brand-500" placeholder="••••••••" />
          </div>
          <button type="submit" class="w-full py-2.5 bg-brand-600 text-white rounded-lg font-medium hover:bg-brand-700 transition-colors">
            Sign Up
          </button>
        </form>

        <p class="text-center text-sm text-text-muted mt-6">
          Already have an account?
          <a routerLink="/login" class="text-brand-600 hover:underline">Sign in</a>
        </p>
      </div>
    </div>
  `
})
export class Register {}
