import EceIcraWorkExperienceContactPreview from "@/components/reference/inputs/icra-eligibility/EceIcraWorkExperienceContactPreview.vue";
import EceWorkExperienceReferenceAssessmentPreview from "@/components/reference/inputs/EceWorkExperienceReferenceAssessmentPreview.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceRecaptcha from "@/components/inputs/EceRecaptcha.vue";
import * as Rules from "@/utils/formRules";
import type { Form } from "@/types/form";

const previewForm: Form = {
  id: "previewForm",
  title: "Review",
  inputs: {

    eceWorkExperienceReferenceAssessmentPreview: {
      id: "eceWorkExperienceReferenceAssessmentPreview",
      component: EceWorkExperienceReferenceAssessmentPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
    },
  },
};

export default previewForm;
