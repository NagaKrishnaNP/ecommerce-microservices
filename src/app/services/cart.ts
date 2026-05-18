import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { getUserIdFromToken } from '../utils/jwt';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class CartService {

  private baseUrl = `${environment.apiUrl}/api/cart`;
  private cartSubject = new BehaviorSubject<any>(null);
  public cart$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {}

  private refreshCart(): void {
    const userId = getUserIdFromToken();
    if (!userId) {
      this.cartSubject.next(null);
      return;
    }
    this.http.get(`${this.baseUrl}/${userId}`).subscribe({
      next: (cart) => this.cartSubject.next(cart),
      error: () => this.cartSubject.next(null)
    });
  }

  addToCart(item: any, userId: string | null) {
    if (!userId) return of(null);
    return this.http.post(`${this.baseUrl}/${userId}/add`, item).pipe(
      tap(() => this.refreshCart())
    );
  }

  getCart() {
    // ensure cart is loaded at least once
    if (this.cartSubject.value === null) {
      this.refreshCart();
    }
    return this.cart$;
  }

  removeFromCart(productId: number) {
    const userId = getUserIdFromToken();
    return this.http.delete(`${this.baseUrl}/${userId}/remove/${productId}`).pipe(
      tap(() => this.refreshCart())
    );
  }

  clearCart() {
    const userId = getUserIdFromToken();
    return this.http.delete(`${this.baseUrl}/${userId}/clear`).pipe(
      tap(() => this.refreshCart())
    );
  }
}