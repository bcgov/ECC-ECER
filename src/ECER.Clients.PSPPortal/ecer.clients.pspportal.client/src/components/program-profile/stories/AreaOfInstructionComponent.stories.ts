import type { Meta, StoryObj } from "@storybook/vue3-vite";
import { useConfigStore } from "@/store/config";
import type { Components } from "@/types/openapi";
import AreaOfInstructionComponent from "../AreaOfInstructionComponent.vue";

// Mock area of instruction data
const mockAreaOfInstructions: Components.Schemas.AreaOfInstruction[] = [
  {
    id: "area-6",
    name: "Practicum",
    minimumHours: 120,
    programTypes: ["SNE", "ITE"],
  },
  {
    id: "area-1",
    name: "Child Development and Learning",
    minimumHours: 45,
    programTypes: ["Basic", "SNE", "ITE"],
  },
  {
    id: "area-2",
    name: "Health, Safety and Nutrition",
    minimumHours: 30,
    programTypes: ["Basic", "SNE", "ITE"],
  },
  {
    id: "area-3",
    name: "Observation and Assessment",
    minimumHours: 20,
    programTypes: ["Basic", "SNE"],
  },
  {
    id: "area-4",
    name: "Program Development, Curriculum and Foundations",
    minimumHours: 40,
    programTypes: ["Basic", "SNE", "ITE"],
  },
  {
    id: "area-5",
    name: "Child Guidance",
    minimumHours: 0,
    programTypes: ["Basic", "SNE", "ITE"],
  },
];

// Mock program data
const mockProgram: Components.Schemas.Program = {
  id: "program-1",
  portalStage: "Draft",
  status: "Draft",
  name: "Early Childhood Education Program",
  postSecondaryInstituteName: "Test Institute",
  programTypes: ["Basic", "SNE", "ITE"],
  courses: [
    // Courses with allocated hours for area-1
    {
      courseNumber: "ECE 101",
      courseTitle: "Introduction to Child Development",
      programType: "Basic",
      courseAreaOfInstruction: [
        {
          courseAreaOfInstructionId: "course-area-1",
          newHours: "15.00",
          areaOfInstructionId: "area-1",
        },
        {
          courseAreaOfInstructionId: "course-area-2",
          newHours: "10.00",
          areaOfInstructionId: "area-2",
        },
      ],
    },
    {
      courseNumber: "ECE 102",
      courseTitle: "Early Learning Theories",
      programType: "Basic",
      courseAreaOfInstruction: [
        {
          courseAreaOfInstructionId: "course-area-3",
          newHours: "20.00",
          areaOfInstructionId: "area-1",
        },
      ],
    },
    // Course with allocated hours for area-2
    {
      courseNumber: "HSN 101",
      courseTitle: "Nutrition Basics",
      programType: "Basic",
      courseAreaOfInstruction: [
        {
          courseAreaOfInstructionId: "course-area-4",
          newHours: "15.00",
          areaOfInstructionId: "area-2",
        },
      ],
    },
    // Non-allocated course (no courseAreaOfInstruction)
    {
      courseNumber: "ECE 200",
      courseTitle: "Professional Development",
      programType: "Basic",
      courseAreaOfInstruction: null,
    },
    // Non-allocated course (empty courseAreaOfInstruction)
    {
      courseNumber: "ECE 250",
      courseTitle: "Child Care Administration",
      programType: "Basic",
      courseAreaOfInstruction: [],
    },
    // Non-allocated course (courseAreaOfInstruction with no valid areaId or hours)
    {
      courseNumber: "ECE 300",
      courseTitle: "Special Topics",
      programType: "Basic",
      courseAreaOfInstruction: [
        {
          courseAreaOfInstructionId: "course-area-5",
          newHours: "0",
          areaOfInstructionId: null,
        },
      ],
    },
  ],
};

// Decorator that initializes the store before rendering
const withMockConfigStore = (storyFn: any, context: any) => {
  // Initialize store with mock data
  const configStore = useConfigStore();
  configStore.areaOfInstructionList = mockAreaOfInstructions;

  return storyFn(context);
};

