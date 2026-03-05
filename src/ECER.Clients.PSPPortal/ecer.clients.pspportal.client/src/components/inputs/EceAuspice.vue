<template>
  <v-row>
    <v-col cols="12">
      <label :for="'auspiceAutocomplete'">Institution type</label>
      <v-autocomplete
        :id="'auspiceAutocomplete'"
        :model-value="modelValue"
        variant="outlined"
        color="primary"
        class="pt-2"
        :items="auspiceList"
        item-title="text"
        item-value="value"
        clearable
        hide-details="auto"
        v-bind="$attrs"
        @update:model-value="
          (value: PsiInstitutionType) => auspiceChanged(value)
        "
      ></v-autocomplete>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import * as Rules from "@/utils/formRules";
import EceTextField from "@/components/inputs/EceTextField.vue";
import { useConfigStore } from "@/store/config";
import type { PsiInstitutionType, Components } from "@/types/openapi";

export default defineComponent({
  name: "EcePsiInstitutionType",
  components: { EceTextField },
  props: {
    modelValue: {
      type: Object as PropType<Components.Schemas.PsiInstitutionType>,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_auspice: PsiInstitutionType) => true,
  },
  setup() {
    const configStore = useConfigStore();
    const auspiceList = [
      { text: "Continuing Education", value: "ContinuingEducation" },
      { text: "Private", value: "Private" },
      { text: "Public", value: "Public" },
      { text: "Public — OOP", value: "PublicOOP" },
    ];
    return { configStore, auspiceList };
  },

  computed: {
    getPsiInstitutionTypeList() {
      return this.auspiceList;
    },
  },
  data() {
    return {
      Rules,
    };
  },
  mounted() {
    // If auspice is not set, default it to public.
    if (!this.modelValue) {
      this.auspiceChanged("Public");
    }
  },
  methods: {
    auspiceChanged(value: PsiInstitutionType) {
      this.$emit("update:model-value", value);
    },
  },
});
</script>
