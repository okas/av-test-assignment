import { createApp } from "vue";
import AppRoot from "./AppRoot.vue";
import { createPinia } from "pinia";
import useAppStore from "./stores/app-store";
import router from "./router";
import apiClient from "./plugins/swaggerClientPlugin";
import { createStoreInterceptors } from "./plugins/swaggerClientPlugin/interceptors";
import translatorPlugin from "./plugins/translatorPlugin";
import { supportedLanguages, fallBackLanguage } from "./translations/index";

const app = createApp(AppRoot);

const swaggerOptions = {
  url: "/swagger/v1/swagger.json",
  ...createStoreInterceptors(useAppStore),
};

const translatorOptions = {
  getLanguageAutomatically: () => useAppStore().language,
  supportedLanguages: supportedLanguages.map((item) => item.iso),
  fallBackLanguage,
  rootFolder: "translations",
};

app
  .use(createPinia())
  .use(router)
  .use(apiClient, swaggerOptions)
  .use(translatorPlugin, translatorOptions);

app.mount("#app-root");
