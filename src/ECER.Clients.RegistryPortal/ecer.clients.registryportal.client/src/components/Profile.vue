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
    <v-row>
      <v-col cols="12" sm="6">
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Legal name</p>
            <p>{{ userStore.legalName }}</p>
            <p>Your legal name is printed on all ECE certificates.</p>
          </div>
        </v-col>
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Preferred first name</p>
            <p>{{ userStore.preferredName }}</p>
            <p>We’ll use this in communications:</p>
            <ul class="ml-10">
              <li>We send to you</li>
              <li>We have with your references</li>
            </ul>
          </div>
        </v-col>
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Previous names</p>
            <div v-if="userStore.verifiedPreviousNames.length > 0">
              <p v-for="(prev, index) in userStore.verifiedPreviousNames" :key="index">
                {{ prev.middleName ? `${prev.firstName} ${prev.middleName} ${prev.lastName}` : `${prev.firstName} ${prev.lastName}` }}
              </p>
            </div>
            <p v-else>—</p>
          </div>
        </v-col>
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Date of birth</p>
            <p>{{ formatDate(userStore.userProfile?.dateOfBirth || "", "LLLL d, yyyy") }}</p>
          </div>
        </v-col>
      </v-col>
      <v-col cols="12" sm="6">
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Home address</p>
            <p
              v-if="
                userStore.userProfile?.residentialAddress &&
                userStore.userProfile?.residentialAddress.line1 &&
                userStore.userProfile?.residentialAddress.city &&
                userStore.userProfile?.residentialAddress.province &&
                userStore.userProfile?.residentialAddress.postalCode &&
                userStore.userProfile?.residentialAddress.country
              "
            >
              {{ userStore.userProfile?.residentialAddress.line1 }}
              <br />
              {{ userStore.userProfile?.residentialAddress.city }}, {{ userStore.userProfile?.residentialAddress.province }}
              <br />
              {{ userStore.userProfile?.residentialAddress.postalCode }}
              <br />
              {{ userStore.userProfile?.residentialAddress.country }}
            </p>
            <p v-else>—</p>
          </div>
        </v-col>
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Mailing address</p>
            <p v-if="areObjectsEqual(userStore.userProfile?.residentialAddress, userStore.userProfile?.mailingAddress)">Same as home address</p>
            <p
              v-else-if="
                userStore.userProfile?.mailingAddress &&
                userStore.userProfile?.mailingAddress.line1 &&
                userStore.userProfile?.mailingAddress.city &&
                userStore.userProfile?.mailingAddress.province &&
                userStore.userProfile?.mailingAddress.postalCode &&
                userStore.userProfile?.mailingAddress.country
              "
            >
              {{ userStore.userProfile?.mailingAddress.line1 }}
              <br />
              {{ userStore.userProfile?.mailingAddress.city }}, {{ userStore.userProfile?.mailingAddress.province }}
              <br />
              {{ userStore.userProfile?.mailingAddress.postalCode }}
              <br />
              {{ userStore.userProfile?.mailingAddress.country }}
            </p>
            <p v-else>—</p>
          </div>
        </v-col>
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Email address</p>
            <p>{{ userStore.userProfile?.email ?? "—" }}</p>
          </div>
        </v-col>
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Primary phone number</p>
            <p>{{ userStore.userProfile?.phone ? formatPhoneNumber(userStore.userProfile?.phone ?? "") : "—" }}</p>
          </div>
        </v-col>
        <v-col class="mt-6">
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold mb-3">Alternate phone number</p>
            <p>{{ userStore.userProfile?.alternateContactPhone ? formatPhoneNumber(userStore.userProfile?.alternateContactPhone ?? "") : "—" }}</p>
          </div>
        </v-col>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getProfile } from "@/api/profile";
import PageContainer from "@/components/PageContainer.vue";
import { useUserStore } from "@/store/user";
import { formatDate } from "@/utils/format";
import { formatPhoneNumber } from "@/utils/format";
import { areObjectsEqual } from "@/utils/functions";

export default defineComponent({
  name: "Profile",
  components: { PageContainer },
  setup: async () => {
    const userProfile = await getProfile();
    const userStore = useUserStore();

    return { userProfile, userStore };
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
  methods: {
    formatDate,
    formatPhoneNumber,
    areObjectsEqual,
  },
});
</script>
