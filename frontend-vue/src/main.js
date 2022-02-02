import { createApp } from "vue";
import Root from "./Root.vue";
import store from "./store";
import router from "./router";
import apiClient from "./plugins/swaggerClientPlugin";
import { createInterceptors } from "./plugins/swaggerClientPlugin/interceptors";
import translatorPlugin from "./plugins/translatorPlugin";

const app = createApp(Root);

let swaggerOptions = Object.assign(
  { url: "/swagger/v1/swagger.json" },
  createInterceptors(app)
);

app.use(store)
  .use(router)
  .use(apiClient, swaggerOptions)
  .use(translatorPlugin);

app.mount("#app");
