import { LoggerLevel } from '../enums';
import { ITransport } from '../interfaces';
import { TransportCompose } from './index';

describe('prm/core/services/transport-compose', () => {
  let transportMock: ITransport;

  beforeEach(() => {
    transportMock = {
      log: jasmine.createSpy('log'),
      debug: jasmine.createSpy('debug'),
      info: jasmine.createSpy('info'),
      warn: jasmine.createSpy('warn'),
      error: jasmine.createSpy('error'),
      trace: jasmine.createSpy('trace')
    };
  });

  describe('log()', () => {
    it('Should return transport log if right level', () => {
      const composeMock = new TransportCompose([transportMock], LoggerLevel.Log);
      composeMock.log('hello');
      expect(transportMock.log).toHaveBeenCalledWith('hello');
    });
  });

  describe('debug()', () => {
    it('Should return transport debug if right level', () => {
      const composeMock = new TransportCompose([transportMock], LoggerLevel.Debug);
      composeMock.debug('hello');
      expect(transportMock.debug).toHaveBeenCalledWith('hello');
    });
  });

  describe('info()', () => {
    it('Should return transport info if right level', () => {
      const composeMock = new TransportCompose([transportMock], LoggerLevel.Info);
      composeMock.info('hello');
      expect(transportMock.info).toHaveBeenCalledWith('hello');
    });
  });

  describe('warn()', () => {
    it('Should return transport info if right level', () => {
      const composeMock = new TransportCompose([transportMock], LoggerLevel.Warn);
      composeMock.warn('hello');
      expect(transportMock.warn).toHaveBeenCalledWith('hello');
    });
  });

  describe('error()', () => {
    it('Should return transport info if right level', () => {
      const composeMock = new TransportCompose([transportMock], LoggerLevel.Error);
      composeMock.error('hello');
      expect(transportMock.error).toHaveBeenCalledWith('hello');
    });
  });

  describe('trace()', () => {
    it('Should return transport info if right level', () => {
      const composeMock = new TransportCompose([transportMock], LoggerLevel.Trace);
      composeMock.trace('hello');
      expect(transportMock.trace).toHaveBeenCalledWith('hello');
    });
  });
});
