<template>
  <v-container fluid>
    <v-data-iterator :items="items"
                     :footer-props="{'items-per-page-options':[10,20,{ text: 'All', value:-1}]}"
                     content-tag="v-layout">

      <template v-slot:default="props">
        <v-row>
          <v-col v-for="item in props.items"
                 :key="item.name"
                 sm="6"
                 md="4"
                 lg="3">
            <instance-connection-list-item :item="item"></instance-connection-list-item>
          </v-col>
        </v-row>
      </template>
    </v-data-iterator>
  </v-container>
</template>

<script>
  import { mapActions, mapState } from 'vuex'
  import InstanceConnectionListItem from './instance-connection-list-item'

  export default {
    components: {
      InstanceConnectionListItem
    },
    mounted() {
      this.getAll()
    },
    computed: {
      ...mapState({
        items: state => Object.values(state.instances.items)
      }),
    },
    methods: {
      ...mapActions('instances', ['getAll']),
    }
  }
</script>