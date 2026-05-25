<template>
  <div v-if="visibleMessages.length" class="d-flex flex-column ga-3 mb-10">
    <Alert
      v-for="(systemMessage, index) in visibleMessages"
      :key="index"
      title="Alert"
    >
      {{ systemMessage.message ?? "" }}
    </Alert>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent } from "vue";
import type { PropType } from "vue";

import Alert from "@/components/Alert.vue";
import { useConfigStore } from "@/store/config";
import type { Components } from "@/types/openapi";

type PageTag = Extract<
  Components.Schemas.PortalTags,
  "LOGIN" | "LOOKUP" | "REFERENCES"
>;

const PORTAL_TAG: Components.Schemas.PortalTags = "CertificationsPortal";

export default defineComponent({
  name: "SystemBanner",
  components: { Alert },
  props: {
    pageTag: {
      type: String as PropType<PageTag>,
      required: true,
    },
  },
  setup(props) {
    const configStore = useConfigStore();

    const visibleMessages = computed(() =>
      configStore.systemMessages.filter(
        (m) =>
          m.portalTags?.includes(PORTAL_TAG) &&
          m.portalTags?.includes(props.pageTag),
      ),
    );

    return { visibleMessages };
  },
});
</script>
