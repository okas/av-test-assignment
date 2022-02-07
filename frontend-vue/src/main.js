import { createApp } from "vue";
import AppRoot from "./AppRoot.vue";
import store from "./store";
import router from "./router";
import apiClient from "./plugins/swaggerClientPlugin";
import { createStoreInterceptors } from "./plugins/swaggerClientPlugin/interceptors";
import translatorPlugin from "./plugins/translatorPlugin";

const app = createApp(AppRoot);

let swaggerOptions = Object.assign(
  { url: "/swagger/v1/swagger.json" },
  createStoreInterceptors(store)
);

app
  .use(store)
  .use(router)
  .use(apiClient, swaggerOptions)
  .use(translatorPlugin, { rootFolder: "translations" });

app.mount("#app-root");
