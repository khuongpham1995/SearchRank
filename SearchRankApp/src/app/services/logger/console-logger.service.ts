import {ILogger} from './logger.interface';
import {Injectable} from '@angular/core';
import {LogLevel} from './log-level.enum';

@Injectable({
    providedIn: 'root'
})

export class ConsoleLoggerService implements ILogger {
    log(message: string): void {
      console.log(`[${LogLevel.Information}]: ${message}`);
    }

    warn(message: string): void {
      console.warn(`[${LogLevel.Warn}]: ${message}`);
    }

    error(message: string): void {
      console.error(`[${LogLevel.Error}]: ${message}`);
    }
  }
