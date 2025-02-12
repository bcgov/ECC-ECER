addressLabel
<template>
  <v-row>
    <v-col cols="12">
      <h2>{{ `${addressLabel} address` }}</h2>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <EceTextField
        ref="line1"
        :label="addressLabel + ' street address'"
        :model-value="modelValue.line1"
        :rules="[Rules.required(`Enter your ${addressLabel.toLowerCase()} address`)]"
        variant="outlined"
        color="primary"
        maxlength="100"
        @update:model-value="(value: string) => updateField('line1', value)"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <EceTextField
        :model-value="modelValue.city"
        label="City/Town"
        :rules="[Rules.required('Select your city/town')]"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="(value: string) => updateField('city', value)"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <EceTextField
        :model-value="modelValue.province"
        label="Province"
        :rules="[Rules.required('Select your province')]"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="(value: string) => updateField('province', value)"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <EceTextField
        :model-value="modelValue.postalCode"
        label="Postal code"
        :rules="[Rules.required('Postal code required'), Rules.postalCode()]"
        variant="outlined"
        color="primary"
        maxlength="7"
        @update:model-value="(value: string) => updateField('postalCode', value)"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12">
      <EceTextField
        :model-value="modelValue.country"
        label="Country"
        :rules="[Rules.required('Select your country')]"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="(value: string) => updateField('country', value)"
      ></EceTextField>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
import EceTextField from "@/components/inputs/EceTextField.vue";

export default defineComponent({
  name: "EceAddress",
  components: { EceTextField },
  props: {
    addressLabel: {
      type: String,
      required: true,
    },
    modelValue: {
      type: Object as () => Components.Schemas.Address,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_addressData: Components.Schemas.Address) => true,
  },
  data: function () {
    return {
      checked: true,
      Rules,
    };
  },
  methods: {
    updateField(fieldName: keyof Components.Schemas.Address, value: string) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
  },
});
</script>
