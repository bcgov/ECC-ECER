import type { Meta, StoryObj } from "@storybook/vue3";
import { action } from "@storybook/addon-actions";

import PureApplicationCardList from "./PureApplicationCardList.vue";

// More on how to set up stories at: https://storybook.js.org/docs/writing-stories
const meta = {
  title: "Example/PureApplicationCardList",
  component: PureApplicationCardList,
  // This component will have an automatically generated docsPage entry: https://storybook.js.org/docs/writing-docs/autodocs
  tags: ["autodocs"],
  args: {
    hasEceAssistantCertification: true,
    "onApply-now": action("apply-now"),
  },
} satisfies Meta<typeof PureApplicationCardList>;

export default meta;
type Story = StoryObj<typeof meta>;

export const HasEceAssistantCertification: Story = {
  args: {
    hasEceAssistantCertification: true,
  },
};

export const HasEceOneYearCertification: Story = {
  args: {
    hasEceOneYearCertification: true,
  },
};

export const HasEceFiveYearCertification: Story = {
  args: {
    hasEceFiveYearCertification: true,
  },
};
