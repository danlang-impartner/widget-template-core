import { LoggerHelper } from './logger-helper';

describe('logger-helper.ts', () => {
  describe('createLoger()', () => {
    it('Should return a new instance of the TransportCompose', () => {
      const log = LoggerHelper.createLogger();

      expect(log).toBeTruthy();
    });
  });
});
