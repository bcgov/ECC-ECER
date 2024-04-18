<template>
  <EceAddress
    :model-value="modelValue.residential"
    v-bind="{ props: props.residential }"
    @update:model-value="(value: any) => updateAddress(AddressType.RESIDENTIAL, value)"
  />
  <v-checkbox v-model="checked" color="primary" label="Mailing Address is the same as Residential Address"></v-checkbox>
  <EceAddress
    v-if="!checked"
    :model-value="modelValue.mailing"
    v-bind="{ props: props.mailing }"
    @update:model-value="(value: any) => updateAddress(AddressType.MAILING, value)"
  />
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceAddress from "@/components/inputs/EceAddress.vue";
import type { EceAddressesProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { AddressType } from "@/utils/constant";
import * as Functions from "@/utils/functions";

export default defineComponent({
  name: "EceAddresses",
  components: { EceAddress },
  props: {
    props: {
      type: Object as () => EceAddressesProps,
      required: true,
    },
    modelValue: {
      type: Object as () => AddressesData,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_addressesData: AddressesData) => true,
  },
  setup: () => {
    return { AddressType };
  },
  data: function () {
    return {
      checked: Functions.areObjectsEqual(this.modelValue.residential, this.modelValue.mailing),
    };
  },
  watch: {
    checked(newCheckedValue, oldCheckedValue) {
      if (newCheckedValue && !oldCheckedValue) {
        // Update mailing address to be equal to residential address when checked changes to true
        this.updateAddress(AddressType.MAILING, this.modelValue.residential);
      }
    },
  },
  methods: {
    updateAddress(addressType: AddressType, updatedAddress: Components.Schemas.Address) {
      if (this.checked) {
        this.$emit("update:model-value", {
          [AddressType.MAILING]: updatedAddress,
          [AddressType.RESIDENTIAL]: updatedAddress,
        });
      } else {
        this.$emit("update:model-value", {
          ...this.modelValue,
          [addressType]: updatedAddress,
        });
      }
    },
  },
});
</script>
