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
  templateUrl: './registration-form.component.html',
  styleUrl: './registration-form.component.css'
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
