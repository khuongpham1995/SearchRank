import { ApplicationConfig, Injector } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { routes } from './app.routes';
import { AuthInterceptor } from './services/authentcation/auth.interceptor';
import { LOGGER_TOKEN } from './services/logger/logger.token';
import { loggerFactory } from './services/logger/logger.factory';

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: LOGGER_TOKEN, useFactory: loggerFactory, deps: [Injector] }
  ],
};
