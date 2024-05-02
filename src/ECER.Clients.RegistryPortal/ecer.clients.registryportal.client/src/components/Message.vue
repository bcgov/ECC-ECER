<template>
  <div v-if="smAndDown">
    <v-dialog
      :model-value="smAndDown && messageStore.currentMessage != null"
      :transition="false"
      fullscreen
      @update:model-value="messageStore.currentMessage = null"
    >
      <v-card>
        <v-toolbar color="white">
          <v-btn prepend-icon="mdi-close" text="Close" @click="messageStore.currentMessage = null"></v-btn>
        </v-toolbar>
        <v-card-text>
          <h3>{{ messageStore.currentMessage?.subject }}</h3>
          <p class="small mt-2">{{ messageDate }}</p>
          <p class="small mt-6">{{ messageStore.currentMessage?.text }}</p>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
  <div v-if="mdAndUp">
    <h3>{{ messageStore.currentMessage?.subject }}</h3>
    <p class="small mt-2">{{ messageDate }}</p>
    <p class="small mt-6">{{ messageStore.currentMessage?.text }}</p>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import { useMessageStore } from "@/store/message";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "Message",
  setup() {
    const messageStore = useMessageStore();
    const { smAndDown, mdAndUp } = useDisplay();

    return { messageStore, smAndDown, mdAndUp };
  },
  computed: {
    messageDate(): string {
      let message = this.messageStore.currentMessage;
      return message ? formatDate(String(message.notifiedOn), "LLL dd, yyyy t") : "";
    },
  },
  methods: {
    formatDate,
  },
});
</script>