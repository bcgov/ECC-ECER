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
    <v-row class="ga-10">
      <v-col cols="12" md="6" lg="4">
        <MessageList />
      </v-col>
      <v-col>
        <Message />
        <!-- fake change -->
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Message from "@/components/Message.vue";
import MessageList from "@/components/MessageList.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useMessageStore } from "@/store/message";

export default defineComponent({
  name: "Messages",
  components: { MessageList, Message, PageContainer },
  setup() {
    const messageStore = useMessageStore();

    return { messageStore };
  },
  data: () => ({
    items: [
      {
        title: "Home",
        disabled: false,
        to: "/",
      },

      {
        title: "Messages",
      },
    ],
  }),
});
</script>
