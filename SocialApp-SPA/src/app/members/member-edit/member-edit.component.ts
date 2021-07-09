import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private userService: UserService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.user = data['user'];
    });

    this.authService.currentPhotoUrl.subscribe(
      (photoUrl) => (this.photoUrl = photoUrl)
    );
  }

  updateUser() {
    //console.log(this.user);
    this.userService
      .updateUser(this.authService.decodedToken.nameid, this.user)
      .subscribe(
        (next) => {
          this.alertify.success(' Profile Updated Successfully. ');
          this.editForm.reset(this.user);
        },
        (error) => {
          this.alertify.error(error);
        }
      );

    //since you clicked save changes button.. now we need to tell angular that form is not dirty
    //import ViewChild and ngForm
    //this.editForm.reset();//resets the form control
    //this wiped out other text areas
    //set it to this.user
  }

  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }
}

/*
Update users module and edit profile of users
Add RouteResolver
use the data to populate form

use template form
use a CanDeactivate Route Guard

CanDeactivate
INTERFACE
Interface that a class can implement to be a guard deciding if a route can be deactivated. If all guards return true, navigation will continue. If any guard returns false, navigation will be cancelled. If any guard returns a UrlTree, current navigation will be cancelled and a new navigation will be kicked off to the UrlTree returned from the guard.

See more...

interface CanDeactivate<T> {
  canDeactivate(component: T, currentRoute: ActivatedRouteSnapshot, currentState: RouterStateSnapshot, nextState?: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree
}


Prevents user from clicking another navigation route




@ViewChild decorator

ViewChild
DECORATOR
Property decorator that configures a view query. The change detector looks for the first element or the directive matching the selector in the view DOM. If the view DOM changes, and a new child matches the selector, the property is updated.

See more...


access members or methods from child component , inside the parent component

Persist changes made to the API



*/

/*
if you upload photos in database , there is a performance degradation in application... we can do this in some cases
but it is better to save pics on the cloud

if you upload them on file system... then you will have to shift computers.. we can't access them from anywhere
another problem is diskspace

we can upload picture on cloud.. and return the URL.. and save URL into our database

but it increases complexity... because database wont have the authentication that we implemented in API

API SERVER
DATABASE
CLIENT
CLOUD PROVIDER


1. Client uploads photo to API SERVER with JWT TOKEN
2. API SERVER UPLOADS PHOTO TO CLOUD PROVIDER
3. CLOUD PROVIDER SAVES.... AND RETURNS PHOTOURL AND PUBLICID BACK TO DATABASE
4. DATABASE SAVES THIS PHOTOURL,PHOTOID (2ATTRIBUTES,STRING AND STRING / STRING AND INT) , GIVES SQL ID
5. 201 CREATED RESPONSE WITH LOCATION HEADER BACK TO CLIENT.

After you install the Cloudinary SDK of your choice and you add these API key, API secret, and cloud name values to your project's global configuration file, you can run the sample code below to try it yourself).



<input name="file" type="file"
   class="file-upload" data-cloudinary-field="image_id"
   data-form-data="{ 'transformation': {'crop':'limit','tags':'samples','width':3000,'height':2000}}"/>

   https://res.cloudinary.com/YOURCLOUDNAME/image/upload/c_fill,g_faces,h_200,w_300/sample.jpg

Uploading is done over HTTPS using a secure protocol based on your account's cloud_name, api_key and api_secret parameters, or using an unsigned upload without an authentication signature. When using Cloudinary's SDKs, the 3 security parameters are generally configured globally, but they can be provided with each call instead.

The following .NET code uploads the dog.mp4 video to the specified account sub-folder using the public_id, my_dog. The video will overwrite the existing my_dog video if it exists. When the video upload is complete, the specified notification URL will receive details about the uploaded media asset.

var uploadParams = new VideoUploadParams()
    {
        File = new FileDescription(@"dog.mp4"),
        ResourceType = "video"
        PublicId = "my_folder/my_sub_folder/my_dog",
        Overwrite = true,
        NotificationUrl = "https://mysite/my_notification_endpoint"
    };
    var uploadResult = cloudinary.Upload(uploadParams);

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

CLOUDINARY_URL=cloudinary://my_key:my_secret@my_cloud_name

Account account = new Account(
    "my_cloud_name",
    "my_api_key",
    "my_api_secret");

Cloudinary cloudinary = new Cloudinary(account);


var uploadParams = new ImageUploadParams()
{
    File = new FileDescription(@"c:\my_image.jpg")
};
var uploadResult = cloudinary.Upload(uploadParams);

*/
