'use strict';

const fs = require('fs-extra');
const glob = require('glob');
const axios = require('axios');

const LANGUAGES = ['es', 'fr', 'pt', 'de'];
const TRANSLATION_ENDPOINT = 'https://translate.yandex.net/api/v1.5/tr.json/translate';
const TRANSLATION_API_KEY =
  'trnsl.1.1.20191004T162156Z.c550852f29ac2d59.a7fc1f42c5c2602ee420040a514de33533187395';

function getFiles(path) {
  return new Promise((resolve, reject) =>
    glob('./src/assets/i18n/**/en.json', (err, files) => {
      if (err) reject(err);
      resolve(files);
    })
  );
}

function readFileAsync(path) {
  return new Promise((resolve, reject) => {
    fs.readFile(path, (err, data) => {
      if (err) reject(err);
      resolve(data);
    });
  });
}

async function translateInCloud(word, fromLanguage, toLanguage) {
  const { data } = await axios({
    method: 'post',
    url: TRANSLATION_ENDPOINT,
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    data: `key=${TRANSLATION_API_KEY}&text=${word}&lang=${fromLanguage}-${toLanguage}`
  });

  if (data.code === 200) {
    return data.text[0];
  }

  return word;
}

async function translateJson(jsonObject, languageCode) {
  for (let key in jsonObject) {
    const value = jsonObject[key];

    if (typeof value === 'string') {
      jsonObject[key] = await translateInCloud(value, 'en', languageCode);
    } else {
      await translateJson(jsonObject[key], languageCode);
    }
  }
}

async function generateLocales() {
  const englishLocalizationFiles = await getFiles('./src/assets/i18n/**/en.json');

  for (let filePath of englishLocalizationFiles) {
    const fileContent = await readFileAsync(filePath);
    const destinationPathTranslations = filePath.replace(/^(.+?i18n\/[^\/]+?)\/en.json$/, '$1');

    for (let languageCode of LANGUAGES) {
      let translatedJsonContent = JSON.parse(fileContent);
      await translateJson(translatedJsonContent, languageCode);

      fs.writeFileSync(
        `${destinationPathTranslations}/${languageCode}.json`,
        JSON.stringify(translatedJsonContent)
      );
    }
  }
}

generateLocales().then(
  res => console.log('Translations ran successfully!'),
  err => console.error('oops an error! : %o', err)
);
