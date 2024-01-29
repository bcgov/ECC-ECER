<template>
  <v-form :ref="form.id" validate-on="blur">
    <template v-for="input in form.inputs" :key="input.id">
      <Component
        :is="input.component"
        v-bind="{ props: input.props }"
        :model-value="formData[input.id as keyof {}]"
        class="my-8"
        @update:model-value="(value: any) => onInputChanged(input.id, value)"
      />
    </template>
  </v-form>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

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
  },
  methods: {
    onInputChanged(id: string, value: any) {
      this.$emit("updatedFormData", { ...this.formData, [id]: value });
    },
  },
});
</script>
