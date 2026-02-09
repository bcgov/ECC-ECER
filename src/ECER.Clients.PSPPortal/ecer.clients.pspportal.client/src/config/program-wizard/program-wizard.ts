import type { Wizard } from "@/types/wizard";
import programOverviewForm from "@/config/program-wizard/program-overview-form.ts";
import earlyChildhoodForm from "@/config/program-wizard/early-childhood-form.ts";
import infantAndToddlerForm from "@/config/program-wizard/infant-and-toddler-form.ts";
import specialNeedsForm from "@/config/program-wizard/special-needs-form.ts";
import reviewForm from "@/config/program-wizard/review-form.ts";

const programWizard: Wizard = {
  id: "form-1",
  steps: {
    programOverview: {
      stage: "ProgramOverview",
      title: "Program Overview",
      form: programOverviewForm,
      key: "item.1",
    },
    earlyChildhood: {
      stage: "EarlyChildhood",
      title: "Basic Early Childhood Educator",
      form: earlyChildhoodForm,
      key: "item.2",
    },
    infantAndToddler: {
      stage: "InfantAndToddler",
      title: "Infant and Toddler Educator",
      form: infantAndToddlerForm,
      key: "item.3",
    },
    specialNeeds: {
      stage: "SpecialNeeds",
      title: "Special Needs Educator",
      form: specialNeedsForm,
      key: "item.4",
    },
    review: {
      stage: "Review",
      title: "Confirm and submit",
      form: reviewForm,
      key: "item.5",
    },
  },
};

export default programWizard;
