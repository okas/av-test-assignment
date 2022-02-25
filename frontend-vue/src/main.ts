import { createApp } from "vue";
import AppRoot from "./AppRoot.vue";
import { createPinia } from "pinia";
import useAppStore from "./stores/app-store";
import router from "./router";
import apiClient from "./plugins/swaggerClientPlugin";
import { useStoreInterceptors } from "./plugins/swaggerClientPlugin/useStoreInterceptors";
import translatorPlugin from "./plugins/translatorPlugin";
import { supportedLanguages, fallBackLanguage } from "./translations/index";
import htmlMetadataMiddleWare, {
  HtmlMetadataMiddleWareConfig,
} from "./middlewares/htmlMetadataMiddleWare";

const appName = "Demo";
const translationsRootFolder = "translations";

const app = createApp(AppRoot);

const htmlMetaDataOptions: HtmlMetadataMiddleWareConfig = {
  titleTemplate: `%s | ${appName}`,
  appName,
  translationsRootFolder,
  useStore: useAppStore,
};

const swaggerOptions = {
  url: "/swagger/v1/swagger.json",
  ...useStoreInterceptors(useAppStore),
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
