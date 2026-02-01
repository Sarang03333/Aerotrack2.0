import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector:'app-navbar',
  standalone:true,
  imports:[RouterLink, RouterLinkActive],
  templateUrl:'./navbar.component.html',
  styleUrls:['./navbar.component.css']
})
export class NavbarComponent{
  constructor(public auth: AuthService) {}
  logout(){ this.auth.logout(); }
}