<template>
  <v-text-field
    ref="line1"
    :model-value="modelValue.line1"
    :rules="[Rules.required()]"
    :label="props.addressLabel + ' Street Address'"
    variant="outlined"
    color="primary"
    maxlength="150"
    class="my-8"
    @input="updateField('line1', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.city"
    :rules="[Rules.required()]"
    label="City/Town"
    variant="outlined"
    color="primary"
    maxlength="50"
    class="my-8"
    @input="updateField('city', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.province"
    :rules="[Rules.required()]"
    label="Province"
    variant="outlined"
    color="primary"
    maxlength="50"
    class="my-8"
    @input="updateField('province', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.postalCode"
    :rules="[Rules.required()]"
    label="Postal Code"
    variant="outlined"
    color="primary"
    maxlength="7"
    class="my-8"
    @input="updateField('postalCode', $event)"
  ></v-text-field>
  <v-text-field
    :model-value="modelValue.country"
    :rules="[Rules.required()]"
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
    updateField(fieldName: string, event: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: event.target.value,
      });
    },
  },
});
</script>
