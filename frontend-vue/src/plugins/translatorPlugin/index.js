import { inject } from "vue";

/**
 * @typedef TranslatorConfig Translator options
 * @type {object} 
 * @property {string} [rootFolder=translations] Root path of translations in `/src` folder
 */

/**
 * @param {string} templateRoot
 * @param {string} modulePath
 * @returns {Promise<Object>}
 */
async function translationResolverAsync(templateRoot, modulePath, language = "") {
  let modulePromise;

  // Consider: https://github.com/tc39/proposal-dynamic-import#import
  // Consider: https://github.com/rollup/plugins/tree/master/packages/dynamic-import-vars#limitations
  switch (language) {
    default:
    case "en":
      modulePromise = import(`../../${templateRoot}/en/${modulePath}.js`);
      break;
    case "et":
      modulePromise = import(`../../${templateRoot}/et/${modulePath}.js`);
      break;
  }

  const module = await modulePromise;

  return module.default;
}

export const pluginSymbol = Symbol("translator plugin: resolver symbol");

/**
 * @param {import("vue").App<any>} app
 * @param {TranslatorConfig} config
 */
export default function install(app, config = { rootFolder: "translations" }) {
  const { globalProperties } = app.config;
  const root = config?.rootFolder ?? "translations";
  // For Options API users.
  globalProperties.$translatorResolverAsync = async (
    /** @type {string} */ modulePath,
    /** @type {string} */ language = ""
  ) => {
    const lng =
      language?.length === 2
        ? language
        : globalProperties.$store?.state?.language ?? "";

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
