<template>
  <v-menu offset-y>
    <template v-slot:activator="{ on }">
      <v-btn text
             v-on="on"
             active-class="ignore"
             :color="color">
        <div v-if="isConnected"
             style="text-align:right"
             class="caption">
          <span class="grey--text text-lowercase">Server </span>
          <span class="white--text">{{connectedInstance.databaseSettings.server}}</span>
          <br>
          <span class="grey--text text-lowercase">Database </span>
          <span class="white--text">{{connectedInstance.databaseSettings.database}}</span>
        </div>
        <span v-else>Disconnected</span>
        <v-icon right>
          {{icon}}
        </v-icon>
      </v-btn>
    </template>
    <v-card>
      <instance-details-toolbar v-if="isConnected"
                                :instance="connectedInstance">
      </instance-details-toolbar>
      <v-card-actions>
        <v-btn v-if="!isConnected"
               to="/connect"
               block
               color="success">
          Connect
        </v-btn>
        <v-btn v-else
               @click="doDisconnect()"
               block
               color="error">
          Disconnect
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-menu>
</template>

<script>
  import { mapActions, mapGetters } from 'vuex'

  import InstanceDetailsToolbar from './instance-details-toolbar'

  export default {
    components: {
      InstanceDetailsToolbar
    },
    computed: {
      ...mapGetters('instances', [
        'isConnected',
        'connectedInstance',
      ]),
      color() {
        return this.isConnected
          ? 'success'
          : 'error'
      },
      icon() {
        return this.isConnected
          ? 'mdi-power-plug'
          : 'mdi-power-plug-off'
      }
    },
    methods: {
      ...mapActions('instances', [
        'disconnect'
      ]),
      doDisconnect() {
        this.disconnect()
        this.$router.push('/connect')
      }
    }
  }
</script>