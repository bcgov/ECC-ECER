import { defineStore } from "pinia";

import { getMessages } from "@/api/message";
import { useUserStore } from "@/store/user"; // Adjust the path to your useUserStore file
import type { Components } from "@/types/openapi";

export interface MessageState {
  messages: Components.Schemas.Communication[] | null | undefined;
}

export const useMessageStore = defineStore("message", {
  state: (): MessageState => ({
    messages: [],
  }),
  getters: {
    messageCount(): number {
      const userStore = useUserStore();
      const unreadMessagesCount = userStore.userInfo?.unreadMessagesCount ?? 0;
      return unreadMessagesCount;
    },
  },
  actions: {
    async fetchMessages() {
      this.messages = await getMessages();
    },
  },
});
