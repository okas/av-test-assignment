import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import apiClient from "./plugins/swaggerClinetPlugin";

createApp(App)
  .use(router)
  .use(apiClient, "/swagger/v1/swagger.json")
  .mount("#app");
