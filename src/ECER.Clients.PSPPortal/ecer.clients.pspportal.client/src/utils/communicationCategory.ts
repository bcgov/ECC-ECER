import type { Components } from "@/types/openapi";

export type CommunicationCategory = Components.Schemas.CommunicationCategory;
export type InitiableCommunicationCategory = Exclude<
  CommunicationCategory,
  "RequestforAdditionalInformation"
>;

type CommunicationCategoryOption<T extends CommunicationCategory> = {
  value: T;
  label: string;
};

export const communicationCategoryOptions: Array<
  CommunicationCategoryOption<CommunicationCategory>
> = [
  { value: "ProgramChangeRequest", label: "Program Change Request" },
  { value: "PracticumInquiry", label: "Practicum Inquiry" },
  {
    value: "ECEProgramApplicationInquiry",
    label: "ECE Program Application Inquiry",
  },
  {
    value: "ECEProgramApplicationRequirements",
    label: "ECE Program Application Requirements",
  },
  { value: "ProgramProfileInquiry", label: "Program Profile Inquiry" },
  {
    value: "IndividualEducationPlanIEP",
    label: "Individual Education Plan (IEP)",
  },
  { value: "MeetingRequest", label: "Meeting Request" },
  {
    value: "RequestforAdditionalInformation",
    label: "Request for Additional Information",
  },
  { value: "Other", label: "Other" },
];

export const initiableCommunicationCategoryOptions: Array<
  CommunicationCategoryOption<InitiableCommunicationCategory>
> = [
  { value: "ProgramChangeRequest", label: "Program Change Request" },
  { value: "PracticumInquiry", label: "Practicum Inquiry" },
  {
    value: "ECEProgramApplicationInquiry",
    label: "ECE Program Application Inquiry",
  },
  {
    value: "ECEProgramApplicationRequirements",
    label: "ECE Program Application Requirements",
  },
  { value: "ProgramProfileInquiry", label: "Program Profile Inquiry" },
  {
    value: "IndividualEducationPlanIEP",
    label: "Individual Education Plan (IEP)",
  },
  { value: "MeetingRequest", label: "Meeting Request" },
  { value: "Other", label: "Other" },
];

export const isInitiableCommunicationCategory = (
  category: CommunicationCategory | null | undefined,
): category is InitiableCommunicationCategory =>
  initiableCommunicationCategoryOptions.some(
    (option) => option.value === category,
  );

export const getCommunicationCategoryLabel = (
  category: CommunicationCategory | null | undefined,
): string => {
  if (!category) {
    return "";
  }

  const match = communicationCategoryOptions.find(
    (option) => option.value === category,
  );
  return match ? match.label : category;
};
