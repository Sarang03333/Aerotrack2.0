import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const roleGuard = (allowed: string[]) : CanActivateFn => {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    if (!auth.isAuthenticated()) { router.navigate(['/login']); return false; }
    if (auth.isInRole('Admin') || allowed.some(r => auth.isInRole(r))) return true;
    router.navigate(['/unauthorized']);
    return false;
  };
};