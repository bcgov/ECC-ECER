import EceDisplayValue from "@/components/inputs/EceDisplayValue.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as rules from "@/utils/formRules";

const programOverviewForm: Form = {
  id: "programOverviewForm",
  title: "Program Overview",
  components: {
    program: {
      id: "institutionName",
      component: EceDisplayValue,
      props: {
        label: "Institution name",
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: (data) => data.draftApplication.postSecondaryInstituteName,
    },
    startDate: {
      id: "startDate",
      component: EceDisplayValue,
      props: {
        label: "Start date",
        isDate: true,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: (data) => data.draftApplication.startDate,
    },
    endDate: {
      id: "endDate",
      component: EceDisplayValue,
      props: {
        label: "End date",
        isDate: true,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: (data) => data.draftApplication.endDate,
    },
    programTypes: {
      id: "programTypes",
      component: EceDisplayValue,
      props: {
        label: "Program types",
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: (data) => data.draftApplication.programTypes?.join(", "),
    },
    programName: {
      id: "programName",
      component: EceTextField,
      props: {
        label: "Program name",
        rules: [rules.required("Required")],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: (data) => data.draftApplication.programName,
    },
  },
};

export default programOverviewForm;
