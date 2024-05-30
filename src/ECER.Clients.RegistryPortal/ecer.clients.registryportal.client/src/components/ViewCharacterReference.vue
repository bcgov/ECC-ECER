<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>
    <h3 class="mt-10">Character reference</h3>
    <div role="doc-subtitle">We’ve sent an email to the following person to request a reference.</div>
    <p class="mt-8"><b>Name</b></p>
    <p>{{ reference?.firstName }} {{ reference?.lastName }}</p>
    <p class="mt-6"><b>Phone number</b></p>
    <p>{{ formatPhoneNumber(reference?.phoneNumber || "") }}</p>
    <p class="mt-6"><b>Email</b></p>
    <p class="mb-10">{{ reference?.emailAddress }}</p>
    <ECEHeader class="mt-10" title="Options" />
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

import ECEHeader from "@/components/ECEHeader.vue";
import { useApplicationStore } from "@/store/application";
import type { Components } from "@/types/openapi";
import { formatPhoneNumber } from "@/utils/format";

export default defineComponent({
  name: "ViewCharacterReference",
  components: { ECEHeader },
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
