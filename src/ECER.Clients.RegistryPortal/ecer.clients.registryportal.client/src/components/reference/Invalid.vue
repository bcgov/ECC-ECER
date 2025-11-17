<template>
  <Alert type="error" title="Sorry, this link is no longer valid" :closable="false" :prominent="true" variant="plain" :rounded="false" />
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Alert from "../Alert.vue";

export default defineComponent({
  name: "Invalid",
  components: { Alert },
});
</script>
