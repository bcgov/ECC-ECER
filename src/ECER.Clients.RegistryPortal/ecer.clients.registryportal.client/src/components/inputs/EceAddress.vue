<template>
  <v-row>
    <v-col cols="12">
      <h2>{{ `${addressLabel} address` }}</h2>
    </v-col>
  </v-row>

  <!-- Country selection -->
  <v-row>
    <v-col cols="12">
      <label :for="`${addressLabel}-country-autocomplete`">Country</label>
      <v-autocomplete
        :id="`${addressLabel}-country-autocomplete`"
        :model-value="modelValue.country"
        variant="outlined"
        color="primary"
        class="pt-2"
        :rules="[Rules.required('Select your country')]"
        :items="configStore?.countryList"
        clearable
        hide-details="auto"
        item-title="countryName"
        item-value="countryName"
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
        :rules="[
          Rules.required(`Enter your ${addressLabel.toLowerCase()} address`),
        ]"
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
        maxlength="100"
        @update:model-value="(value: string) => updateField('city', value)"
      ></EceTextField>
    </v-col>
  </v-row>

  <!-- Province Input: Render based on Country -->
  <!-- For Canada: Show Province/Territory list -->
  <v-row v-if="isCanada">
    <v-col cols="12">
      <label :for="`${addressLabel}-province-autocomplete`">
        Province / Territory
      </label>
      <v-autocomplete
        :id="`${addressLabel}-province-autocomplete`"
        :model-value="modelValue.province"
        variant="outlined"
        color="primary"
        class="pt-2"
        :rules="[
          Rules.required('Select your province/territory'),
          Rules.mustExistInList(filteredProvinceList, 'provinceCode'),
        ]"
        :items="filteredProvinceList"
        item-title="provinceName"
        item-value="provinceCode"
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
        maxlength="100"
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
        :rules="[
          Rules.conditionalWrapper(
            isCanada,
            Rules.required('Postal code required'),
          ),
          Rules.conditionalWrapper(isCanada, Rules.postalCode()),
        ]"
        variant="outlined"
        color="primary"
        maxlength="20"
        @update:model-value="
          (value: string) => updateField('postalCode', value)
        "
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
    isCanada(): boolean {
      return this.modelValue.country === this.configStore.canada?.countryName;
    },
    filteredProvinceList() {
      return (this.configStore?.provinceList || []).filter(
        (province) => province.provinceName !== ProvinceTerritoryType.OTHER,
      );
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
      this.updateField("country", this.configStore.canada?.countryName ?? "");
    }
    // If country is Canada and province is not set, default it to British Columbia.
    if (this.modelValue.country === "Canada" && !this.modelValue.province) {
      this.updateField(
        "province",
        this.configStore.britishColumbia?.provinceCode ?? "",
      );
    }
  },
  watch: {
    // When switching back to Canada, if no province is set, reset it to British Columbia.
    "modelValue.country"(newVal: string) {
      if (newVal === "Canada" && !this.modelValue.province) {
        this.updateField(
          "province",
          this.configStore.britishColumbia?.provinceCode ?? "",
        );
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
