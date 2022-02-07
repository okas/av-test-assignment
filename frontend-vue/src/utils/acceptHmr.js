import { acceptHMRUpdate } from "pinia";

/**
 * @param {import("pinia").StoreDefinition} store
 */
export default function acceptHmr(store) {
  if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(store, import.meta.hot));
  }
}
