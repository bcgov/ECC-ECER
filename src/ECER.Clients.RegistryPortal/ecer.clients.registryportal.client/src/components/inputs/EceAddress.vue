<template>
  <h2>{{ `${props.addressLabel} address` }}</h2>
  <v-text-field
    ref="line1"
    :model-value="modelValue.line1"
    :rules="[Rules.required(`Enter your ${props.addressLabel.toLowerCase()} address`)]"
    :label="props.addressLabel + ' street address'"
    variant="outlined"
    color="primary"
    maxlength="100"
    class="my-8"
    @input="updateField('line1', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.city"
    :rules="[Rules.required('Select your city/town')]"
    label="City/Town"
    variant="outlined"
    color="primary"
    maxlength="50"
    class="my-8"
    @input="updateField('city', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.province"
    :rules="[Rules.required('Select your province')]"
    label="Province"
    variant="outlined"
    color="primary"
    maxlength="50"
    class="my-8"
    @input="updateField('province', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.postalCode"
    :rules="[Rules.required('Postal code required'), Rules.postalCode()]"
    label="Postal code"
    variant="outlined"
    color="primary"
    maxlength="7"
    class="my-8"
    @input="updateField('postalCode', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.country"
    :rules="[Rules.required('Select your country')]"
    label="Country"
    variant="outlined"
    color="primary"
    maxlength="50"
    class="my-8"
    @input="updateField('country', $event)"
  ></v-text-field>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { EceAddressProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceAddress",
  props: {
    props: {
      type: Object as () => EceAddressProps,
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
    updateField(fieldName: keyof Components.Schemas.Address, event: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: event.target.value,
      });
    },
  },
});
</script>
