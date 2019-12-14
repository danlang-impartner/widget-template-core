import { ITransport } from '../interfaces';

export class ConsoleTransport implements ITransport {
  public log<T>(message: string, ...argument: T[]): void {
    console.log(message, ...argument);
  }

  public debug<T>(message: string, ...argument: T[]): void {
    console.debug(message, ...argument);
  }

  public info<T>(message: string, ...argument: T[]): void {
    console.info(message, ...argument);
  }

  public warn<T>(message: string, ...argument: T[]): void {
    console.warn(message, ...argument);
  }

  public error<T>(message: string, ...argument: T[]): void {
    console.error(message, ...argument);
  }

  public trace<T>(message: string, ...argument: T[]): void {
    console.trace(message, ...argument);
  }
}
