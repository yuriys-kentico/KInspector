<template>
  <v-container>
    <v-layout row
              wrap
              class="glass-pane pa-3">
      <v-flex xs12>
        <h1 class="display-2 mb-3">
          Reports
        </h1>
      </v-flex>

      <template v-if="isConnected">
        <report-filters />
        <v-flex xs12>
          <report-list :reports="filteredReports" />
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
  import ReportFilters from './report-filters'
  import ReportList from './report-list'

  export default {
    components: {
      ReportFilters,
      ReportList
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
      ...mapGetters('reports', [
        'filteredReports'
      ])
    },
    methods: {
      ...mapActions('reports', [
        'getAllReports',
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

        this.getAllReports(this.connectedInstanceDetails.guid)
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