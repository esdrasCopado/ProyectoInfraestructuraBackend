import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RecursoPredeterminado, ReporteVpnExpira, Servidor, Solicitud, Usuario } from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);

  getRoles(): Observable<string[]> {
    return this.http.get<string[]>('/api/usuario/roles');
  }

  getUsers(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>('/api/usuario/todos');
  }

  createUser(payload: unknown): Observable<Usuario> {
    return this.http.post<Usuario>('/api/usuario', payload);
  }

  getSolicitudes(): Observable<Solicitud[]> {
    return this.http.get<Solicitud[]>('/api/solicitud/todas');
  }

  createSolicitud(payload: unknown): Observable<Solicitud> {
    return this.http.post<Solicitud>('/api/solicitud', payload);
  }

  markNotificationRead(id: number): Observable<Solicitud> {
    return this.http.put<Solicitud>(`/api/solicitud/${id}/notificacion-leida`, {});
  }

  getServidores(): Observable<Servidor[]> {
    return this.http.get<Servidor[]>('/api/servidor');
  }

  createServidor(payload: unknown): Observable<Servidor> {
    return this.http.post<Servidor>('/api/servidor', payload);
  }

  getRecursosPredeterminados(): Observable<RecursoPredeterminado[]> {
    return this.http.get<RecursoPredeterminado[]>('/api/servidor/recursos-predeterminados');
  }

  getVulnerabilidadesPendientes(): Observable<Servidor[]> {
    return this.http.get<Servidor[]>('/api/servidor/reportes/vulnerabilidades-pendientes');
  }

  getRevisionAnual(): Observable<Servidor[]> {
    return this.http.get<Servidor[]>('/api/servidor/reportes/revision-anual');
  }

  getVpnsPorExpirar(): Observable<ReporteVpnExpira[]> {
    return this.http.get<ReporteVpnExpira[]>('/api/servidor/reportes/vpns-por-expirar?dias=30');
  }

  getHealth(): Observable<{ status: string; provider: string; utc: string }> {
    return this.http.get<{ status: string; provider: string; utc: string }>('/health');
  }
}