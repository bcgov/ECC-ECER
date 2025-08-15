<template>
  <v-container>
    <Breadcrumb />
    <h2 class="mt-10">Work experience reference</h2>
    <div role="doc-subtitle">We’ve sent an email to the following person to request a reference.</div>
    <p class="mt-8"><b>Name</b></p>
    <p>{{ reference?.firstName }} {{ reference?.lastName }}</p>
    <p class="mt-6"><b>Phone number</b></p>
    <p>{{ reference?.phoneNumber }}</p>
    <p class="mt-6"><b>Email</b></p>
    <p>{{ reference?.emailAddress }}</p>
    <p class="mt-6"><b>Work Experience Hours</b></p>
    <p class="mb-10">{{ reference?.hours }}</p>
    <ECEHeader title="Options" />
    <ResendEmail @resend="handleResendReference" />
    <div class="d-flex flex-column ga-3 mt-10">
      <h3 class="mt-4">Change your reference</h3>
      <p>This will delete this individual as your reference and let you add someone new. Or correct any information in the reference listed above.</p>
      <router-link :to="{ name: 'updateWorkExperienceReference', params: { applicationId: applicationId, referenceId: referenceId } }">
        <b>Choose a new reference</b>
      </router-link>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

import { resendWorkExperienceReference } from "@/api/reference";
import ECEHeader from "@/components/ECEHeader.vue";
import ResendEmail from "@/components/ResendEmail.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import type { Components } from "@/types/openapi";
import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
  name: "ViewWorkExperienceReference",
  components: { ECEHeader, ResendEmail, Breadcrumb },

  props: {
    applicationId: {
      type: String,
      required: true,
    },
    referenceId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();
    const router = useRouter();

    // Check store for existing reference
    const reference: Components.Schemas.WorkExperienceReference | undefined = applicationStore.workExperienceReferenceById(props.referenceId);

    if (!reference) {
      router.back();
    }

    return { applicationStore, reference, alertStore };
  },

  methods: {
    async handleResendReference() {
      const { error } = await resendWorkExperienceReference({
        applicationId: this.applicationId,
        referenceId: this.referenceId,
      });

      if (error) {
        this.alertStore.setFailureAlert("Failed to resend email");
      } else {
        this.alertStore.setSuccessAlert("Email resent to this reference with link to respond");
      }
    },
  },
});
</script>
