import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { Pagination, PaginatedResult } from '../../_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  userParams: any = {};
  pagination: Pagination;
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.route.data.subscribe( data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.userParams.minAge = 18;
    this.userParams.maxAge = 96;
    this.userParams.orderBy = 'lastActive';
  }



  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();

  }

  resetFilters(){
    this.userParams.minAge = 18;
    this.userParams.maxAge = 96;
    this.loadUsers();
  }

  //npm install ngx-gallery --save

  loadUsers(){
    this.userService.getUsers(this.pagination.currentPage,this.pagination.itemsPerPage,this.userParams)
    .subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertify.error(error);
    });
  }
}



//SetUpRouting
//RouterLinkActive , make links look like active pages are being loaded (dynamically content is loaded)
//use routing in codes and protect the routes. (anonymous users)
//Protect Multiple Routes at once.. with a single route guard
//used so that index.html is loaded only once, and the entire SPA knows which parts to load
