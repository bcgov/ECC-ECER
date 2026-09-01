import EceReconsiderationPreview from "@/components/inputs/EceReconsiderationPreview.vue";

import type { Form } from "@/types/form";

const reviewReconsiderationForm: Form = {
  id: "reviewForm",
  title: "Review",
  inputs: {
    reconsiderationPreview: {
      id: "reconsiderationPreview",
      component: EceReconsiderationPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default reviewReconsiderationForm;
