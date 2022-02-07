import { createStore } from "vuex";

const state = {
  loading: true,
  language: "et",
};

const getters = {};

const mutations = {
  SET_LOADING: (state, payload) => (state.loading = payload),
  SET_LANGUAGE: (state, payload) => (state.language = payload),
};

export default createStore({
  state,
  getters,
  //actions
  mutations,
  // modules,
});
