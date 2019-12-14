const NAMESPACE = 'com.impartner.widget.custom';
const author = require('./package.json').vendor;

module.exports = {
  output: {
    jsonpFunction: `${NAMESPACE}.${author}${Date.now().toString()}`
  }
};
