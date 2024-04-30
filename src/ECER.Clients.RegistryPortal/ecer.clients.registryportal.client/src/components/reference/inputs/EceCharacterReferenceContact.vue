<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h3>Contact Information</h3>
      <div role="doc-subtitle">We may contact you to verify or clarify information you provide.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :rules="[Rules.required('Enter your last name')]"
            label="Last Name"
            variant="outlined"
            color="primary"
            maxlength="100"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { lastName: value })"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :rules="[Rules.required('Enter your first name')]"
            label="First Name"
            variant="outlined"
            color="primary"
            maxlength="100"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { firstName: value })"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :rules="[Rules.required(), Rules.email('Enter your email in the format \'name@email.com\'')]"
            label="Email"
            variant="outlined"
            color="primary"
            maxlength="200"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { email: value })"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            :rules="[Rules.required(), Rules.phoneNumber('Enter your 10-digit phone number')]"
            label="Phone Number"
            variant="outlined"
            color="primary"
            maxlength="10"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { phoneNumber: value })"
            @keypress="isNumber($event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <h3 class="mt-5">ECE Certification</h3>
      <div role="doc-subtitle">If you are registered as an ECE in Canada, please provide your certification number if applicable.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-autocomplete
            v-model="certificateProvinceId"
            label="Province/Territory Certified/Registered In (Optional)"
            variant="outlined"
            color="primary"
            :items="provinces"
            clearable
            hide-details="auto"
            @update:model-value="certificateProvinceIdChanged"
            @click:clear="provinceClearClicked"
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            v-if="certificateProvinceId"
            ref="certificateNumberRef"
            v-model="certificateNumber"
            :rules="[customOptionalIfNotBCRule()]"
            :label="`ECE Certification/Registration Number${userSelectProvinceIdBC ? '' : ' (Optional)'}`"
            variant="outlined"
            color="primary"
            maxlength="10"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value, { certificateNumber: value })"
            @keypress="isNumber($event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <v-text-field
            v-if="certificateProvinceId && !userSelectProvinceIdBC"
            v-model="dateOfBirth"
            label="Your date of birth (Optional)"
            variant="outlined"
            color="primary"
            hide-details="auto"
            type="date"
            :max="today"
            @update:model-value="(value) => $emit('update:model-value', value, { dateOfBirth: value })"
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

import type { FormData } from "@/store/form";
import { useWizardStore } from "@/store/wizard";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceCharacterReferenceContact",
  components: {},
  emits: {
    "update:model-value": (_value: any, _updateFormData?: FormData) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return { wizardStore };
  },
  data() {
    return {
      selection: undefined,
      Rules,
      certificateProvinceId: "",
      dateOfBirth: "",
      certificateNumber: "",
    };
  },
  computed: {
    provinces() {
      return [
        { title: "British Columbia", value: "BC" },
        { title: "Alberta", value: "AB" },
        { title: "Prince Edward Island", value: "PEI" },
      ];
    },
    userSelectProvinceIdBC(): boolean {
      return this.certificateProvinceId === "BC";
    },
    today() {
      return formatDate(DateTime.now().toString());
    },
  },
  methods: {
    isNumber,
    customOptionalIfNotBCRule() {
      if (this.userSelectProvinceIdBC) {
        return (v: string) => !!(v && v?.trim()) || "Required";
      }
      return true;
    },
    certificateProvinceIdChanged(value: string) {
      (this.$refs.certificateNumberRef as VTextField)?.resetValidation();
      if (value === "BC") {
        this.$emit("update:model-value", value, { dateOfBirth: "", certificateProvinceId: value });
        this.dateOfBirth = "";
      } else {
        this.$emit("update:model-value", value, { certificateProvinceId: value });
      }
    },
    provinceClearClicked() {
      this.certificateNumber = "";
      this.dateOfBirth = "";
      this.certificateProvinceId = "";
      this.$emit("update:model-value", "", { dateOfBirth: "", certificateNumber: "", certificateProvinceId: "" });
    },
  },
});
</script>
