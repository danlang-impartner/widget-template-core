import { ConsoleTransport } from './console.transport';

describe('console.transport.ts', () => {
  let objectUnderTest: ConsoleTransport;

  beforeEach(() => {
    objectUnderTest = new ConsoleTransport();
  });

  describe('log()', () => {
    it('should print in standard console', () => {
      spyOn(console, 'log');

      objectUnderTest.log('this is a test: %s', 'another string');

      expect(console.log).toHaveBeenCalledWith('this is a test: %s', 'another string');
    });
  });

  describe('debug()', () => {
    it('should print in standard console', () => {
      spyOn(console, 'debug');

      objectUnderTest.debug('this is a test: %s', 'another string');

      expect(console.debug).toHaveBeenCalledWith('this is a test: %s', 'another string');
    });
  });

  describe('info()', () => {
    it('should print in standard console', () => {
      spyOn(console, 'info');

      objectUnderTest.info('this is a test: %s', 'another string');

      expect(console.info).toHaveBeenCalledWith('this is a test: %s', 'another string');
    });
  });

  describe('warn()', () => {
    it('should print in standard console', () => {
      spyOn(console, 'warn');

      objectUnderTest.warn('this is a test: %s', 'another string');

      expect(console.warn).toHaveBeenCalledWith('this is a test: %s', 'another string');
    });
  });

  describe('error()', () => {
    it('should print in standard console', () => {
      spyOn(console, 'error');

      objectUnderTest.error('this is a test: %s', 'another string');

      expect(console.error).toHaveBeenCalledWith('this is a test: %s', 'another string');
    });
  });

  describe('trace()', () => {
    it('should print in standard console', () => {
      spyOn(console, 'trace');

      objectUnderTest.trace('this is a test: %s', 'another string');

      expect(console.trace).toHaveBeenCalledWith('this is a test: %s', 'another string');
    });
  });
});
