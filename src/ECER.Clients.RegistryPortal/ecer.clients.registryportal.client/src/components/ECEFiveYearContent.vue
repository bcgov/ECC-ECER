<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12">
        <p>
          This certification allows you to work alone. You can be the primary educator in a licensed child care program for children 3 to 5 years old. You can
          also work alongside an Infant Toddler Educator in a licensed program for children under 36 months old.
        </p>
        <br />
        <p>You must have at least one of the following:</p>
        <br />
        <ul class="ml-10">
          <li>An early childhood education program from a recognized educational institution</li>
          <li>A training program deemed equivalent by the ECE registry</li>
          <li>Equivalent certification or registration in another Canadian province or territory</li>
        </ul>
        <br />
        <p>You must also have completed 500 hours of supervised work experience.</p>
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
