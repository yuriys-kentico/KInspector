<template>
  <div>
    <action-list-item v-for="action in actions"
                      :key="action.codeName"
                      :action="action" />
    <div v-if="actions.length === 0">
      No actions
    </div>
  </div>
</template>

<script>
  import actionListItem from './action-list-item'

  export default {
    components: {
      actionListItem
    },
    props: {
      actions: {
        type: Array,
        default: () => ([])
      }
    }
  }
</script>