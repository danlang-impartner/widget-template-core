process.env.CHROME_BIN = require('puppeteer').executablePath();
const webpackConfig = require('./webpack.config.test');

module.exports = function(config) {
  config.set({
    frameworks: ['jasmine'],
    files: [{ pattern: './src/**/*.spec.ts', watched: true }],
    preprocessors: {
      'src/**/*.spec.ts': ['webpack', 'sourcemap']
    },
    webpack: webpackConfig,
    webpackMiddleware: {
      logLevel: 'info',
      watchOptions: {
        aggregateTimeout: 300,
        poll: 1000 // customize depending on PC power
      }
    },
    port: 8080,
    reporters: ['mocha'],
    browsers: ['ChromeHeadless'],
    customLaunchers: {
      ChromeHeadless: {
        base: 'Chrome',
        flags: ['--headless', '--disable-gpu', '--remote-debugging-port=9222']
      }
    }
  });
};
