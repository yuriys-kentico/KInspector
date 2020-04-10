<template>
  <v-app-bar fixed
             clipped-left
             app
             dark
             color="#262524">
    <span>
      <v-img :src="require('../assets/kentico.svg')"
             contain
             height="38"
             width="38"
             style="background-color: #F05A22" />
    </span>
    <v-toolbar-title class="headline text-uppercase ml-1 mr-2 hidden-xs-only">
      <span>Kentico</span>
      <span class="font-weight-light">Inspector</span>
    </v-toolbar-title>

    <v-btn text
           icon
           to="/">
      <v-icon>mdi-home</v-icon>
    </v-btn>

    <v-btn text
           icon
           :to="`/connect/${this.connectedInstanceDetails.guid}/reports`"
           :disabled="!isConnected">
      <v-icon>mdi-file-chart</v-icon>
    </v-btn>
    <v-btn text
           icon
           :to="`/connect/${this.connectedInstanceDetails.guid}/actions`"
           :disabled="!isConnected">
      <v-icon>mdi-flash</v-icon>
    </v-btn>

    <v-spacer></v-spacer>
    <connection-status-indicator></connection-status-indicator>
  </v-app-bar>
</template>

<script>
  import { mapGetters } from 'vuex'
  import ConnectionStatusIndicator from './connection-status-indicator'
  export default {
    components: {
      ConnectionStatusIndicator
    },
    computed: {
      ...mapGetters('instances', [
        'isConnected',
        'connectedInstanceDetails',

      ])
    }
  }
</script>