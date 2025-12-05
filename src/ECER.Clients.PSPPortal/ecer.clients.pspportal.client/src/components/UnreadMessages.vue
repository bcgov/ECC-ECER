<template>
  <template v-if="messageStore.unreadMessageCount === 0">You have no new messages.</template>
  <template v-if="messageStore.unreadMessageCount === 1">
    You have
    <template v-if="linkable">
      <!-- prettier-ignore -->
      <router-link to="/messages"><b>1 new message</b></router-link>
      <span>.</span>
    </template>
    <template v-else>1 new message.</template>
  </template>
  <template v-if="messageStore.unreadMessageCount > 1">
    You have
    <template v-if="linkable">
      <router-link to="/messages">
        <b>{{ messageStore.unreadMessageCount }} new messages</b>
      </router-link>
      <span>.</span>
    </template>
    <template v-else>{{ messageStore.unreadMessageCount }} new messages.</template>
  </template>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useMessageStore } from "@/store/message";

export default defineComponent({
  name: "UnreadMessages",
  props: {
    linkable: {
      type: Boolean,
      default: true,
    },
  },
  setup: () => {
    const messageStore = useMessageStore();
    return { messageStore };
  },
});
</script>
