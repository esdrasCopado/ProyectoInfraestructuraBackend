import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  imports: [CommonModule, FormsModule],
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  correo = 'admin@local';
  password = 'admin';
  loading = false;
  errorMessage = '';

  constructor() {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  iniciarSesion() {
    this.loading = true;
    this.errorMessage = '';

    this.authService.login(this.correo, this.password).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (error: { error?: { message?: string } | string; message?: string }) => {
        if (typeof error?.error === 'string') {
          this.errorMessage = error.error;
        } else {
          this.errorMessage = error?.error?.message ?? error?.message ?? 'No fue posible iniciar sesión.';
        }
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  recuperarContrasena() {
    alert('Para esta demo usa admin@local / admin o crea más usuarios desde el dashboard.');
  }
}
