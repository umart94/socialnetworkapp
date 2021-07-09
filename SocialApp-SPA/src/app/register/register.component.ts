import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter(); // assign output property to eventemitter
  //model: any = {};
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;// partial makes all of the properties in the class OPTIONAL

  constructor(private authService: AuthService,private router: Router, private alertifyService: AlertifyService,private fb: FormBuilder,


  ) { }

  ngOnInit() {
    /*
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', Validators.required)
    }, this.passwordMatchValidator);*/
    this.bsConfig = {
      containerClass: 'theme-dark-blue'
    }
      this.createRegisterForm();
  }

  createRegisterForm(){
    this.registerForm = this.fb.group({
      gender: ['male'], //add default value to radio button for gender.. so that
      username : ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password : ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)] ],
      confirmPassword : ['', Validators.required]
    }, {validator : this.passwordMatchValidator}

    );
  }

  passwordMatchValidator(g: FormGroup){
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  register() {

    if(this.registerForm.valid){
      //get values from the reactive form and assign them to a user object.. and then save to db
      //we copy the value and properties from a source object to target object, and return that target object
      this.user = Object.assign({}, this.registerForm.value);
      //Object.assign({EMPTYOBJECT},OBJECT TO COPY)

     //subscribe(next?: (value: Object) => void, error?: (error: any) => void, complete?: () => void): Subscription

      this.authService.register(this.user).subscribe(() => {
          this.alertifyService.success('Registration Successful');
      }, error => {
        this.alertifyService.error(error);
      }, () => {
        this.authService.login(this.user).subscribe(
          () => {
            this.router.navigate(['/members']);
          }
        );
      }

      );

    }

     //console.log(this.model);
    /*
    this.authService.register(this.model).subscribe(
      () => {
        this.alertifyService.success('Registration Successfull');
      },
      (error) => {
        this.alertifyService.error(error);
      }
    );
    */

   //console.log(this.registerForm.value);//will output values from reactive form defined above



  }

  cancel() {
    this.cancelRegister.emit(false); //emit can be a boolean or an object .. using bool false in this case emitting to our parent component
  }
}
