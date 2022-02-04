import { inject } from "vue";

/**
 * @param {string} modulePath
 * @returns {Promise<Object>}
 */
async function translationResolverAsync(modulePath, language = "") {
  let modulePromise;

  // Import expressions static prefixes will be bundeled to chunks based WebPack static analyzer.
  switch (language) {
    default:
    case "en":
      modulePromise = () => import("../../translations/en/" + modulePath);
      break;
    case "et":
      modulePromise = () => import("../../translations/et/" + modulePath);
      break;
  }

  return (await modulePromise()).default;
}

export const pluginSymbol = Symbol("translator plugin: resolver symbol");

/**
 * @param {import("vue").App<any>} app
 */
export default function install(app) {
  const { globalProperties } = app.config;
  // For Options API users.
  globalProperties.$translatorResolverAsync = async (
    /** @type {string} */ modulePath,
    /** @type {string} */ language = ""
  ) => {
    const lng =
      language?.length === 2
        ? language
        : globalProperties.$store?.state?.language ?? "";

    return await translationResolverAsync(modulePath, lng);
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
