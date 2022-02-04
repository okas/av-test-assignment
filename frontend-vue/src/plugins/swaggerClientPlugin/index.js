import { inject } from "vue";
import SwaggerClient from "swagger-client";

export const apiClientSymbol = Symbol("swagger-client symbol");

export default function apiClientPlugin(app, options) {
  const apiClient = new SwaggerClient(options);
  /** For Component API */
  app.config.globalProperties.$apiClient = apiClient;
  /** For Composition API */
  app.provide(apiClientSymbol, apiClient);
}

export function useApiClient() {
  const apiClient = inject(apiClientSymbol);
  if (!apiClient) {
    throw new Error(`No api client :${typeof SwaggerClient}!`);
  }
  return apiClient;
}
