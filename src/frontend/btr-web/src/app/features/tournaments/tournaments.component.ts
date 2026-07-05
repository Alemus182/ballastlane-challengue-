import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, Subject, takeUntil } from 'rxjs';

import { TournamentItem, TournamentsService } from './tournaments.service';

@Component({
  selector: 'app-tournaments',
  standalone: true,
  imports: [DatePipe, ReactiveFormsModule],
  template: `
    <section class="tournaments-page">
      <h2>Tournaments</h2>

      @if (errorMessage) {
        <p class="error">{{ errorMessage }}</p>
      }

      <form [formGroup]="form" (ngSubmit)="submit()" class="create-form" novalidate>
        <h3>Create Tournament</h3>

        <label for="name">Name *</label>
        <input id="name" type="text" formControlName="name" />
        @if (form.controls.name.touched && form.controls.name.invalid) {
          <p class="error">Name is required and must be at most 256 characters.</p>
        }

        <label for="location">Location *</label>
        <input id="location" type="text" formControlName="location" />
        @if (form.controls.location.touched && form.controls.location.invalid) {
          <p class="error">Location is required and must be at most 512 characters.</p>
        }

        <label for="startDate">Start Date *</label>
        <input id="startDate" type="date" formControlName="startDate" />
        @if (form.controls.startDate.touched && form.controls.startDate.invalid) {
          <p class="error">Start date is required.</p>
        }

        <label for="endDate">End Date *</label>
        <input id="endDate" type="date" formControlName="endDate" />
        @if (form.controls.endDate.touched && form.controls.endDate.invalid) {
          <p class="error">End date is required.</p>
        }

        <button type="submit" [disabled]="isSubmitting">
          {{ isSubmitting ? 'Saving...' : 'Create Tournament' }}
        </button>
      </form>

      @if (isLoading) {
        <p class="loading">Loading tournaments...</p>
      }

      @if (!isLoading && tournaments.length === 0) {
        <p class="empty">No tournaments found.</p>
      }

      @if (!isLoading && tournaments.length > 0) {
        <table class="tournaments-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Location</th>
              <th>Start Date</th>
              <th>End Date</th>
            </tr>
          </thead>
          <tbody>
            @for (tournament of tournaments; track tournament.id) {
              <tr>
                <td>{{ tournament.name }}</td>
                <td>{{ tournament.location }}</td>
                <td>{{ tournament.startDate | date: 'mediumDate' }}</td>
                <td>{{ tournament.endDate | date: 'mediumDate' }}</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </section>
  `,
  styles: [
    `
      .tournaments-page {
        padding: 1rem;
      }

      .create-form {
        display: grid;
        gap: 0.5rem;
        border: 1px solid #e5e7eb;
        border-radius: 0.5rem;
        padding: 1rem;
        margin-bottom: 1.5rem;
        max-width: 40rem;
      }

      label {
        font-weight: 500;
      }

      input {
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        padding: 0.5rem;
        font-size: 1rem;
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

      button {
        margin-top: 0.5rem;
        border: none;
        border-radius: 0.375rem;
        padding: 0.5rem 1rem;
        background-color: #3b82f6;
        color: white;
        cursor: pointer;
        width: fit-content;
      }

      button:hover:not(:disabled) {
        background-color: #2563eb;
      }

      button:disabled {
        background-color: #9ca3af;
        cursor: not-allowed;
      }

      .tournaments-table {
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
    `
  ]
})
export class TournamentsComponent implements OnInit, OnDestroy {
  private readonly tournamentsService = inject(TournamentsService);
  private readonly fb = inject(FormBuilder);
  private readonly destroy$ = new Subject<void>();

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(256)]],
    location: ['', [Validators.required, Validators.maxLength(512)]],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required]
  });

  tournaments: TournamentItem[] = [];
  isLoading = true;
  isSubmitting = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadTournaments();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadTournaments(): void {
    this.isLoading = true;
    this.tournamentsService.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: (tournaments) => {
        this.tournaments = tournaments;
        this.isLoading = false;
      },
      error: () => {
        this.tournaments = [];
        this.errorMessage = 'Failed to load tournaments.';
        this.isLoading = false;
      }
    });
  }

  submit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      this.errorMessage = 'Please fix the validation errors before creating a tournament.';
      return;
    }

    const startDate = new Date(this.form.value.startDate!);
    const endDate = new Date(this.form.value.endDate!);

    if (endDate < startDate) {
      this.errorMessage = 'End date must be greater than or equal to start date.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const body = {
      name: this.form.value.name!,
      location: this.form.value.location!,
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };

    this.tournamentsService.create(body).pipe(
      takeUntil(this.destroy$),
      finalize(() => {
        this.isSubmitting = false;
      })
    ).subscribe({
      next: () => {
        this.form.reset();
        this.loadTournaments();
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage = err.error?.error || 'Failed to create tournament.';
      }
    });
  }
}