const meta = {
  title: "Components/AreaOfInstructionComponent",
  component: AreaOfInstructionComponent,
  tags: ["autodocs"],
  decorators: [withMockConfigStore],
  argTypes: {
    programType: {
      control: "select",
      options: ["Basic", "SNE", "ITE"],
      description:
        "The program type to filter areas of instruction and courses",
    },
    program: {
      control: "object",
      description:
        "The program object containing courses and program information",
    },
    areaSubtitles: {
      control: "object",
      description:
        "Object mapping areaOfInstructionId to custom subtitle strings",
    },
    includeTotalHours: {
      control: "boolean",
      description: "Whether to show the total hours of instruction card",
    },
    onEdit: {
      action: "edit",
      description:
        "Emitted when edit button is clicked on a course or course area",
    },
  },
  args: {
    programType: "Basic" as Components.Schemas.ProgramTypes,
    program: mockProgram,
    areaSubtitles: {},
    includeTotalHours: false,
  },
  parameters: {
    docs: {
      description: {
        component:
          "A component that displays area of instruction cards for a given program type, filtering areas and courses accordingly. Also displays non-allocated courses in a separate card.",
      },
    },
  },
} satisfies Meta<typeof AreaOfInstructionComponent>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    programType: "Basic",
    program: mockProgram,
    areaSubtitles: {},
    includeTotalHours: false,
  },
  render: (args) => ({
    components: { AreaOfInstructionComponent },
    setup() {
      return { args };
    },
    template: `
      <AreaOfInstructionComponent v-bind="args">
        <template #description>
          <p class="text-grey-darken-1">
            The courses included in your program are shown here grouped by areas of instruction.
          </p>
          <p class="text-grey-darken-1">
            Edit any courses as required to ensure that this program profile reflects the course information.
          </p>
        </template>
      </AreaOfInstructionComponent>
    `,
  }),
};

export const WithTotalHours: Story = {
  args: {
    programType: "Basic",
    program: mockProgram,
    areaSubtitles: {},
    includeTotalHours: true,
  },
  render: (args) => ({
    components: { AreaOfInstructionComponent },
    setup() {
      return { args };
    },
    template: `
      <AreaOfInstructionComponent v-bind="args">
        <template #description>
          <p class="text-grey-darken-1">
            Each area of instruction requires a minimum number of hours. Courses must be allocated to these areas to meet the program requirements.
          </p>
        </template>
      </AreaOfInstructionComponent>
    `,
  }),
};

export const WithEmptyArea: Story = {
  args: {
    programType: "Basic",
    program: {
      ...mockProgram,
      courses: [
        // Only courses for area-1, so area-2 and area-3 should show with no courses
        {
          courseNumber: "ECE 101",
          courseTitle: "Introduction to Child Development",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-1",
              newHours: "15.00",
              areaOfInstructionId: "area-1",
            },
          ],
        },
      ],
    },
    areaSubtitles: {},
    includeTotalHours: false,
  },
  render: (args) => ({
    components: { AreaOfInstructionComponent },
    setup() {
      return { args };
    },
    template: `
      <AreaOfInstructionComponent v-bind="args">
        <template #description>
          <p class="text-grey-darken-1">
            This example shows areas with no courses allocated. Notice that area-2 and area-3 still display their titles and progress bars.
          </p>
        </template>
      </AreaOfInstructionComponent>
    `,
  }),
};

export const WithChildGuidanceCombined: Story = {
  args: {
    programType: "Basic",
    program: {
      ...mockProgram,
      courses: [
        // Course for Program Development, Curriculum and Foundations
        {
          courseNumber: "ECE 301",
          courseTitle: "Curriculum Development",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-6",
              newHours: "20.00",
              areaOfInstructionId: "area-4",
            },
          ],
        },
        {
          courseNumber: "ECE 302",
          courseTitle: "Program Planning",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-7",
              newHours: "15.00",
              areaOfInstructionId: "area-4",
            },
          ],
        },
        // Course for Child Guidance
        {
          courseNumber: "ECE 303",
          courseTitle: "Positive Guidance Strategies",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-8",
              newHours: "10.00",
              areaOfInstructionId: "area-5",
            },
          ],
        },
        {
          courseNumber: "ECE 304",
          courseTitle: "Behavior Management",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-9",
              newHours: "8.00",
              areaOfInstructionId: "area-5",
            },
          ],
        },
      ],
    },
    areaSubtitles: {
      "area-5":
        "Child guidance is included in Program Development, Curriculum and Foundations. There is no set minimum required hours specifically for Child Guidance.",
    },
    includeTotalHours: false,
  },
  render: (args) => ({
    components: { AreaOfInstructionComponent },
    setup() {
      return { args };
    },
    template: `
      <AreaOfInstructionComponent v-bind="args">
        <template #description>
          <p class="text-grey-darken-1">
            This example demonstrates that Child Guidance courses are combined with Program Development, Curriculum and Foundations in a single card. Notice that Child Guidance appears at the bottom of the combined card and does not show as a separate card.
          </p>
        </template>
      </AreaOfInstructionComponent>
    `,
  }),
};

