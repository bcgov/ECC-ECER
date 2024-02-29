<template>
  <v-expansion-panels v-if="mode == 'selection'" v-model="selection" variant="accordion">
    <v-expansion-panel v-for="option in props.options" :key="option.id" :value="option.id" class="rounded-lg">
      <v-expansion-panel-title>
        <v-row no-gutters>
          <v-col cols="12">
            <v-radio-group v-model="selection" :mandatory="true" :hide-details="true">
              <v-radio color="primary" :label="option.title" :value="option.id"></v-radio>
            </v-radio-group>
          </v-col>
          <v-col v-if="option.id === 'FiveYears' && selection !== 'FiveYears'" cols="11" offset="1">
            <p class="small">If you are eligible for an ECE Five Year Certificate, you may also be eligible for one or both specializations:</p>
            <v-checkbox
              v-model="certificationTypeStore.subSelection"
              color="primary"
              label="Infant and Toddler Educator (ITE)"
              value="Ite"
              hide-details="auto"
            ></v-checkbox>
            <v-checkbox
              v-model="certificationTypeStore.subSelection"
              color="primary"
              label="Special Needs Educator (SNE)"
              value="Sne"
              hide-details="auto"
            ></v-checkbox>
          </v-col>
        </v-row>
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <Component :is="option.contentComponent" />
      </v-expansion-panel-text>
    </v-expansion-panel>
  </v-expansion-panels>
  <div v-if="mode == 'requirements'">
    <v-row class="ga-4">
      <v-col cols="1" offset="0" offset-lg="2" offset-md="2" offset-xl="2">
        <v-btn variant="text" @click="$router.back()">
          <v-icon start icon="mdi-arrow-left"></v-icon>
          Back
        </v-btn>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="8" lg="8" xl="8" class="mx-auto">
        <v-card>
          <v-card-text>
            <v-row class="ga-4">
              <div v-for="certificationType in certificationTypes" :key="certificationType">
                <template v-if="certificationType === 'EceAssistant'">
                  <ECEAssistantRequirements />
                </template>
                <template v-if="certificationType === 'OneYear'">
                  <ECEOneYearRequirements />
                </template>
                <template v-if="certificationType === 'FiveYears'">
                  <ECEFiveYearRequirements />
                </template>
                <template v-if="certificationType === 'Sne'">
                  <SneRequirements />
                </template>
                <template v-if="certificationType === 'Ite'">
                  <IteRequirements />
                </template>
              </div>
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import { mapState } from "pinia";
import { defineComponent } from "vue";

import ECEAssistantRequirements from "@/components/ECEAssistantRequirements.vue";
import ECEFiveYearRequirements from "@/components/ECEFiveYearRequirements.vue";
import ECEOneYearRequirements from "@/components/ECEOneYearRequirements.vue";
import IteRequirements from "@/components/IteRequirements.vue";
import SneRequirements from "@/components/SneRequirements.vue";
import { useCertificationTypeStore } from "@/store/certificationType";
import type { EceCertificateTypeProps } from "@/types/input";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "EceCertificationType",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
    SneRequirements,
    IteRequirements,
  },
  props: {
    props: {
      type: Object as () => EceCertificateTypeProps,
      required: true,
    },
    modelValue: {
      type: Object as () => Components.Schemas.CertificationType[],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_certificateTypeList: Components.Schemas.CertificationType[]) => true,
  },
  setup: (props) => {
    const certificationTypeStore = useCertificationTypeStore();
    // If props.modelValue contains "Ite" or "Sne", set the subSelection to those values
    if (props.modelValue.includes("Ite")) {
      certificationTypeStore.subSelection.push("Ite");
    }
    if (props.modelValue.includes("Sne")) {
      certificationTypeStore.subSelection.push("Sne");
    }

    // Set selection to value in props.modelValue
    if (props.modelValue.includes("FiveYears")) {
      certificationTypeStore.selection = "FiveYears";
    } else if (props.modelValue.includes("EceAssistant")) {
      certificationTypeStore.selection = "EceAssistant";
    } else if (props.modelValue.includes("OneYear")) {
      certificationTypeStore.selection = "OneYear";
    }

    return { certificationTypeStore };
  },
  data: () => ({
    mode: "selection",
  }),
  computed: {
    selection: {
      get() {
        return this.certificationTypeStore.selection;
      },
      set(newValue: Components.Schemas.CertificationType) {
        if (newValue !== "FiveYears") {
          this.certificationTypeStore.subSelection = [];
        }
        this.certificationTypeStore.selection = newValue;
      },
    },
    ...mapState(useCertificationTypeStore, ["certificationTypes"]),
  },
  watch: {
    certificationTypes(newValue: Components.Schemas.CertificationType[], _) {
      this.$emit("update:model-value", newValue);
    },
  },
});
</script>
