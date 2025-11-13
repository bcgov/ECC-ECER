<template>
    <v-container>
        <div class="d-flex flex-column ga-10 mt-10">
            <h1>Required invitation</h1>
            <h2 class="mt-20">You must have an invitation to successfully authenticate with the ECE Educational
                Institution Portal.</h2>
            <div class="d-flex flex-column ga-3">
                <p>ensure you have an invitation from the ECE Registry.</p>
                <p>Contact the ECE Registry if you require help.</p>
            </div>
            <v-btn class="mt-8 align-self-start" @click="oidcStore.logout" :size="smAndDown ? 'default' : 'large'"
                color="primary">
                Try again
            </v-btn>
        </div>
    </v-container>
</template>

<script lang="ts">
import { useOidcStore } from "@/store/oidc";
import { useDisplay } from "vuetify";
export default {
    name: "RequiredInvitation",
    setup() {
        const oidcStore = useOidcStore();
        const { smAndDown } = useDisplay();
        return { smAndDown, oidcStore };
    },
};
</script>
