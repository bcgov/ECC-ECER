import type { Meta, StoryObj } from "@storybook/vue3";
import ProgramProfileCard from "../ProgramProfileCard.vue";
import type { Components } from "@/types/openapi";

const meta: Meta<typeof ProgramProfileCard> = {
  title: "Components/ProgramProfileCard",
  component: ProgramProfileCard,
  tags: ["autodocs"],
  argTypes: {
    program: {
      description: "The program object containing profile information",
    },
  },
  decorators: [
    () => ({
      template: '<div style="max-width: 400px; padding: 16px;"><story /></div>',
    }),
  ],
};

export default meta;
type Story = StoryObj<typeof ProgramProfileCard>;

const baseProgram: Components.Schemas.Program = {
  id: "c21d913a-e2ec-f011-8406-7ced8d050c09",
  portalStage: null as unknown as string,
  createdOn: "2026-01-08T22:34:44Z",
  name: "Early Childhood Education Certificate",
  postSecondaryInstituteName: "Selkirk College",
  startDate: "2026-01-08T00:00:00",
  endDate: "2026-01-21T00:00:00",
  programTypes: ["Basic"],
  courses: [],
};

export const Approved: Story = {
  args: {
    program: {
      ...baseProgram,
      status: "Approved",
    },
  },
};

export const UnderReview: Story = {
  args: {
    program: {
      ...baseProgram,
      status: "UnderReview",
    },
  },
};

export const DraftNotActive: Story = {
  name: "Draft (Not Active)",
  args: {
    program: {
      ...baseProgram,
      status: "Draft",
      portalStage: null as unknown as string,
    },
  },
};

export const DraftActiveStep1: Story = {
  name: "Draft (Active - Step 1/5)",
  args: {
    program: {
      ...baseProgram,
      status: "Draft",
      portalStage: "ProgramOverview",
    },
  },
};

export const DraftActiveStep2: Story = {
  name: "Draft (Active - Step 2/5)",
  args: {
    program: {
      ...baseProgram,
      status: "Draft",
      portalStage: "EarlyChildhood",
    },
  },
};

export const DraftActiveStep3: Story = {
  name: "Draft (Active - Step 3/5)",
  args: {
    program: {
      ...baseProgram,
      status: "Draft",
      portalStage: "InfantAndToddler",
    },
  },
};

export const DraftActiveStep4: Story = {
  name: "Draft (Active - Step 4/5)",
  args: {
    program: {
      ...baseProgram,
      status: "Draft",
      portalStage: "SpecialNeeds",
    },
  },
};

export const DraftActiveStep5: Story = {
  name: "Draft (Active - Step 5/5)",
  args: {
    program: {
      ...baseProgram,
      status: "Draft",
      portalStage: "Review",
    },
  },
};

export const Denied: Story = {
  args: {
    program: {
      ...baseProgram,
      status: "Denied",
    },
  },
};

export const Inactive: Story = {
  args: {
    program: {
      ...baseProgram,
      status: "Inactive",
    },
  },
};

export const ChangeRequestInProgress: Story = {
  args: {
    program: {
      ...baseProgram,
      status: "ChangeRequestInProgress",
    },
  },
};

export const MultipleProgramTypes: Story = {
  args: {
    program: {
      ...baseProgram,
      status: "Approved",
      programTypes: ["Basic", "SNE", "ITE"],
    },
  },
};
