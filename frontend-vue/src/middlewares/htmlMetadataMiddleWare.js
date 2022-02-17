import { useTitle } from "@vueuse/core";
import { watchEffect } from "vue";
import { isUselessString } from "../utils/stringHelpers";

/**
 * @typedef HtmlMetadataMiddleWareConfig
 * @type {object}
 * @property  {string} titleTemplate
 * @property  {string} appName
 * @property {string} translationsRootFolder Root path of translations in `/src` folder.
 * @property  {() => LanguageStore} useStore Store with language related property and action
 */

/**
 * @param {import("vue").App<any>} app
 * @param {HtmlMetadataMiddleWareConfig}
 */
export default function install(
  app,
  { titleTemplate, appName, translationsRootFolder, useStore }
) {
  const router = app.config.globalProperties.$router;
  const currentRoute = router.currentRoute;
  const title = useTitle(appName);
  const store = useStore();

  let titlesOfCurrentLanguage = {};

  const titleChanger = (routeName) => {
    const currentTitle = titlesOfCurrentLanguage[routeName];
    title.value = isUselessString(currentTitle)
      ? appName
      : titleTemplate.replace("%s", currentTitle);
  };

  watchEffect(async () => {
    if (!isUselessString(store.language)) {
      const module = await import(
        `../../../src/${translationsRootFolder}/${store.language}/htmlMetadata.js`
      );
      titlesOfCurrentLanguage = module.default;
    }
    if (!isUselessString(currentRoute.value.name)) {
      titleChanger(currentRoute.value.name);
    }
  });

  // This ensures that titlesOfCurrentLanguage has been filled by the time hook runs.
  router.afterEach(({ name }) => titleChanger(name));
}
