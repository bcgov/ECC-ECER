<template>
  <v-form :ref="form.id" validate-on="blur">
    <template v-for="input in form.inputs" :key="input.id">
      <Component :is="input.component" v-bind="{ props: input.props }" v-model="formData[input.id as keyof {}]" class="my-8" />
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
  name: "EceForm",
  components: { FormContainer, PageContainer },
  props: {
    form: {
      type: Object as PropType<Form>,
      default: () => profileInformationForm,
    },
  },

  data: () => ({
    formData: {},
  }),
});
</script>
