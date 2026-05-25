<template>
  <v-row>
    <v-col
      v-for="(application, index) in applications"
      :key="application.id ?? `application-${index}`"
      cols="12"
      md="6"
      lg="4"
    >
      <ProgramApplicationCard
        :program-application="application"
        @refresh-application-list="$emit('refresh-application-list')"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import ProgramApplicationCard from "@/components/program-application/ProgramApplicationCard.vue";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "ProgramApplicationsList",
  components: { ProgramApplicationCard },
  props: {
    applications: {
      type: Array as PropType<Components.Schemas.ProgramApplication[]>,
      required: true,
    },
  },
  emits: ["refresh-application-list"],
});
</script>
