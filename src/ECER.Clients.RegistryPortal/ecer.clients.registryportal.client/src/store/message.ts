import { defineStore } from "pinia";

import { getMessages } from "@/api/message";
import { useUserStore } from "@/store/user"; // Adjust the path to your useUserStore file
import type { Components } from "@/types/openapi";

export interface MessageState {
  messages: Components.Schemas.Communication[] | null | undefined;
  currentPage: number;
  messagesPerPage: number;
}

export const useMessageStore = defineStore("message", {
  state: (): MessageState => ({
    messages: [],
    currentPage: 1,
    messagesPerPage: 10,
  }),
  getters: {
    paginatedMessages(): Components.Schemas.Communication[] {
      const start = (this.currentPage - 1) * this.messagesPerPage;
      const end = start + this.messagesPerPage;
      return this.messages ? this.messages.slice(start, end) : [];
    },
    totalPages(): number {
      return Math.ceil((this.messages?.length ?? 0) / this.messagesPerPage);
    },
    unreadMessageCount(): number {
      const userStore = useUserStore();
      const unreadMessagesCount = userStore.userInfo?.unreadMessagesCount ?? 0;
      return unreadMessagesCount;
    },
  },
  actions: {
    async fetchMessages() {
      this.messages = await getMessages();
    },
    setPage(pageNumber: number) {
      if (pageNumber < 1 || pageNumber > this.totalPages) {
        console.error("Page number out of range");
        this.currentPage = 1;
      } else {
        this.currentPage = pageNumber;
      }
    },
  },
});
