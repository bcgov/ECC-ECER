<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h1 class="mb-5">Declaration and consent</h1>
    <p>You must read and agree to the following to apply for certification.</p>
    <br />
    <ul class="ml-10">
      <li>I understand that the ECE Registry may require additional information (including supporting documents) in connection with this application.</li>
      <li>
        I confirm that the information provided in this application is complete and accurate. I understand if inaccurate information is submitted it may result
        in the denial of certification.
      </li>
      <li>
        I understand that information in this application or subsequently provided information may be reviewed, audited, and verified for the purpose of
        determining or auditing my eligibility for an ECE Certificate in British Columbia.
      </li>
      <li>
        I understand that the ECE Registry may take disciplinary actions against me, including action to cancel my certification, if I have, by omission or
        commission, knowingly given false or misleading information in the course of completing this application.
      </li>
      <li>
        I consent the work experience and character references indicated in this application to provide the ECE Registry with this reference and other
        information as part of my application for certification.
      </li>
    </ul>

    <v-form ref="declarationForm">
      <v-row>
        <v-col>
          <EceCheckbox
            id="chkDeclaration"
            :model-value="checkboxValue"
            label="I understand and agree with the statements above"
            :checkableOnce="true"
            :rules="[Rules.hasCheckbox('required')]"
            @update:model-value="(value) => (checkboxValue = !!value)"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="4">
          <EceTextField :model-value="name" label="Your full legal name" :readonly="true" :rules="[]"></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <EceDateInput :model-value="date" label="Signed date" :readonly="true" :rules="[]"></EceDateInput>
        </v-col>
      </v-row>
    </v-form>
    <v-btn class="mt-6" rounded="lg" color="primary" :loading="loadingStore.isLoading('draftapplication_put')" @click="continueClick" id="btnContinue">
      Continue
    </v-btn>
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
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "Declaration",
  components: { Breadcrumb, EceCheckbox, EceTextField, EceDateInput },
  async setup() {
    const userStore = useUserStore();
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    // Refresh userProfile from the server
    const userProfile = await getProfile();

    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }

    const items =
      applicationStore.isDraftApplicationRenewal || userStore.isRegistrant
        ? [
            {
              title: "Home",
              disabled: false,
              href: "/",
            },
            {
              title: "Requirements",
              disabled: false,
              href: "/application/certification/requirements",
            },
            {
              title: "Declaration",
              disabled: true,
              href: "/application/declaration",
            },
          ]
        : [
            {
              title: "Home",
              disabled: false,
              href: "/",
            },
            {
              title: "Application types",
              disabled: false,
              href: "/application/certification",
            },
            {
              title: "Requirements",
              disabled: false,
              href: "/application/certification/requirements",
            },
            {
              title: "Declaration",
              disabled: true,
              href: "/application/declaration",
            },
          ];

    return { items, Rules, userStore, applicationStore, alertStore, loadingStore, router };
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
        this.router.push("/application");
      }
    },
  },
});
</script>
