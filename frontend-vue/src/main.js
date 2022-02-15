import { createApp } from "vue";
import AppRoot from "./AppRoot.vue";
import { createPinia } from "pinia";
import useAppStore from "./stores/app-store";
import router from "./router";
import apiClient from "./plugins/swaggerClientPlugin";
import { createStoreInterceptors } from "./plugins/swaggerClientPlugin/interceptors";
import translatorPlugin from "./plugins/translatorPlugin";
import { supportedLanguages, fallBackLanguage } from "./translations/index";
import htmlMetadataMiddleWare from "./middlewares/htmlMetadataMiddleWare";

const appName = "Demo";
const translationsRootFolder = "translations";

const app = createApp(AppRoot);

const htmlMetaDataOptions = {
  titleTemplate: `%s | ${appName}`,
  appName,
  translationsRootFolder,
  useStore: useAppStore,
};

const swaggerOptions = {
  url: "/swagger/v1/swagger.json",
  ...createStoreInterceptors(useAppStore),
};

const translatorOptions = {
  useStore: useAppStore,
  supportedLanguages: supportedLanguages.map((item) => item.iso),
  fallBackLanguage,
  rootFolder: translationsRootFolder,
};

app
  .use(createPinia())
  .use(router)
  .use(htmlMetadataMiddleWare, htmlMetaDataOptions)
  .use(translatorPlugin, translatorOptions)
  .use(apiClient, swaggerOptions);

app.mount("#app-root");
