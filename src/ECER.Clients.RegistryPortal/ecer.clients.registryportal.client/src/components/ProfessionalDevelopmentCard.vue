<template>
  <v-card min-width="100%" variant="elevated" rounded="lg" class="custom-card-styling" elevation="4">
    <template #title>
      <v-card-title>
        <v-row>
          <v-col>
            <p class="font-weight-bold">{{ professionalDevelopment.courseName }}</p>
          </v-col>
          <v-spacer></v-spacer>
          <v-col class="text-wrap" cols="auto">
            <p>
              {{
                `${formatDate(professionalDevelopment.startDate || "", "LLLL d, yyyy")} - ${formatDate(professionalDevelopment.endDate || "", "LLLL d, yyyy")}`
              }}
            </p>
          </v-col>
          <v-spacer></v-spacer>
          <v-col>
            <p>{{ `${professionalDevelopment.numberOfHours} hours` }}</p>
          </v-col>
        </v-row>
      </v-card-title>
    </template>
    <template #append>
      <v-tooltip text="Edit professional development" location="top">
        <template #activator="{ props }">
          <v-btn
            v-bind="props"
            icon="mdi-pencil"
            variant="plain"
            @click="editProfessionalDevelopment"
            :loading="loadingStore.isLoading('draftapplication_put')"
          />
        </template>
      </v-tooltip>
      <v-tooltip text="Delete professional development" location="top">
        <template #activator="{ props }">
          <v-btn
            v-bind="props"
            icon="mdi-trash-can-outline"
            variant="plain"
            @click="deleteProfessionalDevelopment"
            :loading="loadingStore.isLoading('draftapplication_put')"
          />
        </template>
      </v-tooltip>
    </template>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useLoadingStore } from "@/store/loading";
import { formatDate } from "@/utils/format";

import type { ProfessionalDevelopmentExtended } from "./inputs/EceProfessionalDevelopment.vue";

export default defineComponent({
  name: "ProfessionalDevelopmentCard",
  props: {
    professionalDevelopment: {
      type: Object as () => ProfessionalDevelopmentExtended,
      required: true,
    },
  },
  setup() {
    const loadingStore = useLoadingStore();
    return { loadingStore, formatDate };
  },
  emits: {
    edit: (_reference: ProfessionalDevelopmentExtended) => true,
    delete: (_reference: ProfessionalDevelopmentExtended) => true,
  },
  methods: {
    editProfessionalDevelopment() {
      // Emit an event or implement edit logic here
      this.$emit("edit", this.professionalDevelopment);
    },
    deleteProfessionalDevelopment() {
      // Emit an event or implement delete logic here
      this.$emit("delete", this.professionalDevelopment);
    },
  },
});
</script>

<style scoped lang="scss">
.custom-card-styling {
  border: 1px solid #adb5bd;
  padding: 0.5rem;
}
</style>
