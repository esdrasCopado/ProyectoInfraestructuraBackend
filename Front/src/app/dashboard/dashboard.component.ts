import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { RecursoPredeterminado, ReporteVpnExpira, Servidor, Solicitud, Usuario } from '../core/models';

type TabName = 'request' | 'requests' | 'users' | 'reports';

interface UserFormModel {
  nombreCompleto: string;
  correo: string;
  password: string;
  rol: string;
  puesto: string;
  celular: string;
  numeroPuesto: string;
}

interface RequestFormModel {
  idUsuario: number | null;
  titulo: string;
  arquitectura: string;
  descripcion: string;
  servicios: string;
  estado: string;
  notificacionNueva: boolean;
  tareasPendientes: string;
}

interface ServerDraft {
  hostname: string;
  ip: string;
  tipoUso: string;
  funcion: string;
  sistemaOperativo: string;
  requiereLlaveLicencia: boolean;
  llaveOS: string;
  nucleos: number;
  ram: number;
  almacenamiento: number;
  descripcion: string;
  plantillaRecursos: string;
  vpnTipo: string;
  vpnExpiracion: string;
  subdominio: string;
  evidenciaRuta: string;
  etapaVulnerabilidades: string;
  ultimaRevisionAnual: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  activeTab: TabName = 'request';
  loading = false;
  statusMessage = '';
  errorMessage = '';
  userDisplay = 'Usuario';
  systemInfo = 'Cargando estado del sistema...';

  roles: string[] = [];
  users: Usuario[] = [];
  solicitudes: Solicitud[] = [];
  servidores: Servidor[] = [];
  templates: RecursoPredeterminado[] = [];

  reportes: {
    vulnerabilidades: Servidor[];
    revision: Servidor[];
    vpns: ReporteVpnExpira[];
    notificaciones: Solicitud[];
  } = {
    vulnerabilidades: [],
    revision: [],
    vpns: [],
    notificaciones: []
  };

  userForm: UserFormModel = this.createEmptyUserForm();
  requestForm: RequestFormModel = this.createEmptyRequestForm();
  serverDrafts: ServerDraft[] = [this.createEmptyServer()];

  ngOnInit(): void {
    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    const currentUser = this.auth.getCurrentUser();
    if (currentUser) {
      this.userDisplay = `${currentUser.nombreCompleto} (${currentUser.rol || currentUser.permisos})`;
    }

    this.refreshAll();
  }

  setTab(tab: TabName): void {
    this.activeTab = tab;
  }

