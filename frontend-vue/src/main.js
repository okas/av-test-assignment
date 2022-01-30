import { createApp } from "vue";
import AppRoot from "./AppRoot.vue";
import appStore from "./store";
import appRouter from "./router";
import apiClientPlugin from "./plugins/swaggerClientPlugin";
import { createStoreInterceptors } from "./plugins/swaggerClientPlugin/interceptors";

const app = createApp(AppRoot);

let swaggerOptions = Object.assign(
  { url: "/swagger/v1/swagger.json" },
  createStoreInterceptors(appStore)
);

app.use(appStore)
  .use(appRouter)
  .use(apiClientPlugin, swaggerOptions);

app.mount("#app");
