<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Messages</h1>
      </v-col>
    </v-row>

    <Loading v-if="loading"></Loading>

    <v-row v-show="!loading" class="ga-10">
      <v-col cols="12" md="6" lg="4">
        <MessageList />
      </v-col>
      <v-col>
        <Message />
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Message from "./Message.vue";
import MessageList from "./MessageList.vue";
import PageContainer from "@/components/PageContainer.vue";
import Loading from "@/components/Loading.vue";
import { useLoadingStore } from "@/store/loading";
import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
  name: "Messages",
  components: { MessageList, Message, PageContainer, Loading, Breadcrumb },
  setup() {
    const loadingStore = useLoadingStore();

    return { loadingStore };
  },
  computed: {
    loading() {
      return (
        this.loadingStore.isLoading("message_get") ||
        this.loadingStore.isLoading("psp_user_manage_get")
      );
    },
  },
});
</script>
