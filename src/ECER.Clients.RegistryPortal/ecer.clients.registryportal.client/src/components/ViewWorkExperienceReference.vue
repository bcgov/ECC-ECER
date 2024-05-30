<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>

    <h3>Work expereince reference</h3>
    <div role="doc-subtitle">We’ve sent an email to the following person to request a reference.</div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "ViewWorkExperienceReference",
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

    console.log(props.applicationId, props.referenceId);

    // Check store for work experience reference
    const workExperienceReference = applicationStore.workExperienceReferenceById(props.referenceId);

    console.log(workExperienceReference);

    if (!workExperienceReference) {
      //   router.back();
    }

    return { applicationStore, workExperienceReference };
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
          title: "Work experience reference",
          disabled: true,
          href: `/manage-application/${this.applicationId}/work-experience-reference/${this.workExperienceReferenceId}`,
        },
      ],
    };
  },
});
</script>
