<template>
  <v-app-bar height="80" elevation="0" border>
    <v-container>
      <v-row align="center">
        <router-link to="/">
          <img src="../assets/bc-gov-logo.png" width="155" class="logo" alt="B.C. Government Logo" />
        </router-link>
        <v-toolbar-title>ECE Post-Secondary Programs Portal</v-toolbar-title>
        <template v-if="userStore.hasUserProfile && userStore.hasAcceptedTermsOfUse">
          <v-menu v-if="$vuetify.display.smAndDown" offset-y bottom transition="slide-y-transition">
            <template #activator="{ props }">
              <v-btn id="btnToggleMenu" icon v-bind="props">
                <v-icon>mdi-menu</v-icon>
              </v-btn>
            </template>
            <v-list>
              <v-list-item link>
                <v-list-item-title>
                  <router-link class="small" to="/">Home</router-link>
                </v-list-item-title>
              </v-list-item>
              <v-list-item link>
                <v-list-item-title>
                  <router-link class="small" to="/messages">Messages</router-link>
                </v-list-item-title>
              </v-list-item>
              <v-divider />
              <v-list-subheader>{{ userStore.firstName }}</v-list-subheader>
              <v-list-item link>
                <v-list-item-title>
                  <router-link class="small" to="/profile/edit">My profile</router-link>
                </v-list-item-title>
              </v-list-item>
              <v-list-item link>
                <v-list-item-title>
                  <p class="small text-decoration-underline text-links" @click="oidcStore.logout">Log out</p>
                </v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>

          <template v-if="$vuetify.display.mdAndUp">
            <v-btn class="font-weight-regular" color="primary" prepend-icon="mdi-home" @click="router.push('/')">Home</v-btn>
            <v-btn class="font-weight-regular" color="primary" prepend-icon="mdi-bell" @click="router.push('/messages')">Messages</v-btn>
            <v-menu offset-y bottom transition="slide-y-transition">
              <template #activator="{ props }">
                <v-btn
                  id="btnUserName"
                  class="font-weight-regular"
                  color="primary"
                  v-bind="props"
                  prepend-icon="mdi-account-circle"
                  append-icon="mdi-chevron-down"
                >
                  {{ userStore.preferredName ? userStore.preferredName : cleanPreferredName(userStore.firstName, userStore.lastName, "first") }}
                </v-btn>
              </template>
              <v-list>
                <v-list-item link>
                  <v-list-item-title>
                    <router-link class="small" to="/profile/edit">My profile</router-link>
                  </v-list-item-title>
                </v-list-item>
                <v-list-item link>
                  <v-list-item-title>
                    <p id="lnkLogOut" class="small text-decoration-underline text-links" @click="oidcStore.logout">Log out</p>
                  </v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
          </template>
        </template>
      </v-row>
    </v-container>
  </v-app-bar>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { cleanPreferredName } from "@/utils/functions";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "NavigationBar",
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const router = useRouter();
    return { userStore, oidcStore, router, cleanPreferredName };
  },
});
</script>
