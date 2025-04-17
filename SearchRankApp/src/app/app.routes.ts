import { Routes } from '@angular/router';
import { AuthGuard } from './services/authentcation/auth.guard';
import { MainPageComponent } from './components/main-page/main-page.component';
import { SearchEngineComponent } from './components/search-engine/search-engine.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { LoginComponent } from './components/login/login.component';
import { NoAuthGuard } from './services/authentcation/auth.no-guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [NoAuthGuard] },
  { path: '', component: MainPageComponent, canActivate: [AuthGuard] },
  { path: 'search-engine', component: SearchEngineComponent, canActivate: [AuthGuard] },

  // Fallback route for invalid URLs
  { path: '**', component: NotFoundComponent },
];
