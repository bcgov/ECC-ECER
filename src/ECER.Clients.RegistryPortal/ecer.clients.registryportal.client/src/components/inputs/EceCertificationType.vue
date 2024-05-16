<template>
  <v-expansion-panels v-if="certificationTypeStore.mode == 'selection'" v-model="selection" variant="accordion">
    <v-expansion-panel v-for="option in props.options" :key="option.id" :value="option.id" class="rounded-lg">
      <v-expansion-panel-title>
        <v-row no-gutters>
          <v-col cols="12">
            <v-radio-group v-model="selection" :mandatory="true" :hide-details="true" :error="errorState" :rules="[!errorState]">
              <v-radio color="primary" :label="option.title" :value="option.id"></v-radio>
            </v-radio-group>
          </v-col>
          <v-col v-if="option.id === CertificationType.FIVE_YEAR && selection !== CertificationType.FIVE_YEAR" cols="11" offset="1">
            <p class="small">If you are eligible for an ECE Five Year Certificate, you may also be eligible for one or both specializations:</p>
            <v-checkbox
              v-model="certificationTypeStore.subSelection"
              color="primary"
              label="Infant and Toddler Educator (ITE)"
              :value="CertificationType.ITE"
              hide-details="auto"
            ></v-checkbox>
            <v-checkbox
              v-model="certificationTypeStore.subSelection"
              color="primary"
              label="Special Needs Educator (SNE)"
              :value="CertificationType.SNE"
              hide-details="auto"
            ></v-checkbox>
          </v-col>
        </v-row>
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <Component :is="option.contentComponent" />
      </v-expansion-panel-text>
    </v-expansion-panel>
    <div v-if="errorState" class="v-messages error-message" role="alert" aria-live="polite">Select a certificate type to begin your application</div>
  </v-expansion-panels>
  <div v-if="certificationTypeStore.mode == 'terms'">
    <div v-for="certificationType in certificationTypes" :key="certificationType">
      <template v-if="certificationType === CertificationType.ECE_ASSISTANT">
        <ECEAssistantRequirements />
      </template>
      <template v-if="certificationType === CertificationType.ONE_YEAR">
        <ECEOneYearRequirements />
      </template>
      <template v-if="certificationType === CertificationType.FIVE_YEAR">
        <ECEFiveYearRequirements />
      </template>
      <template v-if="certificationType === CertificationType.SNE">
        <SneRequirements />
      </template>
      <template v-if="certificationType === CertificationType.ITE">
        <IteRequirements />
      </template>
    </div>
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
import { CertificationType } from "@/utils/constant";

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
    "update:model-value": (_certificationTypes: Components.Schemas.CertificationType[]) => true,
  },
  setup: (props) => {
    const certificationTypeStore = useCertificationTypeStore();
    // If props.modelValue contains "Ite" or "Sne", set the subSelection to those values
    if (props.modelValue.includes(CertificationType.ITE)) {
      certificationTypeStore.subSelection.push(CertificationType.ITE);
    }
    if (props.modelValue.includes(CertificationType.SNE)) {
      certificationTypeStore.subSelection.push(CertificationType.SNE);
    }

    // Set selection to value in props.modelValue
    if (props.modelValue.includes(CertificationType.FIVE_YEAR)) {
      certificationTypeStore.selection = CertificationType.FIVE_YEAR;
    } else if (props.modelValue.includes(CertificationType.ECE_ASSISTANT)) {
      certificationTypeStore.selection = CertificationType.ECE_ASSISTANT;
    } else if (props.modelValue.includes(CertificationType.ONE_YEAR)) {
      certificationTypeStore.selection = CertificationType.ONE_YEAR;
    }

    return { certificationTypeStore, CertificationType };
  },
  computed: {
    selection: {
      get() {
        return this.certificationTypeStore.selection;
      },
      set(newValue: Components.Schemas.CertificationType) {
        if (newValue !== CertificationType.FIVE_YEAR) {
          this.certificationTypeStore.subSelection = [];
        }
        this.certificationTypeStore.selection = newValue;
      },
    },
    ...mapState(useCertificationTypeStore, ["certificationTypes"]),
    errorState() {
      return !this.selection;
    },
  },
  watch: {
    certificationTypes(newValue: Components.Schemas.CertificationType[], _) {
      this.$emit("update:model-value", newValue);
    },
  },
});
</script>

<style>
.error-message {
  color: rgb(var(--v-theme-error));
}
</style>
