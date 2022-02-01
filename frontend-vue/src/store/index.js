import { createStore } from "vuex";

const state = {
  loading: true,
};

const getters = {};

const mutations = {
  SET_LOADING(state, payload) {
    state.loading = payload;
  },
};

export default createStore({
  state,
  getters,
  //actions
  mutations,
  // modules,
});
