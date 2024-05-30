<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>Contact information</h2>
      <div role="doc-subtitle">We may contact you to verify or clarify information you provide.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :model-value="modelValue.lastName"
            :rules="[Rules.required('Enter your last name')]"
            label="Last Name"
            variant="outlined"
            color="primary"
            maxlength="100"
            hide-details="auto"
            @input="updateField('lastName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :model-value="modelValue.firstName"
            :rules="[Rules.required('Enter your first name')]"
            label="First Name"
            variant="outlined"
            color="primary"
            maxlength="100"
            hide-details="auto"
            @input="updateField('firstName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :model-value="modelValue.email"
            :rules="[Rules.required(), Rules.email('Enter your email in the format \'name@email.com\'')]"
            label="Email"
            variant="outlined"
            color="primary"
            maxlength="200"
            hide-details="auto"
            @input="updateField('email', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :model-value="modelValue.phoneNumber"
            :rules="[Rules.required(), Rules.phoneNumber('Enter your 10-digit phone number')]"
            label="Phone Number"
            variant="outlined"
            color="primary"
            maxlength="10"
            hide-details="auto"
            @input="updateField('phoneNumber', $event)"
            @keypress="isNumber($event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <h2 class="mt-5">ECE certification</h2>
      <div v-if="wizardStore.wizardData.inviteType === PortalInviteType.CHARACTER" role="doc-subtitle">
        If you are registered as an ECE in Canada, please provide your certification number.
      </div>
      <div v-if="wizardStore.wizardData.inviteType === PortalInviteType.WORK_EXPERIENCE" role="doc-subtitle">
        We need this information to look up your certificate. Only people with a valid certificate can be a work reference.
      </div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            v-if="wizardStore.wizardData.inviteType === PortalInviteType.CHARACTER"
            :model-value="modelValue.certificateProvinceId"
            label="Province/Territory Certified/Registered In (Optional)"
            variant="outlined"
            color="primary"
            :items="configStore?.provinceList"
            clearable
            hide-details="auto"
            @update:model-value="certificateProvinceIdChanged"
            @click:clear="provinceClearClicked"
          ></v-autocomplete>
          <v-autocomplete
            v-if="wizardStore.wizardData.inviteType === PortalInviteType.WORK_EXPERIENCE"
            :model-value="modelValue.certificateProvinceId"
            label="Province/Territory Certified/Registered In"
            variant="outlined"
            color="primary"
            :items="configStore?.provinceList.filter((province) => province.title !== ProvinceTerritoryType.OTHER)"
            hide-details="auto"
            :rules="[Rules.required()]"
            @update:model-value="certificateProvinceIdChanged"
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            v-if="modelValue.certificateProvinceId"
            ref="certificateNumberRef"
            :model-value="modelValue.certificateNumber"
            :rules="[customOptionalIfNotBCRule()]"
            :label="`ECE Certification/Registration Number${userSelectProvinceIdBC ? '' : ' (Optional)'}`"
            variant="outlined"
            color="primary"
            maxlength="25"
            hide-details="auto"
            @input="updateField('certificateNumber', $event)"
            @keypress="isNumber($event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            v-if="modelValue.certificateProvinceId && !userSelectProvinceIdBC"
            :model-value="modelValue.dateOfBirth"
            label="Your date of birth (Optional)"
            variant="outlined"
            color="primary"
            hide-details="auto"
            type="date"
            :max="today"
            @input="updateField('dateOfBirth', $event)"
          ></v-text-field>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";
import type { VTextField } from "vuetify/components";

import { useConfigStore } from "@/store/config";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { PortalInviteType } from "@/utils/constant";
import { ProvinceTerritoryType } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceReferenceContact",
  props: {
    modelValue: {
      type: Object as () => Components.Schemas.ReferenceContactInformation,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_contactInformationData: Components.Schemas.ReferenceContactInformation) => true,
  },
  setup() {
    const configStore = useConfigStore();
    const wizardStore = useWizardStore();

    return { configStore, wizardStore, PortalInviteType, ProvinceTerritoryType };
  },
  data() {
    return {
      Rules,
    };
  },
  computed: {
    userSelectProvinceIdBC(): boolean {
      const provinceName = this.configStore.provinceName(this.modelValue?.certificateProvinceId as string);
      return provinceName === ProvinceTerritoryType.BC;
    },
    today() {
      return formatDate(DateTime.now().toString());
    },
  },
  mounted() {
    if (this.wizardStore.wizardData.inviteType === PortalInviteType.WORK_EXPERIENCE) {
      const bcProvinceId = this.configStore?.provinceList.find((province) => province.title === ProvinceTerritoryType.BC)?.value;
      this.certificateProvinceIdChanged(bcProvinceId as string);
    }
  },
  methods: {
    isNumber,
    updateField(fieldName: keyof Components.Schemas.ReferenceContactInformation, event: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: event.target.value,
      });
    },
    customOptionalIfNotBCRule() {
      if (this.userSelectProvinceIdBC) {
        return (v: string) => !!(v && v?.trim()) || "Enter your ECE certification/registration in a numeric format";
      }
      return true;
    },
    certificateProvinceIdChanged(value: string) {
      (this.$refs.certificateNumberRef as VTextField)?.resetValidation();
      if (value === "BC") {
        this.$emit("update:model-value", {
          ...this.modelValue,
          certificateProvinceId: value,
          dateOfBirth: null,
        });
      } else {
        this.$emit("update:model-value", {
          ...this.modelValue,
          certificateProvinceId: value,
        });
      }
    },
    provinceClearClicked() {
      this.$emit("update:model-value", {
        ...this.modelValue,
        certificateNumber: "",
        certificateProvinceId: "",
        dateOfBirth: null,
      });
    },
  },
});
</script>
