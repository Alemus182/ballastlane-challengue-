import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { RegistrationFormComponent } from './features/registrations/registration-form/registration-form.component';
import { RegistrationsComponent } from './features/registrations/registrations.component';
import { TournamentsComponent } from './features/tournaments/tournaments.component';

export const routes: Routes = [
	{ path: '', pathMatch: 'full', redirectTo: 'login' },
	{ path: 'login', component: LoginComponent },
	{ path: 'register', component: RegisterComponent },
	{ path: 'tournaments', component: TournamentsComponent, canActivate: [authGuard] },
	{ path: 'registrations/new', component: RegistrationFormComponent, canActivate: [authGuard] },
	{ path: 'registrations/:id/edit', component: RegistrationFormComponent, canActivate: [authGuard] },
	{ path: 'registrations', component: RegistrationsComponent, canActivate: [authGuard] },
	{ path: '**', redirectTo: 'login' }
];
