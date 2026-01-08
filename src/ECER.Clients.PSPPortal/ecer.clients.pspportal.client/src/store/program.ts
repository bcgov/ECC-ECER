import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getPrograms } from "@/api/program.ts";

import programWizard from "@/config/program-wizard/program-wizard";
import type { Components } from "@/types/openapi";
import type {ProgramStage, Wizard} from "@/types/wizard";

import { useWizardStore } from "./wizard";

export interface ProgramState {
  draftProgram: Components.Schemas.Program;
}

export const useProgramStore = defineStore("program", {
  state: (): ProgramState => ({
    draftProgram: {
      id: undefined,
      portalStage: "ProgramOverview",
      createdOn: null,
      status: "Draft",
      name: null
    },
  }),
  persist: {
    pick: ["draftProgram"],
  },
  getters: {
    hasDraftProgram(state): boolean {
      return state.draftProgram.id !== undefined;
    },
    
    applicationConfiguration(): Wizard {
        return programWizard;
    }
  },
  actions: {
    setDraftProgramFromProfile(program: Components.Schemas.Program): void {
      if (!program.portalStage) {
        program.portalStage = "ProgramOverview";
      }
      this.draftProgram = program;
    },
    prepareDraftProgramFromWizard() {
      const wizardStore = useWizardStore();
      this.draftProgram.portalStage = wizardStore.currentStepStage as ProgramStage;
      
    },
    async upsertDraftApplication(): Promise<Components.Schemas.DraftProgramResponse | null | undefined> {
      const { data: draftApplicationResponse } = await createOrUpdateDraftApplication(this.draftProgram);
      if (draftApplicationResponse !== null && draftApplicationResponse !== undefined) {
        this.draftProgram = draftApplicationResponse.program!;
      }
      return draftApplicationResponse;
    },
    resetDraftProgram(): void {
      this.draftProgram = {
        id: undefined,
        portalStage: "ProgramOverview",
        createdOn: null,
        status: "Draft",
        name: null
      };
    },
    async saveDraft(): Promise<Components.Schemas.DraftProgramResponse | null | undefined> {
      this.prepareDraftProgramFromWizard();
      return await this.upsertDraftApplication();
    }
  },
});
