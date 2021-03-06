<template>
  <v-card class="mt-4">
    <v-toolbar flat
               dense class="pt-2">
      <v-toolbar-title>
        <vue-showdown :markdown="action.metadata.details.name" />
      </v-toolbar-title>
      <v-spacer />
      <div class="d-flex">
        <v-chip v-for="tag in Object.values(action.metadata.tags)"
                :key="tag"
                small
                disabled
                text-color="black"
                class="hidden-xs-only mr-1">
          {{ tag }}
        </v-chip>
      </div>

      <v-btn icon :disabled="incompatible" @click="execute(runConfiguration)">
        <v-icon>{{ hasResults ? 'mdi-refresh' : 'mdi-play' }}</v-icon>
      </v-btn>
    </v-toolbar>

    <v-card-text class="pt-0 pb-0 pr-0">
      <v-layout align-center
                justify-center
                row
                fill-height
                mr-1
                px-3
                py-2
                @click="showDescription = !showDescription">
        <v-flex>
          <vue-showdown :markdown="action.metadata.details.shortDescription" />
        </v-flex>
        <v-spacer></v-spacer>
        <v-chip v-if="untested"
                color="amber"
                class="mr-3"
                label
                small>
          Untested
        </v-chip>
        <v-chip v-if="incompatible"
                color="red darken-1"
                class="mr-3"
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
        <vue-showdown :markdown="action.metadata.details.longDescription" />
      </v-card-text>
    </v-slide-y-transition>

    <v-card-text v-if="hasResults" class="pa-0 subheading">
      <v-layout align-center
                justify-center
                fill-height
                pl-3
                pr-4
                py-2
                :class="status"
                @click="showResults = !showResults"
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
        <v-flex shrink>
          <v-icon>{{ showResults ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
        </v-flex>
      </v-layout>
    </v-card-text>

    <v-slide-y-transition>
      <v-card-text v-if="showResults && hasResults">
        TODO: Display results 😉
      </v-card-text>
    </v-slide-y-transition>
  </v-card>
</template>

<style>
  .summary {
    position: relative
  }

    .summary p {
      margin: 2px
    }
</style>

<script>
  import { mapActions, mapGetters } from 'vuex'
  import semver from 'semver'

  export default {
    components: {
    },
    props: {
      action: {
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
      ...mapGetters('actions', ['getActionResult']),
      results: function () {
        const actionResults = this.getActionResult(this.action.codeName, this.connectedInstanceDetails.guid)
        return actionResults.results
      },
      hasResults: function () {
        return !!this.results
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
        return semver.satisfies(this.instanceVersion, this.action.compatibleVersions)
      },
      incompatible: function () {
        return !semver.satisfies(this.instanceVersion, this.action.incompatibleVersions)
      },
      untested: function () {
        return !this.compatible && !this.incompatible
      },
      runConfiguration: function () {
        return {
          codeName: this.action.codeName,
          instanceGuid: this.connectedInstanceDetails.guid,
          options: {
            userId: 53
          }
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
      ...mapActions('actions', ['execute'])
    }
  }
</script>