<template>
  <v-form :id="form.id" :ref="form.id" validate-on="input" @update:model-value="onFormValidationChanged" @submit.prevent>
    <template v-for="input in form.inputs" :key="input.id">
      <v-row class="ma-sm-4">
        <v-col cols="12" :md="input.cols.md" :lg="input.cols.lg" :xl="input.cols.xl">
          <Component
            :is="input.component"
            v-bind="{ props: input.props }"
            :model-value="formData[input.id as keyof {}]"
            @update:model-value="(value: any) => onInputChanged(input.id, value)"
          />
        </v-col>
      </v-row>
    </template>
  </v-form>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { VForm } from "vuetify/components";

import FormContainer from "@/components/FormContainer.vue";
import PageContainer from "@/components/PageContainer.vue";
import profileInformationForm from "@/config/profile-information-form";
import type { Form } from "@/types/form";

export default defineComponent({
  name: "EcerForm",
  components: { FormContainer, PageContainer },
  props: {
    form: {
      type: Object as PropType<Form>,
      default: () => profileInformationForm,
    },
    formData: {
      type: Object as PropType<Record<string, any>>,
      default: () => ({}),
    },
  },
  emits: {
    updatedFormData: (_formData: Record<string, any>) => true,
    updatedValidation: (_validation: boolean | null) => true,
  },
  mounted() {
    setTimeout(this.resetFormValidation, 100);
  },
  methods: {
    onInputChanged(id: string, value: any) {
      this.$emit("updatedFormData", { ...this.formData, [id]: value });
    },
    onFormValidationChanged(value: boolean | null) {
      this.$emit("updatedValidation", value);
    },
    resetFormValidation() {
      (this.$refs[this.form.id] as VForm).resetValidation();
    },
  },
});
</script>
