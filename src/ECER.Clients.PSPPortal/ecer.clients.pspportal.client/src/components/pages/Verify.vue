<template>
    <div></div>
</template>

<script lang="ts">
import { useOidcStore } from "@/store/oidc";

export default {
    setup() {
        const oidcStore = useOidcStore();

        return { oidcStore };
    },
    mounted() {
        this.handleCallback();
    },
    methods: {
        async handleCallback() {
            await this.oidcStore.silentCallback();
        },
    },
};
</script>
