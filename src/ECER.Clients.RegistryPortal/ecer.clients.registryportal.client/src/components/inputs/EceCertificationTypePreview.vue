<template>
  <PreviewCard>
    <div>
      <h3 class="text-lg font-semibold">Certification Selection</h3>
    </div>
    <div>
      <div class="flex flex-col space-y-2">
        <div class="flex items-center space-x-2">
          <span class="font-semibold">Certification Type</span>
          <span>{{ certificationType }}</span>
        </div>
      </div>
    </div>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useCertificationTypeStore } from "@/store/certificationType";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";

export default defineComponent({
  name: "EceCertificationTypePreview",
  components: {
    PreviewCard,
  },
  props: {
    props: {
      type: Object as () => EcePreviewProps,
      required: true,
    },
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const certificationTypeStore = useCertificationTypeStore();

    return {
      wizardStore,
      certificationTypeStore,
    };
  },
  computed: {
    certificationType() {
      let certificationType = "";
      console.log(this.certificationTypeStore.certificationTypes);

      // if (this.certificationTypeStore.certificationTypes.contains("EceAssistant")) {
      //   certificationType = "ECE Assistant";
      // } else if (this.certificationTypeStore.certificationTypes.contains("OneYear")) {
      //   certificationType = "One Year";
      // } else if (this.certificationTypeStore.certificationTypes.contains("FiveYears")) {
      //   certificationType = "Five Year";
      // }
      return certificationType;
    },
  },
});
</script>
