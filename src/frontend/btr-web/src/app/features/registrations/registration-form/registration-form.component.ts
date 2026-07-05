import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { RegistrationService, TournamentItem } from '../registration.service';

@Component({
  selector: 'app-registration-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="form-card">
      <h2>{{ isEditMode ? 'Edit Registration' : 'New Registration' }}</h2>

      @if (errorMessage) {
        <p class="error">{{ errorMessage }}</p>
      }

      <form [formGroup]="form" (ngSubmit)="submit()" novalidate>
        <label for="tournament">Tournament *</label>
        <select id="tournament" formControlName="tournamentId">
          <option value="">-- Select a tournament --</option>
          @for (tournament of tournaments; track tournament.id) {
            <option [value]="tournament.id">{{ tournament.name }}</option>
          }
        </select>
        @if (form.controls.tournamentId.touched && form.controls.tournamentId.invalid) {
          <p class="error">Tournament is required.</p>
        }

        <label for="playerName">Player Name *</label>
        <input id="playerName" type="text" formControlName="playerName" />
        @if (form.controls.playerName.touched && form.controls.playerName.invalid) {
          <p class="error">Player name is required and must be at most 256 characters.</p>
        }

        <label for="nickname">Nickname</label>
        <input id="nickname" type="text" formControlName="nickname" />
        @if (form.controls.nickname.touched && form.controls.nickname.invalid) {
          <p class="error">Nickname must be at most 128 characters.</p>
        }

        <label for="contactInfo">Contact Info *</label>
        <input id="contactInfo" type="text" formControlName="contactInfo" />
        @if (form.controls.contactInfo.touched && form.controls.contactInfo.invalid) {
          <p class="error">Contact info is required and must be at most 512 characters.</p>
        }

        <div class="form-actions">
          <button type="submit" [disabled]="isSubmitting">
            {{ isSubmitting ? 'Saving...' : 'Save' }}
          </button>
          <a routerLink="/registrations">Cancel</a>
        </div>
      </form>
    </section>
  `,
  styles: [
    `
      .form-card {
        border: 1px solid #e5e7eb;
        border-radius: 0.5rem;
        max-width: 32rem;
        padding: 1rem;
      }

      form {
        display: grid;
        gap: 0.5rem;
      }

      input,
      select {
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        padding: 0.5rem;
        font-size: 1rem;
      }

      label {
        display: block;
        font-weight: 500;
        margin-top: 0.5rem;
        margin-bottom: 0.25rem;
      }

      .error {
        color: #dc2626;
        font-size: 0.875rem;
        margin-top: 0.25rem;
      }

      .form-actions {
        display: flex;
        gap: 1rem;
        margin-top: 1rem;
      }

      button {
        background-color: #3b82f6;
        color: white;
        padding: 0.5rem 1rem;
        border: none;
        border-radius: 0.375rem;
        cursor: pointer;
        font-size: 1rem;
      }

      button:hover:not(:disabled) {
        background-color: #2563eb;
      }

      button:disabled {
        background-color: #9ca3af;
        cursor: not-allowed;
      }

      a {
        display: inline-flex;
        align-items: center;
        padding: 0.5rem 1rem;
        background-color: #e5e7eb;
        color: #374151;
        text-decoration: none;
        border-radius: 0.375rem;
        cursor: pointer;
      }

      a:hover {
        background-color: #d1d5db;
      }
    `
  ]
})
export class RegistrationFormComponent implements OnInit, OnDestroy {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly registrationService = inject(RegistrationService);

  form = this.fb.group({
    tournamentId: ['', Validators.required],
    playerName: ['', [Validators.required, Validators.maxLength(256)]],
    nickname: ['', Validators.maxLength(128)],
    contactInfo: ['', [Validators.required, Validators.maxLength(512)]]
  });

  tournaments: TournamentItem[] = [];
  isEditMode = false;
  isSubmitting = false;
  errorMessage = '';
  private registrationId = '';
  private readonly destroy$ = new Subject<void>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngOnInit(): void {
    this.loadTournaments();
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.registrationId = id;
      this.loadRegistration(id);
    }
  }

  private loadTournaments(): void {
    this.registrationService.getTournaments().pipe(takeUntil(this.destroy$)).subscribe({
      next: (tournaments) => {
        this.tournaments = tournaments;
      },
      error: () => {
        this.errorMessage = 'Failed to load tournaments.';
      }
    });
  }

  private loadRegistration(id: string): void {
    this.registrationService.getById(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (registration) => {
        if (!this.form.controls.tournamentId.dirty) {
          this.form.controls.tournamentId.setValue(registration.tournamentId);
        }
        if (!this.form.controls.playerName.dirty) {
          this.form.controls.playerName.setValue(registration.playerName);
        }
        if (!this.form.controls.nickname.dirty) {
          this.form.controls.nickname.setValue(registration.nickname ?? '');
        }
        if (!this.form.controls.contactInfo.dirty) {
          this.form.controls.contactInfo.setValue(registration.contactInfo);
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load registration.';
      }
    });
  }

  submit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      this.errorMessage = 'Please fix the validation errors before saving.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const body = {
      tournamentId: this.form.value.tournamentId!,
      playerName: this.form.value.playerName!,
      nickname: this.form.value.nickname || null,
      contactInfo: this.form.value.contactInfo!
    };

    const request$ = this.isEditMode
      ? this.registrationService.update(this.registrationId, body)
      : this.registrationService.create(body);

    request$.pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.router.navigate(['/registrations']);
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmitting = false;
        this.errorMessage = err.error?.error || 'An error occurred. Please try again.';
      }
    });
  }
}
