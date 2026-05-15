import type { Meta, StoryObj } from "@storybook/vue3";

import UploadFileItem, { type FileItem } from "../UploadFileItem.vue";

function makeFile(overrides: Partial<FileItem> = {}): FileItem {
  return {
    file: new File([], overrides.fileName ?? "test_file1.jpg"),
    fileId: "f1",
    progress: 101,
    fileErrors: [],
    fileSize: "9.22KB",
    fileName: "test_file1.jpg",
    storageFolder: "permanent",
    ...overrides,
  };
}

const meta: Meta<typeof UploadFileItem> = {
  title: "Common/UploadFileItem",
  component: UploadFileItem,
  tags: ["autodocs"],
  decorators: [
    () => ({
      template:
        '<v-sheet max-width="700" class="pa-6 bg-transparent"><story /></v-sheet>',
    }),
  ],
  args: {
    fileItem: makeFile(),
    uploadProgress: 101,
    canDelete: true,
    errors: [],
  },
};

export default meta;
type Story = StoryObj<typeof UploadFileItem>;

export const HappyPath: Story = {};

export const InProgress: Story = {
  args: {
    fileItem: makeFile({ progress: 40 }),
    uploadProgress: 40,
  },
};

export const Linking: Story = {
  args: {
    fileItem: makeFile({ isLinking: true, progress: 0 }),
    uploadProgress: 0,
  },
};

export const Deleting: Story = {
  args: {
    fileItem: makeFile({ isDeleting: true }),
  },
};

export const WithSingleError: Story = {
  args: {
    fileItem: makeFile({
      fileName: "broken.exe",
      fileErrors: [
        "This type of file is not accepted. The following file types are accepted: .txt, .pdf, .doc, .docx, .rtf, .xls, .xlsx, .jpg/jpeg, .gif, .png, .bmp, .tiff, .x-tiff",
      ],
    }),
    errors: [
      "This type of file is not accepted. The following file types are accepted: .txt, .pdf, .doc, .docx, .rtf, .xls, .xlsx, .jpg/jpeg, .gif, .png, .bmp, .tiff, .x-tiff",
    ],
  },
};

export const WithMultipleErrors: Story = {
  args: {
    fileItem: makeFile({
      fileName: "too_big.pdf",
      fileErrors: [
        "This file is too big. Only files 10MB or smaller are accepted.",
        "A file with this name has already been added.",
      ],
    }),
    errors: [
      "This file is too big. Only files 10MB or smaller are accepted.",
      "A file with this name has already been added.",
    ],
  },
};

export const LongFilename: Story = {
  args: {
    fileItem: makeFile({
      fileName:
        "a_filename_long_enough_to_have_been_truncated_in_the_previous_layout_but_visible_now.jpg",
    }),
  },
};

export const NotDeletable: Story = {
  args: {
    canDelete: false,
  },
};
