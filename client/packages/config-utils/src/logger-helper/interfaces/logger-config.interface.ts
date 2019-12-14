import { LoggerLevel } from '../enums';
import { ITransport } from './transport.interface';

export interface ILoggerConfig {
  transports: ITransport[];
  level: LoggerLevel;
}
