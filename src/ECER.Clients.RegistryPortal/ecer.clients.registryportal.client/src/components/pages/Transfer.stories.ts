import type { Meta, StoryObj } from "@storybook/vue3-vite";
import { mocked } from "storybook/test";
import { getCertificationComparisonList } from "@/api/configuration";

import Transfer from "./Transfer.vue";

import {
  getDateMinusYears,
  getTodayDate,
  getDatePlusYears,
} from "@/utils/dateHelpers";
import { useConfigStore } from "@/store/config";
import { useCertificationStore } from "@/store/certification.ts";
import type { Components } from "@/types/openapi.js";
import type { CertificationLevelType } from "@/types/certificationLevelType.js";

// More on how to set up stories at: https://storybook.js.org/docs/writing-stories
const meta = {
  title: "Example/Transfer",
  component: Transfer,
  tags: ["!autodocs"], //Do not generate auto docs for this component. Seems to mess up due to Breadcrumbs.vue
  beforeEach: () => {
    mocked(getCertificationComparisonList).mockImplementation(
      async (provinceId) => {
        switch (provinceId) {
          case "NONE":
            return [];
          case "HAS_ALL":
            return [
              {
                transferringCertificate: {
                  id: "37e6d6af-d084-ef11-ac21-6045bdcd6cb0",
                  certificationType: "One year or Five Year",
                },
                options: [
                  {
                    id: "f404aa39-e884-ef11-ac20-6045bdccf571",
                    bcCertificate: "One Year",
                  },
                  {
                    id: "a81d3567-e884-ef11-ac20-6045bdccf571",
                    bcCertificate: "Five Year Certificate",
                  },
                ],
              },
              {
                transferringCertificate: {
                  id: "484ff8c5-d084-ef11-ac21-6045bdcd6cb0",
                  certificationType: "One year or Five year ITE SNE",
                },
                options: [
                  {
                    id: "e1b20a8e-e884-ef11-ac20-6045bdccf571",
                    bcCertificate: "One Year",
                  },
                  {
                    id: "d6053ca4-e884-ef11-ac20-6045bdccf571",
                    bcCertificate: "Five Year Certificate+ITE+SNE",
                  },
                ],
              },
              {
                transferringCertificate: {
                  id: "f1efbd35-cd84-ef11-ac21-6045bdcd6cb0",
                  certificationType: "Assistant",
                },
                options: [
                  {
                    id: "e344d10b-e184-ef11-ac20-7c1e524093cc",
                    bcCertificate: "Assistant",
                  },
                ],
              },
            ];
          default:
            return [];
        }
      },
    );
  },
  decorators: [
    (story) => {
      const config = useConfigStore();
      config.provinceList = [
        { provinceId: "NONE", provinceName: "NONE", provinceCode: "NONE" },
        {
          provinceId: "HAS_ALL",
          provinceName: "Has All",
          provinceCode: "HAS_ALL",
        },
      ] as Components.Schemas.Province[];
      return story();
    },
  ],
} satisfies Meta<typeof Transfer>;

export default meta;
type Story = StoryObj<typeof meta>;
/*
 *👇 Render functions are a framework specific feature to allow you control on how the component renders.
 * See https://storybook.js.org/docs/api/csf
 * to learn how to use render functions.
 */

// Assistant - no
// work experience question - yes
// upgrade choice - no
// One Year - yes
// Five Year - yes
// Five year ite + sne - yes
export const TransferUserHasNoCertifications: Story = {};

export const TransferUserHasAssistant: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(["Assistant"], getTodayDate(), "Active"),
      ];
      return story();
    },
  ],
};

// Assistant - no
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - yes
// Five year ite + sne - yes
export const TransferUserHasAssistantAndOneYear: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(["Assistant"], getTodayDate(), "Active"),
        generateCertificate(["ECE 1 YR"], getTodayDate(), "Active"),
      ];
      return story();
    },
  ],
};

