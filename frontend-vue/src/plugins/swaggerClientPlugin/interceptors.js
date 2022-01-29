export function createStoreInterceptors(store) {
  const mutation = "SET_LOADING";

  return {
    requestInterceptor(request) {
      store.commit(mutation, true);
      return request;
    },

    responseInterceptor(response) {
      store.commit(mutation, false);
      return response;
    },
  };
}
