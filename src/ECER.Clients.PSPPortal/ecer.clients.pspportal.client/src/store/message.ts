import { defineStore } from "pinia";

import { getMessagesStatus } from "@/api/message";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";

export interface MessageState {
  messages: Components.Schemas.Communication[] | null | undefined;
  currentMessage: Components.Schemas.Communication | null;
  currentThread: Components.Schemas.Communication[] | null;
}

export const useMessageStore = defineStore("message", {
  state: (): MessageState => ({
    messages: [],
    currentMessage: null,
    currentThread: null,
  }),
  getters: {
    unreadMessageCount(): number {
      const userStore = useUserStore();
      const unreadMessagesCount =
        userStore.pspUserProfile?.unreadMessagesCount ?? 0;
      return unreadMessagesCount;
    },
  },
  actions: {
    messageById(id: string): Components.Schemas.Communication | undefined {
      return this.messages?.find((message) => message.id === id);
    },
    async refreshUnreadCount(): Promise<void> {
      try {
        const response = await getMessagesStatus();
        const count = response.data?.status?.count ?? 0;
        const userStore = useUserStore();
        userStore.$patch((state) => {
          if (state.pspUserProfile) {
            state.pspUserProfile.unreadMessagesCount = count;
          }
        });
      } catch {
        // Swallow — keep last known count.
      }
    },
  },
});
