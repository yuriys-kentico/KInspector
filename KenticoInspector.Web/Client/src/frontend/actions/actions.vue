<template>
  <v-container>
    <v-layout row
              wrap
              class="glass-pane pa-3">
      <v-flex xs12>
        <h1 class="display-2 mb-3">
          Actions
        </h1>
      </v-flex>

      <template v-if="isConnected">
        <action-filters />
        <v-flex xs12>
          <action-list :actions="filteredActions" />
        </v-flex>
      </template>

      <v-flex v-else
              xs12>
        <v-card color="error">
          <v-card-text>
            Disconnected
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import { mapActions, mapGetters } from 'vuex'
  import ActionList from './action-list'
  import ActionFilters from './action-filters'

  export default {
    components: {
      ActionList,
      ActionFilters
    },
    props: {
      instanceGuid: {
        type: String,
        required: true
      }
    },
    computed: {
      ...mapGetters('instances', [
        'connectedInstanceDetails',
        'isConnected'
      ]),
      ...mapGetters('actions', [
        'filteredActions'
      ])
    },
    methods: {
      ...mapActions('actions', [
        'getAllActions',
        'resetFilterSettings'
      ]),
      ...mapActions('instances', [
        'getInstances',
        'getInstanceDetails',
      ]),
      initPage: async function () {
        if (!this.isConnected) {
            await this.getInstances();
            await this.getInstanceDetails(this.instanceGuid);
        }

        this.getAllActions(this.connectedInstanceDetails.guid)
        this.resetFilterSettings({ version: this.connectedInstanceDetails.databaseVersion })
      }
    },
    watch: {
      '$route': {
        handler: 'initPage',
        immediate: true
      }
    }
  }
</script>