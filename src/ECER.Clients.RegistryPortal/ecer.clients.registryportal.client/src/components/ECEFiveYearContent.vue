<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12">
        <p class="small">
          Five Year Certification requires completion of an early childhood education program from a recognized educational institution or an equivalent
          program, or equivalent certification/registration in another province or territory. You must have completed 500 hours of supervised work experience.
          <br />
          <br />
          This certification allows you to work alone and/or as the primary educator in licensed child care programs for children 3 to 5 years of age and may
          work alongside an Infant Toddler Educator in licensed programs for children under 36-months.
        </p>
      </v-col>
      <v-col cols="11" offset="1">
        <v-checkbox
          v-model="certificationTypeStore.subSelection"
          color="primary"
          label="Infant and Toddler Educator (ITE)"
          value="Ite"
          hide-details="auto"
          aria-describedby="ITEDescription"
        ></v-checkbox>
        <div id="ITEDescription" class="ml-10 text-caption">
          An Infant and Toddler specialization requires successful completion of an infant and toddler educator training program recognized by the ECE Registry.
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
          A Special Needs specialization requires successful completion of a special needs early childhood educator training program recognized by the ECE
          Registry.
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
