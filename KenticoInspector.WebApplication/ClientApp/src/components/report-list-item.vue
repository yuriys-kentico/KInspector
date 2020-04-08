<template>
  <v-card class="mt-4">
    <v-toolbar flat
               dense>
      <v-toolbar-title>
        <vue-showdown :markdown="report.metadata.details.name" />
      </v-toolbar-title>
      <v-spacer />
      <div class="d-flex">
        <v-chip v-for="tag in report.tags"
                :key="tag"
                small
                disabled
                text-color="black"
                class="hidden-xs-only">
          {{ tag }}
        </v-chip>
      </div>

      <v-btn icon :disabled="incompatible" @click="runReport(runConfiguration)">
        <v-icon>{{ hasResults ? 'mdi-refresh' : 'mdi-play' }}</v-icon>
      </v-btn>
    </v-toolbar>

    <v-card-text class="pa-0">
      <v-layout align-center
                justify-center
                row
                fill-height
                px-3
                py-2
                @click="showDescription = !showDescription">
        <v-flex>
          <vue-showdown :markdown="report.metadata.details.shortDescription" />
        </v-flex>
        <v-spacer></v-spacer>
        <v-chip v-if="untested"
                color="amber"
                label
                small>
          Untested
        </v-chip>
        <v-chip v-if="incompatible"
                color="red darken-1"
                dark
                label
                small>
          Incompatible
        </v-chip>
        <v-flex shrink class="hidden-xs-only">
        </v-flex>
        <v-flex shrink>
          <v-icon>{{ showDescription ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
        </v-flex>
      </v-layout>
    </v-card-text>

    <v-divider v-show="showDescription"></v-divider>

    <v-slide-y-transition>
      <v-card-text v-show="showDescription">
        <vue-showdown :markdown="report.metadata.details.longDescription" />
      </v-card-text>
    </v-slide-y-transition>

    <v-card-text v-if="hasResults" class="pa-0 subheading">
      <v-layout align-center
                justify-center
                row
                fill-height
                px-3
                py-2
                :class="status"
                @click="hasData && (showResults = !showResults)"
                v-ripple>
        <v-flex>
          <v-icon :color="resultIconColor"
                  class="pr-1"
                  style="float: left">
            {{ resultIcon }}
          </v-icon>
          <vue-showdown :markdown="results.summary" class="summary" tag="span" />
        </v-flex>
        <v-spacer></v-spacer>
        <v-flex shrink v-if="hasData">
          <v-icon>{{ showResults ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
        </v-flex>
      </v-layout>
    </v-card-text>

    <v-slide-y-transition>
      <v-card-text v-if="showResults && hasResults">
        <report-result-details :data="results.data" />
      </v-card-text>
    </v-slide-y-transition>
  </v-card>
</template>

<style>
  .summary {
    position: relative
  }

    .summary p {
      margin: 0
    }
</style>

<script>
  import { mapActions, mapGetters } from 'vuex'
  import ReportResultDetails from "./report-result-details"
  import semver from 'semver'

  export default {
    components: {
      ReportResultDetails
    },
    props: {
      report: {
        type: Object,
        required: true
      }
    },
    data: () => ({
      showDescription: false,
      showResults: false
    }),
    computed: {
      ...mapGetters('instances', ['connectedInstanceDetails']),
      ...mapGetters('reports', ['getReportResult']),
      results: function () {
        const reportResults = this.getReportResult(this.report.codeName, this.connectedInstanceDetails.guid)
        return reportResults.results
      },
      hasResults: function () {
        return !!this.results
      },
      hasData: function () {
        return this.results && this.results.data.length > 0
      },
      status: function () {
        let status = ''

        if (this.hasResults) {
          switch (this.results.status) {
            case "Good":
              status = 'success'
              break
            case "Information":
              status = 'info'
              break
            case "Warning":
              status = 'warning'
              break
            case "Error":
              status = 'error'
              break
          }
        }

        return status
      },
      statusDark: function () {
        return this.status == 'Error' || this.status == "Information"
      },
      instanceVersion: function () {
        return this.connectedInstanceDetails.databaseVersion
      },
      compatible: function () {
        return semver.satisfies(this.instanceVersion, this.report.compatibleVersions)
      },
      incompatible: function () {
        return !semver.satisfies(this.instanceVersion, this.report.incompatibleVersions)
      },
      untested: function () {
        return !this.compatible && !this.incompatible
      },
      runConfiguration: function () {
        return {
          codeName: this.report.codeName,
          instanceGuid: this.connectedInstanceDetails.guid
        }
      },
      resultIcon: function () {
        let icon = ""
        switch (this.status) {
          case "success":
            icon = "mdi-checkbox-marked-circle"
            break
          case "info":
            icon = "mdi-information"
            break
          case "warning":
            icon = "mdi-alert"
            break
          case "error":
            icon = "mdi-alert-octagon"
            break
        }

        return icon
      },
      resultIconColor: function () {
        return `${this.status} darken-3`
      }
    },
    methods: {
      ...mapActions('reports', ['runReport'])
    }
  }
</script>