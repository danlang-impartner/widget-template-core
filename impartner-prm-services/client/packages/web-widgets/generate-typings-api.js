/**
 * Script to generate .d.ts file with the definitions of the PRM objects.
 * Use it in this way:
 * ```shell
 * node generate-typings-api.js --username <username> --password <password>
 * ```
 * Where _username_ and _password_ is the user/pass that you use to access to PRM
 */
const axios = require('axios');
const fs = require('fs');
const args = require('minimist')(process.argv.slice(2));

const baseUrl = 'https://thilabs.com/rv.dev/api/objects/v1';
const username = args['username'];
const password = args['password'];

/**
 * Make request to REST API of metadata of an object, and (recursively) generate the
 * interface definition for each new prm object data type that found
 *
 * @param {string} objectName Name of the object to build the interface definition
 * @param {string[]} addedTsInterfaces List of the interfaces that were added to the final interface definition
 */
async function getInterfaceDef(objectName, addedTsInterfaces) {
  let contentTsFile = '';
  const result = await axios.get(`${baseUrl}/${objectName}/_describe`, {
    auth: {
      username: username,
      password: password
    }
  });

  let tsDefinitionSubtypes = [];
  contentTsFile = `export interface ${objectName} {`;
  result.data.data.forEach(fieldDescriptor => {
    let dataType = '';
    switch (fieldDescriptor.fieldType) {
      case 'Currency':
      case 'Id':
      case 'Percent':
      case 'Integer': {
        dataType = 'number';
        break;
      }
      case 'Boolean': {
        dataType = 'boolean';
        break;
      }
      default: {
        dataType = 'string';
        break;
      }
    }

    if (fieldDescriptor.fieldType === 'Fk') {
      dataType = fieldDescriptor.fkFieldType;
      if (!addedTsInterfaces.includes(dataType)) {
        tsDefinitionSubtypes.push(dataType);
        addedTsInterfaces.push(dataType);
      }
    }

    let fieldName = fieldDescriptor.name.charAt(0).toLowerCase() + fieldDescriptor.name.slice(1);
    let isRequired = fieldDescriptor.isRequired ? '' : '?';

    contentTsFile += `${fieldName}${isRequired}: ${dataType};`;
  });
  contentTsFile += '}\n';

  addedTsInterfaces.push(objectName);

  for (let subtype of tsDefinitionSubtypes) {
    contentTsFile += await getInterfaceDef(subtype, addedTsInterfaces);
  }

  return contentTsFile;
}

const createFile = async () => {
  let addedSubtypes = [];

  let results = await Promise.all([
    getInterfaceDef('Deal', addedSubtypes),
    getInterfaceDef('Opportunity', addedSubtypes),
    getInterfaceDef('Sale', addedSubtypes)
  ]);

  let contentFile = results.join('');

  data = `declare module '@impartner/api' {
    export namespace prmObjects {${contentFile}}}`;
  fs.writeFile('./src/typings/@impartner/api/prm-objects.d.ts', data, function(err) {
    if (err) throw err;
    console.log('File was created');
  });
};

createFile();
