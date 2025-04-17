import {Component, inject} from '@angular/core';
import {NgIf} from '@angular/common';
import {MatSidenav, MatSidenavContainer, MatSidenavModule} from '@angular/material/sidenav';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatListItem, MatNavList} from '@angular/material/list';
import {RouterLink, RouterOutlet} from '@angular/router';
import {ThemeService} from './services/theme.service';
import {LoginComponent} from './components/login/login.component';
import {AuthService} from './services/authentcation/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    MatSidenavModule,
    MatSidenavContainer,
    MatNavList,
    MatToolbar,
    MatIcon,
    RouterOutlet,
    LoginComponent,
    MatListItem,
    RouterLink,
    MatIconButton,
    MatSidenav,
    NgIf
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title: string = 'angular-web-app';
  public currentYear: number = new Date().getFullYear();
  opened: boolean = true;
  private readonly themeService = inject(ThemeService);
  private readonly authService = inject(AuthService);

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  hideSidenav() {
    this.opened = !this.opened;
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  logout() {
    this.authService.logout();
  }
}
