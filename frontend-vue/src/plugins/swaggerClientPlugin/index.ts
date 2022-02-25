import { App, inject, InjectionKey } from "vue";
import Swagger from "swagger-client/es";

export declare type SwaggerClient = Promise<Swagger> & Swagger;

export const swaggerClientInjectionKey: InjectionKey<SwaggerClient> = Symbol(
  "swagger-client plugin symbol"
);

export default function apiClientPlugin(app: App<any>, options) {
  const apiClient = new Swagger(options);
  /** For Component API */
  app.config.globalProperties.$apiClient = apiClient;
  /** For Composition API */
  app.provide(swaggerClientInjectionKey, apiClient);
}

export function useApiClient() {
  const apiClient = inject(swaggerClientInjectionKey);
  if (!apiClient) {
    throw new Error(`No api client :${typeof Swagger}!`);
  }
  return apiClient;
}
