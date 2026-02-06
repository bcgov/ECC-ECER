import type { Form } from "@/types/form";
import ProgramOverviewStep from "@/components/program-profile/ProgramOverviewStep.vue";

const programOverviewForm: Form = {
  id: "programOverviewForm",
  title: "Program overview",
  components: {
    programOverview: {
      id: "programOverview",
      component: ProgramOverviewStep,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      getValue: (data) => ({
        institutionName: data.draftApplication?.postSecondaryInstituteName,
        startDate: data.draftApplication?.startDate,
        endDate: data.draftApplication?.endDate,
        programTypes: data.draftApplication?.programTypes?.join(", "),
        programName: data.draftApplication?.programName,
      }),
    },
  },
};

export default programOverviewForm;
