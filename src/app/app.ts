import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { CartService } from './services/cart';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  template: `
  <nav class="navbar navbar-expand-lg navbar-light bg-light px-3">
    <span class="navbar-brand fw-bold">🛒 MicroStore</span>

    <div class="ms-auto d-flex align-items-center gap-3">

      <!-- Show only when logged in -->
      <ng-container *ngIf="isLoggedIn; else loggedOut">

        <!-- Cart -->
        <button class="btn btn-outline-primary position-relative" (click)="goToCart()">
          Cart
          <span
            *ngIf="(cart$ | async)?.items?.length > 0"
            class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
            {{ (cart$ | async)?.items?.length }}
          </span>
        </button>

        <!-- Logout -->
        <button class="btn btn-outline-danger" (click)="logout()">
          Logout
        </button>

      </ng-container>

      <!-- If NOT logged in -->
      <ng-template #loggedOut>
        <button class="btn btn-outline-success" (click)="goToLogin()">
          Login
        </button>
      </ng-template>

    </div>
  </nav>

  <router-outlet></router-outlet>
  `,
})
export class AppComponent implements OnInit {
  cart$: Observable<any> | null = null;
  isLoggedIn = false;

  constructor(
    private router: Router,
    private cartService: CartService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.authService.loggedIn$.subscribe((loggedIn) => {
      this.isLoggedIn = loggedIn;
      if (loggedIn) {
        // Refresh cart when user logs in
        this.cart$ = this.cartService.getCart();
      } else {
        this.cart$ = null;
      }
    });
  }

  logout() {
    this.authService.logout();
    this.cart$ = null;
    this.router.navigate(['/login']);
  }

  goToCart() {
    this.router.navigate(['/cart']);
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  validateSession() {
    if (this.authService.isTokenExpired()) {
      this.authService.logout();
      this.router.navigate(['/login']);
    }
  }
}
