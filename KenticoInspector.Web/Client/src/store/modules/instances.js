import api from "../../services";

const state = {
  items: [],
  currentInstanceDetails: null,
  upserting: false,
  upsertingError: null,
  connecting: false,
  connectionError: null
};

const getters = {
  isConnected: state => {
    return !!state.currentInstanceDetails;
  },

  connectedInstance: (state, getters) => {
    return getters.isConnected ? state.items.find(item => item.guid === state.currentInstanceDetails.guid) : null;
  },

  connectedInstanceDetails: (state, getters) => {
    return getters.isConnected ? state.currentInstanceDetails : null;
  },

  getInstanceDisplayName: state => guid => {
    const name = state.items.find(item => item.guid === guid).name;
    return name ? name : "(UNNAMED)";
  }
};

const actions = {
  async getInstances({ commit }) {
    commit("setItems", await api.getInstances());
  },

  async upsertInstance({ commit, dispatch }, instance) {
    commit("setUpserting", true);
    try {
      const newInstance = await api.upsertInstance(instance);
      dispatch("getInstances");
      return newInstance;
    } catch (error) {
      commit("setUpsertingError", error);
    }

    commit("setUpserting", false);
  },

  async deleteInstance({ dispatch }, guid) {
    await api.deleteInstance(guid);
    await dispatch("getInstances");
  },

  async getInstanceDetails({ commit }, guid) {
    commit("setConnecting", true);

    try {
      const instanceDetails = await api.getInstanceDetails(guid);
      commit("setCurrentInstanceDetails", instanceDetails);
    } catch (error) {
      commit("setConnectionError", error);
    }

    commit("setConnecting", false);
  },

  cancelConnecting: ({ commit }) => {
    commit("setConnecting", false);
    commit("setConnectionError", null);
  },

  disconnect: ({ commit }) => {
    commit("setCurrentInstanceDetails", null);
  }
};

const mutations = {
  setConnecting(state, status) {
    state.connecting = status;
  },

  setConnectionError(state, reason) {
    state.connectionError = reason;
  },

  setUpserting(state, status) {
    state.upserting = status;
  },

  setUpsertingError(state, reason) {
    state.upsertingError = reason;
  },

  setCurrentInstanceDetails(state, instanceDetails) {
    state.currentInstanceDetails = instanceDetails;
  },

  setItems(state, items) {
    state.items = items;
  }
};

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
};