<template>
  <PageContainer>
    <v-stepper min-height="100dvh" :alt-labels="true" bg-color="background" flat color="primary" :items="getStepTitles()">
      <template v-for="step in basic.steps" :key="step.id" #[step.key]>
        <v-container class="pa-4">
          <v-card class="rounded-lg" color="white" :title="step.title" flat>
            <v-form ref="form" validate-on="blur">
              <div class="d-flex flex-column ga-2">
                <template v-for="input in step.inputs" :key="input.id">
                  <Component :is="input.type" v-model="formData[input.id as keyof {}]" class="my-4" v-bind="{ input }" />
                </template>
              </div>
            </v-form>
          </v-card>
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
  data: () => ({
    basic,
    formData: {},
  }),
  methods: {
    getStepTitles() {
      return this.basic.steps.map((step) => step.title);
    },
  },
});
</script>
