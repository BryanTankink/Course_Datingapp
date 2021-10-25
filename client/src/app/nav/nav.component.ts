import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public accountService: AccountService) {}

  ngOnInit(): void {
  }

  Login() {
    let body = new FormData();
    body.append("username", this.model.username);
    body.append("password", this.model.password);

    this.accountService.login(body).subscribe(response => {
      console.log(response);
      //this.loggedIn = true;
    }, error => {
      console.log(error);
    });
  }

  Logout() {
    this.accountService.logout();
  }
}
