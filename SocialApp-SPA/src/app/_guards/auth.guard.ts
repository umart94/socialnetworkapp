//ng g guard auth --spec=false to create the route guard file
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Injectable({
  providedIn: 'root',
})

//we dont need route snapshots
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) { }
  canActivate(): boolean {
    if (this.authService.loggedIn()) {
      return true;
    }
    this.alertify.error('INVALID OPERATION'); //you shall not pass
    this.router.navigate(['/home']); //take note of brackets and slash here
    return false;
  }
}
