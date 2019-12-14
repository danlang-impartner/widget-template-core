import { LoggerLevel } from '../enums';
import { ILoggerConfig, ILoggerLevel, ITransport } from '../interfaces';
import { ConsoleTransport, TransportCompose } from '../transports';

export class LoggerHelper {
  public static createLogger(
    config: ILoggerConfig = this._defaultConfig
  ): ITransport & ILoggerLevel {
    return new TransportCompose(config.transports, config.level);
  }
  private static _defaultConfig: ILoggerConfig = {
    transports: [new ConsoleTransport()],
    level: LoggerLevel.Warn
  };
}
