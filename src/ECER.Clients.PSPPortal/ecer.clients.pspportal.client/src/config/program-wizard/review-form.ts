import type { Form } from "@/types/form";
import EceProgramOverviewPreview from "@/components/inputs/EceProgramOverviewPreview.vue";
import ECETextField from "@/components/inputs/EceTextField.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import EceDeclarationHeader from "@/components/inputs/EceDeclarationHeader.vue";
import EceAreaOfInstructionPreview from "@/components/inputs/EceAreaOfInstructionPreview.vue";

import { DateTime } from "luxon";
import * as Rules from "@/utils/formRules";
import { ProgramType } from "@/utils/constant";

const reviewForm: Form = {
  id: "reviewForm",
  title: "Special Needs Educator",
  components: {
    programOverviewPreview: {
      id: "programOverviewPreview",
      component: EceProgramOverviewPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      isInput: false,
    },
    areaOfInstructionBasicPreview: {
      id: "areaOfInstructionBasicPreview",
      component: EceAreaOfInstructionPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      props: {
        programType: ProgramType.BASIC,
      },
      isInput: false,
    },
    areaOfInstructionITEPreview: {
      id: "areaOfInstructionITEPreview",
      component: EceAreaOfInstructionPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      props: {
        programType: ProgramType.ITE,
      },
      isInput: false,
    },
    areaOfInstructionSNEPreview: {
      id: "areaOfInstructionSNEPreview",
      component: EceAreaOfInstructionPreview,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      props: {
        programType: ProgramType.SNE,
      },
      isInput: false,
    },
    declarationHeader: {
      id: "declarationHeader",
      component: EceDeclarationHeader,
      cols: {
        md: 12,
        lg: 12,
        xl: 12,
      },
      isInput: false,
    },
    declaration: {
      id: "declaration",
      component: EceCheckbox,
      props: {
        label: "I understand and agree with the statements above",
        rules: [Rules.hasCheckbox("required")],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: (data) => data.draftApplication.declaration || false,
    },
    representativeName: {
      id: "representativeName",
      component: ECETextField,
      props: {
        readonly: true,
        label: "Your name",
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: (data) =>
        `${data.userProfile.firstName} ${data.userProfile.lastName}`.trim(),
    },
    signDate: {
      id: "signDate",
      component: EceDateInput,
      props: {
        readonly: true,
        label: "Date",
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
      getValue: () => DateTime.now().toISODate().toString(),
    },
  },
};

export default reviewForm;
