import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from '../../core/services/api.service';

export type TournamentItem = {
  id: string;
  name: string;
  location: string;
  startDate: string;
  endDate: string;
};

type CreateTournamentBody = {
  name: string;
  location: string;
  startDate: string;
  endDate: string;
};

type CreateTournamentResponse = {
  tournamentId: string;
};

@Injectable({ providedIn: 'root' })
export class TournamentsService {
  constructor(private readonly apiService: ApiService) {}

  getAll(): Observable<TournamentItem[]> {
    return this.apiService.get<TournamentItem[]>('/api/v1/tournaments');
  }

  create(body: CreateTournamentBody): Observable<CreateTournamentResponse> {
    return this.apiService.post<CreateTournamentResponse>('/api/v1/tournaments', body);
  }
}
