<template>
    <v-row>
        <v-col cols="12">
        <label :for="'provinceAutocomplete'">Province</label>
        <v-autocomplete
            :id="'provinceAutocomplete'"
            :model-value="modelValue"
            variant="outlined"
            color="primary"
            class="pt-2"
            :rules="[Rules.required('Select your province'), Rules.mustExistInList(filteredProvinceList, 'provinceName')]"
            :items="filteredProvinceList"
            item-title="provinceName"
            item-value="provinceCode"
            clearable
            hide-details="auto"
            @update:model-value="(value: string) => provinceChanged(value)"
        ></v-autocomplete>
        </v-col>
    </v-row>
  </template>

<script lang="ts">
import { defineComponent } from "vue";
import * as Rules from "@/utils/formRules";
import EceTextField from "@/components/inputs/EceTextField.vue";
import { useConfigStore } from "@/store/config";
import { ProvinceTerritoryType } from "@/utils/constant";

export default defineComponent({
  name: "EceProvince",
  components: { EceTextField },
  props: {
    modelValue: {
      type: String,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_province: string) => true,
  },
  setup() {
    const configStore = useConfigStore();
    return { configStore };
  },

  computed: {
    filteredProvinceList() {
      return (this.configStore?.provinceList || []).filter((province) => province.provinceName !== ProvinceTerritoryType.OTHER);
    },
  },
  data() {
    return {
      Rules,
    };
  },
  mounted() {
    // If province is not set, default it to British Columbia.
    if (!this.modelValue) {
      this.provinceChanged(this.configStore.britishColumbia?.provinceName ?? "");
    }
  },
  methods: {
    provinceChanged(value: string) {
      let chosenProvince = this.filteredProvinceList.find(province => province.provinceCode === value);
      this.$emit("update:model-value", chosenProvince?.provinceName ?? "");
    },
  },
});
</script>
