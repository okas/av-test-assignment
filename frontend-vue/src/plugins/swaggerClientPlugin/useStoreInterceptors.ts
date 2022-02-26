import { LoadingStateStore } from "../../stores/types";

export function useStoreInterceptors(useStore: LoadingStateStore) {
  return {
    requestInterceptor(request: any): any {
      useStore().setLoading(true);
      return request;
    },

    responseInterceptor(response: any): any {
      useStore().setLoading(false);
      return response;
    },
  };
}