  refreshAll(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      health: this.api.getHealth(),
      roles: this.api.getRoles(),
      users: this.api.getUsers(),
      solicitudes: this.api.getSolicitudes(),
      servidores: this.api.getServidores(),
      recursos: this.api.getRecursosPredeterminados(),
      vulnerabilidades: this.api.getVulnerabilidadesPendientes(),
      revision: this.api.getRevisionAnual(),
      vpns: this.api.getVpnsPorExpirar()
    }).subscribe({
      next: ({ health, roles, users, solicitudes, servidores, recursos, vulnerabilidades, revision, vpns }) => {
        this.systemInfo = `Proveedor: ${health.provider} • ${this.formatDateTime(health.utc)}`;
        this.roles = roles;
        this.users = users;
        this.solicitudes = solicitudes;
        this.servidores = servidores;
        this.templates = recursos;
        this.reportes = {
          vulnerabilidades,
          revision,
          vpns,
          notificaciones: solicitudes.filter(item => item.notificacionNueva)
        };

        if (!this.requestForm.idUsuario && users.length > 0) {
          this.requestForm.idUsuario = users[0].id;
        }

        if (!this.userForm.rol) {
          this.userForm.rol = roles[0] ?? 'Solicitante';
        }
      },
      error: error => {
        this.errorMessage = this.readError(error);
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  addServer(): void {
    this.serverDrafts = [...this.serverDrafts, this.createEmptyServer()];
  }

  removeServer(index: number): void {
    if (this.serverDrafts.length === 1) {
      this.serverDrafts[0] = this.createEmptyServer();
      return;
    }

    this.serverDrafts = this.serverDrafts.filter((_, currentIndex) => currentIndex !== index);
  }

  applyTemplate(server: ServerDraft): void {
    const template = this.templates.find(item => item.nombre === server.plantillaRecursos);
    if (!template) {
      return;
    }

    server.nucleos = template.nucleos;
    server.ram = template.ram;
    server.almacenamiento = template.almacenamiento;
    server.descripcion = template.descripcion;
  }

  submitUser(): void {
    this.errorMessage = '';
    this.statusMessage = '';

    const payload = {
      nombreCompleto: this.userForm.nombreCompleto,
      correo: this.userForm.correo,
      password: this.userForm.password,
      rol: this.userForm.rol,
      permisos: this.userForm.rol,
      puesto: this.userForm.puesto,
      celular: this.userForm.celular,
      numeroPuesto: this.userForm.numeroPuesto
    };

    this.api.createUser(payload).subscribe({
      next: user => {
        this.statusMessage = `Usuario ${user.nombreCompleto} creado correctamente.`;
        this.userForm = this.createEmptyUserForm();
        this.userForm.rol = this.roles[0] ?? 'Solicitante';
        this.refreshAll();
        this.activeTab = 'users';
      },
      error: error => {
        this.errorMessage = this.readError(error);
      }
    });
  }

  submitRequest(): void {
    this.errorMessage = '';
    this.statusMessage = '';

    if (!this.requestForm.idUsuario) {
      this.errorMessage = 'Selecciona un solicitante.';
      return;
    }

    const payload = {
      idUsuario: this.requestForm.idUsuario,
      titulo: this.requestForm.titulo,
      arquitectura: this.requestForm.arquitectura,
      descripcion: this.requestForm.descripcion,
      servicios: this.requestForm.servicios,
      estado: this.requestForm.estado,
      notificacionNueva: this.requestForm.notificacionNueva,
      tareasPendientes: this.requestForm.tareasPendientes,
      servidores: this.serverDrafts.map(server => ({
        hostname: server.hostname,
        ip: server.ip || null,
        tipoUso: server.tipoUso,
        funcion: server.funcion,
        sistemaOperativo: server.sistemaOperativo,
        requiereLlaveLicencia: server.requiereLlaveLicencia,
        llaveOS: server.llaveOS || null,
        nucleos: server.nucleos,
        ram: server.ram,
        almacenamiento: server.almacenamiento,
        descripcion: server.descripcion || null,
        plantillaRecursos: server.plantillaRecursos,
        tareasPendientes: server.descripcion || null,
        etapaVulnerabilidades: server.etapaVulnerabilidades || null,
        requiereRevisionAnual: true,
        ultimaRevisionAnual: server.ultimaRevisionAnual || null,
        vpns: server.vpnTipo ? [{ tipo: server.vpnTipo, fechaExpiracion: server.vpnExpiracion || null, estado: 'Activa' }] : [],
        subdominios: server.subdominio ? [{ nombreUrl: server.subdominio, estado: 'Activo' }] : [],
        evidenciasPruebas: server.evidenciaRuta ? [{ rutaPdf: server.evidenciaRuta, fecha: new Date().toISOString() }] : []
      }))
    };

    this.api.createSolicitud(payload).subscribe({
      next: solicitud => {
        this.statusMessage = `Solicitud #${solicitud.id} creada correctamente.`;
        this.requestForm = this.createEmptyRequestForm();
        this.requestForm.idUsuario = this.users[0]?.id ?? null;
        this.serverDrafts = [this.createEmptyServer()];
        this.refreshAll();
        this.activeTab = 'requests';
      },
      error: error => {
        this.errorMessage = this.readError(error);
      }
    });
  }

  markRead(id: number): void {
    this.api.markNotificationRead(id).subscribe({
      next: () => {
        this.statusMessage = `Solicitud #${id} marcada como leída.`;
        this.refreshAll();
      },
      error: error => {
        this.errorMessage = this.readError(error);
      }
    });
  }

  logout(): void {
    this.auth.logout();
  }

  vpnCount(server: Servidor): number {
    return server.vpNs?.length ?? server.vpns?.length ?? 0;
  }

  formatDate(value?: string | null): string {
    return value ? new Date(value).toLocaleDateString('es-MX') : 'N/D';
  }

  private formatDateTime(value: string): string {
    return new Date(value).toLocaleString('es-MX');
  }

  private createEmptyServer(): ServerDraft {
    return {
      hostname: '',
      ip: '',
      tipoUso: 'Interno',
      funcion: '',
      sistemaOperativo: 'Ubuntu 22.04',
      requiereLlaveLicencia: false,
      llaveOS: '',
      nucleos: 2,
      ram: 8,
      almacenamiento: 100,
      descripcion: '',
      plantillaRecursos: 'General',
      vpnTipo: '',
      vpnExpiracion: '',
      subdominio: '',
      evidenciaRuta: '',
      etapaVulnerabilidades: '',
      ultimaRevisionAnual: ''
    };
  }

  private createEmptyRequestForm(): RequestFormModel {
    return {
      idUsuario: null,
      titulo: '',
      arquitectura: 'Microservicios',
      descripcion: '',
      servicios: '',
      estado: 'Pendiente',
      notificacionNueva: true,
      tareasPendientes: ''
    };
  }

  private createEmptyUserForm(): UserFormModel {
    return {
      nombreCompleto: '',
      correo: '',
      password: 'admin123',
      rol: 'Solicitante',
      puesto: '',
      celular: '',
      numeroPuesto: ''
    };
  }

  private readError(error: unknown): string {
    const httpError = error as { error?: { message?: string; title?: string } | string; message?: string };

    if (typeof httpError?.error === 'string') {
      return httpError.error;
    }

    if (httpError?.error && typeof httpError.error === 'object') {
      return httpError.error.message ?? httpError.error.title ?? 'Error inesperado';
    }

    return httpError?.message ?? 'Error inesperado';
  }
}