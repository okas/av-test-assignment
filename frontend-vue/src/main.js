import { createApp } from "vue";
import AppRoot from "./AppRoot.vue";
import { createPinia } from "pinia";
import useRootStore from "./stores/app-store";
import router from "./router";
import apiClient from "./plugins/swaggerClientPlugin";
import { createStoreInterceptors } from "./plugins/swaggerClientPlugin/interceptors";
import translatorPlugin from "./plugins/translatorPlugin";

const app = createApp(AppRoot);

const swaggerOptions = {
  url: "/swagger/v1/swagger.json",
  ...createStoreInterceptors(useRootStore),
};

const translatorOptions = {
  getLanguageAutomatically: () => useRootStore().language,
  supportedLanguages: ["en", "et"], // TODO obtain...
  fallBackLanguage: "en",
  rootFolder: "translations",
};

app
  .use(createPinia())
  .use(router)
  .use(apiClient, swaggerOptions)
  .use(translatorPlugin, translatorOptions);

app.mount("#app-root");
