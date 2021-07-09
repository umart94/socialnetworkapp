import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../environments/environment';
import { User } from '../_models/user';

//inject things inside our service
//components are injectable by default
//we need to specifically add the injectable declarator

//PARENT - CHILD
/*
ANY TO ANY COMMUNICATION

ROUTER-OUTLET
PARENT APP COMPONENT -> CHILD NAV
PARENT MEMBER EDIT -> CHILD PHOTO-EDITOR

change property in nav component

add property in auth service
mainphotourl

router outlet -> member edit component -> photo-editor ( components will subscribe to service)


BehaviourSubject - An Observable
it is a subject which is a type of observable
-> can be subscribed to
-> subscribers can receive updated results
-> a subject is an observer ( so we can send values to it)


IT NEEDS
-> an initial value ( must always return a value on subscription)
-> on subscription returns last value of subject
-> can use the getValue() method in non observable code


*/

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }



  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  /*
  post(url: string, body: any, options: { headers?: HttpHeaders | { [header: string]: string | string[]; }; observe?: "body"; params?: HttpParams | { [param: string]: string | string[]; }; reportProgress?: boolean; responseType: "arraybuffer"; withCredentials?: boolean; }): Observable<ArrayBuffer>

  Construct a POST request which interprets the body as an ArrayBuffer and returns it.
  */

  //allow anonymous access, and we wont need header or any third parameter
  //this will return a token in response
  //RXJS is used to send token through pipe method... this allows to chain request

  //take the response , transform it using Observable of the body as an object

  //use Map operator on pipe
  //after base url insert a slash for login route
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  /*
  REGISTER METHOD
  returns an observable

  we then subscribe to it
  */

  /*register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }*/

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
