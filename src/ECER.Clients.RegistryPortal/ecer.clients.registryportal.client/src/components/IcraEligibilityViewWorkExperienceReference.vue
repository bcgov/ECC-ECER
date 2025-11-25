<template>
  <Loading v-if="loadingStore.isLoading('icra_status_get')" />
  <v-container v-else>
    <Breadcrumb />

    <h1 class="mb-6">Employment experience reference</h1>

    <p class="mb-8">
      We sent an email to the following person to request a reference.
    </p>

    <v-row class="mb-12">
      <v-col cols="12" md="6">
        <div class="detail-block">
          <strong class="detail-label">Name</strong>
          <div class="detail-value">
            {{ fullName || "Not available" }}
          </div>
        </div>

        <div class="detail-block">
          <strong class="detail-label">Phone number</strong>
          <div class="detail-value">
            {{ phoneNumber || "Not provided" }}
          </div>
        </div>

        <div class="detail-block">
          <strong class="detail-label">Email</strong>
          <div class="detail-value">
            {{ emailAddress || "Not available" }}
          </div>
        </div>
      </v-col>
    </v-row>


    <div class="section-divider mb-4"></div>

    <h2 class="mb-6">Options</h2>

    <section class="mb-10">
      <h3 class="mb-3">Resend email</h3>

      <p class="mb-4">
        We will send another email to the reference. It will include a link to
        the My ECE Registry to provide a reference for you. It may take a few
        minutes to receive the email.
      </p>

      <p class="mb-3">
        If they do not receive the email:
      </p>

      <ul class="mb-4 ml-6 bullet-list">
        <li class="mb-4">Ask the person to check their spam folder</li>
        <li>Check you provided the correct email address</li>
      </ul>

      <p class="mb-6">
        If you need to correct any information, choose to use a new reference
        below.
      </p>

      <v-btn
        variant="outlined"
        color="primary"
        :loading="resending"
        :disabled="resending || !selectedReference"
        @click.stop.prevent="onResendClick"
      >
        Resend email
      </v-btn>
    </section>

    <section class="mb-8">
      <h3 class="mb-3">Change your reference</h3>

      <p class="mb-4">
        This will delete this individual as your reference and let you add
        someone new, or correct any information in the reference listed above.
      </p>

      <a href="#" @click.prevent="onChangeReferenceClick">
        Choose a new reference
      </a>
    </section>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";
import type { Components } from "@/types/openapi";
import { cleanPreferredName } from "@/utils/functions";

import { getIcraEligibilityStatus, resendIcraEligibilityWorkExperienceReferenceInvite } from "@/api/icra";

import Breadcrumb from "@/components/Breadcrumb.vue";
import Loading from "./Loading.vue";

import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "IcraEligibilityViewWorkExperienceReference",
  components: { Breadcrumb, Loading },
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
      icraEligibilityStatus: {} as Components.Schemas.ICRAEligibilityStatus,
      resending: false as boolean,
    };
  },
  async mounted() {
    this.icraEligibilityStatus =
      (await getIcraEligibilityStatus(this.icraEligibilityId))?.data || {};
  },
  computed: {
    employmentReferences():
      Components.Schemas.EmploymentReferenceStatus[] {
      return this.icraEligibilityStatus?.employmentReferencesStatus ?? [];
    },
    selectedReference():
      | Components.Schemas.EmploymentReferenceStatus
      | undefined {
      return this.employmentReferences.find(
        (ref) => ref.id?.toString() === this.referenceId
      );
    },
    fullName(): string {
      if (!this.selectedReference) return "";
      return cleanPreferredName(
        this.selectedReference.firstName,
        this.selectedReference.lastName
      );
    },
    phoneNumber(): string | null | undefined {
      return this.selectedReference?.phoneNumber ?? null;
    },
    emailAddress(): string | null | undefined {
      return this.selectedReference?.emailAddress ?? null;
    },
  },
  methods: {
    cleanPreferredName,
    async onResendClick() {
      if (this.resending) return;
      if (!this.selectedReference) return;

      this.resending = true;
      try {
        await resendIcraEligibilityWorkExperienceReferenceInvite(
          this.icraEligibilityId,
          this.referenceId,
        );
      } finally {
        this.resending = false;
      }
    },
    onChangeReferenceClick() {
      this.router.push({
        name: "icra-eligibility-add-work-experience-reference",
        params: {
          icraEligibilityId: this.icraEligibilityId,
        },
      });
    },
  }
});
</script>

<style scoped>
.mb-2 {
  margin-bottom: 0.5rem;
}
.mb-3 {
  margin-bottom: 0.75rem;
}
.mb-4 {
  margin-bottom: 1rem;
}
.mb-6 {
  margin-bottom: 1.5rem;
}
.mb-8 {
  margin-bottom: 2rem;
}
.mb-10 {
  margin-bottom: 2.5rem;
}
.mb-12 {
  margin-bottom: 3rem;
}
.ml-6 {
  margin-left: 1.5rem;
}

.detail-block {
  margin-bottom: 2rem;
}

.detail-label {
  display: block;
  margin-bottom: 0.4rem;
}

.section-divider {
  width: 40px;
  height: 4px;
  background-color: #f2a900;
}

.bullet-list li {
  line-height: 1.5;
}
</style>
