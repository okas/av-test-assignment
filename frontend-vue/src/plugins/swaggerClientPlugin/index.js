import SwaggerClient from "swagger-client";

export default {
  install: (app, options) => {
    app.config.globalProperties.$api = new SwaggerClient(options);
  },
};
