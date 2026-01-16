import { defineStore } from "pinia";
import { orderBy } from "lodash";

import { getCertifications } from "@/api/certification";
import type { Components } from "@/types/openapi";
import { expiredMoreThan5Years } from "@/utils/functions";
import type { CertificationLevelType } from "@/types/certificationLevelType";

export interface CertificationState {
  certifications: Components.Schemas.Certification[] | null | undefined;
}

export const useCertificationStore = defineStore("certification", {
  state: (): CertificationState => ({
    certifications: [],
  }),
  persist: true,
  getters: {
    hasCertifications(state): boolean {
      return (
        state.certifications !== null &&
        state.certifications !== undefined &&
        state.certifications.length > 0
      );
    },
    holdsEceFiveYearCertification(state): boolean {
      return (
        state.certifications?.some((cert) =>
          cert.levels?.some((level) => level.type === "ECE 5 YR"),
        ) ?? false
      );
    },
    activeEceFiveYearCertification(
      state,
    ): Components.Schemas.Certification | null {
      return (
        state.certifications?.find(
          (cert) =>
            cert.levels?.some((level) => level.type === "ECE 5 YR") &&
            cert.statusCode === "Active",
        ) ?? null
      );
    },
    holdsAllCertifications(state): boolean {
      if (!state.certifications || state.certifications.length === 0)
        return false;

      const isRenewable = (
        certification: Components.Schemas.Certification,
      ): boolean => {
        return (
          certification.statusCode === "Active" ||
          ((certification.statusCode === "Expired" ||
            certification.statusCode === "Suspended") &&
            !expiredMoreThan5Years(certification))
        );
      };

      return (
        state.certifications.some(
          (cert) =>
            cert.levels?.some((level) => level.type === "Assistant") &&
            isRenewable(cert),
        ) &&
        state.certifications.some(
          (cert) =>
            cert.levels?.some((level) => level.type === "ECE 1 YR") &&
            isRenewable(cert),
        ) &&
        state.certifications.some((cert) =>
          cert.levels?.some((level) => level.type === "ECE 5 YR"),
        ) &&
        state.certifications.some((cert) =>
          cert.levels?.some((level) => level.type === "SNE"),
        ) &&
        state.certifications.some((cert) =>
          cert.levels?.some((level) => level.type === "ITE"),
        )
      );
    },
    holdsPostBasicCertification(state): boolean {
      if (!state.certifications || state.certifications.length === 0)
        return false;
      return (
        state.certifications.some((cert) =>
          cert.levels?.some((level) => level.type === "ECE 5 YR"),
        ) &&
        state.certifications.some((cert) =>
          cert.levels?.some((level) => level.type === "SNE"),
        ) &&
        state.certifications.some((cert) =>
          cert.levels?.some((level) => level.type === "ITE"),
        )
      );
    },
    hasMultipleEceOneYearCertifications(state): boolean {
      let count = 0;
      if (!state.certifications || state.certifications?.length < 2)
        return false;
      for (const cert of state.certifications) {
        if (cert.levels?.some((level) => level.type === "ECE 1 YR")) {
          count++;
        }
      }
      return count >= 2;
    },
    currentCertification(state): Components.Schemas.Certification | undefined {
      if (!state.certifications || state.certifications.length === 0) {
        return undefined;
      }

      //sorts certifications by status code first and then expiry date and then certificate type. Returns first one in the list which should be the latest certificate
      return orderBy(
        state.certifications,
        [
          ({ statusCode }) => {
            switch (statusCode) {
              case "Active":
                return 1;
              case "Cancelled":
                return 2;
              case "Suspended":
                return 3;
              case "Expired":
                return 4;
              default:
                return 5;
            }
          },
          "expiryDate",
          ({ levels }) => {
            //in case expiry date is the same, we will also rank it based on certificateType 5Y+SNE+ITE -> 5Y + SNE||ITE -> 5Y -> Assistant -> 1YR
            if (
              levels?.some((level) => level.type === "ECE 5 YR") &&
              levels?.some((level) => level.type === "ITE") &&
              levels?.some((level) => level.type === "SNE")
            ) {
              return 1;
            }

            if (
              (levels?.some((level) => level.type === "ECE 5 YR") &&
                levels?.some((level) => level.type === "ITE")) ||
              (levels?.some((level) => level.type === "ECE 5 YR") &&
                levels?.some((level) => level.type === "SNE"))
            ) {
              return 2;
            }

            if (levels?.some((level) => level.type === "ECE 5 YR")) {
              return 3;
            }

            if (levels?.some((level) => level.type === "Assistant")) {
              return 4;
            }

            if (levels?.some((level) => level.type === "ECE 1 YR")) {
              return 5;
            }

            console.warn(`unmapped level type ${levels}`);
            return 6;
          },
        ],
        ["asc", "desc", "asc"],
      )[0];
    },
  },
  actions: {
    getCertificationById(
      certificateId: string | null | undefined,
    ): Components.Schemas.Certification | undefined {
      return this.certifications?.find((cert) => cert.id === certificateId);
    },
    hasOtherCertifications(certificateId: string | null | undefined): boolean {
      if (!this.certifications) return false;
      return (
        this.certifications.filter((cert) => cert.id !== certificateId).length >
        0
      );
    },
    getMostRecentCertificationByExpiryDate(
      certificateType: CertificationLevelType,
    ): Components.Schemas.Certification | undefined {
      const mostRecentCertification = orderBy(
        this.certifications?.filter((certification) =>
          certification.levels?.some((level) => level.type === certificateType),
        ),
        ["expiryDate"],
        ["desc"],
      )?.[0];

      return mostRecentCertification;
    },
    async fetchCertifications() {
      // Drop any existing certifications
      this.$reset();

      const { data: certifications } = await getCertifications();
      if (certifications?.length && certifications.length > 0) {
        this.certifications = certifications;
      }
      return certifications;
    },
  },
});
