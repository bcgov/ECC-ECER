import { DateTime } from "luxon";
import type { Components } from "@/types/openapi";

export function formatDate(
  inputDate: string | null | undefined,
  toFormat: DateFormat = "yyyy-MM-dd",
) {
  if (inputDate === undefined || inputDate === null) {
    return "Invalid date";
  }
  if (inputDate) {
    const formattedDate = DateTime.fromISO(inputDate).toFormat(toFormat);

    return formattedDate;
  }

  return inputDate;
}

export function formatAddress(
  location:
    | Components.Schemas.EducationInstitution
    | Components.Schemas.Campus
    | null
    | undefined,
): string {
  if (!location) return "—";
  const parts: string[] = [];
  if (location.street1) parts.push(location.street1);
  if (location.street2) parts.push(location.street2);
  if (location.street3) parts.push(location.street3);

  const cityParts: string[] = [];
  if (location.city) cityParts.push(location.city);
  if (location.province) cityParts.push(location.province);
  if (location.postalCode) cityParts.push(location.postalCode);

  if (cityParts.length > 0) {
    parts.push(cityParts.join(", "));
  }

  return parts.length > 0 ? parts.join(", ") : "—";
}

const institutionTypeMap: Record<
  Components.Schemas.PsiInstitutionType,
  string
> = {
  Public: "Public",
  Private: "Private",
  PublicOOP: "Public — OOP",
  ContinuingEducation: "Continuing Education",
};

export function formatInstitutionType(
  type: Components.Schemas.PsiInstitutionType | null | undefined,
): string {
  if (!type) return "—";
  return institutionTypeMap[type] || type;
}

const privateAuspiceTypeMap: Record<
  Components.Schemas.PrivateAuspiceType,
  string
> = {
  Theologicalinstitution: "Theological",
  FirstNationsmandatedpostsecondaryinstitute:
    "First Nations mandated post-secondary institute",
  Other: "Other",
  Privatetraininginstitution: "Private training institution",
  Indigenouscontrolledpostsecondaryinstitute:
    "Indigenous controlled post-secondary institute",
};

export function formatPrivateAuspiceType(
  type: Components.Schemas.PrivateAuspiceType | null | undefined,
): string {
  if (!type) return "—";
  return privateAuspiceTypeMap[type] || type;
}
