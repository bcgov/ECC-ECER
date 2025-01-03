<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>Contact information</h2>
      <div role="doc-subtitle">We may contact you to verify or clarify information you provide.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :model-value="modelValue.lastName"
            :rules="[Rules.required('Enter your last name')]"
            label="Last Name"
            autocomplete="family-name"
            maxlength="100"
            @input="updateField('lastName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :model-value="modelValue.firstName"
            label="First Name"
            autocomplete="given-name"
            maxlength="100"
            @input="updateField('firstName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :model-value="modelValue.email"
            :rules="[Rules.required(), Rules.email('Enter your email in the format \'name@email.com\'')]"
            label="Email"
            autocomplete="email"
            maxlength="200"
            @input="updateField('email', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            :model-value="modelValue.phoneNumber"
            :rules="[Rules.required(), Rules.phoneNumber('Enter your 10-digit phone number')]"
            label="Phone Number"
            autocomplete="tel"
            maxlength="10"
            @input="updateField('phoneNumber', $event)"
            @keypress="isNumber($event)"
          ></EceTextField>
        </v-col>
      </v-row>
      <div v-if="wizardStore.wizardData.workExperienceType === WorkExperienceType.IS_400_Hours">
        <h2 class="mt-5">ECE certification</h2>
        <div>If you're registered as an ECE in Canada, please provide your certification number.</div>

        <v-row class="mt-5">
          <v-col cols="12" md="8" lg="6" xl="4">
            <label>
              Province/Territory Certified/Registered In (Optional)
              <v-autocomplete
                :model-value="modelValue.certificateProvinceId"
                label=""
                variant="outlined"
                color="primary"
                class="pt-2"
                :items="configStore?.provinceList"
                clearable
                hide-details="auto"
                @update:model-value="certificateProvinceIdChanged"
                @click:clear="provinceClearClicked"
              ></v-autocomplete>
            </label>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="8" lg="6" xl="4">
            <EceTextField
              v-if="modelValue.certificateProvinceId"
              ref="certificateNumberRef"
              :model-value="modelValue.certificateNumber"
              :label="`ECE Certification/Registration Number (Optional)`"
              maxlength="25"
              @input="updateField('certificateNumber', $event)"
              @keypress="isNumber($event)"
            ></EceTextField>
          </v-col>
        </v-row>
      </div>
      <div v-else>
        <h2 class="mt-5">ECE certification</h2>
        <div v-if="wizardStore.wizardData.inviteType === PortalInviteType.CHARACTER" role="doc-subtitle">
          If you are registered as an ECE in Canada, please provide your certification number.
        </div>
        <div v-if="wizardStore.wizardData.inviteType === PortalInviteType.WORK_EXPERIENCE" role="doc-subtitle">
          We need this information to look up your certificate. Only people with a valid certificate can be a work reference.
        </div>
        <v-row class="mt-5">
          <v-col cols="12" md="8" lg="6" xl="4">
            <label v-if="wizardStore.wizardData.inviteType === PortalInviteType.CHARACTER">
              Province/Territory Certified/Registered In (Optional)
              <v-autocomplete
                :model-value="modelValue.certificateProvinceId"
                label=""
                variant="outlined"
                color="primary"
                class="pt-2"
                :items="configStore?.provinceList"
                clearable
                @update:model-value="certificateProvinceIdChanged"
                @click:clear="provinceClearClicked"
              ></v-autocomplete>
            </label>
            <label v-if="wizardStore.wizardData.inviteType === PortalInviteType.WORK_EXPERIENCE">
              Province/Territory Certified/Registered In
              <v-autocomplete
                :model-value="modelValue.certificateProvinceId"
                label=""
                variant="outlined"
                color="primary"
                class="pt-2"
                :items="configStore?.provinceList.filter((province) => province.title !== ProvinceTerritoryType.OTHER)"
                :rules="[Rules.required()]"
                @update:model-value="certificateProvinceIdChanged"
              ></v-autocomplete>
            </label>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="8" lg="6" xl="4">
            <EceTextField
              v-if="modelValue.certificateProvinceId"
              ref="certificateNumberRef"
              :model-value="modelValue.certificateNumber"
              :rules="[customOptionalIfNotBCRule()]"
              :label="`ECE Certification/Registration Number${userSelectProvinceIdBC ? '' : ' (Optional)'}`"
              maxlength="25"
              @input="updateField('certificateNumber', $event)"
              @keypress="isNumber($event)"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="8" lg="6" xl="4">
            <EceDateInput
              v-if="modelValue.certificateProvinceId && !userSelectProvinceIdBC"
              :model-value="modelValue.dateOfBirth"
              label="Your date of birth (Optional)"
              type="date"
              :max="today"
              @input="updateField('dateOfBirth', $event)"
            ></EceDateInput>
          </v-col>
        </v-row>
      </div>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";

import EceTextField, { type ECETextField } from "@/components/inputs/EceTextField.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import { useConfigStore } from "@/store/config";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { PortalInviteType, WorkExperienceType } from "@/utils/constant";
import { ProvinceTerritoryType } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceReferenceContact",
  components: { EceTextField, EceDateInput },
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

    return { configStore, wizardStore, PortalInviteType, ProvinceTerritoryType, WorkExperienceType };
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
    if (
      this.wizardStore.wizardData.inviteType === PortalInviteType.WORK_EXPERIENCE &&
      this.wizardStore.wizardData.workExperienceType === WorkExperienceType.IS_500_Hours
    ) {
      const bcProvinceId = this.configStore?.provinceList.find((province) => province.title === ProvinceTerritoryType.BC)?.value;
      this.certificateProvinceIdChanged(bcProvinceId as string);
    }
  },
  methods: {
    isNumber,
    updateField(fieldName: keyof Components.Schemas.ReferenceContactInformation, value: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
    customOptionalIfNotBCRule() {
      if (this.userSelectProvinceIdBC) {
        return (v: string) => !!(v && v?.trim()) || "Enter your ECE certification/registration in a numeric format";
      }
      return true;
    },
    certificateProvinceIdChanged(value: string) {
      (this.$refs.certificateNumberRef as ECETextField)?.resetValidation();
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
