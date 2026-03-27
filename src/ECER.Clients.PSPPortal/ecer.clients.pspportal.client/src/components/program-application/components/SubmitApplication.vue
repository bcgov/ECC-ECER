<template>
  <Loading v-if="loadingStore.isLoading('program_application_get')"></Loading>
  <v-container v-else>
    <v-row>
      <v-col cols="12">
        <h1 class="mb-3">Submit application</h1>
        <div class="d-flex flex-column ga-3">
          <p>
            Please read the following and complete the required fields below to
            submit this application to the ECE Registry to review.
          </p>
          <div class="declaration-text ml-10">
            {{ application?.declarationText }}
          </div>
        </div>
      </v-col>
    </v-row>
    <v-form>
      <v-row>
        <v-col cols="12">
          <EceCheckbox
            v-model="declarationChecked"
            :checkable-once="true"
            color="primary"
            label="I understand and agree with the statements above."
            hide-details
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <EceTextField
            id="declarantNameTextField"
            v-model="declarantName"
            label="Your name"
            :readonly="true"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <EceDateInput
            id="declarationDateField"
            :model-value="declarationDate"
            label="Date"
            :readonly="true"
          />
        </v-col>
      </v-row>
    </v-form>
    <v-row no no-gutters class="mt-6">
      <v-btn
        :loading="loadingStore.isLoading('program_application_submit_post')"
        :disabled="!declarationChecked || applicationSubmitted"
        @click="handleSubmit"
        size="large"
        color="primary"
      >
        Submit application
      </v-btn>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import * as Rules from "@/utils/formRules";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import {
  getProgramApplicationById,
  submitProgramApplication,
} from "@/api/program-application.ts";
import type { Components } from "@/types/openapi";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import { useAlertStore } from "@/store/alert";
import { getPspUserProfile } from "@/api/psp-rep";
import Loading from "@/components/Loading.vue";
import { formatDate } from "@/utils/format";
import { DateTime } from "luxon";

export default defineComponent({
  name: "SubmitApplication",
  components: { Loading, EceCheckbox, EceDateInput, EceTextField },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      application: null as Components.Schemas.ProgramApplication | null,
      declarationChecked: false,
      declarationDate: "",
      declarantName: "",
      loadError: false,
      Rules,
    };
  },
  async setup() {
    const userStore = useUserStore();
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();

    const userProfile = await getPspUserProfile();

    if (userProfile !== null) {
      userStore.setPspUserProfile(userProfile);
    }
    return {
      userStore,
      loadingStore,
      alertStore,
    };
  },
  async mounted() {
    await this.fetchApplication();

    if (!this.applicationSubmitted && this.application) {
      this.application.declarationAccepted = false;
      this.application.declarantName = null;
      this.application.declarationDate = null;
    }

    this.declarantName = this.application?.declarantName
      ? this.application?.declarantName
      : `${this.userStore.firstName ?? ""} ${this.userStore.lastName}`.trim();
    this.declarationChecked = this.application?.declarationAccepted || false;
    this.declarationDate = this.application?.declarationDate
      ? formatDate(this.application.declarationDate, "yyyy-MM-dd")
      : formatDate(DateTime.now().toString(), "yyyy-MM-dd");
  },
  computed: {
    applicationSubmitted() {
      if (this.application?.status !== "Draft") {
        if (this.application?.statusReasonDetail !== "RFAIrequested") {
          return true;
        }
      }
      return false;
    },
  },
  methods: {
    async fetchApplication() {
      if (!this.programApplicationId) {
        this.loadError = true;
        return;
      }
      const result = await getProgramApplicationById(this.programApplicationId);
      if (result.error || result.data == null) {
        this.loadError = true;
        this.application = null;
        return;
      }
      this.loadError = false;
      this.application = result.data;
    },
    async handleSubmit() {
      if (!this.declarationChecked) return;

      const result = await submitProgramApplication(this.programApplicationId, {
        declaration: this.declarationChecked,
      });
      if (result.error || result.data == null) {
        if (result.error?.error === "ValidationFailed") {
          this.alertStore.setFailureAlert(
            "All questions in your program application must be completed before submission.",
          );
        } else {
          this.alertStore.setFailureAlert(
            "Failed to submit application. Please try again.",
          );
        }
        return;
      }

      this.$router.push({ name: "application-submitted" });
    },
  },
});
</script>
<style>
.declaration-text {
  white-space: pre-line;
}
</style>
