import { defineStore } from "pinia";
import useAcceptHmr from "./helpers";

const useAppStore = defineStore("app", {
  state: () => ({
    loading: true,
    language: "",
  }),

  actions: {
    setLoading(state: Boolean) {
      this.loading = state;
    },

    setLanguage(language: String) {
      this.language = language;
    },
  },
});

useAcceptHmr(useAppStore);

export default useAppStore;
