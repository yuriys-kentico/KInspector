<template>
  <v-list dense
          two-line
          subheader>
    <v-subheader>Administration Configuration</v-subheader>
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title>
          <a :href="instance.url"
             target="_blank">
            {{displayName}}
          </a>
        </v-list-item-title>
        <v-list-item-subtitle>Instance</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title>{{instance.path}}</v-list-item-title>
        <v-list-item-subtitle>Path</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-subheader>Database Configuration</v-subheader>
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title>{{instance.databaseSettings.server}}</v-list-item-title>
        <v-list-item-subtitle>Server</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title>{{instance.databaseSettings.database}}</v-list-item-title>
        <v-list-item-subtitle>Database</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title>{{instance.databaseSettings.user}}</v-list-item-title>
        <v-list-item-subtitle>User</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title>{{instance.databaseSettings.password}}</v-list-item-title>
        <v-list-item-subtitle>Password</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title>{{instance.databaseSettings.integratedSecurity}}</v-list-item-title>
        <v-list-item-subtitle>Integrated Security</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
  </v-list>
</template>

<script>
  import { mapGetters } from 'vuex'
  export default {
    props: {
      instance: {
        type: Object,
        required: true
      }
    },
    computed: {
      ...mapGetters('instances', [
        'getInstanceDisplayName'
      ]),
      displayName: function () {
        const name = this.getInstanceDisplayName(this.instance.guid)
        return name
      }
    }
  }
</script>