export interface ITransport {
  log<T>(message: string, ...argument: T[]): void;
  debug<T>(message: string, ...argument: T[]): void;
  info<T>(message: string, ...argument: T[]): void;
  warn<T>(message: string, ...argument: T[]): void;
  error<T>(message: string, ...argument: T[]): void;
  trace<T>(message: string, ...argument: T[]): void;
}
