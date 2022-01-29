import { createApp } from "vue";
import AppRoot from "./AppRoot.vue";
import storeRoot from "./store/root";
import appRouter from "./router";
import apiClientPlugin from "./plugins/swaggerClientPlugin";
import { createStoreInterceptors } from "./plugins/swaggerClientPlugin/interceptors";

const app = createApp(AppRoot);

let swaggerOptions = Object.assign(
  { url: "/swagger/v1/swagger.json" },
  createStoreInterceptors(storeRoot)
);

app.use(storeRoot)
  .use(appRouter)
  .use(apiClientPlugin, swaggerOptions);

app.mount("#app");
