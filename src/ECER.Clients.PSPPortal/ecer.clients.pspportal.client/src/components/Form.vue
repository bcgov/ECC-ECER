<template>
  <v-form
    :id="form.id"
    :ref="form.id"
    validate-on="input"
    @update:model-value="onFormValidationChanged"
    @submit.prevent
  >
    <template v-for="component in form.components" :key="component.id">
      <v-row>
        <v-col
          cols="12"
          :md="component.cols.md"
          :lg="component.cols.lg"
          :xl="component.cols.xl"
        >
          <Component
            v-if="component.isInput !== false"
            :is="getResolvedComponent(component)"
            v-bind="component.props"
            :model-value="formData[component.id as keyof {}]"
            @update:model-value="
              (value: any) => onInputChanged(component.id, value)
            "
          >
            <template
              v-for="(slotContent, slotName) in component.slots"
              :key="slotName"
              #[slotName]
            >
              <span v-html="slotContent"></span>
            </template>
          </Component>
          <Component
            v-else
            :is="getResolvedComponent(component)"
            v-bind="component.props"
          />
        </v-col>
      </v-row>
    </template>
  </v-form>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import PageContainer from "@/components/PageContainer.vue";
import profileInformationForm from "@/config/profile-form";
import type { Form } from "@/types/form";
import {useProgramStore} from "@/store/program.ts";

export default defineComponent({
  name: "EcerForm",
  components: { PageContainer },
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
  setup() {
    const programStore = useProgramStore();
    return {
      programStore
    };
  },
  emits: {
    updatedFormData: (_formData: Record<string, any>) => true,
    updatedValidation: (_validation: boolean | null) => true,
  },
  methods: {
    onInputChanged(id: string, value: any) {
      this.$emit("updatedFormData", { ...this.formData, [id]: value });
    },
    onFormValidationChanged(value: boolean | null) {
      this.$emit("updatedValidation", value);
    },
    getResolvedComponent(component: any): any {
      if (component.getComponent) {
        const dataSources = {
          draftApplication: this.programStore.draftProgram,
        };
        return component.getComponent(dataSources);
      }
      return component.component;
    },
  },
});
</script>
