<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <v-breadcrumbs class="pa-0" :items="items" color="primary">
          <template #divider>/</template>
        </v-breadcrumbs>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Your Profile</h1>
      </v-col>
    </v-row>
    <v-row no-gutters>
      <v-col cols="12" sm="6">
        <div class="d-flex flex-column ga-3">
          <p class="font-weight-bold mb-3">Legal name</p>
          <p>{{ legalName }}</p>
          <p>Your legal name is printed on all ECE certificates.</p>
        </div>
      </v-col>
      <v-col cols="12" sm="6">
        <div class="d-flex flex-column ga-3">
          <p class="font-weight-bold mb-3">Preferred first name</p>
          <p>{{ userProfile?.preferredName }}</p>
          <p>We’ll use this in communications:</p>
          <ul class="ml-10">
            <li>We send to you</li>
            <li>We have with your references</li>
          </ul>
        </div>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getProfile } from "@/api/profile";
import PageContainer from "@/components/PageContainer.vue";

export default defineComponent({
  name: "Profile",
  components: { PageContainer },
  setup: async () => {
    const userProfile = await getProfile();

    return { userProfile };
  },
  data: () => ({
    items: [
      {
        title: "Home",
        disabled: false,
        to: "/",
      },

      {
        title: "Profile",
      },
    ],
  }),
  computed: {
    legalName(): string {
      if (this.userProfile?.middleName) {
        return `${this.userProfile?.firstName} ${this.userProfile?.middleName} ${this.userProfile?.lastName}`;
      } else {
        return `${this.userProfile?.firstName} ${this.userProfile?.lastName}`;
      }
    },
  },
});
</script>
