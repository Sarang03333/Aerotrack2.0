import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NgIf } from '@angular/common'; // Import NgIf
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  // FIX: Add NgIf here so the HTML *ngIf directives work
  imports: [RouterLink, RouterLinkActive, NgIf], 
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  constructor(public auth: AuthService) {}
  
  logout() { 
    this.auth.logout(); 
  }
}