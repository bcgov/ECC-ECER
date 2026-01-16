<template>
  <PreviewCard title="Character reference" portal-stage="CharacterReferences">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Last name</p>
        </v-col>
        <v-col>
          <p id="characterReferenceLastName" class="small font-weight-bold">
            {{ characterReference.lastName }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">First name</p>
        </v-col>
        <v-col>
          <p id="characterReferenceFirstName" class="small font-weight-bold">
            {{ characterReference.firstName }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Email</p>
        </v-col>
        <v-col>
          <p id="characterReferenceEmail" class="small font-weight-bold">
            {{ characterReference.emailAddress }}
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Phone number</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">
            {{ characterReference.phoneNumber }}
          </p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "EceCharaterReferencePreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return {
      wizardStore,
    };
  },
  computed: {
    characterReference(): Components.Schemas.CharacterReference {
      return {
        firstName:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.characterReferences?.form
              ?.inputs?.characterReferences?.id || ""
          ]?.[0]?.firstName,
        lastName:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.characterReferences?.form
              ?.inputs?.characterReferences?.id || ""
          ]?.[0]?.lastName,
        emailAddress:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.characterReferences?.form
              ?.inputs?.characterReferences?.id || ""
          ]?.[0]?.emailAddress,
        phoneNumber:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.characterReferences?.form
              ?.inputs?.characterReferences?.id || ""
          ]?.[0]?.phoneNumber,
      };
    },
  },
});
</script>
