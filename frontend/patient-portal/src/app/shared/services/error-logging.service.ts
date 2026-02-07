import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

export enum LogLevel {
  Debug = 0,
  Info = 1,
  Warning = 2,
  Error = 3,
  Critical = 4
}

export interface LogEntry {
  timestamp: Date;
  level: LogLevel;
  message: string;
  context?: string;
  data?: any;
  stackTrace?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorLoggingService {
  private logs: LogEntry[] = [];
  private maxLogs = 200; // Reduced from 1000 for better mobile performance
  private minLogLevel: LogLevel = LogLevel.Info;

  /**
   * Log debug message
   */
  debug(message: string, context?: string, data?: any): void {
    this.log(LogLevel.Debug, message, context, data);
  }

  /**
   * Log info message
   */
  info(message: string, context?: string, data?: any): void {
    this.log(LogLevel.Info, message, context, data);
  }

  /**
   * Log warning message
   */
  warning(message: string, context?: string, data?: any): void {
    this.log(LogLevel.Warning, message, context, data);
  }

  /**
   * Log error message
   */
  error(message: string, context?: string, error?: any): void {
    const stackTrace = error instanceof Error ? error.stack : undefined;
    this.log(LogLevel.Error, message, context, error, stackTrace);
  }

  /**
   * Log critical error
   */
  critical(message: string, context?: string, error?: any): void {
    const stackTrace = error instanceof Error ? error.stack : undefined;
    this.log(LogLevel.Critical, message, context, error, stackTrace);
  }

  /**
   * Log HTTP errors specifically
   */
  logHttpError(error: HttpErrorResponse, context?: string): void {
    const message = `HTTP ${error.status}: ${error.message}`;
    const data = {
      url: error.url,
      status: error.status,
      statusText: error.statusText,
      error: error.error
    };
    this.error(message, context, data);
  }

  /**
   * Get all logs
   */
  getLogs(level?: LogLevel): LogEntry[] {
    if (level !== undefined) {
      return this.logs.filter(log => log.level >= level);
    }
    return [...this.logs];
  }

  /**
   * Clear all logs
   */
  clearLogs(): void {
    this.logs = [];
  }

  /**
   * Export logs as JSON string
   */
  exportLogs(): string {
    return JSON.stringify(this.logs, null, 2);
  }

  /**
   * Set minimum log level
   */
  setMinLogLevel(level: LogLevel): void {
    this.minLogLevel = level;
  }

  /**
   * Internal log method
   */
  private log(level: LogLevel, message: string, context?: string, data?: any, stackTrace?: string): void {
    // Only log if level meets minimum threshold
    if (level < this.minLogLevel) {
      return;
    }

    const entry: LogEntry = {
      timestamp: new Date(),
      level,
      message,
      context,
      data,
      stackTrace
    };

    // Add to internal log store
    this.logs.push(entry);

    // Trim logs if exceeds max
    if (this.logs.length > this.maxLogs) {
      this.logs.shift();
    }

    // Console output with appropriate level
    this.outputToConsole(entry);

    // In production, you would also send to a logging service here
    // this.sendToLoggingService(entry);
  }

  /**
   * Output log to console
   */
  private outputToConsole(entry: LogEntry): void {
    const levelName = LogLevel[entry.level];
    const prefix = `[${entry.timestamp.toISOString()}] [${levelName}]`;
    const contextStr = entry.context ? ` [${entry.context}]` : '';
    const message = `${prefix}${contextStr} ${entry.message}`;

    switch (entry.level) {
      case LogLevel.Debug:
        console.debug(message, entry.data);
        break;
      case LogLevel.Info:
        console.info(message, entry.data);
        break;
      case LogLevel.Warning:
        console.warn(message, entry.data);
        break;
      case LogLevel.Error:
        console.error(message, entry.data, entry.stackTrace);
        break;
      case LogLevel.Critical:
        console.error(`CRITICAL: ${message}`, entry.data, entry.stackTrace);
        break;
    }
  }
}
