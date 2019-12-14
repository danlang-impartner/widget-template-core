const path = require('path');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const TsDeclarationWebpackPlugin = require('ts-declaration-webpack-plugin');

const basicConfig = {
  entry: './src/index.ts',
  module: {
    rules: [
      {
        test: /\.(t|j)s?$/,
        use: 'ts-loader',
        exclude: [/node_modules/]
      }
    ]
  },
  resolve: {
    extensions: ['.ts', '.js'],
    modules: [path.resolve(__dirname), 'node_modules']
  }
};

const amdOutput = {
  ...basicConfig,
  // only assign the plugin to the first output config, so the dist folder gets cleaned
  plugins: [
    new CleanWebpackPlugin(['dist/**'], { verbose: true, beforeEmit: true, watch: true }),
    new TsDeclarationWebpackPlugin({
      name: 'config-utils.d.ts',
      options: { umdModuleName: '@impartner/config-utils' }
    })
  ],
  output: {
    filename: 'index.js',
    path: path.resolve(__dirname, 'dist'),
    libraryTarget: 'umd',
    library: '@impartner/config-utils'
  }
};

const singleJsOutput = {
  ...basicConfig,
  output: {
    filename: 'config-utils.js',
    path: path.resolve(__dirname, 'dist'),
    libraryTarget: 'var',
    library: 'configUtils'
  }
};

module.exports = [amdOutput, singleJsOutput];
