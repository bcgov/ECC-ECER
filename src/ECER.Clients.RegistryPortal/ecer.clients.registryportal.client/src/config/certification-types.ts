import ECEAssistantContent from "@/components/ECEAssistantContent.vue";
import ECEFiveYearContent from "@/components/ECEFiveYearContent.vue";
import ECEOneYearContent from "@/components/ECEOneYearContent.vue";
import type { ExpandSelectOption } from "@/types/expand-select";

const certificationTypes: ExpandSelectOption[] = [
  {
    id: "1",
    title: "ECE Assistant",
    contentComponent: ECEAssistantContent,
  },
  {
    id: "2",
    title: "ECE One Year",
    contentComponent: ECEOneYearContent,
  },
  {
    id: "3",
    title: "ECE Five Year",
    contentComponent: ECEFiveYearContent,
  },
];

export default certificationTypes;
