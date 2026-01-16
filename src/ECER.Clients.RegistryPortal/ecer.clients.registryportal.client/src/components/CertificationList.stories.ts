import type { Meta, StoryObj } from "@storybook/vue3-vite";

import CertificationList from "./CertificationList.vue";

// More on how to set up stories at: https://storybook.js.org/docs/writing-stories
const meta = {
  title: "Example/CertificationList",
  component: CertificationList,
  // This component will have an automatically generated docsPage entry: https://storybook.js.org/docs/writing-docs/autodocs
  tags: ["autodocs"],
  argTypes: {},
  args: {
    certifications: [
      {
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
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "Assistant" },
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: "2026-04-23T00:00:00",
        effectiveDate: "2025-04-23T00:00:00",
        date: "2025-04-23T00:00:00",
        printDate: null,
        hasConditions: false,
        levelName: "ECE Assistant",
        statusCode: "Expired",
        certificatePDFGeneration: "Yes",
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "Assistant" },
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
      {
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
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 1 YR" },
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
      {
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
      {
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
      {
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
    ],
  },
} satisfies Meta<typeof CertificationList>;

export default meta;
type Story = StoryObj<typeof meta>;

export const List: Story = {
  args: {},
};
