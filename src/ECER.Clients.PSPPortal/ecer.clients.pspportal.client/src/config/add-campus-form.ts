import EceTextField from "@/components/inputs/EceTextField.vue";
import EceProvince from "@/components/inputs/EceProvince.vue";
import EcePspUser from "@/components/inputs/EcePspUser.vue";
import type { Form } from "@/types/form";
import type { PspUserItem } from "@/components/inputs/EcePspUser.vue";

export const addCampusLocationForm: Form = {
  id: "addCampusLocationForm",
  title: "Location info",
  components: {
    name: {
      id: "name",
      component: EceTextField,
      props: {
        label: "Location name",
        maxLength: 100,
      },
      cols: { md: 8, lg: 6, xl: 4 },
    },
    street1: {
      id: "street1",
      component: EceTextField,
      props: {
        label: "Street address",
        maxLength: 100,
      },
      cols: { md: 8, lg: 6, xl: 4 },
    },
    street2: {
      id: "street2",
      component: EceTextField,
      props: {
        label: "Address 2",
        maxLength: 100,
      },
      cols: { md: 8, lg: 6, xl: 4 },
    },
    street3: {
      id: "street3",
      component: EceTextField,
      props: {
        label: "Address 3",
        maxLength: 100,
      },
      cols: { md: 8, lg: 6, xl: 4 },
    },
    city: {
      id: "city",
      component: EceTextField,
      props: {
        label: "City",
        maxLength: 100,
      },
      cols: { md: 8, lg: 6, xl: 4 },
    },
    province: {
      id: "province",
      component: EceProvince,
      props: {},
      cols: { md: 4, lg: 3, xl: 2 },
    },
    postalCode: {
      id: "postalCode",
      component: EceTextField,
      props: {
        label: "Postal code",
        maxLength: 10,
      },
      cols: { md: 8, lg: 6, xl: 4 },
    },
  },
};

export const createKeyContactForm = (users: PspUserItem[], institutionName: string): Form => ({
  id: "addCampusKeyContactForm",
  title: "Key contact",
  components: {
    keyContact: {
      id: "keyContact",
      component: EcePspUser,
      props: {
        users,
        institutionName,
      },
      cols: { md: 12 },
    },
    otherCampusContact: {
      id: "otherCampusContact",
      component: EceTextField,
      props: {
        label: "Other campus contact",
        maxLength: 100,
      },
      cols: { md: 8, lg: 6, xl: 4 },
    },
  },
});
