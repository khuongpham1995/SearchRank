import { InjectionToken } from '@angular/core';
import { ILogger } from './logger.interface';

export const LOGGER_TOKEN = new InjectionToken<ILogger>('Logger');
