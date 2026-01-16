import type { Meta, StoryObj } from "@storybook/vue3-vite";
import TotalHoursOfInstructionCard from "../TotalHoursOfInstructionCard.vue";

const meta = {
  title: "Components/TotalHoursOfInstructionCard",
  component: TotalHoursOfInstructionCard,
  tags: ["autodocs"],
  argTypes: {
    totalHours: {
      control: "number",
      description: "The total hours of instruction completed",
    },
    requiredHours: {
      control: "number",
      description: "The required hours of instruction",
    },
  },
  args: {
    totalHours: 120,
    requiredHours: 100,
  },
  parameters: {
    docs: {
      description: {
        component:
          "A card component that displays total hours of instruction with a progress bar. Shows green background and border when requirements are met, yellow when not met.",
      },
    },
  },
} satisfies Meta<typeof TotalHoursOfInstructionCard>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    totalHours: 120,
    requiredHours: 100,
  },
};

export const NotMet: Story = {
  args: {
    totalHours: 75,
    requiredHours: 100,
  },
};

export const ExactlyMet: Story = {
  args: {
    totalHours: 100,
    requiredHours: 100,
  },
};

export const OverMet: Story = {
  args: {
    totalHours: 150,
    requiredHours: 100,
  },
};

export const LowProgress: Story = {
  args: {
    totalHours: 25,
    requiredHours: 100,
  },
};

export const ZeroHours: Story = {
  args: {
    totalHours: 0,
    requiredHours: 100,
  },
};
