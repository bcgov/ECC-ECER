import type { Meta, StoryObj } from "@storybook/vue3";

import Question from "../Question.vue";
import { useApplicationFilesStore } from "@/store/applicationFiles";

const FAKE_APP_ID = "00000000-0000-0000-0000-000000000001";

const meta: Meta<typeof Question> = {
  title: "ProgramApplication/Question",
  component: Question,
  tags: ["autodocs"],
  decorators: [
    () => ({
      // Stub the application-files store so the nested
      // ProgramApplicationFileUploader doesn't 404 against a non-existent
      // backend when it calls refreshFiles() on mount.
      setup() {
        const store = useApplicationFilesStore();
        store.filesByApplicationId = { [FAKE_APP_ID]: [] };
        store.refreshFiles = async () => {};
      },
      template:
        '<v-sheet max-width="900" class="pa-6 bg-transparent"><story /></v-sheet>',
    }),
  ],
  args: {
    name: "Mission outline",
    question: "Outline the institution's mission or philosophy",
    programApplicationId: FAKE_APP_ID,
    componentGroupId: "00000000-0000-0000-0000-000000000002",
    componentId: "00000000-0000-0000-0000-000000000003",
    rfaiRequired: false,
    readOnly: false,
    hasUnsavedChanges: false,
    modelValue: { answer: "", files: [] },
  },
};

export default meta;
type Story = StoryObj<typeof Question>;

export const Initial: Story = {};

export const WithRfaiChip: Story = {
  args: {
    rfaiRequired: true,
  },
};

export const WithUnsavedChanges: Story = {
  args: {
    hasUnsavedChanges: true,
  },
};

export const WithRfaiAndUnsavedChanges: Story = {
  args: {
    rfaiRequired: true,
    hasUnsavedChanges: true,
  },
};

export const LongName: Story = {
  args: {
    name: "A program component name long enough to wrap onto multiple lines beside the chip stack",
    rfaiRequired: true,
    hasUnsavedChanges: true,
  },
};

export const WithSelectableExistingFiles: Story = {
  // Per-story decorator that overrides the meta decorator's empty store
  // with fixtures so "Select a previously uploaded file" renders.
  decorators: [
    () => ({
      setup() {
        const store = useApplicationFilesStore();
        store.filesByApplicationId = {
          [FAKE_APP_ID]: [
            {
              documentUrlId: "doc-1",
              shareDocumentUrlId: null,
              fileName: "existing_resume.pdf",
              fileSize: "120.5KB",
              url: null,
              extension: ".pdf",
            },
            {
              documentUrlId: "doc-2",
              shareDocumentUrlId: null,
              fileName: "transcript_2024.pdf",
              fileSize: "320.0KB",
              url: null,
              extension: ".pdf",
            },
          ],
        };
      },
      template: "<story />",
    }),
  ],
};
