import { defineStore } from "pinia";
import {
  getApplicationDocumentUrls,
  type ApplicationFileInfo,
} from "@/api/programApplicationFiles";

export const useApplicationFilesStore = defineStore("applicationFiles", {
  state: () => ({
    filesByApplicationId: {} as Record<string, ApplicationFileInfo[]>,
  }),
  getters: {
    getFiles:
      (state) =>
      (programApplicationId: string): ApplicationFileInfo[] =>
        state.filesByApplicationId[programApplicationId] ?? [],
  },
  actions: {
    async refreshFiles(programApplicationId: string) {
      const result = await getApplicationDocumentUrls(programApplicationId);
      if (result.data) {
        this.filesByApplicationId[programApplicationId] = result.data;
      }
    },
  },
});
