<template>
  <v-container>
    <Breadcrumb />
    <h1 class="mb-5">Declaration and consent</h1>
    <p>You must read and agree to the following to apply for certification.</p>
    <br />
    <p class="ml-10 multiline">{{ configStore.defaultContents.find((content) => content.name == "New BC Recognized Application")?.multiText }}</p>

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
import { defineComponent, type PropType } from "vue";
import type { VForm } from "vuetify/components";
import { getProfile } from "@/api/profile";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useLoadingStore } from "@/store/loading";
import { useConfigStore } from "@/store/config";
import { useUserStore } from "@/store/user";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";
import { useRouter } from "vue-router";

export type StreamType = "Eligibility" | "Application";

export default defineComponent({
  name: "Declaration",
  components: { Breadcrumb, EceCheckbox, EceTextField, EceDateInput },
  async setup() {
    const userStore = useUserStore();
    const applicationStore = useApplicationStore();
    const configStore = useConfigStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    // Refresh userProfile from the server
    const userProfile = await getProfile();

    if (userProfile !== null) {
      userStore.setUserProfile(userProfile);
    }

    return { Rules, userStore, applicationStore, alertStore, loadingStore, configStore, router };
  },
  props: {
    stream: {
      type: String as PropType<StreamType>,
      required: false,
      default: "Application",
    },
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

      if (this.stream === "Application") {
        await this.applicationPath();
      } else if (this.stream === "Eligibility") {
        await this.eligibilityPath();
      } else {
        console.warn("unknown stream type");
      }
    },
    async applicationPath() {
      this.applicationStore.$patch({ draftApplication: { signedDate: this.date } });

      let response = await this.applicationStore.upsertDraftApplication();

      if (response) {
        this.router.push("/application");
      }
    },
    async eligibilityPath() {
      console.log("eligibility path");
    },
  },
});
</script>
