import type { Meta, StoryObj } from "@storybook/vue3-vite";

import CertificationCard from "./CertificationCard.vue";

// More on how to set up stories at: https://storybook.js.org/docs/writing-stories
const meta = {
  title: "Example/CertificationCard",
  component: CertificationCard,
  // This component will have an automatically generated docsPage entry: https://storybook.js.org/docs/writing-docs/autodocs
  tags: ["autodocs"],
  argTypes: {},
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE Assistant",
      statusCode: "Active",
      certificatePDFGeneration: "Yes",
      levels: [{ id: "556b387e-8020-f011-998a-7c1e52871876", type: "Assistant" }],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
} satisfies Meta<typeof CertificationCard>;

export default meta;
type Story = StoryObj<typeof meta>;
/*
 *👇 Render functions are a framework specific feature to allow you control on how the component renders.
 * See https://storybook.js.org/docs/api/csf
 * to learn how to use render functions.
 */
export const Assistant: Story = {
  args: {},
};

export const OneYear: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE One Year",
      statusCode: "Active",
      certificatePDFGeneration: "Yes",
      levels: [{ id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 1 YR" }],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const FiveYears: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE Five Years",
      statusCode: "Active",
      certificatePDFGeneration: "Yes",
      levels: [{ id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" }],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const FiveYearsWithSpecializations: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE Five Years",
      statusCode: "Active",
      certificatePDFGeneration: "Yes",
      levels: [
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
      ],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const Expired: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE Five Years",
      statusCode: "Expired",
      certificatePDFGeneration: "Yes",
      levels: [
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
      ],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const Cancelled: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE Five Years",
      statusCode: "Cancelled",
      certificatePDFGeneration: "Yes",
      levels: [
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
      ],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const Suspended: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE Five Years",
      statusCode: "Suspended",
      certificatePDFGeneration: "Yes",
      levels: [
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
      ],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const HasConditions: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: true,
      levelName: "ECE Five Years",
      statusCode: "Renewed",
      certificatePDFGeneration: "Yes",
      levels: [
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
      ],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const CanRenew: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2025-04-23T00:00:00",
      effectiveDate: "2021-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: true,
      levelName: "ECE Five Years",
      statusCode: "Renewed",
      certificatePDFGeneration: "Yes",
      levels: [
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
      ],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const CertificateFileRequested: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2025-04-23T00:00:00",
      effectiveDate: "2021-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: true,
      levelName: "ECE Five Years",
      statusCode: "Renewed",
      certificatePDFGeneration: "Requested",
      levels: [
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
        { id: "556b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
      ],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
  },
};

export const HasApplication: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2030-04-23T00:00:00",
      effectiveDate: "2025-04-23T00:00:00",
      date: "2025-04-23T00:00:00",
      printDate: null,
      hasConditions: false,
      levelName: "ECE One Year",
      statusCode: "Active",
      certificatePDFGeneration: "Yes",
      levels: [{ id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 1 YR" }],
      files: [
        {
          id: "3979ff88-f262-4747-b294-c289caa2402a",
          url: "ecer_certificate/f403a278-8020-f011-998a-6045bdf9b81b",
          extention: ".pdf",
          size: "322.00 KB",
          name: "Cover Letter-016359.pdf",
        },
      ],
      certificateConditions: [],
    },
    hasApplication: true,
  },
};
