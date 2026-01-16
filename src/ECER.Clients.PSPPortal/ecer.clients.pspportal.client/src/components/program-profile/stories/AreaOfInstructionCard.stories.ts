import type { Meta, StoryObj } from "@storybook/vue3-vite";
import { useConfigStore } from "@/store/config";
import type { Components } from "@/types/openapi";
import AreaOfInstructionCard from "../AreaOfInstructionCard.vue";

interface CourseAreaOfInstructionWithCourse
  extends Components.Schemas.CourseAreaOfInstruction {
  courseTitle?: string | null;
  courseNumber?: string | null;
}

// Mock area of instruction data
const mockAreaOfInstructions: Components.Schemas.AreaOfInstruction[] = [
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
];

// Decorator that initializes the store before rendering
const withMockConfigStore = (storyFn: any, context: any) => {
  // Initialize store with mock data
  // This runs in the component context, so Pinia should be available
  const configStore = useConfigStore();
  configStore.areaOfInstructionList = mockAreaOfInstructions;

  return storyFn(context);
};

const meta = {
  title: "Components/AreaOfInstructionCard",
  component: AreaOfInstructionCard,
  tags: ["autodocs"],
  decorators: [withMockConfigStore],
  argTypes: {
    courseAreaOfInstructions: {
      control: "object",
      description:
        "List of CourseAreaOfInstruction items with course information, grouped by areaOfInstructionId",
    },
    areaSubtitles: {
      control: "object",
      description:
        "Object mapping areaOfInstructionId to custom subtitle strings",
    },
    showProgressBar: {
      control: "boolean",
      description: "Whether to show the progress bar for the first area",
    },
    onEdit: {
      action: "edit",
      description: "Emitted when edit button is clicked",
    },
  },
  args: {
    courseAreaOfInstructions: [
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "15.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Introduction to Child Development",
        courseNumber: "ECE 101",
      },
      {
        courseAreaOfInstructionId: "course-area-2",
        newHours: "20.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Early Learning Theories",
        courseNumber: "ECE 102",
      },
      {
        courseAreaOfInstructionId: "course-area-3",
        newHours: "10.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Developmental Psychology",
        courseNumber: "PSY 201",
      },
    ] as CourseAreaOfInstructionWithCourse[],
  },
  parameters: {
    docs: {
      description: {
        component:
          "A card component that displays CourseAreaOfInstruction items grouped by area of instruction. Each area shows its own title, subtitle, progress bar, and course list. An overall total hours is displayed at the bottom.",
      },
    },
  },
} satisfies Meta<typeof AreaOfInstructionCard>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    courseAreaOfInstructions: [
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "15.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Introduction to Child Development",
        courseNumber: "ECE 101",
      },
      {
        courseAreaOfInstructionId: "course-area-2",
        newHours: "20.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Early Learning Theories",
        courseNumber: "ECE 102",
      },
      {
        courseAreaOfInstructionId: "course-area-3",
        newHours: "10.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Developmental Psychology",
        courseNumber: "PSY 201",
      },
    ] as CourseAreaOfInstructionWithCourse[],
  },
};

export const MultipleAreas: Story = {
  args: {
    courseAreaOfInstructions: [
      // Area 1 courses
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "25.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Child Development Fundamentals",
        courseNumber: "ECE 101",
      },
      {
        courseAreaOfInstructionId: "course-area-2",
        newHours: "20.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Advanced Child Development",
        courseNumber: "ECE 201",
      },
      // Area 2 courses
      {
        courseAreaOfInstructionId: "course-area-3",
        newHours: "15.00",
        areaOfInstructionId: "area-2",
        courseTitle: "Nutrition Basics",
        courseNumber: "HSN 101",
      },
      {
        courseAreaOfInstructionId: "course-area-4",
        newHours: "10.00",
        areaOfInstructionId: "area-2",
        courseTitle: "Health and Safety Protocols",
        courseNumber: "HSN 102",
      },
    ] as CourseAreaOfInstructionWithCourse[],
    areaSubtitles: {
      "area-2":
        "Child guidance is included in Program Development, Curriculum and Foundations. There is no set minimum required hours specifically for Child Guidance.",
    },
  },
};

export const ProgressComplete: Story = {
  args: {
    courseAreaOfInstructions: [
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "25.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Child Development Fundamentals",
        courseNumber: "ECE 101",
      },
      {
        courseAreaOfInstructionId: "course-area-2",
        newHours: "20.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Advanced Child Development",
        courseNumber: "ECE 201",
      },
    ] as CourseAreaOfInstructionWithCourse[],
  },
};

export const ProgressOver100Percent: Story = {
  args: {
    courseAreaOfInstructions: [
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "30.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Child Development Fundamentals",
        courseNumber: "ECE 101",
      },
      {
        courseAreaOfInstructionId: "course-area-2",
        newHours: "25.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Advanced Child Development",
        courseNumber: "ECE 201",
      },
    ] as CourseAreaOfInstructionWithCourse[],
  },
};

export const ProgressLow: Story = {
  args: {
    courseAreaOfInstructions: [
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "5.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Introduction to Child Development",
        courseNumber: "ECE 101",
      },
    ] as CourseAreaOfInstructionWithCourse[],
  },
};

export const MultipleCourses: Story = {
  args: {
    courseAreaOfInstructions: [
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "8.00",
        areaOfInstructionId: "area-3",
        courseTitle: "Observation Techniques",
        courseNumber: "ECE 301",
      },
      {
        courseAreaOfInstructionId: "course-area-2",
        newHours: "6.00",
        areaOfInstructionId: "area-3",
        courseTitle: "Assessment Methods",
        courseNumber: "ECE 302",
      },
      {
        courseAreaOfInstructionId: "course-area-3",
        newHours: "4.00",
        areaOfInstructionId: "area-3",
        courseTitle: "Documentation Practices",
        courseNumber: "ECE 303",
      },
      {
        courseAreaOfInstructionId: "course-area-4",
        newHours: "2.00",
        areaOfInstructionId: "area-3",
        courseTitle: "Portfolio Development",
        courseNumber: "ECE 304",
      },
    ] as CourseAreaOfInstructionWithCourse[],
  },
};

export const WithoutProgressBar: Story = {
  args: {
    courseAreaOfInstructions: [
      {
        courseAreaOfInstructionId: "course-area-1",
        newHours: "25.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Child Development Fundamentals",
        courseNumber: "ECE 101",
      },
      {
        courseAreaOfInstructionId: "course-area-2",
        newHours: "20.00",
        areaOfInstructionId: "area-1",
        courseTitle: "Advanced Child Development",
        courseNumber: "ECE 201",
      },
    ] as CourseAreaOfInstructionWithCourse[],
    showProgressBar: false,
  },
};

export const Empty: Story = {
  args: {
    courseAreaOfInstructions: [] as CourseAreaOfInstructionWithCourse[],
  },
};
