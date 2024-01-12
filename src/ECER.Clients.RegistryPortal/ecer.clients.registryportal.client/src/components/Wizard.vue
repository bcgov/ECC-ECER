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
      <template v-for="step in basic.steps" :key="step.id" #[step.key]>
        <v-card class="rounded-lg" color="white" :title="step.title" flat>
          <v-container class="">
            <v-row>
              <v-col cols="12" md="8" lg="6" xl="4">
                <v-form ref="form" validate-on="blur">
                  <template v-for="input in step.inputs" :key="input.id">
                    <Component :is="input.component" v-bind="{ props: input.props }" v-model="formData[input.id as keyof {}]" class="my-8" />
                  </template>
                </v-form>
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
import { defineComponent } from "vue";

import FormContainer from "@/components/FormContainer.vue";
import PageContainer from "@/components/PageContainer.vue";
import basic from "@/config/basic";

export default defineComponent({
  name: "Wizard",
  components: { FormContainer, PageContainer },
  setup: () => {
    return { basic };
  },
  data: () => ({
    currentStep: 1,
    formData: {},
  }),
  methods: {
    getStepTitles() {
      return this.basic.steps.map((step) => step.title);
    },
    async handleSaveAndContinue() {
      const valid = await (this.$refs.form as any).validate();
      console.log(this.$refs);
      console.log(this.$refs.form);
      if (valid.valid && this.currentStep < this.basic.steps.length) {
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
