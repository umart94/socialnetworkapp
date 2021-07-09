import { Routes } from '@angular/router';

import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent }, /// home
  {
    path: '', // nothing + members
    // if path: 'dummy' then localhost:4200/dummymembers
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard], // dummy route with AuthGuard, protects 3 children routes
    children: [
      // 3 routes to be protected
      {
        path: 'members',
        component: MemberListComponent,
        resolve: { users: MemberListResolver },
      },
      {
        path: 'members/:id',
        component: MemberDetailComponent,
        resolve: { user: MemberDetailResolver },
      },
      { path: 'messages', component: MessagesComponent, resolve: {messages: MessagesResolver} },
      {
        path: 'member/edit',
        component: MemberEditComponent,
        resolve: { user: MemberEditResolver },
        canDeactivate: [PreventUnsavedChanges],
      },
      { path: 'lists', component: ListsComponent, resolve: { users: ListsResolver} },
    ],
  },

  { path: '**', redirectTo: '', pathMatch: 'full' },

  //path match
  //in case of redirection we want to match the full path of the url in order to redirect
  //prefix , wildcard , anything after a path (word) is going to be redirected to home
  //ordering is important
  //the wildcard prefix must come at the very end
  //otherwise all the paths would be matched

  //Dummy routes will have child routes
  //we need to protect those routes
];

//on refresh we would get an Outlet not activated error
//we have specified routes for home but it needs to be empty string
