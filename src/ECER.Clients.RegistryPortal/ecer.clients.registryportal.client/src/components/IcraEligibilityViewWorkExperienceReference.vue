<template>
  <Loading v-if="loadingStore.isLoading('icra_work_reference_by_id_get')" />
  <v-container v-else>
    <Breadcrumb />

    <h1 class="mb-4">Employment experience reference</h1>

    <p class="mb-8">We sent an email to the following person to request a reference.</p>

    <v-row class="mb-12">
      <v-col cols="12" md="6">
        <div class="mb-6">
          <strong class="d-block mb-1">Name</strong>
          <div>
            {{ fullName || "Not available" }}
          </div>
        </div>

        <div class="mb-6">
          <strong class="d-block mb-1">Phone number</strong>
          <div>
            {{ employmentReference?.phoneNumber || "Not provided" }}
          </div>
        </div>

        <div class="mb-6">
          <strong class="d-block mb-1">Email</strong>
          <div>
            {{ employmentReference?.emailAddress || "Not available" }}
          </div>
        </div>
      </v-col>
    </v-row>

    <div class="mb-6">
      <ECEHeader class="mb-6" title="Options" />
    </div>

    <section class="mb-10">
      <h3 class="mb-3">Resend email</h3>

      <p class="mb-4">
        We will send another email to the reference. It will include a link to the My ECE Registry to provide a reference for you. It may take a few minutes to
        receive the email.
      </p>

      <p class="mb-3">If they do not receive the email:</p>

      <ul class="ml-6 mb-4">
        <li class="mb-2">Ask the person to check their spam folder</li>
        <li class="mb-2">Check you provided the correct email address</li>
      </ul>

      <p class="mb-6">If you need to correct any information, choose to use a new reference below.</p>

      <v-btn
        variant="outlined"
        color="primary"
        :loading="loadingStore.isLoading('icra_work_reference_resend_invite_post')"
        :disabled="!employmentReference || loadingStore.isLoading('icra_work_reference_resend_invite_post')"
        @click.stop.prevent="onResendClick"
      >
        Resend email
      </v-btn>
    </section>

    <section class="mb-8">
      <h3 class="mb-3">Change your reference</h3>

      <p class="mb-4">
        This will delete this individual as your reference and let you add someone new, or correct any information in the reference listed above.
      </p>

      <a href="#" @click.prevent="onChangeReferenceClick">Choose a new reference</a>
    </section>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";
import type { Components } from "@/types/openapi";
import { cleanPreferredName } from "@/utils/functions";

import { getIcraWorkExperienceReferenceById, resendIcraEligibilityWorkExperienceReferenceInvite } from "@/api/icra";

import Breadcrumb from "@/components/Breadcrumb.vue";
import Loading from "./Loading.vue";
import ECEHeader from "@/components/ECEHeader.vue";

import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "IcraEligibilityViewWorkExperienceReference",
  components: { Breadcrumb, Loading, ECEHeader },
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
    referenceId: {
      type: String,
      required: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    return {
      smAndUp,
      router,
      loadingStore,
    };
  },
  data() {
    return {
      employmentReference: null as Components.Schemas.EmploymentReference | null,
    };
  },
  async mounted() {
    const response = await getIcraWorkExperienceReferenceById(this.referenceId);
    this.employmentReference = response.data || null;
  },
  computed: {
    fullName(): string {
      if (!this.employmentReference) return "";
      return cleanPreferredName(this.employmentReference.firstName, this.employmentReference.lastName);
    },
  },
  methods: {
    cleanPreferredName,
    async onResendClick() {
      if (!this.employmentReference) return;

      await resendIcraEligibilityWorkExperienceReferenceInvite(this.icraEligibilityId, this.referenceId);
    },
    onChangeReferenceClick() {
      this.router.push({
        name: "icra-eligibility-replace-work-experience-reference",
        params: {
          icraEligibilityId: this.icraEligibilityId,
          referenceId: this.referenceId,
        },
      });
    },
  },
});
</script>
