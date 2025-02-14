<template>
  <v-row>
    <v-col cols="12">
      <h2>{{ `${addressLabel} address` }}</h2>
    </v-col>
  </v-row>

  <!-- Country selection -->
  <v-row>
    <v-col cols="12">
      <label>Country</label>
      <v-autocomplete
        :model-value="modelValue.country"
        variant="outlined"
        color="primary"
        class="pt-2"
        :rules="[Rules.required('Select your country')]"
        :items="configStore?.countryList"
        clearable
        hide-details="auto"
        @update:model-value="(value: string) => updateField('country', value)"
      ></v-autocomplete>
    </v-col>
  </v-row>

  <!-- Street Address (always required) -->
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

  <!-- City/Town (always required) -->
  <v-row>
    <v-col cols="12">
      <EceTextField
        :model-value="modelValue.city"
        label="City or town"
        :rules="[Rules.required('Select your city/town')]"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="(value: string) => updateField('city', value)"
      ></EceTextField>
    </v-col>
  </v-row>

  <!-- Province Input: Render based on Country -->
  <!-- For Canada: Show Province/Territory list -->
  <v-row v-if="isCanada">
    <v-col cols="12">
      <label>Province / Territory</label>
      <v-autocomplete
        :model-value="modelValue.province"
        variant="outlined"
        color="primary"
        class="pt-2"
        :rules="[Rules.required('Select your province/territory')]"
        :items="filteredProvinceList"
        item-title="title"
        item-value="code"
        clearable
        hide-details="auto"
        @update:model-value="(value: string) => updateField('province', value)"
      ></v-autocomplete>
    </v-col>
  </v-row>

  <!-- For non-Canada: Show Province/State text field -->

  <v-row v-else>
    <v-col cols="12">
      <EceTextField
        :model-value="modelValue.province"
        label="Province or State"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="(value: string) => updateField('province', value)"
      ></EceTextField>
    </v-col>
  </v-row>

  <!-- Postal Code Input -->
  <v-row>
    <v-col cols="12">
      <EceTextField
        :model-value="modelValue.postalCode"
        :label="isCanada ? 'Postal code' : 'Postal / Zip code'"
        :rules="postalRules"
        variant="outlined"
        color="primary"
        maxlength="7"
        @update:model-value="(value: string) => updateField('postalCode', value)"
      ></EceTextField>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
import EceTextField from "@/components/inputs/EceTextField.vue";
import { useConfigStore } from "@/store/config";
import { ProvinceTerritoryType } from "@/utils/constant";

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
  setup() {
    const configStore = useConfigStore();
    return { configStore };
  },

  computed: {
    // Check if the selected country is Canada.
    isCanada(): boolean {
      return this.modelValue.country === "Canada";
    },
    // Set postal code validation rules based on the country.
    postalRules(): any[] {
      return this.isCanada ? [Rules.required("Postal code required"), Rules.postalCode()] : [];
    },
    filteredProvinceList() {
      return (this.configStore?.provinceList || []).filter((province) => province.title !== ProvinceTerritoryType.OTHER);
    },
  },
  data() {
    return {
      Rules,
    };
  },
  mounted() {
    // On mount, default the country to Canada if not set.
    if (!this.modelValue.country) {
      this.updateField("country", "Canada");
    }
    // If country is Canada and province is not set, default it to British Columbia.
    if (this.modelValue.country === "Canada" && !this.modelValue.province) {
      this.updateField("province", "BC");
    }
  },
  watch: {
    // When switching back to Canada, if no province is set, reset it to British Columbia.
    "modelValue.country"(newVal: string) {
      if (newVal === "Canada" && !this.modelValue.province) {
        this.updateField("province", "BC");
      }
    },
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
