import Vue from 'vue';
import api from '../../api';
import semver from 'semver';

const state = {
  items: [],
  filterSettings: {
    version: '',
    showIncompatible: false,
    showUntested: false,
    taggedWith: [],
  },
  reportResults: {},
  loadingItems: false,
};

const getters = {
  filtered: (state) => {
    const items = state.items.filter((item) => {
      const version = state.filterSettings.version;
      const showIncompatible = state.filterSettings.showIncompatible;
      const showUntested = state.filterSettings.showUntested;
      const taggedWith = state.filterSettings.taggedWith;

      const isCompatible = semver.satisfies(version, item.compatibleVersions);
      const isIncompatible = !semver.satisfies(version, item.incompatibleVersions);
      const isUntested = !isCompatible && !isIncompatible;

      const meetsCompatibilityFilters =
        isCompatible || (showIncompatible && isIncompatible) || (showUntested && isUntested);

      const meetsTagFilter = taggedWith.length == 0 || item.tags.some((t) => taggedWith.includes(t));

      return meetsCompatibilityFilters && meetsTagFilter;
    });

    return items;
  },

  getTags: (state) => {
    const allTags = state.items.reduce(getTags, []);
    return getUniqueTags(allTags);
  },

  getActionResult: (state) => (codeName, instanceGuid) => {
    const resultId = `${codeName}-${instanceGuid}`;
    const currentResult = state.reportResults[resultId];
    return currentResult
      ? currentResult
      : {
        loading: false,
        results: null,
      };
  },
};

const actions = {
  getAll: ({ commit }, instanceGuid) => {
    api.actionService.getAll(instanceGuid).then((items) => {
      commit('setItems', items);
    });
  },
  execute: ({ commit }, { codeName, instanceGuid, options }) => {
    commit('setItemResults', { codeName, loading: true });
    api.actionService.execute({ codeName, instanceGuid, options }).then((results) => {
      const resultId = `${codeName}-${instanceGuid}`;
      commit('setItemResults', { resultId, loading: false, results });
    });
  },
  resetFilterSettings: (
    { commit },
    { version = '', showIncompatible = false, showUntested = false, taggedWith = [] }
  ) => {
    commit('setFilterSettings', { version, showIncompatible, showUntested, taggedWith });
  },
};

const mutations = {
  setItems(state, items) {
    state.items = items;
  },
  setItemResults(state, { resultId, loading, results }) {
    Vue.set(state.reportResults, resultId, { loading, results });
  },
  setFilterSetting(state, { name, value }) {
    Vue.set(state.filterSettings, name, value);
  },
  setFilterSettings(state, filterSettings) {
    state.filterSettings = filterSettings;
  },
};

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
};

function getTags(allTags, action) {
  allTags.push(...action.tags);
  return allTags;
}

function getUniqueTags(allTags) {
  return [...new Set(allTags)];
}