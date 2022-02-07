/**
 * @param {import("pinia").StoreDefinition} useStore
 */
export function createStoreInterceptors(useStore) {
  return {
    requestInterceptor(request) {
      useStore().setLoading(true);
      return request;
    },

    responseInterceptor(response) {
      useStore().setLoading(false);
      return response;
    },
  };
}