export const BasicWithChildGuidanceSubtitle: Story = {
  args: {
    programType: "Basic",
    program: {
      ...mockProgram,
      courses: [
        {
          courseNumber: "ECE 101",
          courseTitle: "Introduction to Child Development",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-1",
              newHours: "15.00",
              areaOfInstructionId: "area-1",
            },
          ],
        },
        {
          courseNumber: "ECE 301",
          courseTitle: "Curriculum Development",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-6",
              newHours: "20.00",
              areaOfInstructionId: "area-4",
            },
          ],
        },
        {
          courseNumber: "ECE 303",
          courseTitle: "Positive Guidance Strategies",
          programType: "Basic",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-8",
              newHours: "10.00",
              areaOfInstructionId: "area-5",
            },
          ],
        },
        // Non-allocated courses
        {
          courseNumber: "ECE 200",
          courseTitle: "Professional Development",
          programType: "Basic",
          courseAreaOfInstruction: null,
        },
        {
          courseNumber: "ECE 250",
          courseTitle: "Child Care Administration",
          programType: "Basic",
          courseAreaOfInstruction: [],
        },
      ],
    },
    areaSubtitles: {
      "area-5":
        "Child guidance is included in Program Development, Curriculum and Foundations. There is no set minimum required hours specifically for Child Guidance.",
    },
    includeTotalHours: false,
  },
  render: (args) => ({
    components: { AreaOfInstructionComponent },
    setup() {
      return { args };
    },
    template: `
      <AreaOfInstructionComponent v-bind="args">
        <template #description>
          <p class="text-grey-darken-1">
            Basic program type showing Program Development, Curriculum and Foundations combined with Child Guidance. The custom subtitle for Child Guidance is displayed.
          </p>
        </template>
      </AreaOfInstructionComponent>
    `,
  }),
};

export const SNEWithTotalHoursAndPracticumSubtitle: Story = {
  args: {
    programType: "SNE",
    program: {
      ...mockProgram,
      courses: [
        {
          courseNumber: "ECE 101",
          courseTitle: "Introduction to Child Development",
          programType: "SNE",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-1",
              newHours: "15.00",
              areaOfInstructionId: "area-1",
            },
          ],
        },
        {
          courseNumber: "ECE 102",
          courseTitle: "Early Learning Theories",
          programType: "SNE",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-3",
              newHours: "20.00",
              areaOfInstructionId: "area-1",
            },
          ],
        },
        {
          courseNumber: "HSN 101",
          courseTitle: "Nutrition Basics",
          programType: "SNE",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-4",
              newHours: "15.00",
              areaOfInstructionId: "area-2",
            },
          ],
        },
        {
          courseNumber: "OBS 101",
          courseTitle: "Observation Techniques",
          programType: "SNE",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-10",
              newHours: "10.00",
              areaOfInstructionId: "area-3",
            },
          ],
        },
        {
          courseNumber: "PRAC 101",
          courseTitle: "Practicum I",
          programType: "SNE",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-11",
              newHours: "60.00",
              areaOfInstructionId: "area-6",
            },
          ],
        },
        {
          courseNumber: "PRAC 102",
          courseTitle: "Practicum II",
          programType: "SNE",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-12",
              newHours: "50.00",
              areaOfInstructionId: "area-6",
            },
          ],
        },
        // Non-allocated courses
        {
          courseNumber: "SNE 200",
          courseTitle: "Special Needs Administration",
          programType: "SNE",
          courseAreaOfInstruction: null,
        },
        {
          courseNumber: "SNE 250",
          courseTitle: "Advanced Special Education Topics",
          programType: "SNE",
          courseAreaOfInstruction: [],
        },
        {
          courseNumber: "SNE 300",
          courseTitle: "Independent Study",
          programType: "SNE",
          courseAreaOfInstruction: [
            {
              courseAreaOfInstructionId: "course-area-13",
              newHours: "0",
              areaOfInstructionId: null,
            },
          ],
        },
      ],
    },
    areaSubtitles: {
      "area-1": "",
      "area-2": "",
      "area-3": "",
      "area-4": "",
      "area-5": "",
      "area-6":
        "Practicum hours are required for SNE program completion. A minimum of 120 hours is required.",
    },
    includeTotalHours: true,
  },
  render: (args) => ({
    components: { AreaOfInstructionComponent },
    setup() {
      return { args };
    },
    template: `
      <AreaOfInstructionComponent v-bind="args">
        <template #description>
          <p class="text-grey-darken-1">
            SNE program type with total hours displayed. Practicum is the first area of instruction and has a progress bar (first area always shows progress bar). Only Practicum has a custom subtitle; all other areas have no subtitles. Since this is SNE (not Basic), only the first area shows a progress bar.
          </p>
        </template>
      </AreaOfInstructionComponent>
    `,
  }),
};
