export function createInterceptors(app) {
  const mutation = "SET_LOADING";

  return {
    requestInterceptor(request) {
      app.config.globalProperties.$store.commit(mutation, true);
      return request;
    },

    responseInterceptor(response) {
      app.config.globalProperties.$store.commit(mutation, false);
      return response;
    },
  };
}
