import { defineStore } from "pinia";
import useAcceptHmr from "./helpers";

const useAppStore = defineStore("app", {
  state: () => ({
    loading: true,
    language: "",
  }),

  actions: {
    setLoading(state: boolean) {
      this.loading = state;
    },

    setLanguage(language: string) {
      this.language = language;
    },
  },
});

useAcceptHmr(useAppStore);

export default useAppStore;
