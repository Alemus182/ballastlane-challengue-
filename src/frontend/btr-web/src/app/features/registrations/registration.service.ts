import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from '../../core/services/api.service';

export type RegistrationItem = {
  id: string;
  tournamentId: string;
  playerName: string;
  nickname: string | null;
  contactInfo: string;
  createdAtUtc: string;
};

export type TournamentItem = {
  id: string;
  name: string;
  location: string;
  startDate: string;
  endDate: string;
};

type CreateRegistrationBody = {
  tournamentId: string;
  playerName: string;
  nickname: string | null;
  contactInfo: string;
};

type CreateRegistrationResponse = {
  registrationId: string;
};

@Injectable({ providedIn: 'root' })
export class RegistrationService {
  constructor(private readonly apiService: ApiService) {}

  getAll(): Observable<RegistrationItem[]> {
    return this.apiService.get<RegistrationItem[]>('/api/v1/registrations');
  }

  getById(id: string): Observable<RegistrationItem> {
    return this.apiService.get<RegistrationItem>(`/api/v1/registrations/${id}`);
  }

  create(body: CreateRegistrationBody): Observable<CreateRegistrationResponse> {
    return this.apiService.post<CreateRegistrationResponse>('/api/v1/registrations', body);
  }

  update(id: string, body: CreateRegistrationBody): Observable<CreateRegistrationResponse> {
    return this.apiService.put<CreateRegistrationResponse>(`/api/v1/registrations/${id}`, body);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete<void>(`/api/v1/registrations/${id}`);
  }

  getTournaments(): Observable<TournamentItem[]> {
    return this.apiService.get<TournamentItem[]>('/api/v1/tournaments');
  }
}
