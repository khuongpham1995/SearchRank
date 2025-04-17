import { ILogger } from './logger.interface';
import { inject, Injectable } from '@angular/core';
import { SearchRankPresentationRequestsServerLogRequest } from '../../search-rank-api/models';
import { ServerLogService } from '../../search-rank-api/services';
import { LogLevel } from './log-level.enum';

@Injectable({
    providedIn: 'root'
})

export class ServerLoggerService implements ILogger {
  private readonly serverLogService = inject(ServerLogService);

  log(message: string): void {
    this.sendLog(LogLevel.Information, message);
  }

  warn(message: string): void {
    this.sendLog(LogLevel.Warn, message);
  }

  error(message: string): void {
    this.sendLog(LogLevel.Error, message);
  }

  private sendLog(level: string, message: string): void {
    const params: SearchRankPresentationRequestsServerLogRequest = { level, message };
    this.serverLogService.apiLogPost({ body: params }).subscribe();
  }
}
