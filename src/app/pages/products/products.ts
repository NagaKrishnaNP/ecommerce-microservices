import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../services/cart';
import { Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-products',
  imports: [CommonModule, RouterModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {

products$!: Observable<any>;

constructor(private productService: ProductService,private cartService: CartService,private router: Router) {}

ngOnInit() {
  this.products$ = this.productService.getProducts();
}

addToCart(product: any) {
  const cartItem = {
    productId: product.id,
    quantity: product.quantity ? product.quantity : 1,
    productName: product.name,
    price: product.price
  };

  const userId = this.getUserId();
  (this.cartService.addToCart(cartItem, userId) as any).subscribe(() => {
    alert('Added to cart');
  });
}

getUserId() {
  const token = localStorage.getItem('token');
  if (!token) return null;

  const decoded: any = jwtDecode(token);
  return decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]; // 👈 IMPORTANT
}

logout() {
  localStorage.removeItem('token');
  this.router.navigate(['/login']);
}

}
