import type { Wizard } from "@/types/wizard";

import disputeForm from "./reconsideration-form";
import reviewReconsiderationForm from "./review-reconsideration-form";

const disputeWizard: Wizard = {
  id: "form-1",
  steps: {
    reconsideration: {
      stage: "Reconsideration",
      title: "Dispute details",
      form: disputeForm,
      key: "item.1",
    },
    review: {
      stage: "Review",
      title: "Review responses",
      form: reviewReconsiderationForm,
      key: "item.2",
    },
  },
};

export default disputeWizard;
