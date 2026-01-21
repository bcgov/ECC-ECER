import { defineStore } from "pinia";

import { createOrUpdateDraftApplication, getPrograms } from "@/api/program.ts";

import programWizard from "@/config/program-wizard/program-wizard";
import type { Components } from "@/types/openapi";
import type { ProgramStage, Wizard } from "@/types/wizard";

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
      name: null,
      programName: null,
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
    },
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
      this.draftProgram.portalStage =
        wizardStore.currentStepStage as ProgramStage;
      const programNameId =
        wizardStore.wizardConfig.steps?.programOverview?.form.components
          .programName?.id;

      this.draftProgram.programName = programNameId
        ? wizardStore.wizardData[programNameId]
        : "";
      //If programType is offered add it to the array of program types ex. earlyChildhood === true => programTypes = ["Basic"]
      const basicStep =
        wizardStore?.wizardConfig?.steps?.earlyChildhood?.form.components
          .earlyChildhood?.id;
      const specialNeedsStep =
        wizardStore?.wizardConfig?.steps?.specialNeeds?.form.components
          .specialNeeds?.id;
      const infantAndToddlerStep =
        wizardStore?.wizardConfig?.steps?.infantAndToddler?.form.components
          .infantAndToddler?.id;
      this.draftProgram.programTypes = [];
      basicStep &&
        wizardStore.wizardData[basicStep] === true &&
        this.draftProgram.programTypes?.push("Basic");
      specialNeedsStep &&
        wizardStore.wizardData[specialNeedsStep] === true &&
        this.draftProgram.programTypes?.push("SNE");
      infantAndToddlerStep &&
        wizardStore.wizardData[infantAndToddlerStep] === true &&
        this.draftProgram.programTypes?.push("ITE");
    },
    async upsertDraftApplication(): Promise<
      Components.Schemas.DraftProgramResponse | null | undefined
    > {
      const { data: draftApplicationResponse } =
        await createOrUpdateDraftApplication(this.draftProgram);
      if (
        draftApplicationResponse !== null &&
        draftApplicationResponse !== undefined
      ) {
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
        name: null,
        programName: null,
      };
    },
    async saveDraft(): Promise<
      Components.Schemas.DraftProgramResponse | null | undefined
    > {
      this.prepareDraftProgramFromWizard();
      return await this.upsertDraftApplication();
    },
  },
});
