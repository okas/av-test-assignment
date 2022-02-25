import { StoreDefinition } from "pinia";

export declare type LoadingStateStore = any &
  StoreDefinition<
    string,
    {},
    {},
    {
      // eslint-disable-next-line no-unused-vars
      setLoading(state: boolean): void;
    }
  >;

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
