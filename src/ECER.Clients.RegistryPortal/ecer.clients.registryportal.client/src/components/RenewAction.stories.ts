import type { Meta, StoryObj } from "@storybook/vue3-vite";

import RenewAction from "./RenewAction.vue";
import {
  getDateMinusYears,
  getTodayDate,
  getDatePlusYears,
} from "@/utils/dateHelpers";
import { useCertificationStore } from "@/store/certification";

// More on how to set up stories at: https://storybook.js.org/docs/writing-stories
const meta = {
  title: "Example/RenewAction",
  component: RenewAction,
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
  },
} satisfies Meta<typeof RenewAction>;

export default meta;
type Story = StoryObj<typeof meta>;
/*
 *👇 Render functions are a framework specific feature to allow you control on how the component renders.
 * See https://storybook.js.org/docs/api/csf
 * to learn how to use render functions.
 */
export const AssistantCantRenew: Story = {
  args: {},
};

export const AssistantExpiredMoreThan5Years: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: getDateMinusYears(10),
      effectiveDate: getDateMinusYears(11),
      date: getTodayDate(),
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
  },
};

export const AssistantCanRenew: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "KARISSA CAULKINS",
      number: "016359",
      expiryDate: "2025-04-23T00:00:00",
      effectiveDate: "2024-04-23T00:00:00",
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
  },
};

export const OneYearCannotRenewNotExpired: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "TESTING USER",
      number: "016359",
      expiryDate: getDatePlusYears(1),
      effectiveDate: getTodayDate(),
      date: getTodayDate(),
      printDate: null,
      hasConditions: false,
      levelName: "ECE 1 YR",
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
  },
};

export const OneYearCanRenew: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "TESTING USER",
      number: "016359",
      expiryDate: getTodayDate(),
      effectiveDate: getDateMinusYears(1),
      date: getDateMinusYears(1),
      printDate: null,
      hasConditions: false,
      levelName: "ECE 1 YR",
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
  },
};

export const OneYearCannotRenewAgainWithout5YearHistory: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "TESTING USER",
      number: "016359",
      expiryDate: getTodayDate(),
      effectiveDate: getDateMinusYears(1),
      date: getDateMinusYears(1),
      printDate: null,
      hasConditions: false,
      levelName: "ECE 1 YR",
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
  },
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        {
          id: "f403a278-8020-f011-998a-6045bdf9b81b",
          name: "TESTING USER",
          number: "016359",
          expiryDate: getTodayDate(),
          effectiveDate: getDateMinusYears(1),
          date: getDateMinusYears(1),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 1 YR",
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
          name: "TESTING USER",
          number: "016359",
          expiryDate: getTodayDate(),
          effectiveDate: getDateMinusYears(1),
          date: getDateMinusYears(1),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 1 YR",
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
      ];
      return story();
    },
  ],
};

export const OneYearCannotRenewAgainWith5YearHistory: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "TESTING USER",
      number: "016359",
      expiryDate: getTodayDate(),
      effectiveDate: getDateMinusYears(1),
      date: getDateMinusYears(1),
      printDate: null,
      hasConditions: false,
      levelName: "ECE 1 YR",
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
  },
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        {
          id: "f403a278-8020-f011-998a-6045bdf9b81b",
          name: "ONE YEAR CERTIFICATION NEW",
          number: "016359",
          expiryDate: getDateMinusYears(5),
          effectiveDate: getDateMinusYears(5),
          date: getDateMinusYears(5),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 1 YR",
          statusCode: "Expired",
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
          name: "ONE YEAR CERTIFICATION RENEWED",
          number: "016359",
          expiryDate: getDateMinusYears(4),
          effectiveDate: getDateMinusYears(4),
          date: getDateMinusYears(4),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 1 YR",
          statusCode: "Expired",
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
          name: "FIVE YEAR CERTIFICATION",
          number: "016359",
          expiryDate: getDateMinusYears(3),
          effectiveDate: getDateMinusYears(3),
          date: getDateMinusYears(3),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 5 YR",
          statusCode: "Expired",
          certificatePDFGeneration: "Yes",
          levels: [
            { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
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
      ];
      return story();
    },
  ],
};

export const OneYearCanRenewAgainWith5YearHistoryAndSubsequentOneYear: Story = {
  args: {
    certification: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      name: "ONE YEAR CERTIFICATION APPLIED AFTER 5 YEAR",
      number: "016359",
      expiryDate: getDateMinusYears(1),
      effectiveDate: getDateMinusYears(1),
      date: getDateMinusYears(1),
      printDate: null,
      hasConditions: false,
      levelName: "ECE 1 YR",
      statusCode: "Expired",
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
  },
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        {
          id: "f403a278-8020-f011-998a-6045bdf9b81b",
          name: "ONE YEAR CERTIFICATION NEW",
          number: "016359",
          expiryDate: getDateMinusYears(5),
          effectiveDate: getDateMinusYears(5),
          date: getDateMinusYears(5),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 1 YR",
          statusCode: "Expired",
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
          name: "ONE YEAR CERTIFICATION RENEWED",
          number: "016359",
          expiryDate: getDateMinusYears(4),
          effectiveDate: getDateMinusYears(4),
          date: getDateMinusYears(4),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 1 YR",
          statusCode: "Expired",
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
          name: "FIVE YEAR CERTIFICATION",
          number: "016359",
          expiryDate: getDateMinusYears(3),
          effectiveDate: getDateMinusYears(3),
          date: getDateMinusYears(3),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 5 YR",
          statusCode: "Expired",
          certificatePDFGeneration: "Yes",
          levels: [
            { id: "556b387e-8020-f011-998a-7c1e52871876", type: "ECE 5 YR" },
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
          name: "ONE YEAR CERTIFICATION APPLIED AFTER 5 YEAR",
          number: "016359",
          expiryDate: getDateMinusYears(1),
          effectiveDate: getDateMinusYears(1),
          date: getDateMinusYears(1),
          printDate: null,
          hasConditions: false,
          levelName: "ECE 1 YR",
          statusCode: "Expired",
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
      ];
      return story();
    },
  ],
};
