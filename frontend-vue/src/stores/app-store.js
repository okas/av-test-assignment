import { defineStore } from "pinia";
import acceptHmr from "../utils/acceptHmr";

const useRootStore = defineStore("app", {
  state: () => ({
    loading: true,
    language: "",
  }),

  actions: {
    /**@param {boolean} state */
    setLoading(state) {
      this.loading = state;
    },

    /** @param {string} language */
    setLanguage(language) {
      this.language = language;
    },
  },
});

acceptHmr(useRootStore);

export default useRootStore;
