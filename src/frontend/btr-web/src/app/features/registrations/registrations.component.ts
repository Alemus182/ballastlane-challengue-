import { DatePipe } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize, Subject, takeUntil } from 'rxjs';

import { RegistrationItem, RegistrationService } from './registration.service';

@Component({
  selector: 'app-registrations',
  standalone: true,
  imports: [DatePipe, RouterLink],
  template: `
    <section class="registrations-container">
      <div class="registrations-header">
        <h2>Registrations</h2>
        <a routerLink="/registrations/new" class="btn-new">New Registration</a>
      </div>

      @if (isLoading) {
        <p class="loading">Loading registrations...</p>
      }

      @if (errorMessage) {
        <p class="error">{{ errorMessage }}</p>
      }

      @if (!isLoading && registrations.length === 0) {
        <p class="empty">No registrations found.</p>
      }

      @if (!isLoading && registrations.length > 0) {
        <table class="registrations-table">
          <thead>
            <tr>
              <th>Player Name</th>
              <th>Nickname</th>
              <th>Contact Info</th>
              <th>Tournament ID</th>
              <th>Created At</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (registration of registrations; track registration.id) {
              <tr>
                <td>{{ registration.playerName }}</td>
                <td>{{ registration.nickname || '—' }}</td>
                <td>{{ registration.contactInfo }}</td>
                <td>{{ registration.tournamentId }}</td>
                <td>{{ registration.createdAtUtc | date: 'short' }}</td>
                <td class="actions">
                  <a [routerLink]="['/registrations', registration.id, 'edit']" class="btn-edit">Edit</a>
                  <button
                    (click)="delete(registration.id)"
                    class="btn-delete"
                    [disabled]="isDeletingIds.has(registration.id)">
                    {{ isDeletingIds.has(registration.id) ? 'Deleting...' : 'Delete' }}
                  </button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      }
    </section>
  `,
  styles: [
    `
      .registrations-container {
        padding: 1rem;
      }

      .registrations-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 1.5rem;
      }

      .btn-new {
        display: inline-block;
        padding: 0.5rem 1rem;
        background-color: #3b82f6;
        color: white;
        text-decoration: none;
        border-radius: 0.375rem;
      }

      .btn-new:hover {
        background-color: #2563eb;
      }

      .loading,
      .error,
      .empty {
        padding: 1rem;
        border-radius: 0.375rem;
      }

      .loading {
        background-color: #dbeafe;
        color: #0c4a6e;
      }

      .error {
        background-color: #fee2e2;
        color: #7f1d1d;
      }

      .empty {
        background-color: #f3f4f6;
        color: #6b7280;
      }

      .registrations-table {
        width: 100%;
        border-collapse: collapse;
        border: 1px solid #e5e7eb;
        border-radius: 0.375rem;
        overflow: hidden;
      }

      thead {
        background-color: #f9fafb;
      }

      th,
      td {
        padding: 0.75rem;
        text-align: left;
        border-bottom: 1px solid #e5e7eb;
      }

      th {
        font-weight: 600;
        color: #374151;
      }

      tbody tr:hover {
        background-color: #f9fafb;
      }

      .actions {
        display: flex;
        gap: 0.5rem;
      }

      .btn-edit,
      .btn-delete {
        padding: 0.375rem 0.75rem;
        border-radius: 0.25rem;
        font-size: 0.875rem;
        border: none;
        cursor: pointer;
        text-decoration: none;
        display: inline-block;
      }

      .btn-edit {
        background-color: #3b82f6;
        color: white;
      }

      .btn-edit:hover {
        background-color: #2563eb;
      }

      .btn-delete {
        background-color: #ef4444;
        color: white;
      }

      .btn-delete:hover {
        background-color: #dc2626;
      }
    `
  ]
})
export class RegistrationsComponent implements OnInit, OnDestroy {
  private readonly registrationService = inject(RegistrationService);
  private readonly destroy$ = new Subject<void>();

  registrations: RegistrationItem[] = [];
  isLoading = true;
  errorMessage = '';
  readonly isDeletingIds = new Set<string>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngOnInit(): void {
    this.loadRegistrations();
  }

  private loadRegistrations(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.registrationService.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: (registrations) => {
        this.registrations = registrations;
        this.isLoading = false;
      },
      error: () => {
        this.registrations = [];
        this.errorMessage = 'Failed to load registrations.';
        this.isLoading = false;
      }
    });
  }

  delete(id: string): void {
    if (this.isDeletingIds.has(id)) {
      return;
    }

    if (confirm('Delete this registration?')) {
      this.isDeletingIds.add(id);
      this.registrationService.delete(id).pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isDeletingIds.delete(id))
      ).subscribe({
        next: () => {
          this.loadRegistrations();
        },
        error: () => {
          this.errorMessage = 'Failed to delete registration.';
          this.loadRegistrations();
        }
      });
    }
  }
}

