import type { Meta, StoryObj } from "@storybook/vue3";
import ProgramProfilesList from "./ProgramProfilesList.vue";
import type { Components } from "@/types/openapi";

const meta: Meta<typeof ProgramProfilesList> = {
  title: "Components/ProgramProfilesList",
  component: ProgramProfilesList,
  tags: ["autodocs"],
  argTypes: {
    programs: {
      description: "Array of program objects to display",
    },
  },
};

export default meta;
type Story = StoryObj<typeof ProgramProfilesList>;

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
  status: "Approved",
};

//   This story demonstrates that programs with start dates before 2023 are filtered out.
//   The list includes:
//   - 2 programs from 2022 (should NOT appear)
//   - 1 program from 2021 (should NOT appear)
//   - 2 programs from 2023 (should appear)
//   - 1 program from 2024 (should appear)
//   Expected result: Only 3 programs should be visible (2023 and 2024 programs)
export const FiltersOut2022AndOlderProfiles: Story = {
  args: {
    programs: [
      // These should NOT show up (2022 and earlier)
      {
        ...baseProgram,
        id: "program-2022-a",
        name: "2022 Program A - Should NOT Display",
        startDate: "2022-09-01T00:00:00",
        status: "Approved",
      },
      {
        ...baseProgram,
        id: "program-2022-b",
        name: "2022 Program B - Should NOT Display",
        startDate: "2022-01-15T00:00:00",
        status: "Draft",
      },
      {
        ...baseProgram,
        id: "program-2021",
        name: "2021 Program - Should NOT Display",
        startDate: "2021-06-01T00:00:00",
        status: "Approved",
      },
      // These SHOULD show up (2023 and later)
      {
        ...baseProgram,
        id: "program-2023-a",
        name: "2023 Program A - Should Display",
        startDate: "2023-01-01T00:00:00",
        status: "Approved",
      },
      {
        ...baseProgram,
        id: "program-2023-b",
        name: "2023 Program B - Should Display",
        startDate: "2023-09-15T00:00:00",
        status: "UnderReview",
      },
      {
        ...baseProgram,
        id: "program-2024",
        name: "2024 Program - Should Display",
        startDate: "2024-03-01T00:00:00",
        status: "Approved",
      },
    ],
  },
};
