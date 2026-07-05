import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="auth-card">
      <h2>Register</h2>
      <p class="hint">Create your organizer account.</p>

      <form [formGroup]="form" (ngSubmit)="submit()" novalidate>
        <label for="register-username">Username or email</label>
        <input id="register-username" type="text" formControlName="usernameOrEmail" />
        @if (form.controls.usernameOrEmail.touched && form.controls.usernameOrEmail.invalid) {
          <p class="error">Username or email is required and must be at most 256 characters.</p>
        }

        <label for="register-password">Password</label>
        <input id="register-password" type="password" formControlName="password" />
        @if (form.controls.password.touched && form.controls.password.invalid) {
          <p class="error">Password is required and must be at least 6 characters.</p>
        }

        @if (errorMessage) {
          <p class="error">{{ errorMessage }}</p>
        }

        <button type="submit" [disabled]="isSubmitting">{{ isSubmitting ? 'Creating account...' : 'Register' }}</button>
      </form>

      <p class="hint">Already registered? <a routerLink="/login">Back to login</a>.</p>
    </section>
  `,
  styles: [
    `
      .auth-card {
        border: 1px solid #e5e7eb;
        border-radius: 0.5rem;
        max-width: 28rem;
        padding: 1rem;
      }

      form {
        display: grid;
        gap: 0.5rem;
      }

      input {
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        padding: 0.5rem;
      }

      button {
        background: #2563eb;
        border: 0;
        border-radius: 0.375rem;
        color: #ffffff;
        cursor: pointer;
        margin-top: 0.5rem;
        padding: 0.5rem 0.75rem;
      }

      button:disabled {
        cursor: default;
        opacity: 0.7;
      }

      .hint {
        color: #4b5563;
      }

      .error {
        color: #b91c1c;
        margin: 0;
      }

    `
  ]
})
export class RegisterComponent {
  private readonly formBuilder = inject(FormBuilder);

  readonly form = this.formBuilder.nonNullable.group({
    usernameOrEmail: ['', [Validators.required, Validators.maxLength(256)]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  isSubmitting = false;
  errorMessage = '';

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  submit(): void {
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.authService.register(this.form.getRawValue()).subscribe({
      next: () => {
        this.isSubmitting = false;
        void this.router.navigate(['/login'], { queryParams: { registered: 1 } });
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting = false;
        this.errorMessage =
          error?.error?.error ??
          'Registration failed. Please check your inputs and try again.';
      }
    });
  }
}
