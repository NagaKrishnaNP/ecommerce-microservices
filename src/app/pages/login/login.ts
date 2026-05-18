import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  imports: [FormsModule]
})

export class LoginComponent {

  email = '';
  password = '';

  constructor(private auth: AuthService,private router: Router) {}

  login() {
    this.auth.login({
      email: this.email,
      password: this.password
    }).subscribe((res: any) => {
      this.auth.saveToken(res.token);
      this.router.navigate(['/products']);
    });
  }
}