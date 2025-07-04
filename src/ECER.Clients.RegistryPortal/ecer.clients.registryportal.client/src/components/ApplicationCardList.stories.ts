import type { Meta, StoryObj } from "@storybook/vue3";
import { action } from "@storybook/addon-actions";

import ApplicationCardList from "./ApplicationCardList.vue";
import { getTodayDate, getTwoYearsFromToday, getTwoYearsAgo } from "../utils/dateHelpers";

// More on how to set up stories at: https://storybook.js.org/docs/writing-stories
const meta = {
  title: "Example/ApplicationCardList",
  component: ApplicationCardList,
  // This component will have an automatically generated docsPage entry: https://storybook.js.org/docs/writing-docs/autodocs
  tags: ["autodocs"],
  args: {
    certifications: [],
    "onApply-now": action("apply-now"),
  },
} satisfies Meta<typeof ApplicationCardList>;

export default meta;
type Story = StoryObj<typeof meta>;

export const NoCertifications: Story = {
  args: {
    certifications: [],
  },
};

export const EceAssistantPathway: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE 1 YR",
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE Five Year + ITE + SNE",
        statusCode: "Active",
        certificatePDFGeneration: "Yes",
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
          { id: "656b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
          { id: "756b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
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
};

export const EceOneYearPathway: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE Five Year + ITE + SNE",
        statusCode: "Active",
        certificatePDFGeneration: "Yes",
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
          { id: "656b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
          { id: "756b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
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
};

export const EceOneYearEdgeCasePathway: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsAgo(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE Five Year + ITE + SNE",
        statusCode: "Expired",
        certificatePDFGeneration: "Yes",
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
          { id: "656b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
          { id: "756b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
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
};

export const EceFiveYearPathway: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "Assistant",
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
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
    ],
  },
};

export const SpecializedCertificationPathway: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "Assistant",
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE Five Year",
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
    ],
  },
};

export const ItePathway: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "Assistant",
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE Five Year + SNE",
        statusCode: "Active",
        certificatePDFGeneration: "Yes",
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
          { id: "656b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
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
};

export const SnePathway: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "Assistant",
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE Five Year + ITE",
        statusCode: "Active",
        certificatePDFGeneration: "Yes",
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
          { id: "656b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
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
};

export const FullyCertified: Story = {
  args: {
    certifications: [
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "Assistant",
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
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
      {
        id: "f403a278-8020-f011-998a-6045bdf9b81b",
        name: "KARISSA CAULKINS",
        number: "016359",
        expiryDate: getTwoYearsFromToday(),
        effectiveDate: getTwoYearsAgo(),
        date: getTodayDate(),
        printDate: null,
        hasConditions: false,
        levelName: "ECE Five Year + ITE + SNE",
        statusCode: "Active",
        certificatePDFGeneration: "Yes",
        levels: [
          { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
          { id: "656b387e-8020-f011-998a-7c1e52871876", type: "ITE" },
          { id: "756b387e-8020-f011-998a-7c1e52871876", type: "SNE" },
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
};
