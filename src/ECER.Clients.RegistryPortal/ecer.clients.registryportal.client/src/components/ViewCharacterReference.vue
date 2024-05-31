<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>
    <h2 class="mt-10">Character reference</h2>
    <div role="doc-subtitle">We’ve sent an email to the following person to request a reference.</div>
    <p class="mt-8"><b>Name</b></p>
    <p>{{ reference?.firstName }} {{ reference?.lastName }}</p>
    <p class="mt-6"><b>Phone number</b></p>
    <p>{{ formatPhoneNumber(reference?.phoneNumber || "") }}</p>
    <p class="mt-6"><b>Email</b></p>
    <p class="mb-10">{{ reference?.emailAddress }}</p>
    <ECEHeader class="mt-10" title="Options" />
    <ResendEmail />
    <div class="d-flex flex-column ga-3 mt-10">
      <h3 class="mt-4">Change your reference</h3>
      <p>This will delete this individual as your reference and let you add someone new. Or correct any information in the reference listed above.</p>
      <router-link to="/manage-application/:applicationId/character-reference/:referenceId/edit"><b>Choose a new reference</b></router-link>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

import ECEHeader from "@/components/ECEHeader.vue";
import ResendEmail from "@/components/ResendEmail.vue";
import { useApplicationStore } from "@/store/application";
import type { Components } from "@/types/openapi";
import { formatPhoneNumber } from "@/utils/format";

export default defineComponent({
  name: "ViewCharacterReference",
  components: { ECEHeader, ResendEmail },
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
    const router = useRouter();

    // Check store for existing reference
    const reference: Components.Schemas.CharacterReference | undefined = applicationStore.characterReferenceById(props.referenceId);

    if (!reference) {
      router.back();
    }

    return { applicationStore, reference };
  },
  data() {
    return {
      items: [
        {
          title: "Home",
          disabled: false,
          href: "/",
        },
        {
          title: "Application",
          disabled: false,
          href: `/manage-application/${this.applicationId}`,
        },
        {
          title: "Character reference",
          disabled: true,
          href: `/manage-application/${this.applicationId}/character-reference/${this.referenceId}`,
        },
      ],
    };
  },

  methods: {
    formatPhoneNumber,
  },
});
</script>
