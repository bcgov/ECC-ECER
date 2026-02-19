<template>
  <v-row>
    <v-col
      v-for="(program, index) in filteredPrograms"
      :key="getProgramKey(program, index)"
      cols="12"
      md="6"
      lg="4"
    >
      <ProgramProfileCard :program="program" @withdrawn="$emit('withdrawn')" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import ProgramProfileCard from "@/components/program-profile/ProgramProfileCard.vue";
import type { Components } from "@/types/openapi";
import { shouldDisplayProfile } from "@/utils/functions";

export default defineComponent({
  name: "ProgramProfilesList",
  components: {
    ProgramProfileCard,
  },
  props: {
    programs: {
      type: Array as PropType<Components.Schemas.Program[]>,
      required: true,
    },
  },
  computed: {
    filteredPrograms(): Components.Schemas.Program[] {
      return this.programs.filter(shouldDisplayProfile);
    },
  },
  methods: {
    getProgramKey(program: Components.Schemas.Program, index: number): string {
      return program.id ?? `program-${index}`;
    },
  },
});
</script>
