import { defineStore } from "pinia";
import acceptHmr from "./acceptHmr";

const useAppStore = defineStore("app", {
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

acceptHmr(useAppStore);

export default useAppStore;
