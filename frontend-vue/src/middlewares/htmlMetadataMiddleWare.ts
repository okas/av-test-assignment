import { useTitle } from "@vueuse/core";
import { StoreDefinition } from "pinia";
import { watchEffect, App } from "vue";
import { Router } from "vue-router";
import { isUselessString } from "../utils/stringHelpers";

export type LanguageStore = StoreDefinition<string, { language: string }>;

export interface HtmlMetadataMiddleWareConfig {
  titleTemplate: string;
  appName: string;
  translationsRootFolder: string;
  useStore: LanguageStore;
}

export default function install(
  app: App,
  {
    titleTemplate,
    appName,
    translationsRootFolder,
    useStore,
  }: HtmlMetadataMiddleWareConfig
): void {
  const router: Router = app.config.globalProperties.$router;
  const currentRoute = router.currentRoute;
  const title = useTitle(appName);
  const store = useStore();

  let titlesOfCurrentLanguage: Map<string, string> = null;

  const titleChanger = (routeName: string) => {
    const currentTitle = titlesOfCurrentLanguage.get(routeName);
    title.value = isUselessString(currentTitle)
      ? appName
      : titleTemplate.replace("%s", currentTitle);
  };

  watchEffect(async () => {
    if (!isUselessString(store.language)) {
      const module = await import(
        `../../../src/${translationsRootFolder}/${store.language}/htmlMetadata.js`
      );
      titlesOfCurrentLanguage = new Map(Object.entries(module.default));
    }
    const routeName = currentRoute?.value?.name?.toString();
    if (!isUselessString(routeName)) {
      titleChanger(routeName);
    }
  });

  // This ensures that titlesOfCurrentLanguage has been filled by the time hook runs.
  router.afterEach(({ name }) => titleChanger(name.toString()));
}
