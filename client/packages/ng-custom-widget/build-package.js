'use strict';

const fs = require('fs-extra');
const concat = require('concat');
const path = require('path');
const packageJson = require('./package.json');

const FILES = ['runtime', 'main'];

const DIRECTORY_PATH = path.join(__dirname, 'dist/temp');
const DISTRIBUTION_PATH = './dist/package';

async function build() {
  const filesToConcat = [];

  await fs.ensureDir(DISTRIBUTION_PATH);
  const filesToBeDeleted = await fs.readdir(DISTRIBUTION_PATH);
  filesToBeDeleted.forEach(async file => {
    await fs.unlink(`${DISTRIBUTION_PATH}/${file}`);
  });
  await fs.ensureFile(`${DISTRIBUTION_PATH}/.npmignore`); // Include path in published npm package

  fs.readdir(DIRECTORY_PATH, async (err, files) => {
    if (err) {
      return console.log('Unable to scan directory: ' + err);
    }

    files.forEach(function(file) {
      if (file.match('^(' + FILES.join('|') + ')(\\.([^\\.]+?))?\\.js$')) {
        filesToConcat.push(`${DIRECTORY_PATH}/${file}`);
      } else if (
        file.match('-ngfactory.[^\\.]+?.js') ||
        file.match('vendor.[^\\.]+?.js')
      ) {
        let fileOriginPath = `${DIRECTORY_PATH}/${file}`;
        let filenameOfDestination = file;

        if (file.match('vendor.[^\\.]+?.js')) {
          filenameOfDestination = file.replace(/^([^\.]+?)\.[^\.]+?\.js$/, '$1.js');
        }

        fs.copyFile(fileOriginPath, `${DISTRIBUTION_PATH}/${filenameOfDestination}`, err => {
          if (err) throw err;
        });
      }
    });

    try {
      const sortOrder = ['runtime', 'polyfills', 'es2015-polyfills', 'scripts', 'main'];
      const regexpBundleName = /^.+?[\\\/]([^\\\/\.]+?)\.[^\\\/\.]+?\.js$/;

      filesToConcat.sort((curr, prev) => {
        const currIndex = sortOrder.indexOf(curr.replace(regexpBundleName, '$1'));
        const prevIndex = sortOrder.indexOf(prev.replace(regexpBundleName, '$1'));
        return currIndex - prevIndex;
      });

      filesToConcat.forEach(file => {
        console.log(
          '  \x1b[32m added to web-widgets.min: \x1b[37m %s',
          file.replace(/^.+?[\\\/]([^\\\/]+?)$/, '$1')
        );
      });

      await concat(filesToConcat, `${DISTRIBUTION_PATH}/${packageJson.name}.min.js`);
    } catch (error) {
      console.error('error!!!: %o', error);
    }
  });
}

build()
  .then(() => {
    console.info('✨ Done packaging');
  })
  .catch(err => {
    console.error('⚠️  Failed packaging', err);
  });
