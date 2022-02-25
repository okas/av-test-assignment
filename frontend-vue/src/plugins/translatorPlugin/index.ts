import { inject, watchPostEffect } from "vue";
import { isUselessString } from "../../utils/stringHelpers";

/**
 * @typedef TranslatorConfig Translator options.
 * @type {object}
 * @property {() => LanguageStore} useStore Store with language related property and action
 * @property {string[]} supportedLanguages language list, 2 char length.
 * @property {string} fallBackLanguage
 * @property {string} [rootFolder=translations] Root path of translations in `/src` folder.
 */

/**
 * @param {string} templateRoot
 * @param {string} modulePath
 * @param {string} language
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
 * @param {LanguageStore} store
 * @param {TranslatorConfig}
 * @throws {Error} If resolution fails.
 */
function resolveLanguage(
  askedLanguage,
  store,
  { supportedLanguages, fallBackLanguage }
) {
  if (askedLanguage && askedLanguage in supportedLanguages) {
    return askedLanguage;
  }

  const automaticLanguage = store.language;

  if (automaticLanguage) {
    return automaticLanguage;
  }

  if (fallBackLanguage) {
    console.warn("Using fallback language: ", fallBackLanguage);
    return fallBackLanguage;
  }

  throw new Error("Could not resolve language");
}

const storageKey = "app:language";

/**
 * Sets up watcher for `store.language` state and writes changes values to <html lang=""> and `windows.localStorage`.
 * @param {LanguageStore} store
 * @param {TranslatorConfig}
 * @returns Handler function.
 * @throws If attempted value is not in supported languages list, then throws.
 */
function getLanguageStateSideEffects(store, { supportedLanguages }) {
  return () => {
    const newValue = store.language;

    if (isUselessString(newValue)) {
      // In case store.$reset is called, we want to clear these places in reasonable way
      document.documentElement.lang = "";
      window.localStorage.removeItem(storageKey);
      console.warn(
        "Seems like language has been reset, so clearing `<html lang>` and `window.localStorage` in sane way."
      );
      return;
    }

    if (supportedLanguages.includes(newValue)) {
      document.documentElement.lang = newValue;
      window.localStorage.setItem(storageKey, newValue);
    } else {
      throw new Error(
        `Attempt to set unsupported language "${newValue}" fails.`
      );
    }
  };
}

/**
 * Calculates initial language: tries to obtain value from `window.localStore` and validates it.
 * If fails, then validates `navigator.language`. As a last resort, `fallbackLanguage` will be used.
 * @param {LanguageStore} store
 * @param {TranslatorConfig}
 */
function storeInitialLanguage(store, { supportedLanguages, fallBackLanguage }) {
  let initialLang = window.localStorage.getItem(storageKey);

  if (isUselessString(initialLang)) {
    initialLang = supportedLanguages.includes(navigator.language)
      ? navigator.language
      : fallBackLanguage;
  }
  store.setLanguage(initialLang);
}

const storeSetLanguageGuard = ({ name, args: [newValue] }) => {
  if (name === "setLanguage" && isUselessString(newValue)) {
    throw new Error(
      `Attempt to set useless string "${newValue}" as a language fails.`
    );
  }
};

export const pluginSymbol = Symbol("translator plugin: resolver symbol");

/**
 * @param {import("vue").App<any>} app
 * @param {TranslatorConfig} config
 */
export default function install(app, config) {
  const { globalProperties } = app.config;
  const root = config?.rootFolder ?? "translations";

  const store = config.useStore();

  watchPostEffect(getLanguageStateSideEffects(store, config));

  storeInitialLanguage(store, config);

  store.$onAction(storeSetLanguageGuard);

  // For Options API users.
  globalProperties.$translatorResolverAsync = async (
    /** @type {string} */ modulePath,
    /** @type {string} */ language = ""
  ) => {
    const lng = resolveLanguage(language, store, config);
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
