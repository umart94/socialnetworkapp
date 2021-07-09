//from vs code terminal in admin mode
//type SocialApp-SPA\src\app> ng g c nav
//ng create component command .... nav is the name of component

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
/*
Components are the most basic building block of an UI in an Angular application.
An Angular application is a tree of Angular components. Angular components are a subset of directives.
Unlike directives, components always have a template and only one component can be instantiated per an element
in a template.
*/

@Component({
  selector: 'app-nav', //display content of component on page
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  //create object
  model: any = {};
  photoUrl: string;
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
   }

  //subscribe to observable
  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Logged in Successfully');
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    //const token = localStorage.getItem('token'); //we need to validate this as to whether if it is actually a jwt
    //return !!token;
    //for fighting against fake tokens we use angular jwt - a 3rd party library
    //we can check tokens expiry date, and properties etc.
    //we'll be able to validate the jwt

    //WE DOO NOT WANT https://github.com/auth0/angular-jwt
    //WE WANT ANGULAR2-JWT
    //WHAT is installed is @auth0/angular-jwt@3.0.1
    //got updated
    //const token = localStorage.getItem('token');
    //return !!token;
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('logged out !');
    this.router.navigate(['/home']);
  }
}
