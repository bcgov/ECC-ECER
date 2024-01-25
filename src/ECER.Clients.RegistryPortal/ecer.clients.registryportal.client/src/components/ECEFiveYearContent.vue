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
      <v-col cols="12">
        <div class="d-flex flex-row align-center">
          <v-icon class="mr-2" size="large" icon="mdi-alert-circle-outline" />
          <p class="small">
            NOTE: If you are eligible for an ECE Five Year Certificate, the Registry will assess you for one or both specialized certificates below as part of
            your application for the ECE Five Year Certificate. You only need to submit one application.
          </p>
        </div>
      </v-col>
      <v-col cols="11" offset="1">
        <v-checkbox v-model="selection" color="primary" label="Infant and Toddler Educator (ITE)" value="ITE">
          <template #details>
            <div class="ml-10">
              An Infant and Toddler specialization requires successful completion of an infant and toddler educator training program recognized by the ECE
              Registry.
            </div>
          </template>
        </v-checkbox>

        <v-checkbox v-model="selection" color="primary" label="Special Needs Educator (SNE)" value="SNE">
          <template #details>
            <div class="ml-10">
              A Special Needs specialization requires successful completion of a special needs early childhood educator training program recognized by the ECE
              Registry.
            </div>
          </template>
        </v-checkbox>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ECEFiveYearContent",
  emits: {
    selection: (_selected: Array<string>) => true,
  },
  data() {
    return {
      selection: [] as Array<string>,
    };
  },
  watch: {
    selection(selected: Array<string>) {
      this.$emit("selection", selected);
    },
  },
});
</script>
