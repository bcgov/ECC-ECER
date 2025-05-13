<template>
  <v-row>
    <v-col cols="4"><p>Eligible B.C. transfer application</p></v-col>
    <v-col cols="8">
      <p>
        <b>{{ applicationStore.certificateName }}</b>
      </p>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="4"><p>Province/territory of certification</p></v-col>
    <v-col cols="8">
      <p>
        <b>{{ modelValue.labourMobilityProvince?.provinceName }}</b>
      </p>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="4"><p>Certification type</p></v-col>
    <v-col cols="8">
      <p>
        <b>{{ modelValue.existingCertificationType }}</b>
      </p>
    </v-col>
  </v-row>
  <v-row>
    <v-col><h3>Certificate details</h3></v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceTextField
        id="certificateNumber"
        :model-value="modelValue.currentCertificationNumber"
        :rules="[Rules.required('Enter your certificate or registration number')]"
        label="Certificate or registration number"
        maxlength="100"
        @update:model-value="(value: string) => updateField('currentCertificationNumber', value)"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <p>What name is shown on your certificate?</p>
      <br />
      <v-radio-group
        id="radioNameOnCertificate"
        v-model="previousNameRadio"
        :rules="[Rules.requiredRadio('Select an option')]"
        @update:model-value="nameRadioChanges"
      >
        <v-radio v-for="(step, index) in applicantNameRadioOptions" :key="index" :label="step.label" :value="step.value"></v-radio>
      </v-radio-group>
      <div v-if="previousNameRadio === 'other'">
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              :model-value="modelValue.legalFirstName"
              :rules="[Rules.required('Enter your first name')]"
              label="First name on certificate"
              variant="outlined"
              color="primary"
              maxlength="100"
              @update:model-value="(value: string) => updateField('legalFirstName', value)"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              :model-value="modelValue.legalMiddleName"
              label="Middle name(s) on certificate (optional)"
              variant="outlined"
              color="primary"
              maxlength="100"
              @update:model-value="(value: string) => updateField('legalMiddleName', value)"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              :model-value="modelValue.legalLastName"
              :rules="[Rules.required('Enter your last name')]"
              label="Last name on certificate"
              variant="outlined"
              color="primary"
              maxlength="100"
              @update:model-value="(value: string) => updateField('legalLastName', value)"
            ></EceTextField>
          </v-col>
        </v-row>
      </div>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
import { defineComponent } from "vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import { useApplicationStore } from "@/store/application";

interface RadioOptions {
  value: any;
  label: string;
}

export default defineComponent({
  name: "EceCertificateInformation",
  components: { EceTextField },
  props: {
    modelValue: {
      type: Object as () => Components.Schemas.CertificateInformation,
      required: true,
    },
  },
  setup() {
    const userStore = useUserStore();
    const applicationStore = useApplicationStore();

    return { userStore, applicationStore };
  },
  data() {
    return {
      Rules,
      previousNameRadio: undefined as string | undefined,
    };
  },
  emits: {
    "update:model-value": (_addressData: Components.Schemas.CertificateInformation) => true,
  },
  computed: {
    applicantNameRadioOptions(): RadioOptions[] {
      let legalNameRadioOptions: RadioOptions[] = [
        {
          label: this.userStore.legalName,
          value: { firstName: this.userStore.firstName || null, middleName: this.userStore.middleName || null, lastName: this.userStore.lastName || null },
        },
      ];
      return [...legalNameRadioOptions, ...this.previousNameRadioOptions];
    },
    previousNameRadioOptions(): RadioOptions[] {
      let radioOptions: RadioOptions[] = this.userStore.verifiedPreviousNames.map((previousName) => {
        let displayLabel = previousName.firstName ?? "";
        if (previousName.middleName) {
          displayLabel += ` ${previousName.middleName}`;
        }
        displayLabel += ` ${previousName.lastName}`;
        return { label: displayLabel, value: { firstName: previousName.firstName, middleName: previousName.middleName, lastName: previousName.lastName } };
      });

      radioOptions.push({ label: "Other name", value: "other" });
      return radioOptions;
    },
  },
  onMounted() {
    if (this.modelValue.hasOtherName) {
      this.previousNameRadio = "other";
    } else {
      // If matching names set option
      const matchingNameOption = this.applicantNameRadioOptions.find((option) => {
        return (
          option.value.firstName === this.modelValue.legalFirstName &&
          option.value.middleName === this.modelValue.legalMiddleName &&
          option.value.lastName === this.modelValue.legalLastName
        );
      });
      this.previousNameRadio = matchingNameOption ? matchingNameOption.value : this.applicantNameRadioOptions[0].value;
    }
  },
  methods: {
    updateField(fieldName: keyof Components.Schemas.CertificateInformation, value: string | boolean) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
    nameRadioChanges(option: any) {
      const newModel = { ...this.modelValue };

      if (option === "other") {
        newModel.legalFirstName = "";
        newModel.legalMiddleName = "";
        newModel.legalLastName = "";
        newModel.hasOtherName = true;
      } else {
        newModel.legalFirstName = option.firstName;
        newModel.legalMiddleName = option.middleName;
        newModel.legalLastName = option.lastName;
        newModel.hasOtherName = false;
      }

      this.$emit("update:model-value", newModel);
    },
  },
});
</script>
