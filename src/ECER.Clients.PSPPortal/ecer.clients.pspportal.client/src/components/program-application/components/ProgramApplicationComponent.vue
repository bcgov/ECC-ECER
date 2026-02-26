<template>
  <v-container>
    <Loading v-if="loading" />
    <template v-else>
      <v-row v-if="componentGroup?.name || componentGroup?.instruction">
        <v-col cols="12" class="mb-4">
          <h2 v-if="componentGroup?.name" class="text-h6">
            {{ componentGroup.name }}
          </h2>
          <p v-if="componentGroup?.instruction" class="ma-0 mt-2">
            {{ componentGroup.instruction }}
          </p>
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
          />
        </v-col>
      </v-row>
      <p v-else class="ma-0">No components for this group.</p>
    </template>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Loading from "@/components/Loading.vue";
import { getComponentGroupComponents } from "@/api/program-application";
import type { Components } from "@/types/openapi";
import Question from "@/components/program-application/Question.vue";
import type { QuestionModelValue } from "@/components/program-application/Question.vue";

/** Component group with components (flat shape from API). */
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
  components: { Question, Loading },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
    componentGroupId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      componentGroup: null as ComponentGroupWithComponentsFlat | null,
      components: [] as Components.Schemas.ProgramApplicationComponent[],
      loading: true,
      /** Form state per component id; used for v-model and save. */
      formByComponentId: {} as Record<string, QuestionModelValue>,
    };
  },
  watch: {
    programApplicationId: "loadComponents",
    componentGroupId: "loadComponents",
  },
  async mounted() {
    await this.loadComponents();
  },
  methods: {
    async loadComponents() {
      this.loading = true;
      const result = await getComponentGroupComponents(
        this.programApplicationId,
        this.componentGroupId,
      );
      this.loading = false;
      if (result.error) return;
      const payload = result.data as
        | ComponentGroupWithComponentsFlat
        | null
        | undefined;
      this.componentGroup = payload ?? null;
      const list = payload?.components ?? [];
      this.components = list;
      this.formByComponentId = Object.fromEntries(
        list.map((c) => [
          c.id ?? "",
          {
            answer: c.answer ?? "",
            fileIds: c.fileIds ?? [],
          },
        ]),
      );
    },
  },
});
</script>
