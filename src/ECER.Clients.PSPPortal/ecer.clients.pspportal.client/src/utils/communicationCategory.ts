import type { Components } from "@/types/openapi";

export type CommunicationCategory = Components.Schemas.CommunicationCategory;

export const communicationCategoryOptions: Array<{
  value: CommunicationCategory;
  label: string;
}> = [
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
