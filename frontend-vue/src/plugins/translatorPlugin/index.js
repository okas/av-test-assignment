import { inject } from "vue";

/**
 * @typedef TranslatorConfig Translator options.
 * @type {object}
 * @property {() => string} getLanguageAutomatically Will be used to get language from somewhere in case of implicit translation call.
 * @property {string[]} supportedLanguages language list, 2 char length.
 * @property {string} fallBackLanguage
 * @property {string} [rootFolder=translations] Root path of translations in `/src` folder.
 */

/**
 * @param {string} templateRoot
 * @param {string} modulePath
 * @returns {Promise<Object>}
 */
async function translationResolverAsync(templateRoot, modulePath, language) {
  // Consider: https://github.com/tc39/proposal-dynamic-import#import
  // Consider: https://github.com/rollup/plugins/tree/master/packages/dynamic-import-vars#limitations
  const module = await import(
    `../../../src/${templateRoot}/${language}/${modulePath}.js`
  );

  return module.default;
}

/**
 * @param {string} askedLanguage
 * @param {TranslatorConfig} param1
 * @throws {Error} If resolution failes.
 */
function resolveLanguage(
  askedLanguage,
  { supportedLanguages, getLanguageAutomatically, fallBackLanguage }
) {
  if (askedLanguage && askedLanguage in supportedLanguages) {
    return askedLanguage;
  }

  const automaticLanguage = getLanguageAutomatically();

  if (automaticLanguage) {
    return automaticLanguage;
  }

  if (fallBackLanguage) {
    console.warn("Using fallback language: ", fallBackLanguage);
    return fallBackLanguage;
  }

  throw new Error("Could not resolve language");
}

export const pluginSymbol = Symbol("translator plugin: resolver symbol");

/**
 * @param {import("vue").App<any>} app
 * @param {TranslatorConfig} config
 */
export default function install(app, config) {
  const { globalProperties } = app.config;
  const root = config?.rootFolder ?? "translations";
  // For Options API users.
  globalProperties.$translatorResolverAsync = async (
    /** @type {string} */ modulePath,
    /** @type {string} */ language = ""
  ) => {
    const lng = resolveLanguage(language, config);
    return await translationResolverAsync(root, modulePath, lng);
  };
  // For Composition API users.
  app.provide(pluginSymbol, globalProperties.$translatorResolverAsync);
}

/**
 * @returns {(modulePath: String, language = "") => Promise<any>}
 */
export function useTranslator() {
  const funcAsync = inject(pluginSymbol);
  return funcAsync;
}
