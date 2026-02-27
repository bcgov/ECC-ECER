import type { Meta, StoryObj } from "@storybook/vue3";
import ProgramApplicationCard from "../ProgramApplicationCard.vue";
import type { Components } from "@/types/openapi";

const meta: Meta<typeof ProgramApplicationCard> = {
  title: "Components/ProgramApplicationCard",
  component: ProgramApplicationCard,
  tags: ["autodocs"],
  argTypes: {
    programApplication: {
      description:
        "The program application object containing profile information",
    },
  },
  decorators: [
    () => ({
      template: '<div style="max-width: 400px; padding: 16px;"><story /></div>',
    }),
  ],
};

export default meta;
type Story = StoryObj<typeof ProgramApplicationCard>;

const baseProgram: Components.Schemas.ProgramApplication = {
  id: "c21d913a-e2ec-f011-8406-7ced8d050c09",
  postSecondaryInstituteId: "c21d913a-e2ec-f011-8406-7ced8d050c08",
  programApplicationName: "Bachelor of Education",
  programApplicationType: "NewBasicECEPostBasicProgram",
  programTypes: ["Basic"],
  deliveryType: "Hybrid",
};

export const Initiating: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "Draft",
      componentsGenerationCompleted: false,
    },
  },
};

export const Draft: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "Draft",
      componentsGenerationCompleted: true,
    },
  },
};

export const Submitted: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "Submitted",
    },
  },
};

export const ReviewAndAnalysis: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "ReviewAnalysis",
    },
  },
};

export const ReviewAndAnalysisWithRFAI: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "ReviewAnalysis",
      statusReasonDetail: "RFAIrequested",
    },
  },
};

export const InterimRecognition: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "InterimRecognition",
    },
  },
};

export const InterimRecognitionWithRFAI: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "InterimRecognition",
      statusReasonDetail: "RFAIrequested",
    },
  },
};

export const OnGoingRecognition: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "OnGoingRecognition",
    },
  },
};

export const Approved: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "Approved",
    },
  },
};

export const Archived: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "Archived",
    },
  },
};

export const Withdrawn: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "Withdrawn",
    },
  },
};

export const RefuseToApprove: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "RefusetoApprove",
    },
  },
};

export const Inactive: Story = {
  args: {
    programApplication: {
      ...baseProgram,
      status: "Inactive",
    },
  },
};
