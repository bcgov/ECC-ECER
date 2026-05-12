<template>
  <Loading v-if="loading" />
  <template v-else>
    <v-row v-if="componentGroup?.name || componentGroup?.instruction">
      <v-col cols="12" class="mb-4">
        <h2 v-if="componentGroup?.name" class="text-h6">
          {{ componentGroup.name }}
        </h2>
        <p v-if="componentGroup?.instruction" class="multiline mt-2 mb-2">
          {{ componentGroup.instruction }}
        </p>
      </v-col>
    </v-row>
    <v-row v-if="isRFAI && hasRfaiComponents" class="mb-4">
      <v-col cols="12">
        <Callout title="Additional information requested" type="warning">
          <p>
            Refer to your messages for details on resolving this request. You
            may edit the questions in your application where additional
            information is requested.
          </p>
        </Callout>
      </v-col>
    </v-row>
    <v-row v-if="components.length">
      <v-col
        v-for="(comp, index) in components"
        :key="comp.id ?? index"
        cols="12"
        class="mb-4"
      >
        <Question
          v-model="formByComponentId[comp.id ?? '']"
          :name="comp.name ?? ''"
          :question="comp.question ?? ''"
          :rfai-required="comp.rfaiRequired ?? false"
          :read-only="isRFAI"
          :program-application-id="programApplicationId"
          :component-group-id="componentGroupId"
          :component-id="comp.id ?? ''"
        />
      </v-col>
    </v-row>
    <p v-else class="ma-0">No components for this group.</p>
    <v-row class="mt-4">
      <v-col>
        <v-btn
          color="primary"
          :loading="saving"
          :disabled="saving"
          @click="saveAndContinue"
        >
          Save and continue
        </v-btn>
      </v-col>
    </v-row>
  </template>
  <ConfirmationDialog
    ref="confirmationDialogRef"
    title="Unsaved changes"
    accept-button-text="Save changes"
    cancel-button-text="Continue without saving"
    :loading="saving"
    persistent
  >
    <template #confirmation-text>
      <p>You have made changes to this page that have not been saved.</p>
      <br />
      <p>Are you sure you want to leave this page without saving changes?</p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Loading from "@/components/Loading.vue";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import Callout from "@/components/common/Callout.vue";
import {
  getComponentGroupComponents,
  updateComponentGroup,
  getProgramApplicationById,
} from "@/api/program-application";
import type { Components, ComponentGroupWithComponents } from "@/types/openapi";
import Question from "@/components/program-application/Question.vue";
import type { QuestionModelValue } from "@/components/program-application/Question.vue";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import { cloneDeep, isEqual, mapValues, omit } from "lodash";

interface ComponentGroupWithComponentsFlat {
  id?: string | null;
  name?: string | null;
  instruction?: string | null;
  status?: string | null;
  categoryName?: string | null;
  displayOrder?: number;
  components?: Components.Schemas.ProgramApplicationComponent[] | null;
}

export default defineComponent({
  name: "ProgramApplicationComponent",
  components: { Question, Loading, ConfirmationDialog, Callout },
  props: {
    applicationType: { type: String, required: false },
    programApplicationId: {
      type: String,
      required: true,
    },
    componentGroupId: {
      type: String,
      required: true,
    },
  },
  emits: { next: (_payload: NextStepPayload) => true, refreshNav: () => true },
  async beforeRouteLeave(_to, _from, next) {
    const leave = await this.waitForConfirmation();
    next(leave);
  },
  async beforeRouteUpdate(_to, _from, next) {
    const leave = await this.waitForConfirmation();
    next(leave);
  },
  computed: {
    isRFAI(): boolean {
      return (
        this.programApplicationObject?.status !== undefined &&
        (this.programApplicationObject?.status === "InterimRecognition" ||
          this.programApplicationObject?.status === "ReviewAnalysis") &&
        this.programApplicationObject?.statusReasonDetail === "RFAIrequested"
      );
    },
    hasRfaiComponents(): boolean {
      return this.components.some((c) => c.rfaiRequired === true);
    },
    hasChanges(): boolean {
      //we do not need to compare files for changes since those are automatically saved when uploaded
      const componentsWithFormWithoutFiles = mapValues(
        this.formByComponentId,
        (value) => omit(value, ["files"]),
      );
      const originalComponentsWithFormWithoutFiles = mapValues(
        this.originalFormByComponentId,
        (value) => omit(value, ["files"]),
      );
      return !isEqual(
        componentsWithFormWithoutFiles,
        originalComponentsWithFormWithoutFiles,
      );
    },
  },
  data() {
    return {
      componentGroup: null as ComponentGroupWithComponents | null,
      components: [] as Components.Schemas.ProgramApplicationComponent[],
      loading: true,
      saving: false,
      formByComponentId: {} as Record<string, QuestionModelValue>,
      originalFormByComponentId: {} as Record<string, QuestionModelValue>,
      programApplicationObject:
        null as Components.Schemas.ProgramApplication | null,
    };
  },
  watch: {
    programApplicationId: "loadComponents",
    componentGroupId: "loadComponents",
  },
  async mounted() {
    await this.fetchApplication();
    await this.loadComponents();
  },
  methods: {
    async fetchApplication() {
      const result = await getProgramApplicationById(this.programApplicationId);
      if (result.error || result.data == null) {
        console.error("Failed to retrieve program application:", result.error);
      } else {
        this.programApplicationObject = result.data;
      }
    },
    async loadComponents() {
      this.loading = true;
      const result = await getComponentGroupComponents(
        this.programApplicationId,
        this.componentGroupId,
      );
      this.loading = false;
      if (result.error) return;
      const payload = result.data;
      this.componentGroup =
        payload !== null && payload !== undefined ? (payload[0] ?? null) : null;
      const list =
        payload !== null && payload !== undefined
          ? (payload[0]?.components ?? [])
          : [];
      this.components = list.sort(
        (a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0),
      );
      this.formByComponentId = Object.fromEntries(
        list.map((c) => [
          c.id ?? "",
          {
            answer: c.answer ?? "",
            files: c.files ?? [],
          },
        ]),
      );
      this.originalFormByComponentId = cloneDeep(this.formByComponentId);
    },
    async handleSave() {
      if (!this.componentGroup) return;
      this.saving = true;
      const updatedComponents = this.components.map((c) => ({
        ...c,
        answer: this.formByComponentId[c.id ?? ""]?.answer ?? c.answer,
        files: this.formByComponentId[c.id ?? ""]?.files ?? c.files,
      }));
      const payload: Components.Schemas.ComponentGroupWithComponents = {
        ...this.componentGroup,
        components: updatedComponents,
      };
      const result = await updateComponentGroup(
        this.programApplicationId,
        payload,
      );
      this.saving = false;
      if (result.error) return;
    },
    async saveAndContinue() {
      await this.handleSave();
      this.originalFormByComponentId = cloneDeep(this.formByComponentId);
      this.$emit("next", { currentComponentGroupId: this.componentGroupId });
    },
    async waitForConfirmation(): Promise<boolean> {
      if (this.hasChanges) {
        let dialogRef = this.$refs.confirmationDialogRef as InstanceType<
          typeof ConfirmationDialog
        >;
        const saveBeforeLeaving = await dialogRef.open();
        if (saveBeforeLeaving) {
          await this.handleSave();
          dialogRef.close();
          return true;
        } else if (!saveBeforeLeaving) {
          dialogRef.close();
          return true;
        }
      }
      return true;
    },
  },
});
</script>
