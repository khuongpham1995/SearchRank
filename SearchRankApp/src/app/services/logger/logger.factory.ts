// logger.factory.ts

import { Injector } from '@angular/core';
import { ConsoleLoggerService } from './console-logger.service';
import { ServerLoggerService } from './server-logger.service';
import { ILogger } from './logger.interface';
import { environment } from '../../environments/environment';

export function loggerFactory(injector: Injector): ILogger {
  return environment.production
    ? injector.get(ServerLoggerService)
    : injector.get(ConsoleLoggerService);
}
