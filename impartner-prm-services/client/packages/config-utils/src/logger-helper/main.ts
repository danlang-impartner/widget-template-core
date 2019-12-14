import { LoggerHelper } from './helpers';
import { ILoggerLevel, ITransport, IWindow } from './interfaces';

export * from './helpers';
export * from './enums';

export const Logger: ITransport & ILoggerLevel = (() => {
  const COM_IMPARTNER_LOGGER = 'com.impartner.logger';

  if (!(window as IWindow)[COM_IMPARTNER_LOGGER]) {
    (window as IWindow)[COM_IMPARTNER_LOGGER] = LoggerHelper.createLogger();
  }

  return (window as IWindow)[COM_IMPARTNER_LOGGER];
})();
