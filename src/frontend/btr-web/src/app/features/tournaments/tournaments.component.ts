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
  templateUrl: './tournaments.component.html',
  styleUrl: './tournaments.component.css'
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
