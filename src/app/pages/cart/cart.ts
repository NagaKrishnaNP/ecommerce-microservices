import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs/internal/Observable';
import { Subject } from 'rxjs';
import { switchMap, startWith } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { getUserIdFromToken } from '../../utils/jwt';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [CommonModule],
  templateUrl: './cart.html',
})
export class CartComponent implements OnInit {
  constructor(private cartService: CartService, private http: HttpClient,private router: Router) {}

  cart$!: Observable<any>;

refresh$ = new Subject<void>();


  ngOnInit() {
  this.cart$ = this.refresh$.pipe(
    startWith(null),
    switchMap(() => this.cartService.getCart())
  );
}

  remove(productId: number) {
    this.cartService.removeFromCart(productId).subscribe(() => {
      this.refresh$.next();
    });
  }

  clearCart() {
    this.cartService.clearCart().subscribe(() => {
      this.refresh$.next();
    });
  }

  checkout() {
  const userId = getUserIdFromToken(); // already using this

  // 1️⃣ Get cart first
  this.cartService.getCart().subscribe({
    next: (cart: any) => {

      if (!cart || !cart.items || cart.items.length === 0) {
        return;
      }

      // 2️⃣ Prepare order payload
      const order = {
        userId: userId,
        items: cart.items.map((x: any) => ({
          productId: x.productId,
          quantity: x.quantity
        }))
      };

      // 3️⃣ Call Order Service via API Gateway
      this.http.post(`${environment.apiUrl}/order/api/order/checkout`, order)
        .subscribe({
          next: () => {
            alert('Order placed successfully');

            // 4️⃣ Clear cart after success
            this.cartService.clearCart().subscribe(() => {
              this.refresh$.next(); // 🔥 refresh UI
              this.router.navigate(['/products']); // Redirect to products page after checkout
            });
          },
          error: (err) => console.error('Checkout error:', err)
        });

    },
    error: (err) => console.error(err)
  });
}

goToProducts() {
  this.router.navigate(['/products']);
}

logout() {
  localStorage.removeItem('token');
  this.router.navigate(['/login']);
}
}
