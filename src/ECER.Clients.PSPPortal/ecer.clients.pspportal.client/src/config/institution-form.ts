import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Form } from "@/types/form";
import * as Rules from "@/utils/formRules";
import EceProvince from "@/components/inputs/EceProvince.vue";
import EceAuspice from "@/components/inputs/EceAuspice.vue";

const institutionForm: Form = {
  id: "institutionForm",
  title: "My institution details",
  components: {
    auspice: {
      id: "auspice",
      component: EceAuspice,
      props: {
        label: "Institution type",
        rules: [Rules.required("Select your institution type")],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    street1: {
      id: "street1",
      component: EceTextField,
      props: {
        label: "Street address",
        rules: [Rules.required("Enter your street address")],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    street2: {
      id: "street2",
      component: EceTextField,
      props: {
        label: "Address 2",
        rules: [],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    street3: {
      id: "street3",
      component: EceTextField,
      props: {
        label: "Address 3",
        rules: [],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    city: {
      id: "city",
      component: EceTextField,
      props: {
        label: "City",
        rules: [Rules.required("Enter your city")],
        maxLength: 50,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    province: {
      id: "province",
      component: EceProvince,
      props: {
        rules: [Rules.required("Select your province")],
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    postalCode: {
      id: "postalCode",
      component: EceTextField,
      props: {
        label: "Postal Code",
        rules: [Rules.required("Enter your postal code")],
        maxLength: 10,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
    website: {
      id: "website",
      component: EceTextField,
      props: {
        label: "Website",
        maxLength: 100,
      },
      cols: {
        md: 8,
        lg: 6,
        xl: 4,
      },
    },
  },
};

export default institutionForm;
