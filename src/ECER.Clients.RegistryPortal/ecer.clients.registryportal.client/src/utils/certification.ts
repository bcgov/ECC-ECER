import type { Components } from "@/types/openapi";
import { orderBy, findLastIndex } from "lodash";

/**
 * Check if a certification is an ECE Assistant certification
 */
export function isEceAssistant(
  certification: Components.Schemas.Certification,
): boolean {
  return (
    certification.levels?.some((level) => level.type === "Assistant") ?? false
  );
}

/**
 * Check if a certification is an ECE Five Year certification
 */
export function isEceFiveYear(
  certification: Components.Schemas.Certification,
): boolean {
  return (
    certification.levels?.some((level) => level.type === "ECE 5 YR") ?? false
  );
}

/**
 * Check if a certification is an ECE One Year certification
 */
export function isEceOneYear(
  certification: Components.Schemas.Certification,
): boolean {
  return (
    certification.levels?.some((level) => level.type === "ECE 1 YR") ?? false
  );
}

/**
 * Check if a certification has SNE (Special Needs Educator) level
 */
export function hasSNE(
  certification: Components.Schemas.Certification,
): boolean {
  return certification.levels?.some((level) => level.type === "SNE") ?? false;
}

/**
 * Check if a certification has ITE (Infant Toddler Educator) level
 */
export function hasITE(
  certification: Components.Schemas.Certification,
): boolean {
  return certification.levels?.some((level) => level.type === "ITE") ?? false;
}

/**
 * Get certification types for a given certification
 */
export function getCertificationTypes(
  certification: Components.Schemas.Certification,
): Components.Schemas.CertificationType[] {
  const certificationTypes = [] as Components.Schemas.CertificationType[];

  if (isEceAssistant(certification)) {
    certificationTypes.push("EceAssistant");
  }
  if (isEceOneYear(certification)) {
    certificationTypes.push("OneYear");
  }
  if (isEceFiveYear(certification)) {
    certificationTypes.push("FiveYears");
  }
  if (hasSNE(certification)) {
    certificationTypes.push("Sne");
  }
  if (hasITE(certification)) {
    certificationTypes.push("Ite");
  }

  return certificationTypes;
}

/**
 *
 * @param certifications
 * @returns number of one year certifications that occurred after the latest five year certificate
 */
export function countEceOneYearCertificationsSinceLatest5YearIfExists(
  certifications: Components.Schemas.Certification[],
): number {
  //latest certifications will be at the end of the array
  let orderedCertifications = orderBy(certifications, [
    "effectiveDate",
    ["asc"],
  ]);

  let indexOfLatestFiveYearCertification = findLastIndex(
    orderedCertifications,
    (cert) => cert.levels?.some((level) => level.type === "ECE 5 YR") ?? false,
  );

  let filteredSortedCertifications = orderedCertifications.slice(
    indexOfLatestFiveYearCertification + 1,
  );

  return filteredSortedCertifications.filter((cert) =>
    cert.levels?.some((level) => level.type === "ECE 1 YR"),
  ).length;
}
