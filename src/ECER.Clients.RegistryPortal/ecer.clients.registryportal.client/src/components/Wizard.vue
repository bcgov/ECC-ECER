<template>
  <PageContainer>
    <v-stepper
      v-model="currentStep"
      min-height="100dvh"
      :alt-labels="true"
      bg-color="background"
      flat
      color="primary"
      :items="getStepTitles()"
      :mobile="$vuetify.display.mobile"
    >
      <template v-for="step in wizard.steps" :key="step.id" #[step.key]>
        <v-card class="rounded-lg" color="white" :title="step.title" flat>
          <v-container>
            <v-row>
              <v-col cols="12" md="8" lg="6" xl="4">
                <EceForm :form="step.form" />
              </v-col>
            </v-row>
          </v-container>
        </v-card>
      </template>
      <template #actions>
        <v-container>
          <v-row class="justify-space-between ga-4" no-gutters>
            <v-col cols="auto" class="mr-auto">
              <v-btn variant="outlined" color="primary" aut @click="handleBack()">Back</v-btn>
            </v-col>
            <v-col cols="auto">
              <v-btn variant="outlined" color="primary" class="mr-4" primary @click="handleSaveAsDraft()">Save as Draft</v-btn>
              <v-btn color="primary" @click="handleSaveAndContinue()">Save and Continue</v-btn>
            </v-col>
          </v-row>
        </v-container>
      </template>
    </v-stepper>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import EceForm from "@/components/Form.vue";
import PageContainer from "@/components/PageContainer.vue";
import applicationWizard from "@/config/application-wizard";
import type { Wizard } from "@/types/wizard";

export default defineComponent({
  name: "Wizard",
  components: { PageContainer, EceForm },
  props: {
    wizard: {
      type: Object as PropType<Wizard>,
      default: () => applicationWizard,
    },
  },

  data: () => ({
    currentStep: 1,
    formData: {},
  }),
  computed: {
    formRef(): string {
      return `form-${this.currentStep}`;
    },
  },
  methods: {
    getStepTitles() {
      return this.wizard.steps.map((step) => step.title);
    },
    async handleSaveAndContinue() {
      const valid = await (this.$refs[this.formRef] as any).validate();
      if (valid.valid && this.currentStep < this.wizard.steps.length) {
        this.currentStep++;
      }
    },
    handleSaveAsDraft() {
      console.log("Save as Draft");
    },
    handleBack() {
      if (this.currentStep > 1) this.currentStep--;
    },
  },
});
</script>
