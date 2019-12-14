'use strict';
const fs = require('fs-extra');

exports.__esModule = true;
exports['default'] = {
  pre: function() {
    console.debug('pre');
  },

  config: function(webpackConfiguration) {
    webpackConfiguration.plugins = webpackConfiguration.plugins.map(plugin => {
      if (plugin.constructor.name === 'AngularCompilerPlugin') {
        const newLazyDef = {};
        for (const key in plugin.options.additionalLazyModules) {
          const realModuleName = key.split('/').pop();
          console.log('previous module name: %s, new module name: %s', key, realModuleName);
          newLazyDef[realModuleName] = plugin.options.additionalLazyModules[key];
        }
        plugin.options.additionalLazyModules = newLazyDef;
      }

      return plugin;
    });

    const ckeditorCacheGroup = {
      test: (module, chunks) => {
        const moduleName = module.nameForCondition ? module.nameForCondition() : '';

        if (moduleName.includes('ckeditor5-build-impartner')) {
          return true;
        }

        return false;
      },
      enforce: true,
      priority: 20,
      name: 'rich-text-editor-edit-module-ngfactory'
    };

    const safePipeCacheGroup = {
      test: (module, chunks) => {
        const moduleName = module.nameForCondition ? module.nameForCondition() : '';

        if (
          moduleName.includes('safe-pipe') ||
          moduleName.includes('ngx-translate') ||
          moduleName.includes('angular-svg-icon') ||
          moduleName.includes('svg-icons') ||
          moduleName.match(/src[\\\/]app[\\\/]core/)
        ) {
          return true;
        }

        return false;
      },
      enforce: true,
      priority: 25,
      name: 'vendor'
    };

    const httpClient = {
      test: (module, chunks) => {
        const moduleName = module.nameForCondition ? module.nameForCondition() : '';

        if (moduleName.includes('http')) {
          return true;
        }

        return false;
      },
      enforce: true,
      priority: 26,
      name: 'vendor'
    };
    if (typeof webpackConfiguration.externals === 'undefined') {
      webpackConfiguration.externals = [];
    }

    webpackConfiguration.externals.push({
      '@impartner/widget-runtime':
        "window['ImpartnerWidgetRuntime'] || require('@impartner/widget-runtime')"
    });

    webpackConfiguration.optimization.splitChunks.cacheGroups.ckeditor = ckeditorCacheGroup;
    webpackConfiguration.optimization.splitChunks.cacheGroups.safePipe = safePipeCacheGroup;
    webpackConfiguration.optimization.splitChunks.cacheGroups.httpClient = httpClient;
    webpackConfiguration.optimization.splitChunks.cacheGroups.vendor.reuseExistingChunk = true;
    webpackConfiguration.optimization.splitChunks.cacheGroups.common.reuseExistingChunk = true;

    // remove svg from file loader
    webpackConfiguration.module.rules = webpackConfiguration.module.rules.map(rule => {
      if (rule.loader === 'file-loader') {
        rule.test = /\.(eot|cur|jpg|png|webp|gif|otf|ttf|woff|woff2|ani)$/;
      }

      return rule;
    });

    // create svg sprite loader
    webpackConfiguration.module.rules.push({
      test: /\.svg$/,
      loader: 'svg-inline-loader'
    });

    return webpackConfiguration;
  },

  post: function() {
    console.debug('post');
  }
};

function debug(webpackConfiguration) {
  const getCircularReplacer = () => {
    const seen = new WeakSet();
    return (key, value) => {
      if (typeof value === 'object' && value !== null) {
        if (seen.has(value)) {
          return;
        }
        seen.add(value);
      }
      return value;
    };
  };

  fs.writeJsonSync('./webpack-plugin/wp-config.json', webpackConfiguration, {
    replacer: getCircularReplacer()
  });
}