// Assistant - no
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - no
export const TransferUserFullyCertified: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(["Assistant"], getTodayDate(), "Active"),
        generateCertificate(["ECE 1 YR"], getTodayDate(), "Active"),
        generateCertificate(
          ["ECE 5 YR", "ITE", "SNE"],
          getDateMinusYears(1),
          "Active",
        ),
      ];
      return story();
    },
  ],
};

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - yes
// Five year ite + sne - yes
export const TransferUserHasOneYear: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(["ECE 1 YR"], getTodayDate(), "Active"),
      ];
      return story();
    },
  ],
};

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - upgradable
export const TransferUserHasFiveYearShouldBeAbleToUpgradeNoOneYear: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(["ECE 5 YR"], getTodayDate(), "Active"),
      ];
      return story();
    },
  ],
};

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - upgradable
export const TransferUserHasFiveYearIteShouldBeAbleToUpgrade: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(["ECE 5 YR", "ITE"], getTodayDate(), "Active"),
      ];
      return story();
    },
  ],
};

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - upgradable
export const TransferUserHasFiveYearSne: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(["ECE 5 YR", "SNE"], getTodayDate(), "Active"),
      ];
      return story();
    },
  ],
};

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - no
export const TransferUserHasFiveYearIteSne: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(
          ["ECE 5 YR", "SNE", "ITE"],
          getTodayDate(),
          "Active",
        ),
      ];
      return story();
    },
  ],
};

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - upgradable
export const TransferUserHasFiveYearAndOneYearShouldBeAbleToAutoUpgradeToIteSne: Story =
  {
    decorators: [
      (story) => {
        const certificationStore = useCertificationStore();
        certificationStore.certifications = [
          generateCertificate(["ECE 5 YR"], getDateMinusYears(1), "Active"),
          generateCertificate(["ECE 1 YR"], getTodayDate(), "Active"),
        ];
        return story();
      },
    ],
  };

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - upgradable
export const TransferUserHasFiveYearIteAndOneYearShouldBeAbleToAutoUpgradeToSne: Story =
  {
    decorators: [
      (story) => {
        const certificationStore = useCertificationStore();
        certificationStore.certifications = [
          generateCertificate(
            ["ECE 5 YR", "ITE"],
            getDateMinusYears(1),
            "Active",
          ),
          generateCertificate(["ECE 1 YR"], getTodayDate(), "Active"),
        ];
        return story();
      },
    ],
  };

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - yes
// Five Year - no
// Five year ite + sne - no
export const TransferUserHasExpiredFiveYearShouldGetOptionForOneYear: Story = {
  decorators: [
    (story) => {
      const certificationStore = useCertificationStore();
      certificationStore.certifications = [
        generateCertificate(
          ["ECE 5 YR", "SNE", "ITE"],
          getDateMinusYears(1),
          "Expired",
        ),
      ];
      return story();
    },
  ],
};

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - yes
// Five Year - no
// Five year ite + sne - no
export const TransferUserHasExpiredFiveYearAndExpiredOneYearEdgeCaseCanApplyForOneYear: Story =
  {
    decorators: [
      (story) => {
        const certificationStore = useCertificationStore();
        certificationStore.certifications = [
          generateCertificate(
            ["ECE 5 YR", "SNE", "ITE"],
            getTodayDate(),
            "Expired",
          ),
          generateCertificate(["ECE 1 YR"], getDateMinusYears(1), "Expired"),
        ];
        return story();
      },
    ],
  };

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - no
export const TransferUserHasActiveFiveYearAndExpiredOneYearEdgeCaseCannotApplyForOneYear: Story =
  {
    decorators: [
      (story) => {
        const certificationStore = useCertificationStore();
        certificationStore.certifications = [
          generateCertificate(
            ["ECE 5 YR", "SNE", "ITE"],
            getTodayDate(),
            "Active",
          ),
          generateCertificate(["ECE 1 YR"], getDateMinusYears(1), "Expired"),
        ];
        return story();
      },
    ],
  };

// Assistant - yes
// work experience question - no
// upgrade choice - no
// One Year - no
// Five Year - no
// Five year ite + sne - no
export const TransferUserHasExpiredFiveYearAndOneYearAfterShouldNotBeAbleToApplyForOneYear: Story =
  {
    decorators: [
      (story) => {
        const certificationStore = useCertificationStore();
        certificationStore.certifications = [
          generateCertificate(
            ["ECE 5 YR", "SNE", "ITE"],
            getDateMinusYears(1),
            "Expired",
          ),
          generateCertificate(["ECE 1 YR"], getTodayDate(), "Expired"),
        ];
        return story();
      },
    ],
  };

// helper functions to generate certificates
const generateCertificate = (
  levels: CertificationLevelType[],
  date: string,
  status: "Expired" | "Active",
): Components.Schemas.Certification => {
  return {
    id: "f403a278-8020-f011-998a-6045bdf9b81b",
    name: levels.join(","),
    number: "016359",
    expiryDate: date,
    effectiveDate: date,
    date: date,
    printDate: null,
    hasConditions: false,
    levelName: levels.join(","),
    statusCode: status,
    certificatePDFGeneration: "Yes",
    levels: levels.map((level) => {
      return { id: "556b387e-8020-f011-998a-7c1e52871876", type: level };
    }),
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
  };
};
