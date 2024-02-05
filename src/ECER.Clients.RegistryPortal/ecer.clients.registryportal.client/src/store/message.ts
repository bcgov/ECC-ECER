import { defineStore } from "pinia";

// Replace with openapi schema when ECER-825/826 is complete
export interface Message {
  name: string;
  value: string;
}

export interface MessageState {
  messages: Message[];
}

export const useMessageStore = defineStore("message", {
  state: (): MessageState => ({
    messages: [
      { name: "Transcript", value: "Registry has received your transcript on 15 Jan 2023" },
      { name: "References", value: "Registry has received your references on 15 Jan 2023" },
    ],
  }),
  getters: {
    messageCount(state): number {
      return state.messages.length;
    },
  },
});
