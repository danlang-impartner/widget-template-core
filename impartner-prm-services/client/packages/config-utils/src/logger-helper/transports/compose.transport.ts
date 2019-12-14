import { LoggerLevel } from '../enums';
import { ILoggerLevel, ITransport } from '../interfaces';

export class TransportCompose implements ITransport, ILoggerLevel {
  constructor(private readonly _transports: ITransport[], public loggerLevel: LoggerLevel) {}

  public log<T>(message: string, ...argument: T[]): void {
    if (this.loggerLevel >= LoggerLevel.Log) {
      this._transports.forEach((transport: ITransport) => transport.log(message, ...argument));
    }
  }

  public debug<T>(message: string, ...argument: T[]): void {
    if (this.loggerLevel >= LoggerLevel.Debug) {
      this._transports.forEach((transport: ITransport) => transport.debug(message, ...argument));
    }
  }

  public info<T>(message: string, ...argument: T[]): void {
    if (this.loggerLevel >= LoggerLevel.Info) {
      this._transports.forEach((transport: ITransport) => transport.info(message, ...argument));
    }
  }

  public warn<T>(message: string, ...argument: T[]): void {
    if (this.loggerLevel >= LoggerLevel.Warn) {
      this._transports.forEach((transport: ITransport) => transport.warn(message, ...argument));
    }
  }

  public error<T>(message: string, ...argument: T[]): void {
    if (this.loggerLevel >= LoggerLevel.Error) {
      this._transports.forEach((transport: ITransport) => transport.error(message, ...argument));
    }
  }

  public trace<T>(message: string, ...argument: T[]): void {
    if (this.loggerLevel >= LoggerLevel.Error) {
      this._transports.forEach((transport: ITransport) => transport.trace(message, ...argument));
    }
  }
}
