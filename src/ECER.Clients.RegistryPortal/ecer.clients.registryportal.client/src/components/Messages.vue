<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <v-breadcrumbs class="pa-0" :items="items" color="primary">
          <template #divider>/</template>
        </v-breadcrumbs>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Messages</h1>
      </v-col>
    </v-row>
    <MessageList class="message-list" />
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import MessageList from "@/components/MessageList.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useMessageStore } from "@/store/message";

export default defineComponent({
  name: "Messages",
  components: { MessageList, PageContainer },
  setup() {
    const messageStore = useMessageStore();

    return { messageStore };
  },
  data: () => ({
    items: [
      {
        title: "Dashboard",
        disabled: false,
        href: "/",
      },

      {
        title: "Messages",
        disabled: true,
        href: "messages",
      },
    ],
  }),
});
</script>
