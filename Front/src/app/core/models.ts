export interface Usuario {
  id: number;
  nombreCompleto: string;
  rol: string;
  permisos: string;
  correo: string;
  puesto?: string | null;
  celular?: string | null;
  numeroPuesto?: string | null;
}

export interface Vpn {
  id?: number;
  tipo: string;
  fechaAsignacion?: string | null;
  fechaExpiracion?: string | null;
  fecha_Expiracion?: string | null;
  estado?: string | null;
}

export interface Subdominio {
  id?: number;
  nombreUrl?: string;
  nombre_url?: string;
  estado?: string | null;
}

export interface EvidenciaPrueba {
  id?: number;
  rutaPdf?: string;
  ruta_pdf?: string;
  fecha?: string | null;
}

export interface Servidor {
  id?: number;
  idSolicitud?: number | null;
  estado?: string;
  hostname?: string;
  ip?: string | null;
  tipoUso?: string;
  funcion?: string;
  sistemaOperativo?: string;
  requiereLlaveLicencia?: boolean;
  llaveOS?: string | null;
  nucleos?: number;
  ram?: number;
  almacenamiento?: number;
  descripcion?: string | null;
  plantillaRecursos?: string;
  tareasPendientes?: string | null;
  etapaVulnerabilidades?: string | null;
  requiereRevisionAnual?: boolean;
  ultimaRevisionAnual?: string | null;
  vpNs?: Vpn[];
  vpns?: Vpn[];
  subdominios?: Subdominio[];
  evidenciasPruebas?: EvidenciaPrueba[];
}

export interface Solicitud {
  id: number;
  titulo: string;
  estado: string;
  fecha_creacion?: string | null;
  arquitectura: string;
  descripcion?: string | null;
  servicios: string;
  notificacionNueva: boolean;
  tareasPendientes?: string | null;
  usuario?: Usuario | null;
  servidores: Servidor[];
}

export interface LoginResponse {
  token: string;
  user: Usuario;
}

export interface RecursoPredeterminado {
  nombre: string;
  nucleos: number;
  ram: number;
  almacenamiento: number;
  descripcion: string;
}

export interface ReporteVpnExpira {
  servidor: Servidor;
  vpNs?: Vpn[];
  vpns?: Vpn[];
}