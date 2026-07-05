import { DatePipe } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize, Subject, takeUntil } from 'rxjs';

import { RegistrationItem, RegistrationService } from './registration.service';

@Component({
  selector: 'app-registrations',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './registrations.component.html',
  styleUrl: './registrations.component.css'
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

