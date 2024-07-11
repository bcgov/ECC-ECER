<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h3>Declaration and consent</h3>
    <p>You must read and agree to the following to apply for certification</p>
    <v-row>
      <v-col cols="12">
        <ul>
          <v-col cols="12">
            <li>I understand that the ECE Registry require additional information (including supporting documents) in connection with this application</li>
          </v-col>
          <v-col cols="12">
            <li>
              I confirm that the information provided in this application is complete and accurate. I understand if inaccurate information is submitted it may
              results in the denial of certification
            </li>
          </v-col>
          <v-col cols="12">
            <li>
              I understand that information in this application or subsequently provided information may be reviewed, audited, and verified for the purpose of
              determining or auditing my eligibility for ECE Certificate in British Columbia
            </li>
          </v-col>
          <v-col cols="12">
            <li>
              I understand that the ECE Registry May take disciplinary actions against ,e. including action to cancel my certification, if I have, by omission
              or commission, knowingly given false ot misleading information in the course of completing this application
            </li>
          </v-col>
          <v-col cols="12">
            <li>
              I consent the work experience and character references indicated i this application to provide the ECE registry with this reference and other
              information as part of my application for certification
            </li>
          </v-col>
        </ul>
      </v-col>
    </v-row>
    <v-form ref="declarationForm">
      <v-row>
        <v-col>
          <EceCheckbox
            :model-value="checkboxValue"
            :props="{ label: 'I understand and agree with the statements above', checkableOnce: true, rules: [Rules.hasCheckbox('required')] }"
            @update:model-value="(value) => (checkboxValue = !!value)"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="4">
          <EceTextField :model-value="name" :props="{ label: 'Applicant Legal Name', disabled: true, rules: [] }"></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <EceTextField :model-value="date" :props="{ label: 'Signed Date', type: 'date', disabled: true, rules: [] }"></EceTextField>
        </v-col>
      </v-row>
    </v-form>
    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick">Continue</v-btn>
  </v-container>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent } from "vue";
import type { VForm } from "vuetify/components";

import { getProfile } from "@/api/profile";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useUserStore } from "@/store/user";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";

import type { ItemsType } from "../Breadcrumb.vue";

export default defineComponent({
  name: "Declaration",
  components: { Breadcrumb, EceCheckbox, EceTextField },
  async setup() {
    const userStore = useUserStore();
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();
    // Refresh userProfile from the server
    const userProfile = await getProfile();
    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }

    const items: ItemsType[] = [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Application Types",
        disabled: false,
        href: "/application/certification",
      },
      {
        title: "Declaration",
        disabled: true,
        href: "/application/declaration",
      },
    ];
    return { items, Rules, userStore, applicationStore, alertStore };
  },
  data() {
    return {
      checkboxValue: false,
      name: "",
      date: "",
    };
  },
  mounted() {
    this.checkboxValue = this.applicationStore.draftApplication.signedDate ? true : false;
    this.name = this.userStore.fullName;
    this.date =
      this.applicationStore.hasDraftApplication && this.applicationStore?.draftApplication?.signedDate
        ? formatDate(this.applicationStore?.draftApplication?.signedDate, "yyyy-MM-dd")
        : formatDate(DateTime.now().toString(), "yyyy-MM-dd");
  },
  methods: {
    async continueClick() {
      const { valid } = await (this.$refs.declarationForm as VForm).validate();
      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
        return;
      }

      this.applicationStore.$patch({ draftApplication: { signedDate: this.date, stage: "ContactInformation" } });

      let response = await this.applicationStore.upsertDraftApplication();

      if (response) {
        this.$router.push("/application");
      }
    },
  },
});
</script>
