<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12">
        <p>
          This certification allows you to work alone and/or as the primary educator in licensed child care programs for children 3 to 5 years of age and to
          work alongside an Infant Toddler Educator in licensed programs for children under 36-months.
        </p>
        <br />
        <p>This certificate needs to be renewed every 5 years.</p>
      </v-col>
      <v-col cols="12">
        <v-checkbox
          v-model="certificationTypeStore.subSelection"
          color="primary"
          label="Infant and Toddler Educator (ITE)"
          value="Ite"
          hide-details="auto"
          aria-describedby="ITEDescription"
        ></v-checkbox>
        <div id="ITEDescription" class="ml-10 text-caption">
          This certification allows you to work alone and/or as the primary educator in licensed child care programs for children birth to 5 years of age.
        </div>
        <v-checkbox
          v-model="certificationTypeStore.subSelection"
          color="primary"
          label="Special Needs Educator (SNE)"
          value="Sne"
          hide-details="auto"
          aria-describedby="SNEDescription"
        />
        <div id="SNEDescription" class="ml-10 text-caption">
          This certification allows you to work alone and/or as the primary educator in inclusive, licensed child care programs for children 3 to 5 years of
          age.
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useCertificationTypeStore } from "@/store/certificationType";

export default defineComponent({
  name: "ECEFiveYearContent",
  setup: () => {
    const certificationTypeStore = useCertificationTypeStore();

    return { certificationTypeStore };
  },
});
</script>
