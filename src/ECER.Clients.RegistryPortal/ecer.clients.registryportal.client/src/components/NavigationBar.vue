<template>
  <v-app-bar height="80" elevation="0" border absolute>
    <router-link to="/">
      <img src="../assets/bc-gov-logo.png" width="155" class="logo ms-6" alt="B.C. Government Logo" />
    </router-link>
    <v-toolbar-title>My ECE Registry</v-toolbar-title>
    <template v-if="userStore.hasUserInfo">
      <v-menu v-if="$vuetify.display.smAndDown" offset-y bottom transition="slide-y-transition">
        <template #activator="{ props }">
          <v-btn icon v-bind="props">
            <v-icon>mdi-menu</v-icon>
          </v-btn>
        </template>
        <v-list>
          <v-list-item link>
            <v-list-item-title>
              <router-link class="small font-weight-bold" to="/">Home</router-link>
            </v-list-item-title>
          </v-list-item>
          <v-list-item link>
            <v-list-item-title>
              <router-link class="small font-weight-bold" to="/messages">Messages</router-link>
            </v-list-item-title>
          </v-list-item>
          <v-divider />
          <v-list-subheader>{{ userStore.firstName }}</v-list-subheader>
          <v-list-item link>
            <v-list-item-title>
              <router-link class="small font-weight-bold" to="/profile">Edit profile</router-link>
            </v-list-item-title>
          </v-list-item>
          <v-list-item link>
            <v-list-item-title>
              <p class="small font-weight-bold text-decoration-underline text-links" @click="oidcStore.logout">Log out</p>
            </v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>

      <template v-if="$vuetify.display.mdAndUp">
        <v-btn color="primary" prepend-icon="mdi-home" @click="$router.push('/')">Home</v-btn>
        <v-btn color="primary" prepend-icon="mdi-bell" @click="$router.push('/messages')">Messages</v-btn>
        <v-menu offset-y bottom transition="slide-y-transition">
          <template #activator="{ props }">
            <v-btn color="primary" v-bind="props" prepend-icon="mdi-account-circle" append-icon="mdi-chevron-down">{{ userStore.firstName }}</v-btn>
          </template>
          <v-list>
            <v-list-item link>
              <v-list-item-title>
                <router-link class="small font-weight-bold" to="/profile">Edit profile</router-link>
              </v-list-item-title>
            </v-list-item>
            <v-list-item link>
              <v-list-item-title>
                <p class="small font-weight-bold text-decoration-underline text-links" @click="oidcStore.logout">Log out</p>
              </v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </template>
    </template>
  </v-app-bar>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "NavigationBar",
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    return { userStore, oidcStore };
  },
});
</script>
