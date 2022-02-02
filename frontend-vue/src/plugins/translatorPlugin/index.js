import { inject } from "vue";

async function translationResolverAsync(modulePath, language = "")  {
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
};

export const pluginSymbol = Symbol("translator plugin symbol");

export default function translatorPlugin(app) {
  app.config.globalProperties.$translatorResolverAsync = translationResolverAsync;
  app.provide(pluginSymbol, translationResolverAsync);
};

export async function useTranslatorAsync(modulePath, language = "") {
  const resolver = inject(pluginSymbol);
  return await resolver(modulePath, language);
};
