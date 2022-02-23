import { acceptHMRUpdate, StoreDefinition } from "pinia";

export default function useAcceptHmr(store: StoreDefinition) {
  if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(store, import.meta.hot));
  }
}
