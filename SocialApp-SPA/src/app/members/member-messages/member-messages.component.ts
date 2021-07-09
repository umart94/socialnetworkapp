import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';


@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};


  // there are built in angular imports
  // make sure you import custom message model

  // we need id of current user from auth service
  // and also pass the recipient id

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) {

   }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages(){
    const currentUserId = +this.authService.decodedToken.nameid;
    // TYPE ANY by default, add + before it to convert it to number,
    // so that when we compare we compare two number vs number, and not number vs any type
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)

    .pipe(
      // do is reserved keyword in js, so now its tap, tap operator/method is used to loop through messages
    // select import from rxjs, do something before subscribing
      tap(messages => {

        for (let i = 0 ; i < messages.length; i++) {
          // in messages array, check recipientid matches userid , message is read, mark the message as read
            if ( messages[i].isRead === false && messages[i].recipientId === currentUserId){
              this.userService.markAsRead(currentUserId, messages[i].id);
            }
        }
      })
    )
    .subscribe(
      messages => {
        this.messages = messages;
      }, error => {
        this.alertify.error(error);
      }
    );
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
    .subscribe( (message: Message) => {


      //debugger; // just to see what is being returned in message in chrome
      this.messages.unshift(message); // specify type of message, in subscribe function

      // resetForm
      this.newMessage.content = '';
      }, error => {
        this.alertify.error(error);
      });
  }

}
