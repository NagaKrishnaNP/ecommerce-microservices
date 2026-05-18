import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login';
import { authGuard } from './guards/auth-guard';
import { CartComponent } from './pages/cart/cart';
import { Products } from './pages/products/products';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'products',
        component: Products,
        canActivate: [authGuard]
    },
    {
        path: 'cart',
        component: CartComponent,
        canActivate: [authGuard]
    },
];
