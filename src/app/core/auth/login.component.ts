import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loading = false;
  error: string | null = null;

  form = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required],
    remember: [true]
  });

  constructor(
    private fb: FormBuilder, 
    private auth: AuthService, 
    private router: Router
  ) {}

  // --- RESTORED: Quick Fill Feature ---
  quickFill(role: string, pass: string) {
    this.form.patchValue({
      username: role,
      password: pass
    });
  }

  submit() {
    this.error = null;
    if (this.form.invalid) return;

    const { username, password } = this.form.getRawValue();
    this.loading = true;

    this.auth.login(username!, password!).subscribe({
      next: res => {
        this.auth.saveToken(res.access_token);
        this.loading = false;
        this.router.navigate(['/dashboard']); 
      },
      error: _ => {
        this.loading = false;
        this.error = 'Invalid username or password';
      }
    });
  }
}