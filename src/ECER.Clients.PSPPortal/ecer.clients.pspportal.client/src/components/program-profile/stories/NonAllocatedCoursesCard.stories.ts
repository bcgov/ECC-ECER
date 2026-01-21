import type { Meta, StoryObj } from "@storybook/vue3-vite";
import type { Components } from "@/types/openapi";
import NonAllocatedCoursesCard from "../NonAllocatedCoursesCard.vue";

const meta = {
  title: "Components/NonAllocatedCoursesCard",
  component: NonAllocatedCoursesCard,
  tags: ["autodocs"],
  argTypes: {
    courses: {
      control: "object",
      description:
        "List of Course items with no hours allocated toward any required areas of instruction",
    },
    onEdit: {
      action: "edit",
      description: "Emitted when edit button is clicked",
    },
  },
  args: {
    courses: [
      {
        courseNumber: "ECE 100",
        courseTitle: "Introduction to Early Childhood Education",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
      {
        courseNumber: "ECE 250",
        courseTitle: "Child Care Administration",
        courseAreaOfInstruction: null,
        programType: "Basic",
      },
      {
        courseNumber: "ECE 400",
        courseTitle: "Professional Development in ECE",
        courseAreaOfInstruction: null,
        programType: "SNE",
      },
    ] as Components.Schemas.Course[],
  },
  parameters: {
    docs: {
      description: {
        component:
          "A card component that displays courses with no hours allocated toward any required areas of instruction. No progress bar or total hours are shown.",
      },
    },
  },
} satisfies Meta<typeof NonAllocatedCoursesCard>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    courses: [
      {
        courseNumber: "ECE 100",
        courseTitle: "Introduction to Early Childhood Education",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
      {
        courseNumber: "ECE 250",
        courseTitle: "Child Care Administration",
        courseAreaOfInstruction: null,
        programType: "Basic",
      },
      {
        courseNumber: "ECE 400",
        courseTitle: "Professional Development in ECE",
        courseAreaOfInstruction: null,
        programType: "SNE",
      },
    ] as Components.Schemas.Course[],
  },
};

export const SingleCourse: Story = {
  args: {
    courses: [
      {
        courseNumber: "ECE 100",
        courseTitle: "Introduction to Early Childhood Education",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
    ] as Components.Schemas.Course[],
  },
};

export const Empty: Story = {
  args: {
    courses: [] as Components.Schemas.Course[],
  },
};

export const MultipleCourses: Story = {
  args: {
    courses: [
      {
        courseNumber: "ECE 100",
        courseTitle: "Introduction to Early Childhood Education",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
      {
        courseNumber: "ECE 250",
        courseTitle: "Child Care Administration",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
      {
        courseNumber: "ECE 400",
        courseTitle: "Professional Development in ECE",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
      {
        courseNumber: "ECE 350",
        courseTitle: "Special Topics in ECE",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
      {
        courseNumber: "ECE 500",
        courseTitle: "Advanced Child Development",
        courseAreaOfInstruction: null,
        programType: "ITE",
      },
    ] as Components.Schemas.Course[],
  },
};
