const path = require('path');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const TsDeclarationWebpackPlugin = require('ts-declaration-webpack-plugin');

const basicConfig = {
  entry: './src/main.ts',
  module: {
    rules: [
      {
        test: /\.ts?$/,
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
      name: 'widget-runtime.d.ts',
      options: { umdModuleName: 'impartner' }
    })
  ],
  output: {
    filename: 'widget-runtime.umd.js',
    path: path.resolve(__dirname, 'dist'),
    libraryTarget: 'umd',
    library: '@impartner/widget-runtime'
  }
};

const singleJsOutput = {
  ...basicConfig,
  output: {
    filename: 'widget-runtime.js',
    path: path.resolve(__dirname, 'dist'),
    libraryTarget: 'var',
    library: 'ImpartnerWidgetRuntime'
  }
};

module.exports = [amdOutput, singleJsOutput];
