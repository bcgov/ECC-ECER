import ECEAssistantContent from "@/components/ECEAssistantContent.vue";
import ECEFiveYearContent from "@/components/ECEFiveYearContent.vue";
import ECEOneYearContent from "@/components/ECEOneYearContent.vue";
import type { ExpandSelectCertificateTypeOption } from "@/types/expand-select";

const certificationTypes: ExpandSelectCertificateTypeOption[] = [
  {
    id: "EceAssistant",
    title: "ECE Assistant",
    contentComponent: ECEAssistantContent,
    hasSubSelection: false,
  },
  {
    id: "OneYear",
    title: "ECE One Year",
    contentComponent: ECEOneYearContent,
    hasSubSelection: false,
  },
  {
    id: "FiveYears",
    title: "ECE Five Year",
    contentComponent: ECEFiveYearContent,
    hasSubSelection: true,
  },
];

export default certificationTypes;
