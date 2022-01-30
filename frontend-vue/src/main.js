import { createApp } from "vue";
import Root from "./Root.vue";
import store from "./store";
import router from "./router";
import apiClient from "./plugins/swaggerClientPlugin";
import { createInterceptors } from "./plugins/swaggerClientPlugin/interceptors";

const app = createApp(Root);

let swaggerOptions = Object.assign(
  { url: "/swagger/v1/swagger.json" },
  createInterceptors(app)
);

app.use(store).use(router).use(apiClient, swaggerOptions).mount("#app");
