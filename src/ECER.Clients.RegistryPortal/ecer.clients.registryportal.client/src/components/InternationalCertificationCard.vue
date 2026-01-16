<template>
  <v-card
    min-width="100%"
    variant="elevated"
    rounded="lg"
    class="custom-card-styling"
    elevation="4"
  >
    <template #title>
      <v-card-title>
        <v-row>
          <v-col>
            <p class="font-weight-bold">
              {{ internationalCertification.nameOfRegulatoryAuthority }}
            </p>
          </v-col>
          <v-spacer></v-spacer>
          <v-col>
            <p>{{ `${internationalCertification.certificateStatus}` }}</p>
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
            @click="editInternationalCertification"
            :loading="loadingStore.isLoading('icra_put')"
          />
        </template>
      </v-tooltip>
      <v-tooltip text="Delete professional development" location="top">
        <template #activator="{ props }">
          <v-btn
            v-bind="props"
            icon="mdi-trash-can-outline"
            variant="plain"
            @click="deleteInternationalCertification"
            :loading="loadingStore.isLoading('icra_put')"
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

import type { InternationalCertificationExtended } from "./inputs/EceInternationalCertification.vue";

export default defineComponent({
  name: "InternationalCertificationCard",
  props: {
    internationalCertification: {
      type: Object as () => InternationalCertificationExtended,
      required: true,
    },
  },
  setup() {
    const loadingStore = useLoadingStore();
    return { loadingStore, formatDate };
  },
  emits: {
    edit: (_certification: InternationalCertificationExtended) => true,
    delete: (_certification: InternationalCertificationExtended) => true,
  },
  methods: {
    editInternationalCertification() {
      // Emit an event or implement edit logic here
      this.$emit("edit", this.internationalCertification);
    },
    deleteInternationalCertification() {
      // Emit an event or implement delete logic here
      this.$emit("delete", this.internationalCertification);
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
